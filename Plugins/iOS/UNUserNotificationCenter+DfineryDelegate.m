#import <Foundation/Foundation.h>
#import <objc/runtime.h>
#import <UserNotifications/UserNotifications.h>
#import "DfineryUnitySwizzler.h"
#import "DfineryUnitySharedInstance.h"

@implementation UNUserNotificationCenter (DfineryDelegate)

+ (void)load 
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        SEL originalSelector = @selector(setDelegate:);
        SEL swizzledSelector = @selector(dfinery_setDelegate:);
        Method originalMethod = class_getInstanceMethod(self, originalSelector);
        Method swizzledMethod = class_getInstanceMethod(self, swizzledSelector);
        
        method_exchangeImplementations(originalMethod, swizzledMethod);
    });
}

- (void)dfinery_setDelegate:(id<UNUserNotificationCenterDelegate>)delegate
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        if (!delegate) {
            NSLog(@"[DfineryUnity] Warning: UNUserNotificationCenter delegate is nil, skipping swizzling");
            [self dfinery_setDelegate:delegate];
            return;
        }
        
        Class appDelegate = [delegate class];
        NSLog(@"[DfineryUnity] Swizzling UNUserNotificationCenter delegate methods for class: %@", NSStringFromClass(appDelegate));
        
        dfinery_unity_swizzle(self.class,
                @selector(dfinery_UserNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:),
                appDelegate,
                @selector(userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:));
        dfinery_unity_swizzle(self.class,
                @selector(dfinery_UserNotificationCenter:willPresentNotification:withCompletionHandler:),
                appDelegate,
                @selector(userNotificationCenter:willPresentNotification:withCompletionHandler:));
        [self dfinery_setDelegate:delegate];
    });
}

- (void)dfinery_UserNotificationCenter:(UNUserNotificationCenter *)center didReceiveNotificationResponse:(UNNotificationResponse *)response withCompletionHandler:(void (^)(void))completionHandler
{
    [[DfineryUnitySharedInstance sharedInstance] handleNotificationResponse:response];
    
    if ([self respondsToSelector:@selector(dfinery_UserNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:)]) {
        [self dfinery_UserNotificationCenter:center didReceiveNotificationResponse:response withCompletionHandler:completionHandler];
    } else {
        if (completionHandler) {
            completionHandler();
        }
    }
}
    
- (void)dfinery_UserNotificationCenter:(UNUserNotificationCenter *)center willPresentNotification:(UNNotification *)notification withCompletionHandler:(void (^)(UNNotificationPresentationOptions))completionHandler
{
    BOOL handled = [[DfineryUnitySharedInstance sharedInstance] canHandleForeground:notification completionHandler:completionHandler];
    
    if ([self respondsToSelector:@selector(dfinery_UserNotificationCenter:willPresentNotification:withCompletionHandler:)]) {
        [self dfinery_UserNotificationCenter:center willPresentNotification:notification withCompletionHandler:completionHandler];
    } else {
        if (!handled && completionHandler) {
            completionHandler(UNNotificationPresentationOptionNone);
        }
    }
}

@end 