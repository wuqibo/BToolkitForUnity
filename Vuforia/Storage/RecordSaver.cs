using BToolkit;
using UnityEngine;

public class RecordSaver : BImageTarget {

    public enum ShowType { Video, Model }

    public ShowType showType;
    public string targetName;
    public string showName;

    protected override void OnTrackingFound()
    {
        StorageManager.Instance.AddScanedRecord(this);
    }

    protected override void OnTrackingLost()
    {
        
    }
}
