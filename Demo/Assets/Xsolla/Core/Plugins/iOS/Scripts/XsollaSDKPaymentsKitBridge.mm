#import "XsollaSDKPaymentsKitObjectiveC/XsollaSDKPaymentsKitObjectiveC-Swift.h"
#import "XsollaUtils.h"

#pragma mark - C interface

typedef void(ActionStringCallbackDelegate)(void *actionPtr, const char *data);
typedef void(ActionBoolCallbackDelegate)(void *actionPtr, const bool data);

extern "C" {

    void _configurePaymentsAnalytics(char* gameEngine, char* gameEngineVersion) {
        NSString* gameEngineString = [XsollaUtils createNSStringFrom:gameEngine];
        NSString* gameEngineVersionString = [XsollaUtils createNSStringFrom:gameEngineVersion];
        
        [[PaymentsKitObjectiveC shared] configureAnalyticsWithGameEngine:gameEngineString gameEngineVersion:gameEngineVersionString];
    }
    
    void _setPaystationVersion(int paystationVersion){
        [[PaymentsKitObjectiveC shared] setPaystationVersionWithPaystationVersion:paystationVersion];
    }
    
    void _performPayment(char* token, bool isSandbox, char* redirectUrl, 
        ActionStringCallbackDelegate errorCallback, void *errorActionPtr, 
        ActionBoolCallbackDelegate browserCallback, void *browserCallbackActionPtr) {

        NSString* tokenString = [XsollaUtils createNSStringFrom:token];
        NSString* redirectUrlString = [XsollaUtils createNSStringFrom:redirectUrl];     
        BOOL isSandboxBool = isSandbox;

        [[PaymentsKitObjectiveC shared] performPaymentWithPaymentToken:tokenString presenter:UnityGetGLViewController() isSandbox:isSandboxBool redirectUrl:redirectUrlString completionHandler:^(NSError * _Nullable error)
        {
            if(error != nil && error.code != NSError.cancelledByUserError) {
                NSLog(@"Error code: %ld", error.code);
                errorCallback(errorActionPtr, [XsollaUtils createCStringFrom:error.localizedDescription]);
            }
            
            BOOL isManually = (error != nil && error.code == NSError.cancelledByUserError);
            browserCallback(browserCallbackActionPtr, isManually);
        }];
    }

    extern UIViewController *UnityGetGLViewController();
}




