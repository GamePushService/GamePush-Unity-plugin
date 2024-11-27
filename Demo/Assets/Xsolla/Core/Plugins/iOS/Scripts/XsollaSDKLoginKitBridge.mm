#import <AuthenticationServices/AuthenticationServices.h>

#import "XsollaSDKLoginKitObjectiveC/XsollaSDKLoginKitObjectiveC-Swift.h"
#import "XsollaUtils.h"

#pragma mark - C interface

typedef void(ActionVoidCallbackDelegate)(void *actionPtr);
typedef void(ActionStringCallbackDelegate)(void *actionPtr, const char *data);

extern "C" {

    void _configureLoginAnalytics(char* gameEngine, char* gameEngineVersion) {
        NSString* gameEngineString = [XsollaUtils createNSStringFrom:gameEngine];
        NSString* gameEngineVersionString = [XsollaUtils createNSStringFrom:gameEngineVersion];
        
        [[LoginKitObjectiveC shared] configureAnalyticsWithGameEngine:gameEngineString gameEngineVersion:gameEngineVersionString];
    }

    void _authWithXsollaWidget(char* loginId, int clientId, char* state, char* redirectUri, char* locale,
                              ActionStringCallbackDelegate authSuccessCallback, void *authSuccessActionPtr,
                              ActionStringCallbackDelegate errorCallback, void *errorActionPtr,
                              ActionVoidCallbackDelegate cancelCallback, void *cancelActionPtr) {
        
        NSString* loginIdString = [XsollaUtils createNSStringFrom:loginId];
        NSString* stateString = [XsollaUtils createNSStringFrom:state];
        NSString* redirectUriString = [XsollaUtils createNSStringFrom:redirectUri];
        NSString* localeString = [XsollaUtils createNSStringFrom:locale];

        OAuth2Params *oauthParams = [[OAuth2Params alloc] initWithClientId:clientId
                                                             state:stateString
                                                             scope:@"offline"
                                                             redirectUri:redirectUriString];
        
        JWTGenerationParams *jwtGenerationParams = [[JWTGenerationParams alloc] initWithGrantType:TokenGrantTypeAuthorizationCode
                                                             clientId:clientId
                                                             refreshToken:nil
                                                             clientSecret:nil
                                                             redirectUri:redirectUriString];
        
        if (@available(iOS 13.4, *)) {
            WebAuthenticationPresentationContextProvider* context = [[WebAuthenticationPresentationContextProvider alloc] initWithPresentationAnchor:UnityGetMainWindow()];
            
            [[LoginKitObjectiveC shared] authWithXsollaWidgetWithLoginId:loginIdString oAuth2Params:oauthParams jwtParams:jwtGenerationParams locale:localeString presentationContextProvider:context completion:^(AccessTokenInfo * _Nullable accesTokenInfo, NSError * _Nullable error){
                
                if(error != nil) {
                    NSLog(@"Error code: %ld", error.code);

                    if(error.code == NSError.loginKitErrorCodeASCanceledLogin) {
                        cancelCallback(cancelActionPtr);
                        return;
                    }

                    errorCallback(errorActionPtr, [XsollaUtils createCStringFrom:error.localizedDescription]);
                    return;
                }

                NSString* tokenInfoString = [XsollaUtils serializeTokenInfo:accesTokenInfo];
                authSuccessCallback(authSuccessActionPtr, [XsollaUtils createCStringFrom:tokenInfoString]);
            }];
            
        } else {
            NSLog(@"Authentication vith xsolla widget is not supported for current iOS version.");
        }
    }

	void _authBySocialNetwork(char* platform, int clientId, char* state, char* redirectUri,
		ActionStringCallbackDelegate authSuccessCallback, void *authSuccessActionPtr,
		ActionStringCallbackDelegate errorCallback, void *errorActionPtr,
		ActionVoidCallbackDelegate cancelCallback, void *cancelActionPtr) {

		NSString* platformString = [XsollaUtils createNSStringFrom:platform];
		NSString* stateString = [XsollaUtils createNSStringFrom:state];
		NSString* redirectUriString = [XsollaUtils createNSStringFrom:redirectUri];

		OAuth2Params *oauthParams = [[OAuth2Params alloc] initWithClientId:clientId
															 state:stateString
															 scope:@"offline"
															 redirectUri:redirectUriString];

		JWTGenerationParams *jwtGenerationParams = [[JWTGenerationParams alloc] initWithGrantType:TokenGrantTypeAuthorizationCode
															 clientId:clientId
															 refreshToken:nil
															 clientSecret:nil
															 redirectUri:redirectUriString];

		if (@available(iOS 13.4, *)) {
			WebAuthenticationPresentationContextProvider* context = [[WebAuthenticationPresentationContextProvider alloc] initWithPresentationAnchor:UnityGetMainWindow()];

			[[LoginKitObjectiveC shared] authBySocialNetwork:platformString oAuth2Params:oauthParams jwtParams:jwtGenerationParams presentationContextProvider:context completion:^(AccessTokenInfo * _Nullable accesTokenInfo, NSError * _Nullable error){

				if(error != nil) {
					NSLog(@"Error code: %ld", error.code);

					if(error.code == NSError.loginKitErrorCodeASCanceledLogin) {
						cancelCallback(cancelActionPtr);
						return;
					}

					errorCallback(errorActionPtr, [XsollaUtils createCStringFrom:error.localizedDescription]);
					return;
				}

				NSString* tokenInfoString = [XsollaUtils serializeTokenInfo:accesTokenInfo];
				authSuccessCallback(authSuccessActionPtr, [XsollaUtils createCStringFrom:tokenInfoString]);
			}];
		} else {
			NSLog(@"Authentication via social networks with Xsolla is not supported for current iOS version.");
		}
    }
}
