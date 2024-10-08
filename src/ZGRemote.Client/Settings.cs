﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGRemote.Client
{
    internal static class Settings
    {
        public static byte[] RSACSPBLOB;

        public static string PROGRAM_FILE_NAME;

        static Settings()
        {
            PROGRAM_FILE_NAME = Process.GetCurrentProcess().MainModule.FileName;
            RSACSPBLOB = new byte[] { 6, 2, 0, 0, 0, 164, 0, 0, 82, 83, 65, 49, 0, 8, 0, 0, 1, 0, 1, 0, 41, 192, 232, 21, 247, 126, 223, 117, 7, 4, 217, 242, 151, 73, 66, 190, 78, 150, 218, 71, 81, 152, 254, 194, 177, 71, 77, 108, 104, 3, 115, 195, 75, 50, 99, 223, 14, 67, 154, 142, 63, 201, 79, 131, 125, 112, 46, 87, 146, 179, 41, 132, 58, 70, 39, 130, 236, 42, 200, 181, 188, 37, 166, 56, 242, 151, 205, 16, 3, 14, 105, 237, 62, 27, 77, 45, 104, 174, 22, 244, 168, 68, 168, 32, 111, 147, 159, 215, 162, 183, 230, 17, 38, 61, 183, 37, 172, 175, 251, 175, 208, 1, 175, 116, 171, 166, 79, 94, 61, 52, 211, 187, 79, 39, 209, 51, 29, 210, 176, 242, 193, 192, 226, 48, 99, 160, 26, 132, 15, 5, 43, 37, 44, 203, 13, 243, 244, 5, 161, 19, 146, 127, 170, 17, 24, 122, 240, 11, 152, 117, 56, 133, 155, 13, 218, 87, 95, 192, 157, 139, 7, 61, 142, 24, 14, 239, 236, 239, 165, 245, 76, 222, 162, 123, 194, 197, 225, 187, 147, 76, 132, 55, 102, 201, 101, 200, 186, 172, 103, 133, 180, 241, 123, 140, 8, 128, 155, 3, 160, 3, 211, 161, 32, 8, 163, 82, 169, 71, 120, 182, 210, 214, 127, 102, 240, 36, 37, 237, 26, 58, 161, 214, 247, 222, 99, 223, 56, 208, 84, 180, 219, 237, 215, 18, 147, 100, 186, 238, 100, 53, 225, 99, 113, 240, 236, 24, 169, 209, 73, 35, 184, 18, 162, 137, 86, 181 };
        }
    }
}