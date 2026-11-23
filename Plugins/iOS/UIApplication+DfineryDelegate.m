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
        
        dfinery_unity_swizzle(self.class,
                      @selector(dfinery_application:didFailToRegisterForRemoteNotificationsWithError:),
                      appDelegate,
                      @selector(application:didFailToRegisterForRemoteNotificationsWithError:));
        
        [self dfinery_setDelegate:delegate];
    });
}

#pragma mark - Method Swizzling

- (BOOL)dfinery_application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions 
{
    // UNUserNotificationCenter delegate 설정
    [[UNUserNotificationCenter currentNotificationCenter] setDelegate:(id<UNUserNotificationCenterDelegate>)application.delegate];
    
    // registerForRemoteNotifications는 serviceId와 무관하게 항상 호출
    // 토큰은 나중에 SDK 초기화 후에도 사용 가능
    NSLog(@"[DfineryUnity] Calling registerForRemoteNotifications...");
    [application registerForRemoteNotifications];
    NSLog(@"[DfineryUnity] registerForRemoteNotifications called, waiting for APNS callback...");
    
    // Info.plist에 serviceId가 있으면 자동 초기화, 없으면 Unity 코드에서 수동 초기화 예상
    NSDictionary *infoDictionary = [[NSBundle mainBundle] infoDictionary];
    NSString *serviceId = infoDictionary[@"com.igaworks.dfinery.unity.serviceId"];
    
    if (serviceId && [serviceId isKindOfClass:[NSString class]] && [serviceId length] > 0) {
        NSNumber *logEnabled = infoDictionary[@"com.igaworks.dfinery.unity.iosLogEnable"];
        if (!logEnabled || ![logEnabled isKindOfClass:[NSNumber class]]) {
            logEnabled = @NO;
        }
        
        NSDictionary *config = @{
            @"df_config_log_enable": logEnabled
        };
        
        [[DfineryUnitySharedInstance sharedInstance] initSDKWithServiceId:serviceId config:config];
        NSLog(@"[DfineryUnity] SDK auto-initialized with serviceId: %@, logEnabled: %@", serviceId, logEnabled);
    } else {
        NSLog(@"[DfineryUnity] serviceId not in Info.plist, expecting manual initialization from Unity code");
    }
    
    if ([self respondsToSelector:@selector(dfinery_application:didFinishLaunchingWithOptions:)])
        return [self dfinery_application:application didFinishLaunchingWithOptions:launchOptions];
    return YES;
}

- (void)dfinery_application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
{
    NSLog(@"[DfineryUnity] didRegisterForRemoteNotificationsWithDeviceToken called!");
    NSLog(@"[DfineryUnity] Token length: %lu bytes", (unsigned long)deviceToken.length);
    
    [[DfineryUnitySharedInstance sharedInstance] setPushTokenWithData:deviceToken];
    
    NSLog(@"[DfineryUnity] Device token registered and saved");
    
    if([self respondsToSelector:@selector(dfinery_application:didRegisterForRemoteNotificationsWithDeviceToken:)])
        [self dfinery_application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
}

- (void)dfinery_application:(UIApplication *)application didFailToRegisterForRemoteNotificationsWithError:(NSError *)error
{
    NSLog(@"[DfineryUnity] Failed to register for remote notifications: %@", error.localizedDescription);
    NSLog(@"[DfineryUnity] Error code: %ld, domain: %@", (long)error.code, error.domain);
    
    if([self respondsToSelector:@selector(dfinery_application:didFailToRegisterForRemoteNotificationsWithError:)])
        [self dfinery_application:application didFailToRegisterForRemoteNotificationsWithError:error];
}

@end 
