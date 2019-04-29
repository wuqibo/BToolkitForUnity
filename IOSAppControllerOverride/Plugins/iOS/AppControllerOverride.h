#import "UnityAppController.h"
//#import "QQAPI.h"
#import "WeiXinAPI.h"
//#import "AlipayAPI.h"
//#import "RemotePush.h"

NSString * const OpenUrlFor_None = @"OpenUrlFor_None";
NSString * const OpenUrlFor_WeiXin_Login = @"OpenUrlFor_WeiXin_Login";
NSString * const OpenUrlFor_WeiXin_Pay = @"OpenUrlFor_WeiXin_Pay";
NSString * const OpenUrlFor_QQ = @"OpenUrlFor_QQ";
NSString * const OpenUrlFor_AliPay = @"OpenUrlFor_AliPay";

@interface AppControllerOverride : UnityAppController

@property NSString *openUrlFor;

+ (void)setOpenUrlFor:(NSString *)str;
+ (NSString *)getOpenUrlFor;

@end
