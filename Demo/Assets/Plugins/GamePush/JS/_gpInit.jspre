let GamePush;

function _GP(){
    return GamePush || window.GamePush;
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

setTimeout(() => {
    if ('GamePushUnity' in window) return;

    window.onGPError = async () => {
        await _waitFor((w) => "_malloc" in w);
        SendMessage('GamePushSDK', 'CallOnSDKError');
    };

    window.onGPInit = async (gp) => {
        if (showPreloaderAd == 'True') {
            gp.ads.showPreloader();
        }

        GamePush = new GamePushUnityInner(gp);
        gp.player.ready.finally( async () => {
            await _waitFor((w) => "_malloc" in w);
            SendMessage('GamePushSDK', 'CallOnSDKReady');
        });

        if (autocallGameReady != null && parseFloat(autocallGameReady) > 0) {
            setTimeout(() => gp.gameStart(), parseFloat(autocallGameReady));
        }
    };

    ((g, a, m, e) => {
        let o = () => {
            let p = document.createElement('script');
            (p.src = `${a[0]}?projectId=${m}&publicToken=${e}`),
                (p.onerror = () => {
                    a.shift(),
                        a.length > 0
                            ? (o(), p.remove())
                            : 'onGPError' in g && g.onGPError();
                }),
                document.head.appendChild(p);
        };
        o();
    })(
        window,
        [
            'https://gs.eponesh.com/sdk/gamepush.js',
            'https://s3.eponesh.com/files/gs/sdk/gamepush.js',
            'TemplateData/gp_bundle/gamepush.js'
        ],
        dataProjectId,
        dataPublicToken
    );
}, 0);
