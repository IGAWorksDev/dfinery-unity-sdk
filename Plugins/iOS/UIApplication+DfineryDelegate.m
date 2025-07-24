#import <Foundation/Foundation.h>
#import <objc/runtime.h>
#import <UserNotifications/UserNotifications.h>
#import "DfineryUnitySwizzler.h"
#import "DfineryUnitySharedInstance.h"

@implementation UIApplication (DfineryDelegate)

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

- (void)dfinery_setDelegate:(id<UIApplicationDelegate>)delegate 
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        if (!delegate) {
            NSLog(@"[DfineryUnity] Warning: App delegate is nil, skipping swizzling");
            [self dfinery_setDelegate:delegate];
            return;
        }
        
        Class appDelegate = [delegate class];
        NSLog(@"[DfineryUnity] Swizzling app delegate methods for class: %@", NSStringFromClass(appDelegate));
        
        dfinery_unity_swizzle(self.class,
                      @selector(dfinery_application:didFinishLaunchingWithOptions:),
                      appDelegate,
                      @selector(application:didFinishLaunchingWithOptions:));
        
        dfinery_unity_swizzle(self.class,
                      @selector(dfinery_application:didRegisterForRemoteNotificationsWithDeviceToken:),
                      appDelegate,
                      @selector(application:didRegisterForRemoteNotificationsWithDeviceToken:));
        
        [self dfinery_setDelegate:delegate];
    });
}

#pragma mark - Method Swizzling

- (BOOL)dfinery_application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions 
{
    NSDictionary *infoDictionary = [[NSBundle mainBundle] infoDictionary];
    if (!infoDictionary) {
        NSLog(@"[DfineryUnity] Failed to get Info.plist dictionary");
        if ([self respondsToSelector:@selector(dfinery_application:didFinishLaunchingWithOptions:)])
            return [self dfinery_application:application didFinishLaunchingWithOptions:launchOptions];
        return YES;
    }
    
    NSString *serviceId = [infoDictionary objectForKey:@"com.igaworks.dfinery.unity.serviceId"];
    if (!serviceId || ![serviceId isKindOfClass:[NSString class]] || [serviceId length] == 0) {
        NSLog(@"[DfineryUnity] Invalid or missing serviceId in Info.plist");
        if ([self respondsToSelector:@selector(dfinery_application:didFinishLaunchingWithOptions:)])
            return [self dfinery_application:application didFinishLaunchingWithOptions:launchOptions];
        return YES;
    }
    
    NSNumber *logEnabled = [infoDictionary objectForKey:@"com.igaworks.dfinery.unity.iosLogEnable"];
    if (!logEnabled || ![logEnabled isKindOfClass:[NSNumber class]]) {
        logEnabled = @NO;
    }
    
    NSDictionary *config = @{
        @"df_config_log_enable": logEnabled
    };
    
    [[DfineryUnitySharedInstance sharedInstance] initSDKWithServiceId:serviceId config:config];
    
    NSLog(@"[DfineryUnity] SDK initialized with serviceId: %@, logEnabled: %@", serviceId, logEnabled);
    
    [[UNUserNotificationCenter currentNotificationCenter] setDelegate:(id<UNUserNotificationCenterDelegate>)application.delegate];
    
    [application registerForRemoteNotifications];
    
    if ([self respondsToSelector:@selector(dfinery_application:didFinishLaunchingWithOptions:)])
        return [self dfinery_application:application didFinishLaunchingWithOptions:launchOptions];
    return YES;
}

- (void)dfinery_application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
{
    [[DfineryUnitySharedInstance sharedInstance] setPushTokenWithData:deviceToken];
    
    NSLog(@"[DfineryUnity] Device token registered and saved");
    
    if([self respondsToSelector:@selector(dfinery_application:didRegisterForRemoteNotificationsWithDeviceToken:)])
        [self dfinery_application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
}

@end 
