using BToolkit;
using LitJson;
using System;
using UnityEngine;

public class StorageManager
{

    private static StorageManager instance;
    public static StorageManager Instance { get { return instance ?? (instance = new StorageManager()); } }
    private StorageManager() { }

    private string FilePath = Application.persistentDataPath + "/ScanRecord.json";

    /// <summary>
    /// 读取扫描记录
    /// </summary>
    public RecordModel ReadScanedRecords()
    {
        try
        {
            string content = Storage.ReadFileToString(FilePath);
            return JsonMapper.ToObject<RecordModel>(content);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    /// <summary>
    /// 添加扫描记录
    /// </summary>
    public void AddScanedRecord(RecordSaver recordSaver)
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
    /// 脱卡后的显示方式
    /// </summary>
    public bool IsARHideWhenOffCard
    {
        get
        {
            return PlayerPrefs.GetInt("OffCard", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("OffCard", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
