let GamePush;

function _GP(){
    return GamePush || window.GamePush;
}

setTimeout(() => {
    if ('GamePushUnity' in window) return;

    window.onGPError = () => {
        SendMessage('GamePushSDK', 'CallOnSDKError');
    };

    window.onGPInit = async (gp) => {
        if (showPreloaderAd == 'True') {
            gp.ads.showPreloader();
        }

        GamePush = new GamePushUnityInner(gp);
        gp.player.ready.finally(() => {
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
