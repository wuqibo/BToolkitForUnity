using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BToolkit
{
    public class CsvData
    {

        public int index;
        Dictionary<string, string[]> dictionary;

        public Dictionary<string, string[]> Dictionary { get { return dictionary; } }

        string[][] array;
        string[] lineArray;
        string resourcePath;

        public CsvData(string resourcePath) : this(resourcePath, 0)
        {
        }

        public CsvData(string resourcePath, int index)
        {
            this.resourcePath = resourcePath;
            this.index = index;
            TextAsset binAsset = Resources.Load(resourcePath, typeof(TextAsset)) as TextAsset;
            if (binAsset == null)
            {
                Debug.LogError(resourcePath + " 不存在");
            }
            setConstructor(binAsset.text);
        }

        public CsvData(string strTotal, bool forStrTotal)
        {
            setConstructor(strTotal);
        }

        private void setConstructor(string strTotal)
        {
            lineArray = strTotal.Trim().Split("\r"[0]);
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

        public string GetDataByIndex(int iRow, int iCol)
        {
            if (array.Length <= 0 || iRow >= array.Length)
            {
                return Path + " iRow超出范围";
            }
            if (iCol >= array[0].Length)
            {
                return Path + " iCol超出范围";
            }
            return array[iRow][iCol].Trim();
        }

        public string GetDataByID(string colFirstStr, int iCol)
        {
            if (colFirstStr == null)
            {
                return Path + "colFirstStr为null";
            }
            colFirstStr = colFirstStr.Trim();
            if (array.Length <= 0)
            {
                return Path + "没有数据源";
            }
            if (!dictionary.ContainsKey(colFirstStr))
            {
                return Path + " Key: " + colFirstStr + "不存在";
            }
            if (iCol >= dictionary[colFirstStr].Length)
            {
                return Path + " iCol超出范围";
            }
            string[] rowArr = dictionary[colFirstStr];
            return rowArr[iCol].Trim();
        }

        public int RowLeng { get { return lineArray.Length; } }

        public string Path { get { return resourcePath; } }

    }
}