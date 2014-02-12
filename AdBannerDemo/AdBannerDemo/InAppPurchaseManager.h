//
//  InAppPurchaseManager.h
//  TD
//
//  Created by kevinhuang on 12-12-6.
//
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>
#import "ARCSingletonTemplate.h"

@class NSMutabledata;

//
// IAP 支付相关管理类
//
@interface InAppPurchaseManager : NSObject<SKProductsRequestDelegate, SKPaymentTransactionObserver>
{
    // 是否正在购买
    BOOL isbuyingProduct_;
    // 是否能取消购买
    BOOL isCancelBuyProduct_;
}

@property (nonatomic, readonly) NSMutableArray* transactions;

SYNTHESIZE_SINGLETON_FOR_HEADER(InAppPurchaseManager)

// 购买物品
- (void)buy:(NSString*)buyProductIDTag;

// 是否能购买
- (BOOL)canMakePay;

//
// 是否能取消交易
//
- (BOOL)canCancelBuy;

- (BOOL)dequeueTransactionByTransactionId:(NSString *)transactionId;
#pragma mark -
#pragma mark 内部用接口
// 请求商品信息
- (void)requestProductData:(NSString*)buyProductIDTag;
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions;
- (void)purchasedTransaction: (SKPaymentTransaction *)transaction;
- (void)completeTransaction: (SKPaymentTransaction *)transaction;
- (void)failedTransaction: (SKPaymentTransaction *)transaction;
- (void)paymentQueueRestoreCompletedTransactionsFinished: (SKPaymentTransaction *)transaction;
- (void)paymentQueue:(SKPaymentQueue *) paymentQueue restoreCompletedTransactionsFailedWithError:(NSError *)error;
- (void)restoreTransaction: (SKPaymentTransaction *)transaction;
#pragma mark -

@end
