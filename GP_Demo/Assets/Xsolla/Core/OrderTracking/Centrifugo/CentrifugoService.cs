using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Xsolla.Core.Centrifugo
{
	internal static class CentrifugoService
	{
		private const string Url = "wss://ws-store.xsolla.com/connection/websocket";
		private const float PingInterval = Constants.WEB_SOCKETS_PING_INTERVAL;
		private const float TimeoutLimit = Constants.WEB_SOCKETS_PING_LIMIT;

		private static readonly List<OrderTrackerByCentrifugo> Trackers = new List<OrderTrackerByCentrifugo>();
		private static CentrifugoClient CentrifugoClient;
		private static MainThreadExecutor MainThreadExecutor;
		private static Coroutine PingCoroutine;

		private static float PingCounter;
		private static float TimeoutCounter;

		public static event Action<OrderStatusData> OrderStatusUpdate;
		public static event Action Error;
		public static event Action Close;

		public static void AddTracker(OrderTrackerByCentrifugo tracker)
		{
			Trackers.Add(tracker);

			if (!MainThreadExecutor)
				MainThreadExecutor = MainThreadExecutor.Instance;

			if (CentrifugoClient == null)
				CreateCentrifugoClient();
		}

		public static void RemoveTracker(OrderTrackerByCentrifugo tracker)
		{
			Trackers.Remove(tracker);

			if (Trackers.Count == 0 && CentrifugoClient != null)
				TerminateCentrifugoClient();
		}

		private static void CreateCentrifugoClient()
		{
			CentrifugoClient = new CentrifugoClient(Url, MainThreadExecutor);
			CentrifugoClient.MessageReceived += OnCentrifugoMessageReceived;
			CentrifugoClient.Error += OnCentrifugoError;
			CentrifugoClient.Closed += OnCentrifugoClosed;

			var connectionMessage = new ConnectionMessage {
				connect = new ConnectCommand {
					data = new ConnectionData {
						auth = XsollaToken.AccessToken,
						project_id = int.Parse(XsollaSettings.StoreProjectId)
					}
				},
				id = Mathf.Abs(Guid.NewGuid().ToString().GetHashCode())
			};

			CentrifugoClient.Connect();
			CentrifugoClient.Send(connectionMessage);
			PingCoroutine = CoroutinesExecutor.Run(DoPing());

			XDebug.Log("Centrifugo client created");
		}

		private static void TerminateCentrifugoClient()
		{
			CentrifugoClient.MessageReceived -= OnCentrifugoMessageReceived;
			CentrifugoClient.Error -= OnCentrifugoError;
			CentrifugoClient.Closed -= OnCentrifugoClosed;
			CentrifugoClient.Disconnect();
			CentrifugoClient = null;

			CoroutinesExecutor.Stop(PingCoroutine);
			PingCoroutine = null;

			XDebug.Log("Centrifugo client terminated");
		}

		private static void OnCentrifugoMessageReceived(string message)
		{
			PingCounter = 0;

			var orderStatusData = TryParseOrderStatusData(message);
			if (orderStatusData != null)
				OrderStatusUpdate?.Invoke(orderStatusData);
		}

		private static void OnCentrifugoError(string message, Exception exception)
		{
			Error?.Invoke();
		}

		private static void OnCentrifugoClosed(string reason)
		{
			Close?.Invoke();
		}

		private static IEnumerator DoPing()
		{
			const float deltaTime = 1f;
			while (true)
			{
				yield return new WaitForSeconds(deltaTime);
				PingCounter += deltaTime;

				if (PingCounter >= PingInterval)
				{
					PingCounter = 0;

					if (CentrifugoClient.IsAlive)
					{
						TimeoutCounter = 0;
					}
					else
					{
						TimeoutCounter += PingInterval;
						if (TimeoutCounter >= TimeoutLimit)
						{
							XDebug.LogError("Centrifugo connection timeout limit exceeded");
							OnCentrifugoClosed("Network problems");
							yield break;
						}
					}
				}
			}
		}

		private static OrderStatusData TryParseOrderStatusData(string message)
		{
			OrderStatusMessage statusMessage = null;

			try
			{
				statusMessage = JsonConvert.DeserializeObject<OrderStatusMessage>(message);
			}
			catch (Exception e)
			{
				XDebug.LogError($"Could not parse centrifugo message with exception: {e.Message}");
			}

			return statusMessage?.push?.pub?.data;
		}
	}
}