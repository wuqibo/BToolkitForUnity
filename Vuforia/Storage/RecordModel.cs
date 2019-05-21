using System.Collections.Generic;

public class RecordModel
{

    public string[] targetName;
    public int[] showType;
    public string[] showName;

    public RecordSaver ToRecordSaver(int index)
    {
        RecordSaver recordSaver = new RecordSaver();
        recordSaver.targetName = targetName[index];
        recordSaver.showType = (RecordSaver.ShowType)showType[index];
        recordSaver.showName = showName[index];
        return recordSaver;
    }

    /// <summary>
    /// 添加到索引0
    /// </summary>
    public void AddToIndex0(string targetName, int showType, string showName)
    {
        if (this.targetName != null)
        {
            List<string> targetNames = new List<string>(this.targetName);
            targetNames.Insert(0, targetName);
            this.targetName = targetNames.ToArray();

            List<int> showTypes = new List<int>(this.showType);
            showTypes.Insert(0, showType);
            this.showType = showTypes.ToArray();

            List<string> showNames = new List<string>(this.showName);
            showNames.Insert(0, showName);
            this.showName = showNames.ToArray();
        }
        else
        {
            this.targetName = new string[] { targetName };
            this.showType = new int[] { showType };
            this.showName = new string[] { showName };
        }
    }

    /// <summary>
    /// 删除某个索引上的数据
    /// </summary>
    public void RemoveAt(int index)
    {
        List<string> targetNames = new List<string>(this.targetName);
        targetNames.RemoveAt(index);
        this.targetName = targetNames.ToArray();

        List<int> showTypes = new List<int>(this.showType);
        showTypes.RemoveAt(index);
        this.showType = showTypes.ToArray();

        List<string> showNames = new List<string>(this.showName);
        showNames.RemoveAt(index);
        this.showName = showNames.ToArray();
    }

}
