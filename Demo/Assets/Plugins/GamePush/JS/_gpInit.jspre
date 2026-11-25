let GamePush;

function _GP(){
    return GamePush || window.GamePush;
}

function _ToBuff(value){
    if (value === undefined || value === null) {
        value = "";
    } else if (typeof value !== "string") {
        value = String(value);
    }

    var bufferSize = lengthBytesUTF8(value) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(value, buffer, bufferSize);
    return buffer;
}

var _unityInnerAwaiter = {};
    _unityInnerAwaiter.ready = new Promise((resolve) => {
      _unityInnerAwaiter.done = resolve;
    });

function _UnityReady() {
    console.log("Unity is ready");
    if(_unityInnerAwaiter != null)
        _unityInnerAwaiter.done();
}

function _waitFor(check, timeout) {
    return new Promise((resolve, reject) => {
      let intervalId = 0
  
      function checkReady() {
        if (check(window)) {
          clearInterval(intervalId)
          resolve()
        }
      }
  
      if (check(window)) {
        resolve()
        return
      }
  
      intervalId = setInterval(checkReady, 100)
      if (timeout) {
        setTimeout(reject, timeout)
      }
    })
  }

// Channel events use the SDK socket; the native multiplayer bridge owns its own room connection.

setTimeout(() => {
    if ('GamePushUnity' in window) return;

    window.onGPError = async () => {
        await _unityInnerAwaiter.ready;
        SendMessage('GamePushSDK', 'CallOnSDKError');
    };

    window.onGPInit = async (gp) => {

        GamePush = new GamePushUnityInner(gp);
        Object.defineProperty(window, '__GamePushUnityBridge', {
            configurable: true,
            value: GamePush,
        });

        if (showPreloaderAd == 'True') {
            gp.ads.showPreloader();
        }

        // if (autocallGameReady != null && parseFloat(autocallGameReady) > 0) {
        //     setTimeout(() => {
        //         gp.gameStart();
        //         gp.logger.log("GameReady autocall");
        //         gp.logger.log(autocallGameReady);
        //     }, parseFloat(autocallGameReady));
        // }

        gp.player.ready.finally( async () => {
            await _unityInnerAwaiter.ready;
            SendMessage('GamePushSDK', 'CallOnSDKReady');
            
        });

        
    };

    (function loadGamePushSdk(urls, projectId, publicToken) {
        var query = '?projectId=' + encodeURIComponent(projectId) +
            '&publicToken=' + encodeURIComponent(publicToken) +
            '&callback=onGPInit';
        var override = window.__GS_BOOT_CFG__ && window.__GS_BOOT_CFG__.sdkSrc;
        var list = override ? [override] : urls.slice();
        var loading = false;
        var tried = {};
        var ready = [];
        var headsDone = 0;
        var failed = false;

        function fail() {
            if (failed) return;
            failed = true;
            if (typeof window.onGPError === 'function') window.onGPError();
        }

        function inject(url) {
            if (loading || tried[url] || failed) return;
            loading = true;
            tried[url] = true;
            var script = document.createElement('script');
            script.async = 1;
            script.src = url + query;
            script.onerror = function () {
                script.remove();
                loading = false;
                next();
            };
            document.head.appendChild(script);
        }

        function next() {
            var i;
            for (i = 0; i < ready.length; i++) {
                if (!tried[ready[i]]) {
                    inject(ready[i]);
                    return;
                }
            }
            for (i = 0; i < list.length; i++) {
                if (!tried[list[i]]) {
                    inject(list[i]);
                    return;
                }
            }
            if (!loading) fail();
        }

        if (override) {
            inject(override);
            return;
        }

        list.forEach(function (url) {
            if (!/^https?:/i.test(url)) {
                headsDone++;
                if (headsDone >= list.length && !loading) next();
                return;
            }
            var done = function (ok) {
                headsDone++;
                if (ok) {
                    ready.push(url);
                    inject(url);
                } else if (headsDone >= list.length && !loading) {
                    next();
                }
            };
            try {
                fetch(url, { method: 'HEAD' }).then(function (res) {
                    done(!!(res && res.ok));
                }).catch(function () { done(false); });
            } catch (err) {
                done(false);
            }
        });

        setTimeout(function () {
            if (!loading) next();
        }, 5000);
    })(
        [
            'https://gs.eponesh.com/sdk/game-score.js',
            'https://s3.gamepush.com/files/gs/sdk/game-score.js',
            'https://s3-eu.gamepush.com/sdk/game-score.js',
            'https://gamepush.com/sdk/game-score.js',
            'https://gs.eponesh.com/sdk/gamepush.js',
            'https://s3.eponesh.com/files/gs/sdk/gamepush.js',
            'TemplateData/gp_bundle/gamepush.js'
        ],
        dataProjectId,
        dataPublicToken
    );
}, 0);
