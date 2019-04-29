#import "AppControllerOverride.h"

IMPL_APP_CONTROLLER_SUBCLASS(AppControllerOverride)//声明程序入口启动AppControllerOverride不再启动UnityAppController(若不生效可手动修改main.mm第33行的UIApplicationMain(argc, argv, nil, @"AppControllerOverride");为启动继承的类)

static NSString *openUrlFor = nil;
@implementation AppControllerOverride
+ (void)setOpenUrlFor:(NSString *)str{
    openUrlFor = str;
}
+ (NSString *)getOpenUrlFor{
    return openUrlFor;
}

- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options{
    if([AppControllerOverride getOpenUrlFor] == OpenUrlFor_WeiXin_Login || [AppControllerOverride getOpenUrlFor] == OpenUrlFor_WeiXin_Pay){
        [[WeiXinAPI instance] application:app openURL:url options:options];
    }else if([AppControllerOverride getOpenUrlFor] == OpenUrlFor_QQ){
        //[[QQAPI instance] application:app openURL:url options:options];
    }else if([AppControllerOverride getOpenUrlFor] == OpenUrlFor_AliPay){
        //[[AlipayAPI instance] application:app openURL:url options:options];
    }
    //此处无需super
    return YES;
}

//>ios8 use
- (void)application:(UIApplication *)application didRegisterUserNotificationSettings:(UIUserNotificationSettings *)notificationSettings{
    [[RemotePush instance] application:application didRegisterUserNotificationSettings:notificationSettings];
    //此处无需super
}

- (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken{
    [super application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
    [[RemotePush instance] application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
}

- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))handler{
    [super application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:handler];
    [[RemotePush instance] application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:handler];
}

@end
