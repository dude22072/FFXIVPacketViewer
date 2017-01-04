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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFXIVPacketViewer
{
    public partial class frmMain : Form
    {
        byte[][] packets = new byte[1000][];
        int currentPacket = 0;
        Unpacker _unpacker = new SimpleUnpacker();
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            processBytes(txtInput.Text);
            processPacket(currentPacket);
        }
        private void processBytes(string input)
        {
            /*Convert the 010 editor output to hexstring*/
            string replaced = input.Replace(' ', '-');
            /*turn the hexstring into a byte array*/
            byte[] hexArray = FromHex(replaced);
            /*Gets the size from the basepacket header*/
            string size = "";
            size += BitConverter.ToString(hexArray, 5, 1);
            size += BitConverter.ToString(hexArray, 4, 1);
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
                for(int I = 0; I<result; I++)
                {
                    packets[Iteration][I] = shifted[I];
                }
                //new itteration
                Iteration++;
                byte[] shifted2 = new byte[shifted.Length-res];
                Buffer.BlockCopy(shifted, res, shifted2, 0, shifted.Length - res);
                shifted = shifted2;
                if (shifted.Length<1)
                {
                    dataRemains = false;
                    break;
                }
                size = "";
                size += BitConverter.ToString(shifted, 5, 1);
                size += BitConverter.ToString(shifted, 4, 1);

            }
#if DEBUG
            Debug.Print("Breakpoint");
#endif
        }
        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
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
            String size = "";
            size += BitConverter.ToString(data, 5, 1);
            size += BitConverter.ToString(data, 4, 1);
            String subpackets = "";
            subpackets += BitConverter.ToString(data, 7, 1);
            subpackets += BitConverter.ToString(data, 6, 1);
            Decimal sizeresult = long.Parse(size, System.Globalization.NumberStyles.HexNumber);
            Decimal subresult = long.Parse(subpackets, System.Globalization.NumberStyles.HexNumber);
            String timestamp = "";
            timestamp += BitConverter.ToString(data, 15, 1);
            timestamp += BitConverter.ToString(data, 14, 1);
            timestamp += BitConverter.ToString(data, 13, 1);
            timestamp += BitConverter.ToString(data, 12, 1);
            timestamp += BitConverter.ToString(data, 11, 1);
            timestamp += BitConverter.ToString(data, 10, 1);
            timestamp += BitConverter.ToString(data, 9, 1);
            timestamp += BitConverter.ToString(data, 8, 1);
            byte[] remainingData = new byte[data.Length - 16];
            Buffer.BlockCopy(data, 16, remainingData, 0, data.Length - 16);
            displayPacket((data[1] == 0x01 ? true : false), Convert.ToInt16(sizeresult), Convert.ToInt16(subresult), timestamp, remainingData);
            updatePacketNumber();
        }
        private void displayPacket(Boolean wasCompressed, Int16 orignialSize, Int16 subPackets, string timestamp, byte[] remainingdata)
        {
            string display = "Base Packet\r\n";
            if (wasCompressed) { display += "Compressed\r\n"; } else { display += "Uncompressed\r\n"; }
            display += "Size: " + orignialSize + " (0x" + orignialSize.ToString("X" + 4) + ")\r\n" + "SubPackets: " + subPackets + "\r\n";
            Decimal resulttime = long.Parse(timestamp, System.Globalization.NumberStyles.HexNumber);
            display += "Timestamp: " + UnixTimeStampToDateTimeMiliseconds(Convert.ToDouble(resulttime)) + "\r\n\r\n";
            
            //Seperating SubPackets Begin
            string size = "";
            size += BitConverter.ToString(remainingdata, 1, 1);
            size += BitConverter.ToString(remainingdata, 0, 1);
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
                size = "";
                size += BitConverter.ToString(shifted, 1, 1);
                size += BitConverter.ToString(shifted, 0, 1);

            }
            //Seperating SubPackets End

            for(int I = 0; I < subPackets; I++)
            {
                byte[] workingData = subPacketsData[I];
                size = "";
                size += BitConverter.ToString(workingData, 1, 1);
                size += BitConverter.ToString(workingData, 0, 1);
                string sourceID = "";
                sourceID += BitConverter.ToString(workingData, 7, 1);
                sourceID += BitConverter.ToString(workingData, 6, 1);
                sourceID += BitConverter.ToString(workingData, 5, 1);
                sourceID += BitConverter.ToString(workingData, 4, 1);
                string targetID = "";
                targetID += BitConverter.ToString(workingData, 11, 1);
                targetID += BitConverter.ToString(workingData, 10, 1);
                targetID += BitConverter.ToString(workingData, 9, 1);
                targetID += BitConverter.ToString(workingData, 8, 1);
                string opcode = "";
                opcode += BitConverter.ToString(workingData, 19, 1);
                opcode += BitConverter.ToString(workingData, 18, 1);
                string subtimestamp = "";
                subtimestamp += BitConverter.ToString(workingData, 27, 1);
                subtimestamp += BitConverter.ToString(workingData, 26, 1);
                subtimestamp += BitConverter.ToString(workingData, 25, 1);
                subtimestamp += BitConverter.ToString(workingData, 24, 1);
                byte[] finalData = new byte[workingData.Length - 31];
                Buffer.BlockCopy(workingData, 32, finalData, 0, workingData.Length - 32);
                display += displaySubPacket(I+1, Convert.ToInt16(long.Parse(size, System.Globalization.NumberStyles.HexNumber)), Convert.ToInt32(long.Parse(sourceID, System.Globalization.NumberStyles.HexNumber)), Convert.ToInt32(long.Parse(targetID, System.Globalization.NumberStyles.HexNumber)), Convert.ToInt16(long.Parse(opcode, System.Globalization.NumberStyles.HexNumber)), Convert.ToInt32(long.Parse(subtimestamp, System.Globalization.NumberStyles.HexNumber)), finalData);
            }

            lblReadable.Text = display;
        }
        private string displaySubPacket(int I, Int16 subPacketSize, Int32 sourceID, Int32 targetID, Int16 opcode, Int32 Timestamp, byte[] data)
        {
            string display = "SubPacket " + I + "\r\n";
            display += "Size: " + subPacketSize + " (0x" + subPacketSize.ToString("X" + 4) + ")\r\n";
            display += "SourceID: " + sourceID + " (0x" + sourceID.ToString("X" + 8) + ")\r\n";
            display += "TargetID: " + targetID + " (0x" + targetID.ToString("X" + 8) + ")\r\n";
            display += "OpCode: 0x" + opcode.ToString("X" + 4) + "\r\n";
            display += "Timestamp: " + UnixTimeStampToDateTimeSeconds(Timestamp) + "\r\n";
            display += "Data: \r\n";
            for(int J = 0; J < data.Length; J++)
            {
                display += BitConverter.ToString(data, J, 1) + " ";
                if((J+1)%16==0)
                {
                    display += "\r\n";
                }
            }
            display += "\r\n\r\n";
            return display;
        }
        private void updatePacketNumber()
        {
            lblCurentPacket.Text = "Current Packet: " + currentPacket.ToString();
        }
        private void btnNextPacket_Click(object sender, EventArgs e)
        {
            currentPacket++;
            if (currentPacket > 1000) { currentPacket = 1000; }
            processPacket(currentPacket);
        }
        private void btnLastPacket_Click(object sender, EventArgs e)
        {
            currentPacket--;
            if (currentPacket < 0) { currentPacket = 0; }
            processPacket(currentPacket);
        }
        public static DateTime UnixTimeStampToDateTimeMiliseconds(double unixTimeStamp)
        {
            // Unix timestamp is Miliseconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public static DateTime UnixTimeStampToDateTimeSeconds(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
