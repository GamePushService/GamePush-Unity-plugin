let GamePush;

function _GP(){
    return GamePush || window.GamePush;
}

function _ToBuff(value){
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

setTimeout(() => {
    if ('GamePushUnity' in window) return;

    window.onGPError = async () => {
        await _unityInnerAwaiter.ready;
        SendMessage('GamePushSDK', 'CallOnSDKError');
    };

    window.onGPInit = async (gp) => {

        GamePush = new GamePushUnityInner(gp);

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
