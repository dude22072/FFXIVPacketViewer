using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFXIVPacketViewer
{
    class OpCodeInterpreter //TODO - Add size checking
    {
        UInt16[] knownClientOpCodes = { 0x0001, 0x00CA, 0x00CD, 0x012D };
        UInt16[] knownServerOpCodes = { 0x0001, 0x00CA, 0x00CB, 0x00CF, 0x0131, 0x0134, 0x0137, 0x0139, 0x013D, 0x0177, 0x018B, 0x018D };
        OpCodeInterpreterClient interpClient = new OpCodeInterpreterClient();
        OpCodeInterpreterServer interpServer = new OpCodeInterpreterServer();

        private Boolean isValidOpcode(bool isServer, UInt16 opCode)
        {
            if(isServer)
            {
                return (knownServerOpCodes.Contains(opCode)) ? true: false;
            }
            else
            {
                return (knownClientOpCodes.Contains(opCode)) ? true : false;
            }
        }
        public string interpretOpCode(bool isServer, UInt16 opCode, byte[] data)
        {
            if(isValidOpcode(isServer,opCode))
            {
                return isServer ? interpServer.interpretOpCode(opCode, data) : interpClient.interpretOpCode(opCode, data);
            }
            else
            {
                return "Undocumented or improper OpCode";
            }
        }

        #region common
        //Pulling functions from Common
        public static DateTime UnixTimeStampToDateTimeMiliseconds(double unixTimeStamp)
        {
            return Common.UnixTimeStampToDateTimeMiliseconds(unixTimeStamp);
        }
        public static DateTime UnixTimeStampToDateTimeSeconds(double unixTimeStamp)
        {
            return Common.UnixTimeStampToDateTimeSeconds(unixTimeStamp);
        }
        public static byte[] FromHex(string hex)
        {
            return Common.FromHex(hex);
        }
        public static string endianInterpreter(byte[] input, int size, int firstByte)
        {
            return Common.endianInterpreter(input, size, firstByte);
        }
        public static UInt16 HexToUInt16(string input)
        {
            return Common.HexToUInt16(input);
        }
        public static UInt32 HexToUInt32(string input)
        {
            return Common.HexToUInt32(input);
        }
        public static UInt64 HexToUInt64(string input)
        {
            return Common.HexToUInt64(input);
        }
        #endregion

        private class OpCodeInterpreterClient
        {
            public string interpretOpCode(UInt16 opCode, byte[] data)
            {
                string toReturn = "";
                switch (opCode)
                {
                    case 0x0001: //Ping
                        string pingTicks = endianInterpreter(data, 4, 3);
                        return "Ping: Look for a pong responding to " + HexToUInt32(pingTicks) + "(0x" + pingTicks + ")";
                    case 0x00CA: //Position Update -- TODO - x/y/z/rot to proper numbers?
                        string time = endianInterpreter(data, 8, 7); //time = binReader.ReadUInt64(); ???
                        string x = endianInterpreter(data, 4, 11);
                        string y = endianInterpreter(data, 4, 15);
                        string z = endianInterpreter(data, 4, 19);
                        string rot = endianInterpreter(data, 4, 23);
                        string moveState = endianInterpreter(data, 2, 25);
                        //Int32 was overflowing?
                        toReturn = "Position Update\r\nX:" + HexToUInt32(x) + " (0x" + x + ")\r\nY:" + HexToUInt32(y) + " (0x" + y + ")\r\nZ:" + HexToUInt32(z) + " (0x" + z + ")\r\nRotation:" + HexToUInt32(rot) + " (0x" + rot + ")\r\nMove State:";
                        switch(HexToUInt16(moveState))
                        {
                            case 0: toReturn += "Standing";break;
                            case 1: toReturn += "Walking";break;
                            case 2: toReturn += "Running"; break;
                            case 3: toReturn += "Active Stance: Running"; break; //Maybe?
                            default: toReturn += "Unknown"; break;
                        }
                        toReturn += " (0x" + moveState + ")";
                        return toReturn;
                    case 0x00CD: //Target Selected
                        string targetID = endianInterpreter(data, 4, 3);
                        string otherVal = endianInterpreter(data, 4, 7);
                        return "SetTarget\r\nTarget ID:"+ HexToUInt32(targetID) + " (0x"+targetID+")\r\notherVal:"+ HexToUInt32(otherVal)+" (0x"+otherVal+")";
                    case 0x012D:

                        return "Start Event";
                    default:
                        return "You shouldn't see this error...";
                }
            }
        }
        private class OpCodeInterpreterServer
        {
            List<Tuple<Int64, string>> ActorMainState = new List<Tuple<Int64, string>>();
            List<Tuple<Int64, string>> ActorSubState = new List<Tuple<Int64, string>>();
            public OpCodeInterpreterServer()
            {
                ActorMainState.Add(new Tuple<Int64, string>(0, "MAIN_STATE_PASSIVE"));
                ActorMainState.Add(new Tuple<Int64, string>(1, "MAIN_STATE_DEAD"));
                ActorMainState.Add(new Tuple<Int64, string>(2, "MAIN_STATE_ACTIVE"));
                ActorMainState.Add(new Tuple<Int64, string>(3, "MAIN_STATE_DEAD2"));
                ActorMainState.Add(new Tuple<Int64, string>(11, "MAIN_STATE_SITTING_OBJECT"));
                ActorMainState.Add(new Tuple<Int64, string>(13, "MAIN_STATE_SITTING_FLOOR"));
                ActorMainState.Add(new Tuple<Int64, string>(15, "MAIN_STATE_MOUNTED"));
                ActorMainState.Add(new Tuple<Int64, string>(0x0E, "MAIN_STATE_UNKNOWN1"));
                ActorMainState.Add(new Tuple<Int64, string>(0x1E, "MAIN_STATE_UNKNOWN2"));
                ActorMainState.Add(new Tuple<Int64, string>(0x1F, "MAIN_STATE_UNKNOWN3"));
                ActorMainState.Add(new Tuple<Int64, string>(0x20, "MAIN_STATE_UNKNOWN4"));
                ActorSubState.Add(new Tuple<Int64, string>(0x00, "SUB_STATE_NONE"));
                ActorSubState.Add(new Tuple<Int64, string>(0xBF, "SUB_STATE_PLAYER"));
                ActorSubState.Add(new Tuple<Int64, string>(0x03, "SUB_STATE_MONSTER"));
            }
            String returnTupleString(Int64 toFind, List<Tuple<Int64, string>> toIterate)
            {
                foreach(Tuple<Int64, string> tup in toIterate)
                {
                    if(tup.Item1 == toFind)
                    {
                        return tup.Item2;
                    }
                }
                return null;
            }

            public string interpretOpCode(UInt16 opCode, byte[] data)
            {
                string toReturn = ""; //For those that use it
                switch (opCode)
                {
                    case 0x0001: //Pong
                        string pingTicks = endianInterpreter(data, 4, 3);
                        return "Pong: Response to ping " + HexToUInt32(pingTicks) + "(0x" + pingTicks + ")";
                    case 0x00CA: //Create Actor
                        //TODO
                        return "Create Actor";
                    case 0x00CB: //Delete Actor
                        string actorID = endianInterpreter(data, 4, 3);
                        return "Delete Actor\r\nID:"+ HexToUInt32(actorID)+" (0x"+actorID+")";
                    case 0x00CF: //Move Actor to Position
                        string x = endianInterpreter(data, 4, 11);
                        string y = endianInterpreter(data, 4, 15);
                        string z = endianInterpreter(data, 4, 19);
                        string rot = endianInterpreter(data, 4, 23);
                        string moveState = endianInterpreter(data, 2, 25);
                        toReturn = "Move Actor to Position\r\nX:" + HexToUInt32(x) + " (0x" + x + ")\r\nY:" + HexToUInt32(y) + " (0x" + y + ")\r\nZ:" + HexToUInt32(z) + " (0x" + z + ")\r\nRotation:" + HexToUInt32(rot) + " (0x" + rot + ")\r\nMove State:";
                        switch (HexToUInt16(moveState))
                        {
                            case 0: toReturn += "Standing"; break;
                            case 1: toReturn += "Walking"; break;
                            case 2: toReturn += "Running"; break;
                            case 3: toReturn += "Active Stance: Running"; break; //Maybe?
                            default: toReturn += "Unknown"; break;
                        }
                        toReturn += " (0x" + moveState + ")";
                        return toReturn;
                    case 0x0131: //End Client Order Event
                        //TODO: https://bitbucket.org/Ioncannon/ffxiv-classic-server/src/4b0ffb38827301bd1f4625a6c2c3a4a8102b279a/FFXIVClassic%20Map%20Server/packets/send/events/EndEventPacket.cs?at=group_work&fileviewer=file-view-default
                        return "End Event";
                    case 0x0134: //Set Actor State
                        //TODO: https://bitbucket.org/Ioncannon/ffxiv-classic-server/src/4b0ffb38827301bd1f4625a6c2c3a4a8102b279a/FFXIVClassic%20Map%20Server/packets/send/Actor/SetActorStatePacket.cs?at=group_work&fileviewer=file-view-default
                        string combinedState = endianInterpreter(data, 2, 1);
                        UInt32 combinedStateInt = HexToUInt32(combinedState);
                        UInt32 mainStateInt = combinedStateInt & 0xFF;
                        UInt32 subStateInt = (combinedStateInt >> 8 ) & 0xFF ;
                        string mainState = returnTupleString(mainStateInt, ActorMainState);
                        string subState = returnTupleString(subStateInt, ActorSubState);
                        return "Set Actor State\r\nMain State:" + (mainState == null ? "Unknown" : mainState) + " (0x" + mainStateInt.ToString("X") + ")\r\nSub State:" + (subState == null ? "Unknown" : subState) + " (0x" + subStateInt.ToString("X") + ")";
                    case 0x0137: //Set Actor Propery (AKA: SynchMemory)

                        return "Set Actor Propery (AKA: SynchMemory)\r\nThis is a complicated packet and should be read on a case-by-case basis.\r\nA.K.A. I don't understand it.";
                    case 0x0139: //Battle Action (x01 Log/Effect)
                        //TODO: https://bitbucket.org/Ioncannon/ffxiv-classic-server/src/4b0ffb38827301bd1f4625a6c2c3a4a8102b279a/FFXIVClassic%20Map%20Server/packets/send/Actor/battle/BattleActionX01Packet.cs?at=group_work&fileviewer=file-view-default
                        return "Single Battle Action";
                    case 0x013D: //Set Actor Name
                        toReturn = "Set Actor Name\r\n";
                        string displayID = endianInterpreter(data, 4, 3);
                        if((HexToUInt32(displayID)== 0) || (HexToUInt32(displayID) == 0xFFFFFFFF))
                        {
                            toReturn += "Custom Name: ";
                        }
                        else
                        {
                            toReturn += "Display Name ID:" + HexToUInt32(displayID) + " (0x" + displayID + ")";
                        }
                        return toReturn;
                    case 0x0177: //Set Single status
                        toReturn = "Single Status Update\r\n"; //TODO: What status is updated?
                        string index = endianInterpreter(data, 2, 1);
                        toReturn += "Index:" + HexToUInt16(index) + " (0x"+index+")\r\n";
                        string statusCode = endianInterpreter(data, 2, 3);
                        toReturn += "Status Code:" + HexToUInt16(statusCode) + " (0x" + statusCode + ")";
                        return toReturn;
                    case 0x018B://Group Update Member (x1)
                        //TODO: ???
                        return "Group Update Member (x1)";
                    case 0x018D: //Group Update Member (x16, variable) (??)
                        //TODO: https://bitbucket.org/Ioncannon/ffxiv-classic-server/src/4b0ffb38827301bd1f4625a6c2c3a4a8102b279a/FFXIVClassic%20Map%20Server/packets/send/groups/GroupMembersX16Packet.cs?at=group_work&fileviewer=file-view-default
                        return "Group Update Member (x16, variable) (??)\r\n This packet is still being worked on.";
                    default:
                        return "You shouldn't see this error...";
                }
            }
        }
    }

    

    
}
