#import <AlipaySDK/AlipaySDK.h>

@interface AlipayAPI : NSObject

+ (instancetype)instance;
//在UnityAppController.mm中添加调用
- (void)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options;
@property NSString *callbackGo;
@property NSString *callbackFun;
@end

