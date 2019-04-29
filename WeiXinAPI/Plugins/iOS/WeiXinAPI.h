#import "WXApi.h"

@interface WeiXinAPI : NSObject<WXApiDelegate>

+ (instancetype)instance;
//在AppControllerOverride.mm中添加调用
- (void)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options;

@property NSString *callbackGo;
@property NSString *callbackFun;
@property NSString *appId;
@property NSString *secret;

@end
