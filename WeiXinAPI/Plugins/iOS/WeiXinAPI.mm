#import "WeiXinAPI.h"
#import "AppControllerOverride.h"

@implementation WeiXinAPI
+ (instancetype)instance {
    static WeiXinAPI *_instance = nil;
    if (_instance == nil) {
        _instance = [[WeiXinAPI alloc] init];
    }
    return _instance;
}
- (void)init:(const char *)appId secret:(const char *)secret{
    _appId = [@(appId) copy];
    _secret = [@(secret) copy];
    
    NSLog(@">>>>>>>>>%@",_appId);
    NSLog(@">>>>>>>>>%@",_secret);
    
    [WXApi registerApp:_appId];
}
//登录///////////////
- (void)login:(const char *)callbackGo fun:(const char *)callbackFun{
    [AppControllerOverride setOpenUrlFor:OpenUrlFor_WeiXin_Login];
    _callbackGo = [@(callbackGo) copy];
    _callbackFun = [@(callbackFun) copy];
    [self sendAuthRequest];
}
-(void)sendAuthRequest{
    NSLog(@"%@",@"sendAuthRequest");
    SendAuthReq* req = [[SendAuthReq alloc] init];
    req.scope = @"snsapi_userinfo" ;
    req.state = @"none" ;
    [WXApi sendReq:req];
}
- (void)getTokenByCode:(NSString *)code{
    //Http GET请求
    NSString *urlStr=[NSString stringWithFormat:@"https://api.Weixin.qq.com/sns/oauth2/access_token?appid=%@&secret=%@&code=%@&grant_type=authorization_code",self.appId,self.secret,code];
    NSURL *url=[NSURL URLWithString:urlStr];
    NSURLRequest *request=[NSURLRequest requestWithURL:url];
    NSURLSession *session = [NSURLSession sharedSession];
    NSURLSessionDataTask *dataTask = [session dataTaskWithRequest:request completionHandler:^(NSData * _Nullable data, NSURLResponse * _Nullable response, NSError * _Nullable error) {
        if (error == nil) {
            NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:kNilOptions error:nil];
            NSString *openId = [dict objectForKey:@"openid"];
            NSString *token = [dict objectForKey:@"access_token"];
            [self getUserInfoByToken:openId token:token];
        }
    }];
    [dataTask resume];
}
- (void)getUserInfoByToken:(NSString *)openId token:(NSString *)token{
    //Http GET请求
    NSString *urlStr=[NSString stringWithFormat:@"https://api.Weixin.qq.com/sns/userinfo?openid=%@&access_token=%@",openId,token];
    NSURL *url=[NSURL URLWithString:urlStr];
    NSURLRequest *request=[NSURLRequest requestWithURL:url];
    NSURLSession *session = [NSURLSession sharedSession];
    NSURLSessionDataTask *dataTask = [session dataTaskWithRequest:request completionHandler:^(NSData * _Nullable data, NSURLResponse * _Nullable response, NSError * _Nullable error) {
        if (error == nil) {
            NSString *jsonStr = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
            UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, jsonStr.UTF8String);
        }
    }];
    [dataTask resume];
}
/////////////
//分享
- (void)share:(int)contentType toTarget:(int)toTarget url:(const char *)url title:(const char *)title content:(const char *)content imgLocalPath:(const char *)imgLocalPath{
    [AppControllerOverride setOpenUrlFor:OpenUrlFor_None];
    SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
    
    NSLog(@">>>>>>>>>%d",contentType);
    NSLog(@">>>>>>>>>%d",toTarget);
    NSLog(@">>>>>>>>>%@",[NSString stringWithUTF8String:url]);
    NSLog(@">>>>>>>>>%@",[NSString stringWithUTF8String:title]);
    NSLog(@">>>>>>>>>%@",[NSString stringWithUTF8String:content]);
    NSLog(@">>>>>>>>>%@",[NSString stringWithUTF8String:imgLocalPath]);
    
    if(contentType == 0){
        //赋值
        req.text = [NSString stringWithUTF8String:content];
        req.bText = YES;
    }else if(contentType == 1){
        WXMediaMessage *message = [WXMediaMessage message];
        UIImage *bigImg = [[UIImage alloc]initWithContentsOfFile:[NSString stringWithUTF8String:imgLocalPath]];
        UIImage *thumbImg = [WeiXinAPI thumbWithImage:bigImg size:CGSizeMake(256, 256)];
        //缩略图
        [message setThumbImage:thumbImg];
        //大图
        WXImageObject *imageObject = [WXImageObject object];
        imageObject.imageData = [NSData dataWithContentsOfFile:[NSString stringWithUTF8String:imgLocalPath]];
        message.mediaObject = imageObject;
        //赋值
        req.bText = NO;
        req.message = message;
    }else if(contentType == 2){
        WXMediaMessage *message = [WXMediaMessage message];
        //文字
        message.title = [NSString stringWithUTF8String:title];
        message.description = [NSString stringWithUTF8String:content];
        //缩略图
        UIImage *bigImg = [[UIImage alloc]initWithContentsOfFile:[NSString stringWithUTF8String:imgLocalPath]];
        UIImage *thumbImg = [WeiXinAPI thumbWithImage:bigImg size:CGSizeMake(120, 120)];
        [message setThumbImage:thumbImg];
        //网页信息
        WXWebpageObject *webpageObject = [WXWebpageObject object];
        webpageObject.webpageUrl = [NSString stringWithUTF8String:url];
        message.mediaObject = webpageObject;
        //赋值
        req.bText = NO;
        req.message = message;
    }
    if(toTarget == 0){
        //分享给朋友或群
        req.scene = WXSceneSession;
    }else if(toTarget == 1){
        //分享到朋友圈
        req.scene = WXSceneTimeline;
    }else if(toTarget == 2){
        //添加到微信收藏夹
        req.scene = WXSceneFavorite;
    }
    [WXApi sendReq:req];
}
///////////////支付
- (void)pay:(const char *)partnerId prepayId:(const char *)prepayId package:(const char *)package nonceStr:(const char *)nonceStr timeStamp:(int)timeStamp sign:(const char *)sign go:(const char *)callbackGo fun:(const char *)callbackFun{
    [AppControllerOverride setOpenUrlFor:OpenUrlFor_WeiXin_Pay];
    _callbackGo = [@(callbackGo) copy];
    _callbackFun = [@(callbackFun) copy];
    PayReq *request = [[PayReq alloc] init];
    request.partnerId = [NSString stringWithUTF8String:partnerId];
    request.prepayId= [NSString stringWithUTF8String:prepayId];
    request.package = [NSString stringWithUTF8String:package];
    request.nonceStr= [NSString stringWithUTF8String:nonceStr];
    request.timeStamp = timeStamp;
    request.sign= [NSString stringWithUTF8String:sign];
    [WXApi sendReq:request];
}
/////////////
/////工具/////从大图获取缩略图
+ (UIImage *)thumbWithImage:(UIImage *)image size:(CGSize)asize{
    UIImage *newimage;
    if (nil == image) {
        newimage = nil;
    }else{
        CGSize oldsize = image.size;
        CGRect rect;
        if (asize.width/asize.height > oldsize.width/oldsize.height) {
            rect.size.width = asize.height*oldsize.width/oldsize.height;
            rect.size.height = asize.height;
            rect.origin.x = (asize.width - rect.size.width)/2;
            rect.origin.y = 0;
        }else{
            rect.size.width = asize.width;
            rect.size.height = asize.width*oldsize.height/oldsize.width;
            rect.origin.x = 0;
            rect.origin.y = (asize.height - rect.size.height)/2;
        }
        UIGraphicsBeginImageContext(asize);
        CGContextRef context = UIGraphicsGetCurrentContext();
        CGContextSetFillColorWithColor(context, [[UIColor clearColor] CGColor]);
        UIRectFill(CGRectMake(0, 0, asize.width, asize.height));//clear background
        [image drawInRect:rect];
        newimage = UIGraphicsGetImageFromCurrentImageContext();
        UIGraphicsEndImageContext();
    }
    return newimage;
}
//////

