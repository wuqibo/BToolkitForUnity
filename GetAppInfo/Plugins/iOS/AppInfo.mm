/*
 读取APP版本号和版本Build号
 */

//该方法要先写，否则下方函数调用不到
extern "C" char* MakeStringCopy_AppInfo(const char* string){
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

extern "C" char* _GetAppPackageName(){
    NSString *bundleId = [[NSBundle mainBundle]bundleIdentifier];
    //返回字符串需要分配内存，否则原有数据内存回收后会报错
    return MakeStringCopy_AppInfo([bundleId UTF8String]);
}

extern "C" char* _GetAppVersionName(){
    NSDictionary *infoDictionary = [[NSBundle mainBundle] infoDictionary];
    NSString *version = [infoDictionary objectForKey:@"CFBundleShortVersionString"];
    //返回字符串需要分配内存，否则原有数据内存回收后会报错
    return MakeStringCopy_AppInfo([version UTF8String]);
}

extern "C" int _GetAppVersionCode(){
    NSDictionary *infoDictionary = [[NSBundle mainBundle] infoDictionary];
    NSString *build = [infoDictionary objectForKey:@"CFBundleVersion"];
    return [build intValue];
}
