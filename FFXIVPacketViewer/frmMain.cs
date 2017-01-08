using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
#if DEBUG
using System.Diagnostics;
#endif
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFXIVPacketViewer
{
    public partial class frmMain : Form
    {
        byte[][] packets = new byte[2000][];
        int currentPacket = 0;
        int packetsInCapture = 0;
        UInt64[] opcodesocurence = new UInt64[0xffff];
        Boolean modifiedSinceDataRead;
        Boolean isXIVmonPackets = false;
        String dataReport = "";
        Unpacker _unpacker = new SimpleUnpacker();
        OpCodeInterpreter opinterp = new OpCodeInterpreter();
        public frmMain()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void btnLoadText_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Text Files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string tmp = File.ReadAllText(openFileDialog1.FileName);
                    string tmp2 = System.Text.RegularExpressions.Regex.Replace(tmp, @"\t|\n|\r", " ");
                    txtInput.Text = tmp2;
                    btnDataReport.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private void btnLoadHex_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialogHex = new OpenFileDialog();

            openFileDialogHex.Filter = "Hex Files (*.hex)|*.hex|"/*XIVmon Files|*.xiv|*/+"Bin Logs|*.bin";
            openFileDialogHex.FilterIndex = 1;
            openFileDialogHex.RestoreDirectory = true;

            if (openFileDialogHex.ShowDialog() == DialogResult.OK)
            {
                //try
                //{
                    FileStream readStream;
                    readStream = new FileStream(openFileDialogHex.FileName, FileMode.Open, FileAccess.Read);
                    BinaryReader readBinary = new BinaryReader(readStream);

                    byte inbyte;
                    string outbyte = "";
                    string ext = Path.GetExtension(openFileDialogHex.FileName);
                    if (ext == ".xiv")
                    {
                        outbyte = "XIV ";
                    }
                    while (readBinary.BaseStream.Position < readBinary.BaseStream.Length)
                    {
                        inbyte = readBinary.ReadByte();
                        outbyte += Convert.ToString(String.Format("{0:X2}", inbyte)) + " ";
                    }
                    string tmp2 = System.Text.RegularExpressions.Regex.Replace(outbyte, @"\t|\n|\r", " ");
                    txtInput.Text = tmp2;

                    //string tmp = File.ReadAllText(openFileDialogHex.FileName);
                    //string tmp2 = System.Text.RegularExpressions.Regex.Replace(tmp, @"\t|\n|\r", " ");
                    //txtInput.Text = tmp2;
                    btnDataReport.Enabled = true;
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                //}
            }
        }
        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            if (txtInput.Text.Substring(0, 3) == "XIV")
            {
                isXIVmonPackets = true;
                processXIVBytes(txtInput.Text);
                processXIVPacket(currentPacket);
            }
            else
            {
                isXIVmonPackets = false;
                processBytes(txtInput.Text);
                processPacket(currentPacket);
            }
           
            modifiedSinceDataRead = true;
        }
        private void processBytes(string input)
        {
            /*Convert the 010 editor output to hexstring*/
            string replaced = input.Replace(' ', '-');
            /*turn the hexstring into a byte array*/
            byte[] hexArray = FromHex(replaced);
            /*Gets the size from the basepacket header*/
            string size = endianInterpreter(hexArray, 2, 5);
            Decimal result = long.Parse(size, System.Globalization.NumberStyles.HexNumber);
            Boolean dataRemains = true;
            int Iteration = 0;
            byte[] shifted = hexArray;
            while (dataRemains)
            {
#if DEBUG
                Debug.Print(Iteration.ToString());
#endif
                result = long.Parse(size, System.Globalization.NumberStyles.HexNumber);
                int res = Convert.ToInt16(result);
                packets[Iteration] = new byte[res];
                for (int I = 0; I < result; I++)
                {
                    packets[Iteration][I] = shifted[I];
                }
                //new itteration
                Iteration++;
                byte[] shifted2 = new byte[shifted.Length - res];
                Buffer.BlockCopy(shifted, res, shifted2, 0, shifted.Length - res);
                shifted = shifted2;
                if (shifted.Length < 1)
                {
                    dataRemains = false;
                    break;
                }
                size = endianInterpreter(shifted, 2, 5);

            }
            packetsInCapture = Iteration - 1; //Because it gets added to before the empty data check happens
#if DEBUG
            Debug.Print("Breakpoint");
#endif
        }
        private void processPacket(int packetToProcess)
        {
            byte[] data = packets[packetToProcess];
            if (data[1] == 0x01)
            {
                string tmp = BitConverter.ToString(data).Replace("-", string.Empty);
                string tmp2 = _unpacker.UnpackPacket(tmp);
                byte[] tmpbytes = FromHex(tmp2.Replace(" ", "-"));
                data = tmpbytes;
            }
            lblHexOut.Text = "";
            for (int J = 0; J < data.Length; J++)
            {
                lblHexOut.Text += BitConverter.ToString(data, J, 1) + " ";
                if ((J + 1) % 16 == 0)
                {
                    lblHexOut.Text += "\r\n";
                }
            }
            string size = endianInterpreter(data, 2, 5);
            string subpackets = endianInterpreter(data, 2, 7); ;
            Decimal sizeresult = long.Parse(size, System.Globalization.NumberStyles.HexNumber);
            Decimal subresult = long.Parse(subpackets, System.Globalization.NumberStyles.HexNumber);
            String timestamp = endianInterpreter(data, 8, 15);
            byte[] remainingData = new byte[data.Length - 16];
            Buffer.BlockCopy(data, 16, remainingData, 0, data.Length - 16);
            displayPacket((data[1] == 0x01 ? true : false), Convert.ToInt16(sizeresult), Convert.ToInt16(subresult), timestamp, remainingData);
            updatePacketDisplay();
        }

        private void processXIVBytes(string input)
        {
            /*Convert the xivmon input to hexstring*/
            string replaced = input.Substring(4,input.Length-5).Replace(' ', '-');
            /*turn the hexstring into a byte array*/
            byte[] withHeaderArray = FromHex(replaced);
            byte[] hexArray = new byte[(withHeaderArray.Length - 0x0f)];
            for (int iguess = 0; iguess < (withHeaderArray.Length - 0x0f); iguess++)
            {
                hexArray[iguess] = withHeaderArray[iguess + 0x0f];
            }

            String subpackets = "";
            subpackets += hexArray[0x02];
            subpackets += hexArray[0x03];
            UInt16 subpacketsnum= HexToUInt16(subpackets);
            UInt16[] subpacketslength = new UInt16[subpacketsnum];
            for (int J = 0; J < subpacketsnum; J++)
            {
                if (J == 0)
                {
                    string tmplength = endianInterpreter(hexArray, 2, 0x12);
                    subpacketslength[0] = HexToUInt16(tmplength);
                }
                else
                {
                    UInt16 offset = 0;
                    for (int K = 0; K < J; K++) //Shutup
                    {
                        offset += subpacketslength[K];
                    }
                    string tmplength = endianInterpreter(hexArray, 2, 0x12+offset);
                    subpacketslength[J] = HexToUInt16(tmplength);
                }
            }
            //Decimal result = long.Parse(size, System.Globalization.NumberStyles.HexNumber);
            Decimal result = 0x0F;
            foreach (UInt16 num in subpacketslength)
            {
                result += num;
            }
            Boolean dataRemains = true;
            int Iteration = 0;
            byte[] shifted = hexArray;
            while (dataRemains)
            {
#if DEBUG
                Debug.Print(Iteration.ToString());
#endif
                result = 0x10;
                foreach (UInt16 num in subpacketslength)
                {
                    result += num;
                }
                int res = Convert.ToUInt16(result);
                packets[Iteration] = new byte[res];
                for (int I = 0; I < result; I++)
                {
                    packets[Iteration][I] = shifted[I];
                }
                //new itteration
                Iteration++;
                byte[] shifted2 = new byte[shifted.Length - res];
                Buffer.BlockCopy(shifted, res, shifted2, 0, shifted.Length - res);
                shifted = shifted2;
                if (shifted.Length < 1)
                {
                    dataRemains = false;
                    break;
                }
                subpackets = "";
                subpackets += shifted[0x02];
                subpackets += shifted[0x03];
                subpacketsnum = HexToUInt16(subpackets);
                subpacketslength = new UInt16[subpacketsnum];
                for (int J = 0; J < subpacketsnum; J++)
                {
                    if (J == 0)
                    {
                        string tmplength = endianInterpreter(shifted, 2, 0x11);
                        subpacketslength[0] = HexToUInt16(tmplength);
                    }
                    else
                    {
                        UInt16 offset = 0;
                        for (int K = 0; K < J; K++) //Shutup
                        {
                            offset += subpacketslength[K];
                        }
                        string tmplength = endianInterpreter(shifted, 2, 0x11 + offset);
                        subpacketslength[J] = HexToUInt16(tmplength);
                    }
                }

            }
            packetsInCapture = Iteration - 1; //Because it gets added to before the empty data check happens


#if DEBUG
            Debug.Print("Breakpoint");
#endif
        }
        private void processXIVPacket(int packetToProcess)
        {
            byte[] data = packets[packetToProcess];
            /*if (data[1] == 0x01)
            {
                string tmp = BitConverter.ToString(data).Replace("-", string.Empty);
                string tmp2 = _unpacker.UnpackPacket(tmp);
                byte[] tmpbytes = FromHex(tmp2.Replace(" ", "-"));
                data = tmpbytes;
            }*/
            lblHexOut.Text = "";
            for (int J = 0; J < data.Length; J++)
            {
                lblHexOut.Text += BitConverter.ToString(data, J, 1) + " ";
                if ((J + 1) % 16 == 0)
                {
                    lblHexOut.Text += "\r\n";
                }
            }

            String subpackets = endianInterpreter(data, 2, 3);
            UInt16 subpacketsnum = HexToUInt16(subpackets);
            UInt16[] subpacketslength = new UInt16[subpacketsnum];
            for (int J = 0; J < subpacketsnum; J++)
            {
                if (J == 0)
                {
                    string tmplength = endianInterpreter(data, 2, 0x11);
                    subpacketslength[0] = HexToUInt16(tmplength);
                }
                else
                {
                    UInt16 offset = 0;
                    for (int K = 0; K < J; K++) //Shutup
                    {
                        offset += subpacketslength[K];
                    }
                    string tmplength = endianInterpreter(data, 2, 0x11 + offset);
                    subpacketslength[J] = HexToUInt16(tmplength);
                }
            }
            //Decimal result = long.Parse(size, System.Globalization.NumberStyles.HexNumber);
            Decimal result = 0x0F;
            foreach (UInt16 num in subpacketslength)
            {
                result += num;
            }
            ////
            //string size = endianInterpreter(data, 2, 5);
            //string subpackets = endianInterpreter(data, 2, 7); ;
            //Decimal sizeresult = long.Parse(size, System.Globalization.NumberStyles.HexNumber);
            Decimal subresult = long.Parse(subpackets, System.Globalization.NumberStyles.HexNumber);
            //String timestamp = endianInterpreter(data, 8, 15);
            byte[] remainingData = new byte[data.Length - 16];
            Buffer.BlockCopy(data, 16, remainingData, 0, data.Length - 16);
            displayPacket((data[1] == 0x01 ? true : false), Convert.ToInt16(result), Convert.ToInt16(subresult), "0", remainingData);
            updatePacketDisplay();
        }

        private void displayPacket(Boolean wasCompressed, Int16 orignialSize, Int16 subPackets, string timestamp, byte[] remainingdata)
        {
            string display = "Base Packet\r\n";
            if (wasCompressed) { display += "Compressed\r\n"; } else { display += "Uncompressed\r\n"; }
            display += "Size: " + orignialSize + " (0x" + orignialSize.ToString("X" + 4) + ")\r\n" + "SubPackets: " + subPackets + "\r\n";
            Decimal resulttime = long.Parse(timestamp, System.Globalization.NumberStyles.HexNumber);
            display += "Timestamp: " + UnixTimeStampToDateTimeMiliseconds(Convert.ToDouble(resulttime)) + "\r\n\r\n";

            //Seperating SubPackets Begin
            string size = endianInterpreter(remainingdata, 2, 1);
            Decimal result = long.Parse(size, System.Globalization.NumberStyles.HexNumber);
            Boolean dataRemains = true;
            int Iteration = 0;
            byte[] shifted = remainingdata;
            byte[][] subPacketsData = new byte[subPackets][];
            while (dataRemains)
            {
#if DEBUG
                Debug.Print(Iteration.ToString());
#endif
                result = long.Parse(size, System.Globalization.NumberStyles.HexNumber);
                int res = Convert.ToInt16(result);
                subPacketsData[Iteration] = new byte[res];
                for (int I = 0; I < result; I++)
                {
                    subPacketsData[Iteration][I] = shifted[I];
                }
                //new itteration
                Iteration++;
                byte[] shifted2 = new byte[shifted.Length - res];
                Buffer.BlockCopy(shifted, res, shifted2, 0, shifted.Length - res);
                shifted = shifted2;
                if (shifted.Length < 1)
                {
                    dataRemains = false;
                    break;
                }
                size = endianInterpreter(shifted, 2, 1);

            }
            //Seperating SubPackets End

            for (int I = 0; I < subPackets; I++)
            {
                byte[] workingData = subPacketsData[I];
                size = endianInterpreter(workingData, 2, 1);
                string sourceID = endianInterpreter(workingData, 4, 7);
                string targetID = endianInterpreter(workingData, 4, 11);
                string opcode = endianInterpreter(workingData, 2, 19);
                string subtimestamp = endianInterpreter(workingData, 4, 27);
                byte[] finalData = new byte[workingData.Length - 32];
                Buffer.BlockCopy(workingData, 32, finalData, 0, workingData.Length - 32);
                display += displaySubPacket(I + 1, wasCompressed, HexToUInt16(size), HexToUInt32(sourceID), HexToUInt32(targetID), HexToUInt16(opcode), HexToUInt32(subtimestamp), finalData);
            }

            txtReadable.Text = display;
        }
        private string displaySubPacket(int I, bool isServer, UInt16 subPacketSize, UInt32 sourceID, UInt32 targetID, UInt16 opcode, UInt32 Timestamp, byte[] data)
        {
            string display = "SubPacket " + I + "\r\n";
            display += "Size: " + subPacketSize + " (0x" + subPacketSize.ToString("X" + 4) + ")\r\n";
            display += "SourceID: " + sourceID + " (0x" + sourceID.ToString("X" + 8) + ")\r\n";
            var sourceActorType = (byte)((sourceID >> 28) & 0xF);
            var sourceZoneId = (ushort)((sourceID >> 19) & 0x1FF);
            var sourceActorIndex = (ushort)(sourceID & 0xFFF);
            display += "     Actor Type:" + (sourceActorType == 0 ? "Player" : (sourceActorType == 4 ? "NPC/Monster" : (sourceActorType == 5 ? "World Master/Zone Master" : "Unknown"))) + " (0x" + sourceActorType.ToString("X" + 2) + ")\r\n";
            if (sourceActorType == 4)
            {
                display += "     Zone ID:" + zoneIDs[sourceZoneId] + " (0x" + sourceZoneId.ToString("X" + 4) + ")\r\n";
                display += "     Actor Index:" + sourceActorIndex + " (0x" + sourceActorIndex.ToString("X" + 4) + ")\r\n";
            }
            display += "TargetID: " + targetID + " (0x" + targetID.ToString("X" + 8) + ")\r\n";
            var targetActorType = (byte)((targetID >> 28) & 0xF);
            var targetZoneId = (ushort)((targetID >> 19) & 0x1FF);
            var targetActorIndex = (ushort)(targetID & 0xFFF);
            display += "     Actor Type:" + (targetActorType == 0 ? "Player" : (targetActorType == 4 ? "NPC/Monster" : "Unknown")) + " (0x" + targetActorType.ToString("X" + 2) + ")\r\n";
            if (targetActorType != 0)
            {
                display += "     Zone ID:" + zoneIDs[targetZoneId] + " (0x" + targetZoneId.ToString("X" + 4) + ")\r\n";
                display += "     Actor Index:" + targetActorIndex + " (0x" + targetActorIndex.ToString("X" + 4) + ")\r\n";
            }
            display += "OpCode: 0x" + opcode.ToString("X" + 4) + "\r\n";
            display += "Timestamp: " + UnixTimeStampToDateTimeSeconds(Timestamp) + "\r\n";
            display += "Data: \r\n";
            for (int J = 0; J < data.Length; J++)
            {
                display += BitConverter.ToString(data, J, 1) + " ";
                if ((J + 1) % 16 == 0)
                {
                    display += "\r\n";
                }
            }
            display += "\r\n";
            display += "Meaning:\r\n" + opinterp.interpretOpCode(isServer, opcode, data) + "\r\n\r\n";
            return display;
        }
        /// <summary> Updates the packet display using the common variables.</summary>
        private void updatePacketDisplay()
        {
            updatePacketDisplay(currentPacket, packetsInCapture);
        }
        private void updatePacketDisplay(int current)
        {
            updatePacketDisplay(current, packetsInCapture);
        }
        /**
         * <summary>Updates the "Curent packet:" display.</summary>
         * <param name="current">Number before the slash.</param>
         * <param name="max">Number after the slash.</param>
         */
        private void updatePacketDisplay(int current, int max)
        {
            lblCurentPacket.Text = "Current Packet: " + current.ToString() + "/" + max.ToString();
        }
        private void btnNextPacket_Click(object sender, EventArgs e)
        {
            currentPacket++;
            if (currentPacket > 1000) { currentPacket = 1000; } //In case we have more than 1000 packets in a capture, as the packets[][] has a size of 1000.
            if (currentPacket > packetsInCapture) { currentPacket = packetsInCapture; }
            processPacket(currentPacket);
        }
        private void btnLastPacket_Click(object sender, EventArgs e)
        {
            currentPacket--;
            if (currentPacket < 0) { currentPacket = 0; }
            processPacket(currentPacket);
        }
        private void btnDataReport_Click(object sender, EventArgs e)
        {
            if (modifiedSinceDataRead)
            {
                progressBar1.Visible = true;
                lblBar.Visible = true;
                lblBar.Text = "Reading packets...";
                progressBar1.Maximum = packetsInCapture;
                int packetWas = currentPacket;
                for (int packetNumb = 0; packetNumb < packetsInCapture; packetNumb++)
                {
                    processPacket(packetNumb);
                    string[] regexed = Regex.Split(txtReadable.Text.Replace("\r\n", ""), "(?<=OpCode: 0x)");
                    for (int I = 1; I < regexed.Length; I++)
                    {
                        opcodesocurence[HexToUInt32(regexed[I].Substring(0, 4))]++;
                    }
                    progressBar1.Value = packetNumb + 1;
                }
                currentPacket = packetWas;
                processPacket(currentPacket);

                progressBar1.Value = 0;
                progressBar1.Maximum = 0xffff;
                lblBar.Text = "Processing packets...";

                for (int I = 0; I < 0xFFFF; I++)
                {
                    if (opcodesocurence[I] > 0)
                    {
                        dataReport += ("0x" + I.ToString("X" + 4) + " - " + opcodesocurence[I].ToString()) + "\r\n";
                    }
                    progressBar1.Value = I + 1;
                }
                progressBar1.Visible = false;
                lblBar.Visible = false;
                modifiedSinceDataRead = false;
            }
            MessageBox.Show(dataReport, "Data Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region common
        //Pulling functions from Common
        private static DateTime UnixTimeStampToDateTimeMiliseconds(double unixTimeStamp)
        {
            return Common.UnixTimeStampToDateTimeMiliseconds(unixTimeStamp);
        }
        private static DateTime UnixTimeStampToDateTimeSeconds(double unixTimeStamp)
        {
            return Common.UnixTimeStampToDateTimeSeconds(unixTimeStamp);
        }
        private static byte[] FromHex(string hex)
        {
            return Common.FromHex(hex);
        }
        private static string endianInterpreter(byte[] input, int size, int firstByte)
        {
            return Common.endianInterpreter(input, size, firstByte);
        }
        //TODO - 
        private static UInt16 HexToUInt16(string input)
        {
            return Common.HexToUInt16(input);
        }
        private static UInt32 HexToUInt32(string input)
        {
            return Common.HexToUInt32(input);
        }
        private static UInt64 HexToUInt64(string input)
        {
            return Common.HexToUInt64(input);
        }
        private static Single HexToFloat(String input)
        {
            return Common.HexToFloat(input);
        }
        private static string[] zoneIDs = Common.zoneIDs;

        #endregion


    }
}
