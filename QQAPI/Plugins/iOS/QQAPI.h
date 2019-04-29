#import <UIKit/UIKit.h>
#import "TencentOpenAPI/TencentOAuth.h"

@interface QQAPI : NSObject<TencentSessionDelegate>

+ (instancetype)instance;
//在UnityAppController.mm中添加调用
- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options;

@property TencentOAuth *tencentOAuth;
@property NSString *callbackGo;
@property NSString *callbackFun;
@end
