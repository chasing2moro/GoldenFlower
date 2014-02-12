//
//  ARCSingletonTemplate.h
//  IAPDemo
//
//  Created by yaowan on 13-11-16.
//  Copyright (c) 2013年 bobo. All rights reserved.
//

#define SYNTHESIZE_SINGLETON_FOR_HEADER(className) \
\
+ (className *)shared##className;

#define SYNTHESIZE_SINGLETON_FOR_CLASS(className) \
\
+ (className *)shared##className { \
static className *shared##className = nil; \
static dispatch_once_t onceToken; \
dispatch_once(&onceToken, ^{ \
shared##className = [[self alloc] init]; \
}); \
return shared##className; \
}

//有些变量只需要初始化一次（如从文件中读取配置参数，读取设备型号等等），可以使用dispatch_once来进行读取优化，保证只调用API一次，以后就只要直接访问变量即可