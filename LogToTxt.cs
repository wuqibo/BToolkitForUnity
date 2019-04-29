using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BToolkit
{
    public class LogToTxt
    {

        private static FileStream fileStream;
        private static StreamWriter streamWriter;
        private const string fullPath = "C:/log.txt";

        public static void Write(string str)
        {
            streamWriter = File.AppendText(fullPath);
            streamWriter.WriteLine(str);
            streamWriter.Flush();
            streamWriter.Close();
        }

    }
}