- (void)application:(UIApplication *)application handleOpenURL:(NSURL *)url{
    [WXApi handleOpenURL:url delegate:self];
}
- (void)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options{
    [WXApi handleOpenURL:url delegate:self];
}
- (void)onReq:(BaseReq *)req{
    NSLog(@"%@",@"onReq");
}
- (void)onResp:(BaseResp *)resp{
    NSLog(@">>>>>>>>%@",@"onResp");
    if([AppControllerOverride getOpenUrlFor] == OpenUrlFor_WeiXin_Login){
        if([resp isKindOfClass:[SendAuthResp class]]){
            SendAuthResp *sendAuthResp = (SendAuthResp *)resp;
            NSLog(@">>>>>>>>%@",sendAuthResp.code);
            [self getTokenByCode:sendAuthResp.code];
        }
    }else if([AppControllerOverride getOpenUrlFor] == OpenUrlFor_WeiXin_Pay){
        if([resp isKindOfClass:[PayResp class]]){
            PayResp*response=(PayResp*)resp;
            switch(response.errCode){
                case WXSuccess:
                    //服务器端查询支付通知或查询API返回的结果再提示成功
                    NSLog(@"支付成功");
                    UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, @"success".UTF8String);
                    break;
                default:
                    NSLog(@"支付失败，retcode=%@",resp.errStr);
                    UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, @"".UTF8String);
                    break;
            }
        }
    }
}
@end

//判断微信是否已安装
extern "C" bool _IsWeiXinInstalled(){
    return [WXApi isWXAppInstalled];
}
//用AppId和secret初始化
extern "C" void _WeiXinInit(const char *appId,const char *secret){
    [[WeiXinAPI instance] init:appId secret:secret];
}
//微信登录
extern "C" void _WeiXinLogin(const char *callbackGo,const char *callbackFun){
    [[WeiXinAPI instance] login:callbackGo fun:callbackFun];
}
//分享
extern "C" void _Share(int contentType,int toTarget,const char *url,const char *title,const char *content,const char *imgLocalPath){
    [[WeiXinAPI instance] share:contentType toTarget:toTarget url:url title:title content:content imgLocalPath:imgLocalPath];
}
//支付
extern "C" void _WeiXinPay(const char *partnerId,const char *prepayId,const char *package,const char *nonceStr,int timeStamp,const char *sign,const char *callbackGo,const char *callbackFun){
    [[WeiXinAPI instance] pay:partnerId prepayId:prepayId package:package nonceStr:nonceStr timeStamp:timeStamp sign:sign go:callbackGo fun:callbackFun];
}
