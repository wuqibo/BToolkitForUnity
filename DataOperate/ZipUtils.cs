using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

namespace BToolkit
{
    public class ZipUtils
    {

        public static void CreateZipFile(string filesPath, string zipFilePath)
        {
            Debuger.Log("------------------------------");
            if (!File.Exists(filesPath))
            {
                Debuger.LogError("Cannot find directory" + filesPath);
                return;
            }

            try
            {
                string[] filenames = Directory.GetFiles(filesPath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {

                    s.SetLevel(9); // 压缩级别 0-9
                    //s.Password = ""; //压缩密码
                    byte[] buffer = new byte[4096]; //缓冲区大小
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = System.DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }

                File.Delete(filesPath);
            }
            catch (System.Exception ex)
            {
                Debuger.Log("Exception during processing " + ex);
                //TestinAgentHelper.LogHandledException(ex);
            }
        }

        public static bool UnZipFile(string zipFilePath)
        {
            string rootFile = "";
            try
            {
                //读取压缩文件(zip文件)，准备解压缩
                ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath.Trim()));
                //s.Password = "";解压密码
                ZipConstants.DefaultCodePage = 0;//该设置可解决WindowsPlayer平台报错问题

                int index = zipFilePath.LastIndexOf("/");
                string fileDir = zipFilePath.Substring(0, index);//解压出来的文件保存的路径
                string path = fileDir;          //解压出来的文件保存的路径 

                string rootDir = "";            //根目录下的第一个子文件夹的名称

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    rootDir = Path.GetDirectoryName(theEntry.Name);             //得到根目录下的第一级子文件夹的名称
                    if (rootDir.IndexOf("/") >= 0)
                    {
                        rootDir = rootDir.Substring(0, rootDir.IndexOf("/") + 1);
                    }
                    string dir = Path.GetDirectoryName(theEntry.Name);          //根目录下的第一级子文件夹的下的文件夹的名称
                    string fileName = Path.GetFileName(theEntry.Name);          //根目录下的文件名称

                    if (!Directory.Exists(fileDir + "/" + dir))
                    {
                        string path_2 = fileDir + "/" + dir; //在指定的路径创建文件夹 
                        Directory.CreateDirectory(path_2);
                    }
                    if (dir != "" && fileName == "")                            //创建根目录下的子文件夹,不限制级别
                    {
                        if (!Directory.Exists(fileDir + "/" + dir))
                        {
                            path = fileDir + "/" + dir;                        //在指定的路径创建文件夹
                            Directory.CreateDirectory(path);
                        }
                    }
                    else if (dir == "" && fileName != "")                       //根目录下的文件
                    {
                        path = fileDir;
                        rootFile = fileName;
                    }
                    else if (dir != "" && fileName != "")                       //根目录下的第一级子文件夹下的文件
                    {
                        if (dir.IndexOf("/") > 0)                              //指定文件保存的路径
                        {
                            path = fileDir + "/" + dir;
                        }
                    }

                    if (dir == rootDir)                                         //判断是不是需要保存在根目录下的文件
                    {
                        path = fileDir + "/" + rootDir;
                    }

                    //以下为解压缩zip文件的基本步骤
                    //基本思路就是遍历压缩文件里的所有文件，创建一个相同的文件。
                    if (fileName != String.Empty)
                    {
                        FileStream streamWriter = File.Create(path + "/" + fileName);

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        streamWriter.Close();
                    }
                }
                s.Close();

                File.Delete(zipFilePath);
                return true;
            }
            catch (Exception ex)
            {
                Debuger.Log("---------解压出错了！" + ex.Message);
                //TestinAgentHelper.LogHandledException(ex);
                return false;
            }
        }
    }
}