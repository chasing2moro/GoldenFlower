//
//  ViewController.m
//  AdBannerDemo
//
//  Created by yaowan on 13-11-23.
//  Copyright (c) 2013年 bobo. All rights reserved.
//

#import "ViewController.h"
#import "InAppPurchaseManager.h"
#define kEliminateADProductID @"com.yaowan.tiantiansanguoba.eliminateAD"
#define animatedDuration 0.5
#define kEliminateADButtonTag 2000
#define kEliminateADButtonWidth 20

static ViewController *_instance;

@interface ViewController ()
@property (weak, nonatomic) IBOutlet UIView *contentView;
@property (weak, nonatomic) IBOutlet UITextView *textView;
@property (weak, nonatomic) IBOutlet UILabel *timerLabel;

@end

@implementation ViewController {
    ADBannerView *_bannerView;
    NSTimer *_timer;
    CFTimeInterval _ticks;
}
#pragma mark - 初始化方法
- (void)_initializeTextView{
    static dispatch_once_t once;
    dispatch_once(&once, ^{
        NSData *ipsumData = [NSData dataWithContentsOfURL:[[NSBundle mainBundle] URLForResource:@"ipsums" withExtension:@"plist"]
                                                  options:NSDataReadingMappedIfSafe
                                                    error:nil];
        NSDictionary *ipsumDic = [NSPropertyListSerialization propertyListWithData:ipsumData
                                                                           options:NSPropertyListImmutable
                                                                            format:nil
                                                                             error:nil];
        self.title = NSLocalizedString(@"Original", @"Original");
        self.textView.text = ipsumDic[@"Original"];
    });
}

- (void)_initializeADBanner{
    static dispatch_once_t once;
    dispatch_once(&once, ^{
        if ([ADBannerView instancesRespondToSelector:@selector(initWithAdType:)]){
            _bannerView = [[ADBannerView alloc] initWithAdType:ADAdTypeBanner];
        }else{
            _bannerView = [[ADBannerView alloc] init];
        }
        
        _bannerView.delegate = self;
        [self.view addSubview:_bannerView];
    });
}

#pragma mark - 继承方法

- (void)viewDidLoad{
    [super viewDidLoad];
	
    _instance = self;
    
    //初始化 textView
    [self _initializeTextView];
    
     if (![CommonHelper purchasedEliminateAd])
         //初始化 广告
         [self _initializeADBanner];
}

- (void)viewDidAppear:(BOOL)animated{
    [super viewDidAppear:animated];
    [self layoutAdBannerWithAnimated:NO];
    [self _startTimer];
}

- (void)viewDidDisappear:(BOOL)animated{
    [super viewDidDisappear:animated];
    [self _stopTimer];
}

