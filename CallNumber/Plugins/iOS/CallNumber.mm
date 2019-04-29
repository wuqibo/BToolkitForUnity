/*
 拨打电话
 */
extern "C" void _CallNumber(const char *number){
    NSString *nsNumber = [NSString stringWithUTF8String: number];
    NSString *callStr = [NSString stringWithFormat:@"%@%@", @"tel://", nsNumber ];
    [[UIApplication sharedApplication]openURL:[NSURL URLWithString:callStr]];
}
