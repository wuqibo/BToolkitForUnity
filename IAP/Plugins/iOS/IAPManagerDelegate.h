typedef NS_ENUM(NSInteger, the_enum_type_name){
    PurchaseRestore = -1,//支付还原
    PurchaseSucceed = 0,//支付成功
    CanNotMakePayment = 1,//APP内支付开关未打开
    RequestProductFailed = 2,//查询商品失败
    PurchaseFailed = 3//支付失败
};

@interface IAPManagerDelegate : NSObject

+ (instancetype)shareManager;

@end
