using System.Collections.Generic;

namespace BToolkit
{
    public class CsvData
    {
        public Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
        string[][] array;

        public CsvData(string inputString)
        {
            string[] lineArray = inputString.Trim().Split("\r"[0]);
            array = new string[lineArray.Length][];
            for (int i = 0; i < lineArray.Length; i++)
            {
                array[i] = lineArray[i].Split(',');
                //每个元素去空格
                for (int j = 0; j < array[i].Length; j++)
                {
                    array[i][j] = array[i][j].Trim();
                }
            }
            //放入字典
            dictionary = new Dictionary<string, string[]>();
            for (int i = 0; i < lineArray.Length; i++)
            {
                string key = array[i][0];
                if (!string.IsNullOrEmpty(key))
                {
                    if (!dictionary.ContainsKey(key.Trim()))
                    {
                        dictionary.Add(key.Trim(), array[i]);
                    }
                }
            }
        }

        public string GetData(string firstColValue, int colIndex)
        {
            if (!dictionary.ContainsKey(firstColValue) || dictionary[firstColValue] == null)
            {
                Debuger.LogError("=============FirstColValue不存在");
                return null;
            }
            if (colIndex >= dictionary[firstColValue].Length)
            {
                Debuger.LogError("=============Col超出范围");
                return null;
            }
            string[] rowArr = dictionary[firstColValue];
            return rowArr[colIndex].Trim();
        }

        public string GetData(int rowIndex, int colIndex)
        {
            if (array.Length == 0 || array[rowIndex].Length == 0)
            {
                Debuger.LogError("=============没有数据源");
                return null;
            }
            return array[rowIndex][colIndex].Trim();
        }

    }
}