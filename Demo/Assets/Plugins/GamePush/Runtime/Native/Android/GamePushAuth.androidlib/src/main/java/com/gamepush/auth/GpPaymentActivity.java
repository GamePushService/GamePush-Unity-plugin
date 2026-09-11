package com.gamepush.auth;

import android.app.Activity;
import android.graphics.Bitmap;
import android.os.Build;
import android.os.Bundle;
import android.webkit.CookieManager;
import android.webkit.JavascriptInterface;
import android.webkit.WebChromeClient;
import android.webkit.WebResourceRequest;
import android.webkit.WebResourceResponse;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;

public class GpPaymentActivity extends Activity {
    public static final String EXTRA_URL = "url";
    public static final String EXTRA_PREFIX = "prefix";
    public static final String EXTRA_HTML = "html";

    private static final String LOADER = "cdn.gamepush.com/pages/loader.html";
    private static final String PAYMENT_RESULT = "cdn.gamepush.com/pages/payment-result.html";

    /**
     * Same contract as game-score-sdk:
     * loader.html posts JSON.stringify({ type: 'gs:pageReady' }) to window.parent;
     * robokassa/payment/App.tsx JSON.parse(e.data) and the parent overlay closes.
     */
    private static final String HOOK_JS =
            "(function(){if(window.__gpPayHook)return;window.__gpPayHook=true;" +
            "function gpClose(){try{if(window.GpPayment)window.GpPayment.done('done');" +
            "else if(window.top&&window.top.GpPayment)window.top.GpPayment.done('done');}catch(x){}}" +
            "function gpHit(d){var t='';try{if(typeof d==='string')d=JSON.parse(d);" +
            "if(d&&typeof d==='object')t=d.type||d.action||'';}catch(x){return;}" +
            "if(t==='gs:pageReady'||t==='GS_PAYMENT_RESULT_MESSAGE'||" +
            "t==='requestToCloseWindow'||t==='closeRobokassaFrame')gpClose();}" +
            "window.addEventListener('message',function(e){gpHit(e.data);});" +
            "var h=String(location.href||'');" +
            "if(h.indexOf('cdn.gamepush.com/pages/loader.html')>=0||" +
            "h.indexOf('cdn.gamepush.com/pages/payment-result.html')>=0)gpClose();" +
            "})();";

    private static GpPaymentActivity instance;
    private boolean handled;
    private WebView web;
    private String donePrefix;

    public static void closeFromUnity() {
        final GpPaymentActivity activity = instance;
        if (activity == null)
            return;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                activity.finishWith("done");
            }
        });
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        instance = this;
        String startUrl = getIntent().getStringExtra(EXTRA_URL);
        donePrefix = getIntent().getStringExtra(EXTRA_PREFIX);
        String html = getIntent().getStringExtra(EXTRA_HTML);

        web = new WebView(this);
        WebSettings settings = web.getSettings();
        settings.setJavaScriptEnabled(true);
        settings.setDomStorageEnabled(true);
        settings.setDatabaseEnabled(true);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
            settings.setMixedContentMode(WebSettings.MIXED_CONTENT_ALWAYS_ALLOW);
        CookieManager.getInstance().setAcceptCookie(true);
        CookieManager.getInstance().setAcceptThirdPartyCookies(web, true);
        web.addJavascriptInterface(new Bridge(), "GpPayment");
        web.setWebChromeClient(new WebChromeClient());
        setContentView(web);

        web.setWebViewClient(new WebViewClient() {
            @Override
            public boolean shouldOverrideUrlLoading(WebView view, WebResourceRequest request) {
                if (request == null || request.getUrl() == null)
                    return false;
                return hit(request.getUrl().toString());
            }

            @Override
            public boolean shouldOverrideUrlLoading(WebView view, String url) {
                return hit(url);
            }

            @Override
            public WebResourceResponse shouldInterceptRequest(WebView view, WebResourceRequest request) {
                if (request != null && request.getUrl() != null)
                    considerDone(request.getUrl().toString());
                return super.shouldInterceptRequest(view, request);
            }

            @Override
            public WebResourceResponse shouldInterceptRequest(WebView view, String url) {
                considerDone(url);
                return super.shouldInterceptRequest(view, url);
            }

            @Override
            public void onPageStarted(WebView view, String url, Bitmap favicon) {
                considerDone(url);
                super.onPageStarted(view, url, favicon);
            }

            @Override
            public void onLoadResource(WebView view, String url) {
                considerDone(url);
                super.onLoadResource(view, url);
            }

            @Override
            public void doUpdateVisitedHistory(WebView view, String url, boolean isReload) {
                considerDone(url);
                super.doUpdateVisitedHistory(view, url, isReload);
            }

            @Override
            public void onPageFinished(WebView view, String url) {
                considerDone(url);
                if (view != null)
                    view.evaluateJavascript(HOOK_JS, null);
            }
        });
        if (html != null && html.length() > 0)
            web.loadDataWithBaseURL("https://cdn.gamepush.com/", html, "text/html", "UTF-8", null);
        else
            web.loadUrl(startUrl != null ? startUrl : "about:blank");
    }

    private boolean hit(String url) {
        if (!isDoneUrl(url))
            return false;
        finishWith("done");
        return true;
    }

    private void considerDone(final String url) {
        if (!isDoneUrl(url))
            return;
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                finishWith("done");
            }
        });
    }

    private boolean isDoneUrl(String url) {
        if (url == null || url.length() == 0)
            return false;
        String lower = url.toLowerCase();
        if (lower.contains(LOADER) || lower.contains(PAYMENT_RESULT))
            return true;
        return donePrefix != null && donePrefix.length() > 0 && url.startsWith(donePrefix);
    }

    @Override
    public void onBackPressed() {
        if (web != null && web.canGoBack()) {
            web.goBack();
            return;
        }
        finishWith("cancel");
    }

    @Override
    protected void onDestroy() {
        if (instance == this)
            instance = null;
        if (!handled)
            sendToUnity("cancel");
        if (web != null) {
            web.destroy();
            web = null;
        }
        super.onDestroy();
    }

    private void finishWith(String value) {
        if (handled)
            return;
        handled = true;
        if (web != null)
            web.stopLoading();
        sendToUnity(value);
        finish();
    }

    private static void sendToUnity(String value) {
        try {
            Class<?> player = Class.forName("com.unity3d.player.UnityPlayer");
            player.getMethod("UnitySendMessage", String.class, String.class, String.class)
                .invoke(null, "GamePushSDK", "OnGpPaymentResult", value);
        } catch (Exception ignored) {
        }
    }

    private class Bridge {
        @JavascriptInterface
        public void done(String value) {
            final String result = (value == null || value.length() == 0) ? "done" : value;
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    finishWith(result);
                }
            });
        }
    }
}
