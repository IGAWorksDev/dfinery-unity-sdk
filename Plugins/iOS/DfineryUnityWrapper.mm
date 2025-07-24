#import <UserNotifications/UNUserNotificationCenter.h>
#import <DfinerySDK/DfinerySDK.h>
#import "DfineryUnitySharedInstance.h"

extern void UnitySendMessage(const char* gameObjectName, const char* methodName, const char* message);

extern "C" {

void _dfineryInit(const char* serviceId) {
    [[DfineryUnitySharedInstance sharedInstance] initSDKWithServiceId:[NSString stringWithUTF8String:serviceId]];
}

void _dfineryInitWithConfig(const char* serviceId, const char* properties) {
    NSDictionary *config = [NSJSONSerialization JSONObjectWithData:[[NSString stringWithUTF8String:properties] dataUsingEncoding:NSUTF8StringEncoding]
                                                           options:0
                                                             error:nil];
    
    [[DfineryUnitySharedInstance sharedInstance] initSDKWithServiceId:[NSString stringWithUTF8String:serviceId]
                                                               config:config];
}

void _dfineryLogEvent(const char* eventName) {
    [[DfineryUnitySharedInstance sharedInstance] logEvent:[NSString stringWithUTF8String:eventName]
                                           withProperties:nil];
}

void _dfineryLogEventWithProps(const char* eventName, const char* properties) {
    NSDictionary *props = [NSJSONSerialization JSONObjectWithData:[[NSString stringWithUTF8String:properties] dataUsingEncoding:NSUTF8StringEncoding]
                                                          options:0
                                                            error:nil];
    [[DfineryUnitySharedInstance sharedInstance] logEvent:[NSString stringWithUTF8String:eventName]
                                           withProperties:props];
}

void _dfineryEnableSDK(void) {
    [[DfineryUnitySharedInstance sharedInstance] enableSDK];
}

void _dfineryDisableSDK(void) {
    [[DfineryUnitySharedInstance sharedInstance] disableSDK];
}

void _dfinerySetUserProfile(const char* key, const char* value) {
    [[DfineryUnitySharedInstance sharedInstance] setUserProfileForKey:[NSString stringWithUTF8String:key]
                                                                value:[NSString stringWithUTF8String:value]];
}

void _dfinerySetUserProfiles(const char* properties) {
    NSDictionary *props = [NSJSONSerialization JSONObjectWithData:[[NSString stringWithUTF8String:properties] dataUsingEncoding:NSUTF8StringEncoding]
                                                          options:0
                                                            error:nil];
    [[DfineryUnitySharedInstance sharedInstance] setUserProfileWithDict:props];
}

void _dfinerySetIdentity(const char* key, const char* value) {
    [[DfineryUnitySharedInstance sharedInstance] setIdentityForKey:[NSString stringWithUTF8String:key]
                                                             value:[NSString stringWithUTF8String:value]];
}

void _dfinerySetIdentities(const char* properties) {
    NSDictionary *props = [NSJSONSerialization JSONObjectWithData:[[NSString stringWithUTF8String:properties] dataUsingEncoding:NSUTF8StringEncoding]
                                                          options:0
                                                            error:nil];
    [[DfineryUnitySharedInstance sharedInstance] setIdentityWithDict:props];
}

void _dfineryResetIdentity(void) {
    [[DfineryUnitySharedInstance sharedInstance] resetIdentity];
}

void _dfinerySetPushToken(const char* pushToken) {
    NSString *tokenString = [NSString stringWithUTF8String:pushToken];
    
    if (!tokenString || [tokenString length] % 2 != 0) {
        NSLog(@"[DfineryUnity] Invalid push token: %@", tokenString);
        return;
    }
    
    NSCharacterSet *hexCharacterSet = [NSCharacterSet characterSetWithCharactersInString:@"0123456789abcdefABCDEF"];
    NSCharacterSet *invalidCharacterSet = [hexCharacterSet invertedSet];
    NSRange invalidCharacterRange = [tokenString rangeOfCharacterFromSet:invalidCharacterSet];
    
    if (invalidCharacterRange.location != NSNotFound) {
        NSLog(@"[DfineryUnity] Push token contains invalid characters: %@", tokenString);
        return;
    }
    
    NSMutableData *data = [[NSMutableData alloc] initWithCapacity:tokenString.length / 2];
    
    for (int i = 0; i < tokenString.length; i += 2) {
        if (i + 2 > tokenString.length) {
            NSLog(@"[DfineryUnity] Invalid push token length: %@", tokenString);
            return;
        }
        NSString *hexChar = [tokenString substringWithRange:NSMakeRange(i, 2)];
        unsigned int hexValue = 0;
        NSScanner *scanner = [NSScanner scannerWithString:hexChar];
        if (![scanner scanHexInt:&hexValue]) {
            NSLog(@"[DfineryUnity] Failed to scan hex value from push token: %@", hexChar);
            return;
        }
        unsigned char byte = (unsigned char)hexValue;
        [data appendBytes:&byte length:1];
    }
    [[DfineryUnitySharedInstance sharedInstance] setPushTokenWithData:data];
}

void _dfineryGetPushToken(void) {
    [[DfineryUnitySharedInstance sharedInstance] sendPushTokenToUnity];
}

}