- (void)didReceiveMemoryWarning{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (void)touchesEnded:(NSSet *)touches withEvent:(UIEvent *)event{
  //  [self removeAdBannerView];
    [[InAppPurchaseManager sharedInAppPurchaseManager] buy:kEliminateADProductID];
}

- (void)viewDidLayoutSubviews{
    [super viewDidLayoutSubviews];
    [self layoutAdBannerWithAnimated:[UIView areAnimationsEnabled]];
}
#pragma mark - 公共方法
- (void)removeAdBannerView{
    if (_bannerView) {
        [_bannerView removeFromSuperview];
    }
    
    UIView *buttonView = [self.view viewWithTag:kEliminateADButtonTag];
    if (buttonView) {
        [buttonView removeFromSuperview];
    }
    [self layoutAdBannerWithAnimated:NO];
}

+ (ViewController *)currentInstance{
    return _instance;
}
#pragma mark
- (void)layoutAdBannerWithAnimated:(BOOL)animated{
    CGRect contentViewFrame = self.view.bounds;
    
    //已经购买
    if ([CommonHelper purchasedEliminateAd]) {
        NSLog(@"已经购买");
        _contentView.frame = contentViewFrame;
        return;
    }
    //兼容iOS6 以前 
        if (contentViewFrame.size.width > contentViewFrame.size.height)
            _bannerView.currentContentSizeIdentifier = ADBannerContentSizeIdentifierLandscape; //landscape
        else
            _bannerView.currentContentSizeIdentifier = ADBannerContentSizeIdentifierPortrait;//portrait
    
    CGRect bannerViewFrame = _bannerView.frame;
    
    //设置布局Frame
    if (_bannerView.bannerLoaded) {
        contentViewFrame.size.height -= bannerViewFrame.size.height;
        bannerViewFrame.origin.y = contentViewFrame.size.height;
    }else{
        bannerViewFrame.origin.y = contentViewFrame.size.height;
    }
    
    CGRect bannerViewUnderLine = bannerViewFrame;
    bannerViewUnderLine.origin.y = self.view.bounds.size.height;
    
    [UIView animateWithDuration:animated ? animatedDuration : 0.0
                     animations:^{
                         _contentView.frame = contentViewFrame;
                         [_contentView layoutIfNeeded];
                         _bannerView.frame = bannerViewFrame;
                     }
                     completion:^(BOOL finished) {
                         
                         //动态添加一个按钮 (添加到bannerView上，会有问题，不信的不妨试一下)
                         if (_bannerView.bannerLoaded) {
                             static dispatch_once_t once;
                             dispatch_once(&once, ^{
                                 //NSLog(@"动态添加一个按钮");
                                 CGRect frame = _bannerView.frame;
                                 frame.size = CGSizeMake(kEliminateADButtonWidth, kEliminateADButtonWidth);
                                 UIButton *button = [UIButton buttonWithType:UIButtonTypeRoundedRect];
                                 button.frame = frame;
                                 [button setTitle:@"X" forState:UIControlStateNormal];
                                 button.backgroundColor = [UIColor colorWithRed:0 green:1 blue:0 alpha:0.5];
                                 button.tag = kEliminateADButtonTag;
                                 [button addTarget:self action:@selector(eliminateADButtonClicked:) forControlEvents:UIControlEventTouchUpInside];
                                 [self.view addSubview:button];
                             });
                         }

                         //显示、隐藏一个按钮
                         UIView *button = [self.view viewWithTag:kEliminateADButtonTag];
                         if (button) {
                             //NSLog(@"显示、隐藏一个按钮");
                             button.hidden = !_bannerView.bannerLoaded;
                         }

                     }
     ];
    
    
    
}

- (void)eliminateADButtonClicked:(id)sender{
    [[InAppPurchaseManager sharedInAppPurchaseManager] buy:kEliminateADProductID];
}

#pragma mark - 时间计时
- (void)_startTimer{
    if (_timer == nil) {
        _timer = [NSTimer scheduledTimerWithTimeInterval:0.1
                                                  target:self
                                                selector:@selector(_timerTick:)
                                                userInfo:nil
                                                 repeats:YES];
    }
}

- (void)_stopTimer{
    [_timer invalidate];
    _timer = nil;
}

- (void)_timerTick:(NSTimer *)timer{
    // Timers are not guaranteed to tick at the nominal rate specified, so this isn't technically accurate.
    // However, this is just an example to demonstrate how to stop some ongoing activity, so we can live with that inaccuracy.
    
    //TRUNC(number,num_digits)
    //Number 需要截尾取整的数字。
    //Num_digits 用于指定取整精度的数字。Num_digits 的默认值为 0。
    _ticks += 0.1;
    double seconds = fmod(_ticks, 60.0);
    double minutes = fmod(trunc(_ticks / 60.0), 60.0);
    double hours = trunc(_ticks / 3600.0);
    self.timerLabel.text = [NSString stringWithFormat:@"%02.0f:%02.0f:%04.1f", hours, minutes, seconds];
}
#pragma mark - 旋转设置
//最低 版本要求 < iOS6.0
#if __IPHONE_OS_VERSION_MIN_REQUIRED < __IPHONE_6_0
- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation{
    return YES;//NO;
}
#endif

- (NSUInteger)supportedInterfaceOrientations{
    return UIInterfaceOrientationMaskAll;//UIInterfaceOrientationMaskPortrait;
}

#pragma mark - ADBannerViewDelegate
//Called when the user taps on the banner and some action is to be taken.
- (BOOL)bannerViewActionShouldBegin:(ADBannerView *)banner willLeaveApplication:(BOOL)willLeave{
    //停止 计时
    [self _stopTimer];
    return YES;
}

//Called when a modal action has completed and control is returned to the application.
- (void)bannerViewActionDidFinish:(ADBannerView *)banner{
    //开始 计时
    [self _startTimer];
}
-(void) bannerViewDidLoadAd:(ADBannerView *)banner{
    NSLog(@"%s", __FUNCTION__);
    //弹出 广告
    [self layoutAdBannerWithAnimated:YES];
}

-(void) bannerView:(ADBannerView *)banner didFailToReceiveAdWithError:(NSError *)error{
    NSLog(@"Error:%@ (%s)", error.localizedDescription, __FUNCTION__);
    //退下 广告
    [self layoutAdBannerWithAnimated:YES];
}

@end
