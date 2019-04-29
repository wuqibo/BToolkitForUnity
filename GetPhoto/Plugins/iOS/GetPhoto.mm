/*
 通过拍照获取照片或者直接获取相册里的照片
 */
@interface GetPhotoTool : UIViewController<UIImagePickerControllerDelegate,UINavigationControllerDelegate>
@property NSString *saveFullPath;
@property NSString *callbackGo;
@property NSString *callbackFun;
@end

#import <AVFoundation/AVFoundation.h>
@implementation GetPhotoTool
//公用方法
-(void)getPhoto:(const char *)saveFullPath callbackGameObject:(const char *)callbackGo callbackFunction:(const char *)callbackFun actionIndex:(NSInteger)index{
    _saveFullPath = [@(saveFullPath) copy];
    _callbackGo = [@(callbackGo) copy];
    _callbackFun = [@(callbackFun) copy];
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    switch (index) {
        case 0:
            if (![UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera]) {
               UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, (@"").UTF8String);
                return;
            }
            picker.sourceType = UIImagePickerControllerSourceTypeCamera;
            break;
        case 1:
            if (![UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeSavedPhotosAlbum]) {
                UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, (@"").UTF8String);
                return;
            }
            picker.sourceType = UIImagePickerControllerSourceTypeSavedPhotosAlbum;
            break;
        case 2:
            if (![UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypePhotoLibrary]) {
                UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, (@"").UTF8String);
                return;
            }
            picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
            break;
    }
    picker.delegate = self;
    picker.allowsEditing = YES;
    UIViewController *vc = UnityGetGLViewController();
    [vc.view addSubview: self.view];
    [self presentViewController:picker animated:YES completion:nil];
}
// 打开相册后选择照片时的响应方法(系统调用)
- (void)imagePickerController:(UIImagePickerController*)picker didFinishPickingMediaWithInfo:(NSDictionary*)info{
    NSString *key = nil;
    if (picker.allowsEditing) {
        key = UIImagePickerControllerEditedImage;
    } else {
        key = UIImagePickerControllerOriginalImage;
    }
    //获取图片
    UIImage *image = [info objectForKey:key];
    NSData *imageData = UIImageJPEGRepresentation(image, 1.0f);
    if ([imageData writeToFile:self.saveFullPath atomically:YES]) {
        UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, self.saveFullPath.UTF8String);
    }else{
        UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, (@"").UTF8String);
    }
    // 关闭相册
    [picker dismissViewControllerAnimated:YES completion:nil];
}
//检查相机是否可用
- (BOOL)checkCamera {
    NSString *mediaType = AVMediaTypeVideo;
    AVAuthorizationStatus authStatus = [AVCaptureDevice authorizationStatusForMediaType:mediaType];
    if(AVAuthorizationStatusRestricted == authStatus || AVAuthorizationStatusDenied == authStatus) {
        return NO;
    }
    return YES;
}
@end

extern "C" void _GetPhotoFromCamera(char* saveFullPath,char* callbackGo,char* callbackFun ){
    GetPhotoTool * getPhotoTool = [[GetPhotoTool alloc] init];
    [getPhotoTool getPhoto:saveFullPath callbackGameObject:callbackGo callbackFunction:callbackFun actionIndex:0];
}
extern "C" void _GetPhotoFromAlbums(char* saveFullPath,char* callbackGo,char* callbackFun ){
    GetPhotoTool * getPhotoTool = [[GetPhotoTool alloc] init];
    [getPhotoTool getPhoto:saveFullPath callbackGameObject:callbackGo callbackFunction:callbackFun actionIndex:1];
}
extern "C" void _GetPhotoFromLibrary(char* saveFullPath,char* callbackGo,char* callbackFun ){
    GetPhotoTool * getPhotoTool = [[GetPhotoTool alloc] init];
    [getPhotoTool getPhoto:saveFullPath callbackGameObject:callbackGo callbackFunction:callbackFun actionIndex:2];
}
