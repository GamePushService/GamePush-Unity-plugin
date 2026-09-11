var LibraryGPNativeWs = {
  $GPNativeWs: {
    ws: null,
    nextId: 1,
    gameObject: "GamePushSDK",
    endpoint: "",
    token: "",
    wanted: {},
    pending: {},
    connected: false,
    closedByUser: false,
    reconnectTimer: 0,
    reconnectDelay: 500,

    clearTimer: function () {
      if (GPNativeWs.reconnectTimer) {
        clearTimeout(GPNativeWs.reconnectTimer);
        GPNativeWs.reconnectTimer = 0;
      }
    },

    log: function (msg) {
      if (typeof gpFullLogs !== "undefined" && gpFullLogs)
        console.log("[GP] " + msg);
    },

    sendJson: function (obj) {
      if (!GPNativeWs.ws || GPNativeWs.ws.readyState !== 1) return false;
      GPNativeWs.ws.send(JSON.stringify(obj));
      return true;
    },

    sendCmd: function (cmd) {
      var id = GPNativeWs.nextId++;
      cmd.id = id;
      if (cmd.subscribe && cmd.subscribe.channel)
        GPNativeWs.pending[id] = cmd.subscribe.channel;
      return GPNativeWs.sendJson(cmd);
    },

    isPing: function (raw, msg) {
      if (typeof raw === "string") {
        var trimmed = raw.replace(/^\uFEFF/, "").trim();
        if (trimmed === "{}" || trimmed === "[]") return true;
      }
      if (!msg || typeof msg !== "object") return false;
      var keys = Object.keys(msg);
      if (keys.length === 0) return true;
      if (keys.length === 1 && keys[0] === "ping") return true;
      return false;
    },

    noteDisconnect: function (info) {
      var reason = "disconnect";
      if (info && typeof info === "object")
        reason = String(info.reason || info.code || "disconnect");
      GPNativeWs.connected = false;
      GPNativeWs.log("ws disconnect " + reason);
      SendMessage(GPNativeWs.gameObject, "OnNativeWsReconnecting", reason);
    },

    handleOne: function (raw, msg) {
      if (GPNativeWs.isPing(raw, msg)) {
        if (GPNativeWs.ws && GPNativeWs.ws.readyState === 1)
          GPNativeWs.sendJson({});
        return;
      }
      if (!msg) return;
      if (msg.error) {
        var errCh = msg.id && GPNativeWs.pending[msg.id] ? GPNativeWs.pending[msg.id] : "";
        delete GPNativeWs.pending[msg.id];
        var errText = (msg.error && (msg.error.message || msg.error.code)) || "error";
        console.warn("[GP] ws error " + errText + (errCh ? " ch=" + errCh : ""));
        return;
      }
      if (msg.disconnect) {
        GPNativeWs.noteDisconnect(msg.disconnect);
        return;
      }
      if (msg.push && msg.push.disconnect) {
        GPNativeWs.noteDisconnect(msg.push.disconnect);
        return;
      }
      if (msg.connect) {
        GPNativeWs.connected = true;
        GPNativeWs.reconnectDelay = 500;
        GPNativeWs.log("ws connected");
        SendMessage(GPNativeWs.gameObject, "OnNativeWsOpen", "");
        GPNativeWs.flushSubs();
      }
      var subCh = "";
      if (msg.subscribe && msg.subscribe.channel)
        subCh = msg.subscribe.channel;
      if (!subCh && msg.id && GPNativeWs.pending[msg.id])
        subCh = GPNativeWs.pending[msg.id];
      if (subCh) {
        delete GPNativeWs.pending[msg.id];
        GPNativeWs.log("ws subscribed " + subCh);
        SendMessage(GPNativeWs.gameObject, "OnNativeWsSubscribed", subCh);
      }
      if (msg.push && msg.push.pub) {
        var channel = msg.push.channel || "";
        var data = msg.push.pub.data;
        var bytes = typeof data === "string" ? data : JSON.stringify(data || {});
        var b64 = btoa(unescape(encodeURIComponent(bytes)));
        SendMessage(GPNativeWs.gameObject, "OnNativeWsMessage", channel + "|" + b64);
      }
    },

    handleFrame: function (raw) {
      if (raw && typeof raw !== "string" && raw.byteLength !== undefined)
        raw = new TextDecoder().decode(raw);
      if (typeof raw === "string" && GPNativeWs.isPing(raw, null)) {
        GPNativeWs.handleOne(raw, {});
        return;
      }
      var msg = null;
      try {
        msg = typeof raw === "string" ? JSON.parse(raw) : raw;
      } catch (e) {
        return;
      }
      if (Array.isArray(msg)) {
        var i;
        for (i = 0; i < msg.length; i++)
          GPNativeWs.handleOne(raw, msg[i]);
        return;
      }
      GPNativeWs.handleOne(raw, msg);
    },

    flushSubs: function () {
      var ch;
      for (ch in GPNativeWs.wanted) {
        if (!Object.prototype.hasOwnProperty.call(GPNativeWs.wanted, ch)) continue;
        GPNativeWs.log("ws subscribe " + ch);
        GPNativeWs.sendCmd({
          subscribe: { channel: ch, token: GPNativeWs.wanted[ch] || "" }
        });
      }
    },

    scheduleReconnect: function () {
      GPNativeWs.clearTimer();
      var delay = GPNativeWs.reconnectDelay;
      GPNativeWs.log("ws reconnect in " + delay + "ms");
      GPNativeWs.reconnectDelay = Math.min(20000, GPNativeWs.reconnectDelay * 2);
      GPNativeWs.reconnectTimer = setTimeout(function () {
        GPNativeWs.reconnectTimer = 0;
        if (GPNativeWs.closedByUser) return;
        GPNativeWs.openSocket();
      }, delay);
    },

    openSocket: function () {
      GPNativeWs.clearTimer();
      GPNativeWs.connected = false;
      try {
        if (GPNativeWs.ws) {
          try {
            GPNativeWs.ws.onclose = null;
            GPNativeWs.ws.onerror = null;
            GPNativeWs.ws.onmessage = null;
            GPNativeWs.ws.close();
          } catch (e) {}
        }
        var ws = new WebSocket(GPNativeWs.endpoint);
        ws.binaryType = "arraybuffer";
        GPNativeWs.ws = ws;
        ws.onopen = function () {
          GPNativeWs.nextId = 1;
          GPNativeWs.pending = {};
          GPNativeWs.log("ws open " + GPNativeWs.endpoint);
          GPNativeWs.sendJson({ id: GPNativeWs.nextId++, connect: { token: GPNativeWs.token || "" } });
        };
        ws.onmessage = function (ev) {
          GPNativeWs.handleFrame(ev.data);
        };
        ws.onclose = function (ev) {
          GPNativeWs.connected = false;
          var reason = String((ev && ev.reason) || "closed");
          var code = ev && ev.code != null ? ev.code : 0;
          GPNativeWs.log("ws close " + code + " " + reason);
          if (GPNativeWs.closedByUser) {
            SendMessage(GPNativeWs.gameObject, "OnNativeWsClose", reason);
            return;
          }
          SendMessage(GPNativeWs.gameObject, "OnNativeWsReconnecting", reason);
          GPNativeWs.scheduleReconnect();
        };
        ws.onerror = function () {};
      } catch (e) {
        GPNativeWs.connected = false;
        if (GPNativeWs.closedByUser) {
          SendMessage(GPNativeWs.gameObject, "OnNativeWsClose", String(e));
          return;
        }
        SendMessage(GPNativeWs.gameObject, "OnNativeWsReconnecting", String(e));
        GPNativeWs.scheduleReconnect();
      }
    }
  },

  GP_NativeWs_Connect: function (endpointPtr, tokenPtr) {
    var url = UTF8ToString(endpointPtr) || "";
    var token = UTF8ToString(tokenPtr) || "";
    url = url.replace("?format=protobuf", "").replace("&format=protobuf", "");
    GPNativeWs.endpoint = url;
    GPNativeWs.token = token;
    GPNativeWs.closedByUser = false;
    GPNativeWs.reconnectDelay = 500;
    GPNativeWs.pending = {};
    GPNativeWs.openSocket();
  },

  GP_NativeWs_Subscribe: function (channelPtr, tokenPtr) {
    var channel = UTF8ToString(channelPtr) || "";
    var token = UTF8ToString(tokenPtr) || "";
    GPNativeWs.wanted[channel] = token;
    if (GPNativeWs.connected) {
      GPNativeWs.log("ws subscribe " + channel);
      GPNativeWs.sendCmd({ subscribe: { channel: channel, token: token } });
    }
  },

  GP_NativeWs_Publish: function (channelPtr, base64Ptr) {
    var channel = UTF8ToString(channelPtr) || "";
    var b64 = UTF8ToString(base64Ptr) || "";
    if (!GPNativeWs.connected) return;
    var json = "";
    try { json = decodeURIComponent(escape(atob(b64))); } catch (e) { json = ""; }
    var data;
    try { data = JSON.parse(json); } catch (e) { data = json; }
    GPNativeWs.sendCmd({
      publish: { channel: channel, data: data }
    });
  },

  GP_NativeWs_Unsubscribe: function (channelPtr) {
    var channel = UTF8ToString(channelPtr) || "";
    delete GPNativeWs.wanted[channel];
    if (!GPNativeWs.connected) return;
    GPNativeWs.sendCmd({
      unsubscribe: { channel: channel }
    });
  },

  GP_NativeWs_Close: function () {
    GPNativeWs.closedByUser = true;
    GPNativeWs.clearTimer();
    GPNativeWs.wanted = {};
    GPNativeWs.pending = {};
    GPNativeWs.connected = false;
    if (!GPNativeWs.ws) return;
    try { GPNativeWs.ws.close(); } catch (e) {}
    GPNativeWs.ws = null;
  }
};

autoAddDeps(LibraryGPNativeWs, "$GPNativeWs");
mergeInto(LibraryManager.library, LibraryGPNativeWs);
