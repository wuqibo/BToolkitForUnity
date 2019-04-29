#import "DeviceID.h"

@implementation DeviceID

+ (NSString *)getDeviceIDInKeychain:(NSString *)appBundleId
{
    NSString *getUDIDInKeychain = (NSString *)[DeviceID load:appBundleId];
    NSLog(@"尝试从keychain中获取UDID: %@",getUDIDInKeychain);
    if (!getUDIDInKeychain || [getUDIDInKeychain isEqualToString:@""] || [getUDIDInKeychain isKindOfClass:[NSNull class]]) {
        CFUUIDRef puuid = CFUUIDCreate(nil);
        CFStringRef uuidString = CFUUIDCreateString(nil, puuid);
        NSString *result = (NSString *)CFBridgingRelease(CFStringCreateCopy(NULL, uuidString));
        CFRelease(puuid);
        CFRelease(uuidString);
        NSLog(@"keychain还没有UUID,存储一个uuid到keychain: %@",result);
        [DeviceID save:appBundleId data:result];
        getUDIDInKeychain = (NSString *)[DeviceID load:appBundleId];
    }
    NSLog(@"返回UUID: %@",getUDIDInKeychain);
    return getUDIDInKeychain;
}

+ (id) load:(NSString *)service {
    id ret  = nil;
    NSMutableDictionary *keychainQuery  = [self getKeyChainQuery:service];
    [keychainQuery setObject:(id)kCFBooleanTrue forKey:(id)kSecReturnData];
    [keychainQuery setObject:(id)kSecMatchLimitOne forKey:(id)kSecMatchLimit];
    CFDataRef keyData   = NULL;
    if (SecItemCopyMatching((CFDictionaryRef)keychainQuery, (CFTypeRef *) &keyData) == noErr) {
        //@try {
            ret = [NSKeyedUnarchiver unarchiveObjectWithData:(__bridge NSData *)keyData];
        //} @catch (NSException *exception) {
        //    NSLog(@"Unarchive of %@ failed: %@",service,exception);
        //} @finally {}
    }
    if (keyData) {
        CFRelease(keyData);
    }
    return ret;
}

+ (void)save:(NSString *)service data:(id)data
{
    // Get search dictionary
    NSMutableDictionary *keychainQuery  = [self getKeyChainQuery:service];
    // Delete old item before add new item
    SecItemDelete((CFDictionaryRef)keychainQuery);
    // Add new object to search dictionary(Attention:the data format)
    [keychainQuery setObject:[NSKeyedArchiver archivedDataWithRootObject:data] forKey:(id)kSecValueData];
    // Add item to keychain with the search dictionary
    SecItemAdd((CFDictionaryRef)keychainQuery, NULL);
}

+ (NSMutableDictionary *)getKeyChainQuery:(NSString *)service
{
    return [NSMutableDictionary dictionaryWithObjectsAndKeys:(id)kSecClassGenericPassword,(id)kSecClass,service,(id)kSecAttrService,service,(id)kSecAttrAccount,(id)kSecAttrAccessibleAfterFirstUnlock,(id)kSecAttrAccessible, nil];
}

@end
