using BToolkit;

public class RecordSaver : BImageTarget {

    public enum ShowType { Video, Model }

    public ShowType showType;
    public string imgName;
    public string showName;

    protected override void OnTrackingFound()
    {
        StorageManager.Instance.AddLocalARScanedRecord(this);
    }

    protected override void OnTrackingLost()
    {
        
    }
}
