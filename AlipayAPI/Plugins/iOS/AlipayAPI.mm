#import "AlipayAPI.h"
#import "BAppController.h"

@implementation AlipayAPI
+ (instancetype)instance {
    static AlipayAPI *_instance = nil;
    if (_instance == nil) {
        _instance = [[AlipayAPI alloc] init];
    }
    return _instance;
}
/////////////
//支付
- (void)pay:(const char *)orderInfo go:(const char *)callbackGo fun:(const char *)callbackFun{
    [BAppController setOpenUrlFor:OpenUrlFor_AliPay];
    _callbackGo = [@(callbackGo) copy];
    _callbackFun = [@(callbackFun) copy];
    //应用注册scheme,在AliSDKDemo-Info.plist定义URL types
    NSString *appScheme = @"alipay2017102509522622";
    // NOTE: 调用支付结果开始支付
    [[AlipaySDK defaultService] payOrder:[NSString stringWithUTF8String:orderInfo] fromScheme:appScheme callback:^(NSDictionary *resultDic) {
        NSLog(@"payOrder:%@",resultDic);
        [self executeCallback:resultDic];
    }];
}
/////////////
- (void)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options{
    NSLog(@"--------%@",url.host);
    if ([url.host isEqualToString:@"safepay"]) {
        // 支付跳转支付宝钱包进行支付，处理支付结果
        [[AlipaySDK defaultService] processOrderWithPaymentResult:url standbyCallback:^(NSDictionary *resultDic) {
            NSLog(@"processOrderWithPaymentResult:%@",resultDic);
            [self executeCallback:resultDic];
        }];
        
        // 授权跳转支付宝钱包进行支付，处理支付结果
        [[AlipaySDK defaultService] processAuth_V2Result:url standbyCallback:^(NSDictionary *resultDic) {
            NSLog(@"processAuth_V2Result:%@",resultDic);
            [self executeCallback:resultDic];
        }];
    }
}
- (void)executeCallback:(NSDictionary *) resultDic{
    NSString *resultStr = [self dataToJsonString:resultDic];
    NSLog(@"resultStr:%@",resultStr);
    if(resultStr != nil){
        UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, resultStr.UTF8String);
    }else{
        UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, @"".UTF8String);
    }
}
- (NSString*)dataToJsonString:(id)object{
    NSString *jsonString = nil;
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:object
                                                       options:NSJSONWritingPrettyPrinted // Pass 0 if you don't care about the readability of the generated string
                                                         error:&error];
    if (! jsonData) {
        NSLog(@"Got an error: %@", error);
    } else {
        jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
    return jsonString;
}
@end

//判断支付宝是否已安装
extern "C" bool _IsAliayInstalled(){
    return true;
    //NSURL * myURL_APP_A = [NSURL URLWithString:@"alipay:"];
    //return [[UIApplication sharedApplication] canOpenURL:myURL_APP_A];
}

//支付
extern "C" void _AliPay(const char *orderInfo,const char *callbackGo,const char *callbackFun){
    [[AlipayAPI instance] pay:orderInfo go:callbackGo fun:callbackFun];
}
