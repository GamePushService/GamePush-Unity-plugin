using System;
using System.Security.Authentication;
using Newtonsoft.Json;
using WebSocketSharp;

namespace Xsolla.Core.Centrifugo
{
	internal class CentrifugoClient
	{
		private const string PingMessage = "{}";
		private const string PongMessage = "{}";

		private readonly WebSocket WebSocket;
		private readonly MainThreadExecutor MainThreadMainThreadExecutor;
		private readonly bool IsLog;
		private readonly bool IsLogPing;

		public event Action Opened;
		public event Action<string> MessageReceived;
		public event Action<string, Exception> Error;
		public event Action<string> Closed;

		public bool IsAlive => WebSocket.IsAlive;

		public CentrifugoClient(string url, MainThreadExecutor mainThreadExecutor)
		{
			WebSocket = new WebSocket(url);
			WebSocket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;

			MainThreadMainThreadExecutor = mainThreadExecutor;
			IsLog = false;
			IsLogPing = false;
		}

		public void Connect()
		{
			WebSocket.OnOpen += OnSocketOpen;
			WebSocket.OnMessage += OnSocketMessage;
			WebSocket.OnError += OnSocketError;
			WebSocket.OnClose += OnSocketClose;
			WebSocket.Connect();
		}

		public void Disconnect()
		{
			WebSocket.OnOpen -= OnSocketOpen;
			WebSocket.OnMessage -= OnSocketMessage;
			WebSocket.OnError -= OnSocketError;
			WebSocket.OnClose -= OnSocketClose;
			WebSocket.Close();
		}

		public void Send(object data)
		{
			var json = JsonConvert.SerializeObject(data);
			Log($"Websocket send data: {json}");
			WebSocket.Send(json);
		}

		private void OnSocketOpen(object sender, EventArgs _)
		{
			MainThreadMainThreadExecutor.Enqueue(() => {
				Log("Websocket open");
				Opened?.Invoke();
			});
		}

		private void OnSocketMessage(object _, MessageEventArgs e)
		{
			if (e.Data == PingMessage)
			{
				MainThreadMainThreadExecutor.Enqueue(() => {
					if (IsLogPing)
						Log("Websocket handshake");
				});

				WebSocket.Send(PongMessage);
				return;
			}

			MainThreadMainThreadExecutor.Enqueue(() => {
				Log($"Websocket message. IsPing: {e.IsText}. IsText: {e.IsText}. IsBinary: {e.IsBinary}. Data: {e.Data}");
				MessageReceived?.Invoke(e.Data);
			});
		}

		private void OnSocketError(object _, ErrorEventArgs e)
		{
			MainThreadMainThreadExecutor.Enqueue(() => {
				Log($"Websocket error. Message: {e.Message}. Exception: {e.Exception}");
				Error?.Invoke(e.Message, e.Exception);
			});
		}

		private void OnSocketClose(object _, CloseEventArgs e)
		{
			MainThreadMainThreadExecutor.Enqueue(() => {
				Log($"Websocket close. Reason: {e.Reason}. Code: {e.Code}. WasClean: {e.WasClean}");
				Closed?.Invoke(e.Reason);
			});
		}

		private void Log(string message)
		{
			if (IsLog)
				XDebug.Log($"Centrifugo. {message}");
		}
	}
}