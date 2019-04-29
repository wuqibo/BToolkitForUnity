/*
 以bytes数组的形式保存相片到相册
 Info.plist配置里添加保存到相册的权限
 Privacy - Photo Library Additions Usage Description
 */

@interface SaveImageTool : NSObject
- (void)saveImage:(UIImage *)image;
@end

@implementation SaveImageTool
- (void)saveImage:(UIImage *)image{
    if (image) {
        UIImageWriteToSavedPhotosAlbum(image, self, @selector(image:didFinishSavingWithError:contextInfo:), nil);
    }
}
- (void)image:(UIImage *) image didFinishSavingWithError:(NSError *)error contextInfo: (void *) contextInfo{
}
@end

extern "C" void _SaveImgToPhotosAlbum(const char *imageBytes, int64_t length){
    NSData *data = [NSData dataWithBytesNoCopy:(void *)imageBytes length:length freeWhenDone:NO];
    if (data) {
        SaveImageTool *saveImageTool = [[SaveImageTool alloc] init];
        UIImage *image = [UIImage imageWithData:data];
        [saveImageTool saveImage:image];
    }
}
