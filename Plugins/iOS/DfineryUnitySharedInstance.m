#import "DfineryUnitySharedInstance.h"

extern void UnitySendMessage(const char* gameObjectName, const char* methodName, const char* message);

@implementation DfineryUnitySharedInstance

static DfineryUnitySharedInstance *_sharedInstance = nil;
static dispatch_once_t onceToken;

+ (instancetype)sharedInstance {
    dispatch_once(&onceToken, ^{
        _sharedInstance = [[self alloc] init];
    });
    return _sharedInstance;
}

+ (instancetype)alloc {
    @synchronized(self) {
        if (_sharedInstance == nil) {
            _sharedInstance = [super alloc];
            return _sharedInstance;
        }
    }
    return _sharedInstance;
}

- (id)copyWithZone:(NSZone *)zone {
    return self;
}

- (instancetype)init {
    self = [super init];
    if (self) {
        _isSDKInitialized = NO;
        _pendingResponses = [[NSMutableArray alloc] init];
        _deviceToken = nil;
    }
    return self;
}

- (void)initSDKWithServiceId:(NSString *)serviceId {
    [[Dfinery shared] sdkInitWithServiceId:serviceId];
    
    @synchronized(self) {
        self.isSDKInitialized = YES;
        [self processStoredResponsesIfNeeded];
    }
}

- (void)initSDKWithServiceId:(NSString *)serviceId config:(NSDictionary *)config {
    [[Dfinery shared] sdkInit:serviceId withConfig:config];
    
    @synchronized(self) {
        self.isSDKInitialized = YES;
        [self processStoredResponsesIfNeeded];
    }
}

- (void)logEvent:(NSString *)eventName withProperties:(NSDictionary *)properties {
    [[Dfinery shared] logEvent:eventName withProperties:properties];
}

- (void)setUserProfileForKey:(NSString *)key value:(id)value {
    [[Dfinery shared] setUserProfileForKey:key value:value];
}

- (void)setUserProfileWithDict:(NSDictionary *)values {
    [[Dfinery shared] setUserProfileWithDict:values];
}

- (void)setIdentityForKey:(NSString *)key value:(NSString *)value {
    [[Dfinery shared] setIdentityForKey:key value:value];
}

- (void)setIdentityWithDict:(NSDictionary *)values {
    [[Dfinery shared] setIdentityWithDict:values];
}

- (void)resetIdentity {
    [[Dfinery shared] resetIdentity];
}

- (void)enableSDK {
    [[Dfinery shared] enableSDK];
}

- (void)disableSDK {
    [[Dfinery shared] disableSDK];
}

- (void)setPushTokenWithData:(NSData *)data {
    @synchronized(self) {
        self.deviceToken = data;
    }
    [[Dfinery shared] setPushTokenWithData:data];
}

- (void)sendPushTokenToUnity {
    NSString *tokenString;
    @synchronized(self) {
        tokenString = [self getPushTokenAsHexStringUnsafe];
    }
    
    if ([tokenString length] == 0) {
        NSLog(@"[DfineryUnity] No device token available to send to Unity");
        return;
    }
    
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0.1 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{

        if (tokenString == nil || ![tokenString isKindOfClass:[NSString class]] || [tokenString length] == 0) {
            NSLog(@"[DfineryUnity][Error] Push token is nil or invalid, cannot send to Unity");
        } else {
            UnitySendMessage("DfineryiOSCallbackReceiver", "OnPushTokenReceived", [tokenString UTF8String]);
            NSLog(@"[DfineryUnity] Push token sent to Unity successfully");
        }
    });
}

- (NSString *)getPushTokenAsHexString {
    @synchronized(self) {
        return [self getPushTokenAsHexStringUnsafe];
    }
}

- (NSString *)getPushTokenAsHexStringUnsafe {
    if (!self.deviceToken) {
        return @"";
    }
    
    const unsigned char *dataBuffer = (const unsigned char *)[self.deviceToken bytes];
    if (!dataBuffer) {
        NSLog(@"[DfineryUnity] Failed to convert device token to buffer");
        return @"";
    }
    
    NSUInteger dataLength = [self.deviceToken length];
    NSMutableString *hexString = [NSMutableString stringWithCapacity:(dataLength * 2)];
    
    for (int i = 0; i < dataLength; ++i) {
        [hexString appendString:[NSString stringWithFormat:@"%02x", dataBuffer[i]]];
    }
    
    return [NSString stringWithString:hexString];
}

- (void)processStoredResponsesIfNeeded {
    if (self.isSDKInitialized && self.pendingResponses.count > 0) {
        NSArray<UNNotificationResponse *> *responsesToProcess = [self.pendingResponses copy];
        [self.pendingResponses removeAllObjects];
        
        dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
            for (UNNotificationResponse *response in responsesToProcess) {
                (void)[[Dfinery shared] canHandleNotificationWithResponse:response];
            }
        });
    }
}

- (void)handleNotificationResponse:(UNNotificationResponse *)response {
    @synchronized(self) {
        if (self.isSDKInitialized) {
            dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
                (void)[[Dfinery shared] canHandleNotificationWithResponse:response];
            });
        } else {
            [self.pendingResponses addObject:response];
            NSLog(@"[DfineryUnity] Notification response queued (pending SDK init). Queue size: %lu", (unsigned long)self.pendingResponses.count);
        }
    }
}

- (BOOL)canHandleForeground:(UNNotification *)notification completionHandler:(void (^)(UNNotificationPresentationOptions))completionHandler {
    BOOL isInitialized;
    @synchronized(self) {
        isInitialized = self.isSDKInitialized;
    }
    
    if (isInitialized) {
        return [[Dfinery shared] canHandleForeground:notification completionHandler:completionHandler];
    } else {
        NSLog(@"[DfineryUnity] Foreground notification received but SDK not initialized yet");
        return NO;
    }
}

@end 