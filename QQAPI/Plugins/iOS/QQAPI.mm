#import "QQAPI.h"
#import "BAppController.h"

@implementation QQAPI
+ (instancetype)instance {
    static QQAPI *_instance = nil;
    if (_instance == nil) {
        _instance = [[QQAPI alloc] init];
    }
    return _instance;
}

- (void)init:(const char *)appId{
    if(_tencentOAuth == NULL){
        _tencentOAuth = [[TencentOAuth alloc] initWithAppId:[NSString stringWithUTF8String:appId] andDelegate:self];
        _tencentOAuth.redirectURI = @"";//填写注册APP时填写的回调域名。默认可以不用填写。建议不用填写
    }
}

//登录///////////////
- (void)login:(const char *)callbackGo fun:(const char *)callbackFun{
    [BAppController setOpenUrlFor:OpenUrlFor_QQ];
    _callbackGo = [@(callbackGo) copy];
    _callbackFun = [@(callbackFun) copy];
    NSArray *permissions = [NSArray arrayWithObjects:kOPEN_PERMISSION_GET_SIMPLE_USER_INFO, nil];
    [_tencentOAuth authorize:permissions inSafari:NO];
}
/////////////

- (void)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options{
    [TencentOAuth HandleOpenURL:url];
}
- (void)tencentDidLogin {
    if (_tencentOAuth.accessToken && 0 != [_tencentOAuth.accessToken length]){
        [_tencentOAuth getUserInfo];
    } else{
        UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, @"".UTF8String);
    }
}
- (void)tencentDidNotLogin:(BOOL)cancelled {
    UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, @"".UTF8String);
}
- (void)tencentDidNotNetWork {
    UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, @"".UTF8String);
}
- (void)getUserInfoResponse:(APIResponse *)response{
    //NSLog(@">>>>>>>>>%d",response.retCode);
    //NSLog(@">>>>>>>>>%@",response.jsonResponse);
    NSDictionary *jsonDic = response.jsonResponse;
    NSString *openId = _tencentOAuth.openId;
    NSString *nickname = [jsonDic objectForKey:@"nickname"];
    NSString *gender = [jsonDic objectForKey:@"gender"];
    NSString *figureurl_2 = [jsonDic objectForKey:@"figureurl_2"];
    NSDictionary * dicNew = [NSDictionary dictionaryWithObjectsAndKeys:openId,@"openid",nickname,@"nickname", gender, @"gender", figureurl_2, @"figureurl_2", nil];
    NSData * jsonData = [NSJSONSerialization dataWithJSONObject:dicNew options:NSJSONWritingPrettyPrinted error:nil];
    NSString * jsonStr = [[NSString alloc]initWithData:jsonData encoding:NSUTF8StringEncoding];
    UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, jsonStr.UTF8String);
}
@end

//判断QQ是否已安装
extern "C" bool _IsQQInstalled(){
    return [TencentOAuth iphoneQQInstalled];
}
//用AppId和secret初始化
extern "C" void _QQInit(const char *appId){
    [[QQAPI instance] init:appId];
}
//QQ登录
extern "C" void _QQLogin(const char *callbackGo,const char *callbackFun){
    [[QQAPI instance] login:callbackGo fun:callbackFun];
}

