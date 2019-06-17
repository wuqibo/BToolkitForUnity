using BToolkit;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager
{

    private static StorageManager instance;
    public static StorageManager Instance { get { return instance ?? (instance = new StorageManager()); } }
    private StorageManager() { }

    private string FilePath = Application.persistentDataPath + "/ScanRecord.json";

    /// <summary>
    /// 读取扫描记录(针对本地AR)
    /// </summary>
    public RecordModel ReadLocalARScanedRecords()
    {
        try
        {
            string content = Storage.ReadFileToString(FilePath);
            return JsonMapper.ToObject<RecordModel>(content);
        }
        catch (Exception e)
        {
            Debuger.LogError(e);
            return null;
        }
    }

    /// <summary>
    /// 读取扫描记录(针对云AR)
    /// </summary>
    public string ReadCloudARScanedRecords()
    {
        try
        {
            return Storage.ReadFileToString(FilePath);
        }
        catch (Exception e)
        {
            Debuger.LogError(e);
            return null;
        }
    }

    /// <summary>
    /// 添加扫描记录
    /// </summary>
    public void AddLocalARScanedRecord(RecordSaver recordSaver)
    {
        try
        {
            RecordModel recordModel = null;
            string content = Storage.ReadFileToString(FilePath);
            if (content != null)
            {
                recordModel = JsonMapper.ToObject<RecordModel>(content);
                for (int i = 0; i < recordModel.targetName.Length; i++)
                {
                    if (recordModel.targetName[i].Equals(recordSaver.imgName))
                    {
                        recordModel.RemoveAt(i);
                    }
                }
            }
            else
            {
                recordModel = new RecordModel();
            }
            recordModel.AddToIndex0(recordSaver.imgName, (int)recordSaver.showType, recordSaver.showName);
            Storage.SaveStringToFile(FilePath, JsonMapper.ToJson(recordModel));
        }
        catch (Exception err)
        {
            Debuger.LogError(err);
        }
    }

    /// <summary>
    /// 添加扫描记录
    /// </summary>
    public void AddCloudARScanedRecord(string targetId)
    {
        int MaxRecord = 20;
        try
        {
            string content = Storage.ReadFileToString(FilePath);
            if (content != null)
            {
                List<string> idList = new List<string>(content.Split('|'));
                for (int i = 0; i < idList.Count; i++)
                {
                    if (targetId.Equals(idList[i]))
                    {
                        if (i == 0)
                        {
                            return;//如果所要加的id已经在第一位，则不错任何处理
                        }
                        idList.RemoveAt(i);
                        break;
                    }
                }
                idList.Insert(0, targetId);
                if (idList.Count > MaxRecord)
                {
                    idList.RemoveAt(idList.Count - 1);
                }
                string newContent = "";
                int count = idList.Count;
                for (int i = 0; i < count; i++)
                {
                    newContent += idList[i];
                    if (i < count-1)
                    {
                        newContent += "|";
                    }
                }
                Storage.SaveStringToFile(FilePath, newContent);
            }
            else
            {
                Storage.SaveStringToFile(FilePath, targetId);
            }

        }
        catch (Exception err)
        {
            Debuger.LogError(err);
        }
    }

    /// <summary>
    /// 脱卡后的显示方式
    /// </summary>
    public bool IsARHideWhenOffCard
    {
        get
        {
            return PlayerPrefs.GetInt("OffCard", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("OffCard", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
