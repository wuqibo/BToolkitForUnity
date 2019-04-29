#import <Foundation/Foundation.h>
#import <Security/Security.h>

@interface DeviceID : NSObject

+ (NSString *)getDeviceIDInKeychain:(NSString *)appBundleId;

@end
