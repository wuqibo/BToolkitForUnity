using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

namespace BToolkit
{
    public class ExcelEditor : EditorWindow
    {
        /// <summary>
        /// 当前编辑器窗口实例
        /// </summary>
        private static ExcelEditor instance;

        /// <summary>
        /// Excel文件列表
        /// </summary>
        private static List<string> excelList;

        /// <summary>
        /// 项目根路径	
        /// </summary>
        private static string pathRoot;

        /// <summary>
        /// 转换后保存的目录	
        /// </summary>
        private static string output = "Assets/Resources/Configs";

        /// <summary>
        /// 滚动窗口初始位置
        /// </summary>
        private static Vector2 scrollPos;

        /// <summary>
        /// 输出格式索引
        /// </summary>
        private static int indexOfFormat = 0;

        /// <summary>
        /// 输出格式
        /// </summary>
        private static string[] formatOption = new string[] { "JSON", "CSV", "XML", "LUA" };

        /// <summary>
        /// 编码索引
        /// </summary>
        private static int indexOfEncoding = 0;

        /// <summary>
        /// 编码选项
        /// </summary>
        private static string[] encodingOption = new string[] { "UTF-8", "GB2312" };

        /// <summary>
        /// 是否保留原始文件
        /// </summary>
        private static bool keepSource = true;

        [MenuItem("BToolkit/Excel To/Json")]
        static void SetJsonAndShowExcelTools()
        {
            indexOfFormat = 0;
            ShowExcelUtls();
        }

        [MenuItem("BToolkit/Excel To/XML")]
        static void SetXMLAndShowExcelTools()
        {
            indexOfFormat = 1;
            ShowExcelUtls();
        }

        [MenuItem("BToolkit/Excel To/Csv")]
        static void SetCsvAndShowExcelTools()
        {
            indexOfFormat = 2;
            ShowExcelUtls();
        }

        [MenuItem("BToolkit/Excel To/Lua")]
        static void SetLuaAndShowExcelTools()
        {
            indexOfFormat = 3;
            ShowExcelUtls();
        }
        static void ShowExcelUtls()
        {
            Init();
            //加载Excel文件
            LoadExcel();
            instance.Show();
        }

        void OnGUI()
        {
            DrawOptions();
            DrawExport();
        }

        /// <summary>
        /// 绘制插件界面配置项
        /// </summary>
        private void DrawOptions()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("编码类型:", GUILayout.Width(85));
            indexOfEncoding = EditorGUILayout.Popup(indexOfEncoding, encodingOption, GUILayout.Width(125));
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制插件界面输出项
        /// </summary>
        private void DrawExport()
        {
            if (excelList == null)
                return;
            if (excelList.Count < 1)
            {
                EditorGUILayout.LabelField("目前没有Excel文件被选中哦!");
            }
            else
            {
                EditorGUILayout.LabelField("To " + formatOption[indexOfFormat] + ":");
                GUILayout.BeginVertical();
                scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(80));
                foreach (string s in excelList)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Toggle(true, s);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                EditorGUILayout.LabelField("输出路径:" + output);
                //输出
                if (GUILayout.Button("转换"))
                {
                    Convert();
                }
            }
        }

        /// <summary>
        /// 转换Excel文件
        /// </summary>
        private static void Convert()
        {
            foreach (string assetsPath in excelList)
            {
                //获取Excel文件的绝对路径
                string excelPath = pathRoot + "/" + assetsPath;
                //构造Excel工具类
                ExcelUtility excel = new ExcelUtility(excelPath);

                //判断编码类型
                Encoding encoding = null;
                if (indexOfEncoding == 0 || indexOfEncoding == 3)
                {
                    encoding = Encoding.GetEncoding("utf-8");
                }
                else if (indexOfEncoding == 1)
                {
                    encoding = Encoding.GetEncoding("gb2312");
                }

                //判断输出类型
                string fileName = GetFileNameFormPath(excelPath);
                string path = pathRoot + "/" + output + "/" + fileName;
                string[] arr = path.Split('.');
                string oldValue = "." + arr[arr.Length - 1];
                if (indexOfFormat == 0)
                {
                    path = path.Replace(oldValue, ".json");
                    excel.ConvertToJson(path, encoding);
                }
                else if (indexOfFormat == 1)
                {
                    path = path.Replace(oldValue, ".csv");
                    excel.ConvertToCSV(path, encoding);
                }
                else if (indexOfFormat == 2)
                {
                    path = path.Replace(oldValue, ".xml");
                    excel.ConvertToXml(path);
                }
                else if (indexOfFormat == 3)
                {
                    path = path.Replace(oldValue, ".lua");
                    excel.ConvertToLua(path, encoding);
                }

                //判断是否保留源文件
                if (!keepSource)
                {
                    FileUtil.DeleteFileOrDirectory(excelPath);
                }

                //刷新本地资源
                AssetDatabase.Refresh();
            }

            //转换完后关闭插件
            //这样做是为了解决窗口
            //再次点击时路径错误的Bug
            instance.Close();

        }

        /// <summary>
        /// 从路径里读取文件名
        /// </summary>
        private static string GetFileNameFormPath(string path)
        {
            string[] stringArr = path.Split('/');
            return stringArr[stringArr.Length - 1];
        }

        /// <summary>
        /// 加载Excel
        /// </summary>
        private static void LoadExcel()
        {
            if (excelList == null)
                excelList = new List<string>();
            excelList.Clear();
            //获取选中的对象
            object[] selection = (object[])Selection.objects;
            //判断是否有对象被选中
            if (selection.Length == 0)
                return;
            //遍历每一个对象判断不是Excel文件
            foreach (Object obj in selection)
            {
                string objPath = AssetDatabase.GetAssetPath(obj);
                //旧版的.xls文件不支持
                if (objPath.EndsWith(".xlsx"))
                {
                    excelList.Add(objPath);
                }
            }
        }

        private static void Init()
        {
            //获取当前实例
            instance = EditorWindow.GetWindow<ExcelEditor>();
            //初始化
            pathRoot = Application.dataPath;
            //注意这里需要对路径进行处理
            //目的是去除Assets这部分字符以获取项目目录
            //我表示Windows的/符号一直没有搞懂
            pathRoot = pathRoot.Substring(0, pathRoot.LastIndexOf("/"));
            excelList = new List<string>();
            scrollPos = new Vector2(instance.position.x, instance.position.y + 75);
        }

        void OnSelectionChange()
        {
            //当选择发生变化时重绘窗体
            Show();
            LoadExcel();
            Repaint();
        }
    }
}