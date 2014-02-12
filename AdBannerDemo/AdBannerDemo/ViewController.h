//
//  ViewController.h
//  AdBannerDemo
//
//  Created by yaowan on 13-11-23.
//  Copyright (c) 2013年 bobo. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <iAd/iAd.h>
@interface ViewController : UIViewController <ADBannerViewDelegate>
+ (ViewController *)currentInstance;
- (void)removeAdBannerView;
@end
