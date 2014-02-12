//
//  CommonHelper.h
//  tiantiansanguoba
//
//  Created by yaowan on 13-11-3.
//  Copyright (c) 2013年 bobo. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface CommonHelper : NSObject
+ (void)showTipWithTitle:(NSString *)title msg:(NSString *)msg;
+ (UIActivityIndicatorView *)createdIndicatorAddToSubViewCenterBySubView:(UIView *)subView;
+ (BOOL)purchasedEliminateAd;
+ (void)setPurchasedEliminateAd:(BOOL)purchased;
@end
