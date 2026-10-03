package com.gamepush.auth;

import android.app.Activity;
import android.net.Uri;
import android.os.Bundle;
import android.webkit.WebResourceRequest;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;

public class GpAuthActivity extends Activity {
    public static final String EXTRA_URL = "url";
    public static final String EXTRA_PREFIX = "prefix";
    public static final String EXTRA_PARAM = "param";

    private boolean handled;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        String startUrl = getIntent().getStringExtra(EXTRA_URL);
        String prefix = getIntent().getStringExtra(EXTRA_PREFIX);
        String param = getIntent().getStringExtra(EXTRA_PARAM);
        if (param == null || param.length() == 0)
            param = "code";

        WebView web = new WebView(this);
        WebSettings settings = web.getSettings();
        settings.setJavaScriptEnabled(true);
        settings.setDomStorageEnabled(true);
        setContentView(web);

        final String redirectPrefix = prefix;
        final String queryParam = param;
        web.setWebViewClient(new WebViewClient() {
            @Override
            public boolean shouldOverrideUrlLoading(WebView view, WebResourceRequest request) {
                if (request == null || request.getUrl() == null)
                    return false;
                return hit(request.getUrl().toString(), redirectPrefix, queryParam);
            }

            @Override
            public boolean shouldOverrideUrlLoading(WebView view, String url) {
                return hit(url, redirectPrefix, queryParam);
            }
        });
        web.loadUrl(startUrl != null ? startUrl : "about:blank");
    }

    private boolean hit(String url, String prefix, String param) {
        if (handled || url == null || prefix == null || prefix.length() == 0)
            return false;
        if (!url.startsWith(prefix))
            return false;
        Uri uri = Uri.parse(url);
        String token = uri.getQueryParameter(param);
        if (token == null || token.length() == 0)
            return false;
        handled = true;
        sendToUnity(token);
        finish();
        return true;
    }

    @Override
    public void onBackPressed() {
        if (!handled) {
            handled = true;
            sendToUnity("cancel");
        }
        super.onBackPressed();
    }

    private static void sendToUnity(String value) {
        try {
            Class<?> player = Class.forName("com.unity3d.player.UnityPlayer");
            player.getMethod("UnitySendMessage", String.class, String.class, String.class)
                .invoke(null, "GamePushSDK", "OnGpAuthResult", value);
        } catch (Exception ignored) {
        }
    }
}
