#import "DfineryUnitySwizzler.h"

@implementation DfineryUnitySwizzler

void dfinery_unity_swizzle(Class dfineryClass, SEL dfinerySel, Class orgClass, SEL orgSel)
{
    Method dfineryMethod = class_getInstanceMethod(dfineryClass, dfinerySel);
    IMP dfineryImp = method_getImplementation(dfineryMethod);
    const char* dfineryMethodType = method_getTypeEncoding(dfineryMethod);
    
    BOOL didAddNewMethod = class_addMethod(orgClass, orgSel, dfineryImp, dfineryMethodType);
    
    if (!didAddNewMethod) {
        class_addMethod(orgClass, dfinerySel, dfineryImp, dfineryMethodType);
        
        dfineryMethod = class_getInstanceMethod(orgClass, dfinerySel);
        Method orgMethod = class_getInstanceMethod(orgClass, orgSel);
        
        method_exchangeImplementations(dfineryMethod, orgMethod);
    }
}
@end 