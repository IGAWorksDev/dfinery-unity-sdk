#import <Foundation/Foundation.h>
#import <objc/runtime.h>

@interface DfineryUnitySwizzler : NSObject

void dfinery_unity_swizzle(Class dfineryClass, SEL dfinerySel, Class orgClass, SEL orgSel);

@end 