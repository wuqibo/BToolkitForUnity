#import "Reachability.h"
#import "DeviceID.h"

//该方法要先写，否则下方函数调用不到
extern "C" char* MakeStringCopy_SystemInfo(const char* string){
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

//获取当前电量进度（0-1）
extern "C" float _GetBatteryProgress(){
    [[UIDevice currentDevice] setBatteryMonitoringEnabled:YES];
    return [[UIDevice currentDevice] batteryLevel];
}

//获取当前网络类型
extern "C" char* _GetInternetStatus(){
    Reachability *reachability = [Reachability reachabilityWithHostName:@"www.baidu.com"];
    NetworkStatus internetStatus = [reachability currentReachabilityStatus];
    NSString *net = @"NoNet";
    switch (internetStatus) {
        case ReachableViaWiFi:
            net = @"Wifi";
            break;
        case ReachableViaWWAN:
            net = @"CellularNetwork";
            break;
        case NotReachable:
            net = @"NoNet";
        default:
            break;
    }
    //返回字符串需要分配内存，否则原有数据内存回收后会报错
    return MakeStringCopy_SystemInfo([net UTF8String]);
}

//获取设备唯一识别码
extern "C" char* _GetDeviceId(const char* appBundleId){
    NSString *_appBundleId = [NSString stringWithUTF8String:appBundleId];
    NSString *deviceId = [DeviceID getDeviceIDInKeychain:_appBundleId];
    return MakeStringCopy_SystemInfo([deviceId UTF8String]);
}
