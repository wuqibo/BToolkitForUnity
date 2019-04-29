#import "IAPManagerDelegate.h"
#import <StoreKit/StoreKit.h>

@interface IAPManagerDelegate()<SKPaymentTransactionObserver,SKProductsRequestDelegate>
@property (nonatomic, copy) NSString *receipt;//交易成功后拿到的一个64编码字符串,完整写法要拿这个去苹果服务器二次校验
@property NSString *callbackGoForRestore;
@property NSString *callbackFunForRestore;
@property NSString *callbackGo;
@property NSString *callbackFun;

@end

@implementation IAPManagerDelegate

+ (instancetype)shareManager {
    static IAPManagerDelegate *_instance = nil;
    if (_instance == nil) {
        _instance = [[IAPManagerDelegate alloc] init];
    }
    return _instance;
}

- (void)init:(const char *)callbackGo fun:(const char *)callbackFun{
    _callbackGoForRestore = [@(callbackGo) copy];
    _callbackFunForRestore = [@(callbackFun) copy];
    //注册回调paymentQueue
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
    //读取以支付过可以直接恢复购买的非消耗产品
    [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

- (void)purchase:(const char *)productId go:(const char *)callbackGo fun:(const char *)callbackFun{
    _callbackGo = [@(callbackGo) copy];
    _callbackFun = [@(callbackFun) copy];
    //判断应用是否允许内购买
    if (![SKPaymentQueue canMakePayments]) {
        NSLog(@"IAP>>>>>>>>APP内支付开关未打开");
        [self sendToUnity:CanNotMakePayment];
        return;
    }
    //检测队列里是否有未完成的交易，有则关闭
    NSArray* transactions = [SKPaymentQueue defaultQueue].transactions;
    NSLog(@"1>>>> transactions.count:%d",(int)transactions.count);
    if (transactions.count > 0) {
        SKPaymentTransaction* transaction = [transactions firstObject];
        if (transaction.transactionState == SKPaymentTransactionStatePurchased) {
            [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
            NSLog(@"IAP>>>>>>>>队列里是否有未完成的交易");
            return;
        }
    }
    NSLog(@"2>>>> transactions.count:%d",(int)transactions.count);
    //请求产品列表信息
    NSString *productIdNSString = [NSString stringWithUTF8String:productId];
    NSSet *productIdNSSet = [NSSet setWithObject: productIdNSString];
    SKProductsRequest *request= [[SKProductsRequest alloc] initWithProductIdentifiers:productIdNSSet];
    request.delegate = self;
    [request start];
}

//查询失败后的回调
- (void)request:(SKRequest *)request didFailWithError:(NSError *)error {
    NSLog(@"IAP>>>>>>>>查询商品失败");
    [self sendToUnity:RequestProductFailed];
}

//请求产品列表信息系统回调
- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
    NSLog(@"IAP>>>>>>>>无效的ProductId:%@",response.invalidProductIdentifiers);
    NSLog(@"IAP>>>>>>>>Product Count:%d", (int)[response.products count]);
    for (SKProduct *product in response.products) {
        NSLog(@"IAP--------product------------------");
        NSLog(@"IAP>>>>>>>>Product description:%@",[product description]);
        NSLog(@"IAP>>>>>>>>Product localized title:%@",[product localizedTitle]);
        NSLog(@"IAP>>>>>>>>Product localized description:%@",[product localizedDescription]);
        NSLog(@"IAP>>>>>>>>Product price:%@",product.price);
        NSLog(@"IAP>>>>>>>>Product identifier:%@",product.productIdentifier);
    }
    if (response.products.count > 0) {
        //发起购买请求
        NSLog(@"IAP>>>>>>>>Product description:%@",@"发起购买请求");
        SKPayment *payment = [SKPayment paymentWithProduct:response.products[0]];
        [[SKPaymentQueue defaultQueue] addPayment:payment];
    } else {
        NSLog(@"IAP>>>>>>>>获取商品信息个数为0");
        [self sendToUnity:RequestProductFailed];
    }
}

//购买操作后的回调
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(nonnull NSArray<SKPaymentTransaction *> *)transactions {
    NSLog(@"IAP>>>>>>>>>>>>>>>>>>>>>>>paymentQueue");
    for (SKPaymentTransaction *transaction in transactions) {
        switch (transaction.transactionState) {
            case SKPaymentTransactionStatePurchasing://正在交易
            {
                NSLog(@"IAP>>>>>>>>paymentQueue:%@",@"正在交易");
            }
                break;
            case SKPaymentTransactionStatePurchased://交易完成
            {
                NSLog(@"IAP>>>>>>>>paymentQueue:%@",@"交易完成");
                //获取交易成功后的购买凭证
                NSURL *receiptUrl = [[NSBundle mainBundle] appStoreReceiptURL];
                NSData *receiptData = [NSData dataWithContentsOfURL:receiptUrl];
                self.receipt = [receiptData base64EncodedStringWithOptions:0];
                //[self saveReceipt]; //存储交易凭证
                //[self checkIAPFiles];//把self.receipt发送到服务器验证是否有效
                //结束交易
                [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
                [self sendToUnity:PurchaseSucceed];
            }
                break;
            case SKPaymentTransactionStateFailed://交易失败
            {
                NSLog(@"IAP>>>>>>>>paymentQueue:%@",@"交易失败");
                //结束交易
                [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
                [self sendToUnity:PurchaseFailed];
            }
                break;
            case SKPaymentTransactionStateRestored://已经购买过该商品
            {
                NSLog(@"IAP>>>>>>>>paymentQueue:%@",@"已经购买过该商品");
                //结束交易
                [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
                [self sendToUnity:PurchaseRestore];
            }
                break;
            default:
                //结束交易
                [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
                [self sendToUnity:PurchaseFailed];
                break;
        }
    }
}

- (void)restore{
    [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

- (void) paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue
{
    NSLog(@"IAP>>>>>>>>received restored transactions: %d", (int)queue.transactions.count);
    NSString *resultStr = @"";
    for (SKPaymentTransaction *transaction in queue.transactions)
    {
        NSString *productID = transaction.payment.productIdentifier;
        NSLog(@"IAP>>>>>>>>restored productID:%@",productID);
        resultStr = [NSString stringWithFormat:@"%@%@%@",resultStr,@"|",productID];
    }
    [self sendRestoreProductIdsToUnity:resultStr];
}

- (void)sendRestoreProductIdsToUnity:(NSString *)resultStr{
    if(self.callbackGoForRestore!=nil && self.callbackFunForRestore!=nil){
        NSLog(@"IAP>>>>>>>>sendToUnityForRestore:%@",resultStr);
        UnitySendMessage(self.callbackGoForRestore.UTF8String, self.callbackFunForRestore.UTF8String, resultStr.UTF8String);
    }
}

- (void)sendToUnity:(int)result{
    if(self.callbackGo!=nil && self.callbackFun!=nil){
        NSString *resultStr = [NSString stringWithFormat:@"%d",result];
        NSLog(@"IAP>>>>>>>>sendToUnity:%@",resultStr);
        UnitySendMessage(self.callbackGo.UTF8String, self.callbackFun.UTF8String, resultStr.UTF8String);
    }
}

-(void)dealloc{
    //注销paymentQueue监听
    [[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
}

extern "C" void Init(const char *callbackGo,const char *callbackFun){
    [[IAPManagerDelegate shareManager] init:callbackGo fun:callbackFun];
}

extern "C" void Purchase(const char *productId,const char *callbackGo,const char *callbackFun){
    [[IAPManagerDelegate shareManager] purchase:productId go:callbackGo fun:callbackFun];
}

extern "C" void Restore(){
    [[IAPManagerDelegate shareManager] restore];
}

@end
