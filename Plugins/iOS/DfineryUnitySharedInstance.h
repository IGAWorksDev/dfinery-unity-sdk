#import <Foundation/Foundation.h>
#import <UserNotifications/UserNotifications.h>
#import <DfinerySDK/DfinerySDK.h>

@interface DfineryUnitySharedInstance : NSObject

+ (instancetype)sharedInstance;

@property (nonatomic, strong) NSData *deviceToken;
@property (nonatomic, strong) NSMutableArray<UNNotificationResponse *> *pendingResponses;
@property (nonatomic, assign) BOOL isSDKInitialized;

- (void)initSDKWithServiceId:(NSString *)serviceId;
- (void)initSDKWithServiceId:(NSString *)serviceId config:(NSDictionary *)config;

- (void)logEvent:(NSString *)eventName withProperties:(NSDictionary *)properties;
- (void)setUserProfileForKey:(NSString *)key value:(id)value;
- (void)setUserProfileWithDict:(NSDictionary *)values;
- (void)setIdentityForKey:(NSString *)key value:(NSString *)value;
- (void)setIdentityWithDict:(NSDictionary *)values;
- (void)resetIdentity;
- (void)enableSDK;
- (void)disableSDK;
- (void)setPushTokenWithData:(NSData *)data;

- (void)sendPushTokenToUnity;
- (NSString *)getPushTokenAsHexString;
- (NSString *)getPushTokenAsHexStringUnsafe;

- (void)handleNotificationResponse:(UNNotificationResponse *)response;
- (BOOL)canHandleForeground:(UNNotification *)notification completionHandler:(void (^)(UNNotificationPresentationOptions))completionHandler;

@end 