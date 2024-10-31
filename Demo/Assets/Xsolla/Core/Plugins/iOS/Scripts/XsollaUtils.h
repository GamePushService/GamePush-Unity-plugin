#import <Foundation/Foundation.h>
#import <UIKit/UIImage.h>
#import <AuthenticationServices/AuthenticationServices.h>

#import "XsollaSDKLoginKitObjectiveC/XsollaSDKLoginKitObjectiveC-Swift.h"

@interface XsollaUtils : NSObject

// Converts C style string to NSString
+ (NSString *)createNSStringFrom:(const char *)cstring;

// Conver NSString to C style string
+ (char *)createCStringFrom:(NSString *)string;

// Serialize NSDictionaty to NSString
+ (NSString *)serializeDictionary:(NSDictionary *)dictionary;

// Serialize Xsolla token data
+ (NSString *)serializeTokenInfo:(AccessTokenInfo*)tokenData;

@end
