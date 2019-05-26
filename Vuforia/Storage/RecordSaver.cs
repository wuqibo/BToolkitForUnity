using BToolkit;
using UnityEngine;

public class RecordSaver : BImageTarget {

    public enum ShowType { Video, Model }

    public ShowType showType;
    public string imgName;
    public string showName;

    protected override void OnTrackingFound()
    {
        StorageManager.Instance.AddScanedRecord(this);
    }

    protected override void OnTrackingLost()
    {
        
    }
}
