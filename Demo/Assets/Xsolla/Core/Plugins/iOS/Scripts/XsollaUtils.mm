#import "XsollaUtils.h"

@implementation XsollaUtils

+ (NSString *)createNSStringFrom:(const char *)cstring {
    return [NSString stringWithUTF8String:(cstring ?: "")];
}

+ (char *)cStringCopy:(const char *)string {
    char *res = (char *) malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

+ (char *)createCStringFrom:(NSString *)string {
    if (!string) {
        string = @"";
    }
    return [self cStringCopy:[string UTF8String]];
}

+ (NSString *)serializeDictionary:(NSDictionary *)dictionary {
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dictionary options:0 error:&error];
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

+ (NSString *)serializeTokenInfo:(AccessTokenInfo*)tokenInfo {
    NSMutableDictionary *tokenInfoDictionary = [NSMutableDictionary dictionary];
	tokenInfoDictionary[@"access_token"] = tokenInfo.accessToken;
	tokenInfoDictionary[@"refresh_token"] = tokenInfo.refreshToken;
	tokenInfoDictionary[@"token_type"] = tokenInfo.tokenType;
	tokenInfoDictionary[@"expires_in"] = @(tokenInfo.expiresIn);
	tokenInfoDictionary[@"scope"] = @"offline";
    return [self serializeDictionary:tokenInfoDictionary];
}

@end
