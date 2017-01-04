﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVPacketViewer
{
    class SimpleUnpacker : Unpacker
    {
        protected override string PrintStreamContents(Stream stream)
        {
            var result = new StringBuilder();
            var outputBuffer = new byte[0x10];
            while (true)
            {
                int readAmount = stream.Read(outputBuffer, 0, outputBuffer.Length);
                if (readAmount == 0) break;
                for (int i = 0; i < readAmount; i++)
                {
                    result.AppendFormat("{0:X2} ", outputBuffer[i]);
                }
            }
            return result.ToString();
        }
        protected override string PrintCommandHeader(uint commandSize) { return ""; }
    }
}
