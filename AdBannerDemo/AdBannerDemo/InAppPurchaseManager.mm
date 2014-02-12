//
//  InAppPurchaseMgr.m
//  TD
//
//  Created by kevinhuang on 12-12-6.
//
//

#import "InAppPurchaseManager.h"
#import "ViewController.h"

#define iOSMainObj @"_iOSMainObj"
//#define USEUnitySendMessage


@implementation InAppPurchaseManager{
    SKPayment *_payment;
}

SYNTHESIZE_SINGLETON_FOR_CLASS(InAppPurchaseManager);

- (id)init
{
    if( (self=[super init]) )
    {
        _transactions = [[NSMutableArray alloc] init];
        [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
        
        isbuyingProduct_ = NO;
        isCancelBuyProduct_ = YES;
    }
    
	return self;
}

- (void)dealloc
{
    [[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
}

- (void)enqueueTransaction:(SKPaymentTransaction *)transaction{
    [_transactions addObject:transaction];
}

- (BOOL)dequeueTransactionByTransactionId:(NSString *)transactionId{
    SKPaymentTransaction *transaction = nil;
    for (SKPaymentTransaction *aTransaction in _transactions) {
        if ([transactionId isEqualToString:aTransaction.transactionIdentifier]) {
            transaction = aTransaction;
            [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
            break;
        }
    }
    
    if (transaction != nil) {
        [_transactions removeObject:transaction];
        return YES;
    }else{
        return NO;
    }
    
}

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex{
    if (buttonIndex == 0) {
        NSLog(@"---------Request payment------------\n");
        // 发送购买请求
        [[SKPaymentQueue defaultQueue] addPayment:_payment];
    }
}

#pragma mark -
#pragma mark 请求商品信息
//
// 请求产品信息
//
- (void)requestProductData:(NSString*)buyProductIDTag
{
    NSAssert([self canMakePay], @"This device doesn't support IAP");
    if(isbuyingProduct_)
    {
        NSLog(@"物品正在购买,请稍后...");
        return;
    }
    
    isbuyingProduct_ = YES;
    
    NSLog(@"---------Request product information------------\n");
    
    //buyProductIDTag_ = [buyProductIDTag retain];
    NSArray* product = [[NSArray alloc] initWithObjects:buyProductIDTag, nil];
    NSSet *nsset = [NSSet setWithArray:product];
    SKProductsRequest *request=[[SKProductsRequest alloc] initWithProductIdentifiers:nsset];
    request.delegate = self;
    [request start]; //向苹果发送请求
}

#pragma mark Delegate (SKProductsRequestDelegate)
// 返回请求结果
- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
    NSLog(@"-----------Getting product information--------------\n");
    
    NSArray *myProduct = response.products;
    NSLog(@"invalid Product IDs:%@\n",response.invalidProductIdentifiers);
    NSLog(@"Product count: %lu\n", (unsigned long)[myProduct count]);
    if([myProduct count] > 0)
    {
#ifdef DEBUG
        // populate UI
        SKProduct *product = [myProduct objectAtIndex:0];
 
            NSLog(@"Detail product info\n");
            NSLog(@"SKProduct description: %@\n", [product description]);
            NSLog(@"Product localized title: %@\n" , product.localizedTitle);
            NSLog(@"Product localized descitption: %@\n" , product.localizedDescription);
            NSLog(@"Product price: %@\n" , product.price);
            NSLog(@"Product identifier: %@\n" , product.productIdentifier);

#endif
        _payment = [SKPayment paymentWithProduct:product];
        
        isCancelBuyProduct_ = NO;
        
        [[[UIAlertView alloc] initWithTitle:nil
                                   message:[NSString stringWithFormat:@"Confirm buying %@ to %@", product.localizedTitle, product.localizedDescription]
                                  delegate:self
                         cancelButtonTitle:@"YES"
                         otherButtonTitles:@"NO", nil] show];
    }else
    {
        NSLog(@"购买失败，请稍后重试！");
        isbuyingProduct_ = NO;
    }
}
#pragma mark Delegate (SKRequestDelegate)
//
// 弹出错误信息
//
- (void)request:(SKRequest *)request didFailWithError:(NSError *)error
{
    NSLog(@"Error:%@, %s", error.localizedDescription, __FUNCTION__);
    [[[UIAlertView alloc] initWithTitle:nil
                               message:error.localizedDescription
                              delegate:nil
                     cancelButtonTitle:@"OK"
                     otherButtonTitles:nil] show];
    isbuyingProduct_ = NO;
}

//
// 请求完成
//
- (void)requestDidFinish:(SKRequest*)request
{
    NSLog(@"----------Request finished--------------\n");
}


#if 1
- (void)purchasedTransaction: (SKPaymentTransaction *)transaction
{
    NSLog(@"-----Purchased Transaction----\n");
    
    NSArray *transactions =[[NSArray alloc] initWithObjects:transaction, nil];
    [self paymentQueue:[SKPaymentQueue defaultQueue] updatedTransactions:transactions];
}
#endif

#pragma mark - 交易结果处理

//
// 交易完成 ()
//
- (void)completeTransaction: (SKPaymentTransaction *)transaction
{
    NSLog(@"------交易完成-------\n");
    
   // NSString *transactionString = [[NSString alloc] initWithData:[transaction transactionReceipt] encoding:NSUTF8StringEncoding];
    
  //  NSLog(@"transactionString:%@",transactionString);
  //  NSLog(@"transation id:%@", transaction.transactionIdentifier);
    
    isbuyingProduct_ = NO;
    
    //处理事务之前
    [self enqueueTransaction:transaction];
    
    //处理事务
    ViewController *viewController = [ViewController currentInstance];
    if (viewController) {
        [viewController removeAdBannerView];
    }else{
        NSLog(@"obtaining viewController encounter error");
    }
    [CommonHelper setPurchasedEliminateAd:YES];
    
    //处理完事务
    [self dequeueTransactionByTransactionId:transaction.transactionIdentifier];
    
//    NSLog(@"成功消息收到，告诉服务器做进一步处理");
}

//
// 交易失败
//
- (void)failedTransaction: (SKPaymentTransaction *)transaction
{
    NSLog(@"-----交易失败--------\n");
    NSString *responseString = transaction.error.localizedDescription;
    isbuyingProduct_ = NO;
    if (transaction.error.code == SKErrorPaymentCancelled) {
        NSLog(@"你已经取消购买");
    }
    
    NSLog(@"Failed error:%@", responseString);

    // 从payment队列中删除交易
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
    
}

//
// 还原购买
//
- (void)restoreTransaction: (SKPaymentTransaction *)transaction
{
    NSLog(@"-----还原购买--------\n");
    // 从payment队列中删除交易
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
}

#pragma mark - Delegate (SKPaymentTransactionObserver)

// Sent when the transaction array has changed (additions or state changes).  Client should check state of transactions and finish as appropriate.
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
    NSLog(@"-----Transaction array 数量/状态发生改变--------");
    
    for (SKPaymentTransaction *transaction in transactions)
    {
        switch (transaction.transactionState)
        {
            case SKPaymentTransactionStatePurchased: // 交易完成
                [self completeTransaction:transaction];
                break;
            case SKPaymentTransactionStateFailed: // 交易失败
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored: // 还原购买
                NSLog(@"SKPaymentTransactionStateRestored");
                [self restoreTransaction:transaction];
                break;
            case SKPaymentTransactionStatePurchasing://正在购买(queue数目发生增加的时候，会执行这个case)
                NSLog(@"SKPaymentTransactionStatePurchasing");
                break;
            default:
                break;
        }
    }
    
    isCancelBuyProduct_ = YES;
}

// Sent when an error is encountered while adding transactions from the user's purchase history back to the queue.
- (void)paymentQueue:(SKPaymentQueue *) paymentQueue restoreCompletedTransactionsFailedWithError:(NSError *)error
{
    NSLog(@"An error is encountered while adding transactions from the user's purchase history back to the queue.\n");
}

// Sent when all transactions from the user's purchase history have successfully been added back to the queue.
- (void)paymentQueueRestoreCompletedTransactionsFinished: (SKPaymentTransaction *)transaction
{
    NSLog(@"All transactions from the user's purchase history have successfully been added back to the queue.\n");
}

#pragma mark - 公共方法

- (void)buy:(NSString*)buyProductIDTag
{
    [self requestProductData:buyProductIDTag];
}

- (BOOL)canMakePay
{
    return [SKPaymentQueue canMakePayments];
}

- (BOOL)canCancelBuy
{
    return isCancelBuyProduct_;
}

@end
