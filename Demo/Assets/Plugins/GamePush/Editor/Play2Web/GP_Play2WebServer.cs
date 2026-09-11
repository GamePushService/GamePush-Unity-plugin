using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace GamePushEditor.Play2Web
{
    sealed class GP_Play2WebServer
    {
        const int DefaultPort = 8765;

        readonly string _root;
        HttpListener _listener;
        Thread _thread;
        volatile bool _running;
        readonly List<SseClient> _clients = new List<SseClient>();
        readonly Queue<string> _pending = new Queue<string>();
        readonly object _clientsLock = new object();
        int _lastEmptyLogMs;

        public int Port { get; private set; }
        public string Origin => "http://127.0.0.1:" + Port + "/";
        public event Action<string> Log;
        public event Action<string> ClientMessage;

        public GP_Play2WebServer(string root) => _root = root;

        public bool Start(int preferredPort)
        {
            Stop();
            Port = preferredPort > 0 ? preferredPort : DefaultPort;
            if (!IsPortFree(Port))
                Port = FindFreePort();

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://127.0.0.1:{Port}/");
            _listener.Prefixes.Add($"http://localhost:{Port}/");
            try
            {
                _listener.Start();
            }
            catch (Exception ex)
            {
                Log?.Invoke("HttpListener failed: " + ex.Message);
                return false;
            }

            _running = true;
            _thread = new Thread(ListenLoop) { IsBackground = true, Name = "GP_Play2Web" };
            _thread.Start();
            WritePortFile();
            Log?.Invoke("Serving " + Origin);
            return true;
        }

        public void Stop()
        {
            _running = false;
            lock (_clientsLock)
            {
                foreach (var client in _clients)
                    client.Close();
                _clients.Clear();
                _pending.Clear();
            }
            try { _listener?.Close(); } catch { /* ignored */ }
            _listener = null;
        }

        public void Broadcast(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;
            lock (_clientsLock)
            {
                if (_clients.Count == 0)
                {
                    if (_pending.Count < 64)
                        _pending.Enqueue(json);
                    var now = Environment.TickCount;
                    if (now - _lastEmptyLogMs > 2000)
                    {
                        _lastEmptyLogMs = now;
                        Log?.Invoke("Unity→SDK queued, overlay EventSource not connected");
                    }
                    return;
                }
                FlushPendingLocked();
                FanoutLocked(json);
            }
        }

        void FlushPendingLocked()
        {
            while (_pending.Count > 0 && _clients.Count > 0)
                FanoutLocked(_pending.Dequeue());
        }

        void FanoutLocked(string json)
        {
            for (var i = _clients.Count - 1; i >= 0; i--)
            {
                var client = _clients[i];
                client.Send(json);
                if (client.IsDead)
                {
                    _clients.RemoveAt(i);
                    client.Close();
                }
            }
        }

        void ListenLoop()
        {
            while (_running && _listener != null && _listener.IsListening)
            {
                try
                {
                    var context = _listener.GetContext();
                    ThreadPool.QueueUserWorkItem(_ => Handle(context));
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    if (_running)
                        Log?.Invoke(ex.Message);
                }
            }
        }

        void Handle(HttpListenerContext context)
        {
            try
            {
                if (context.Request.HttpMethod == "OPTIONS")
                {
                    WriteCors(context.Response);
                    context.Response.StatusCode = 204;
                    context.Response.Close();
                    return;
                }

                var path = (context.Request.Url?.AbsolutePath ?? "/").TrimEnd('/');
                if (string.IsNullOrEmpty(path))
                    path = "/";

                if (path.Equals("/events", StringComparison.OrdinalIgnoreCase))
                {
                    HandleEvents(context);
                    return;
                }

                if (path.Equals("/from-browser", StringComparison.OrdinalIgnoreCase))
                {
                    HandleFromBrowser(context);
                    return;
                }

                ServeFile(context);
            }
            catch (Exception ex)
            {
                Log?.Invoke(ex.Message);
                try { context.Response.Abort(); } catch { /* ignored */ }
            }
        }

        void HandleEvents(HttpListenerContext context)
        {
            var response = context.Response;
            WriteCors(response);
            response.StatusCode = 200;
            response.ContentType = "text/event-stream; charset=utf-8";
            response.Headers["Cache-Control"] = "no-cache";
            response.Headers["Connection"] = "keep-alive";
            response.Headers["X-Accel-Buffering"] = "no";
            response.SendChunked = true;

            var client = new SseClient(response);
            lock (_clientsLock)
            {
                _clients.Add(client);
                FlushPendingLocked();
            }
            Log?.Invoke("Browser connected (" + _clients.Count + " EventSource)");

            try
            {
                while (_running && !client.IsDead)
                {
                    client.Ping();
                    for (var i = 0; i < 50 && _running && !client.IsDead; i++)
                        Thread.Sleep(100);
                }
            }
            finally
            {
                lock (_clientsLock)
                    _clients.Remove(client);
                client.Close();
            }
        }

        void HandleFromBrowser(HttpListenerContext context)
        {
            string body;
            using (var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                body = reader.ReadToEnd();

            if (!string.IsNullOrEmpty(body))
                ClientMessage?.Invoke(body);

            var bytes = Encoding.UTF8.GetBytes("ok");
            WriteCors(context.Response);
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.ContentLength64 = bytes.Length;
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            context.Response.Close();
        }

        static void WriteCors(HttpListenerResponse response)
        {
            response.Headers["Access-Control-Allow-Origin"] = "*";
            response.Headers["Access-Control-Allow-Methods"] = "GET, POST, OPTIONS";
            response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
        }

        void ServeFile(HttpListenerContext context)
        {
            var path = context.Request.Url.AbsolutePath;
            if (string.IsNullOrEmpty(path) || path == "/")
                path = "/index.html";
            path = Uri.UnescapeDataString(path).TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            if (path.Contains(".."))
            {
                context.Response.StatusCode = 403;
                context.Response.Close();
                return;
            }

            var full = Path.Combine(_root, path);
            if (!File.Exists(full))
            {
                context.Response.StatusCode = 404;
                var bytes = Encoding.UTF8.GetBytes("Not found");
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                context.Response.Close();
                return;
            }

            var ext = Path.GetExtension(full).ToLowerInvariant();
            context.Response.ContentType = Mime(ext);
            context.Response.Headers["Cache-Control"] = "no-cache";
            WriteCors(context.Response);
            var data = File.ReadAllBytes(full);
            context.Response.ContentLength64 = data.Length;
            context.Response.OutputStream.Write(data, 0, data.Length);
            context.Response.Close();
        }

        static string Mime(string ext)
        {
            switch (ext)
            {
                case ".html": return "text/html; charset=utf-8";
                case ".js": return "application/javascript; charset=utf-8";
                case ".css": return "text/css; charset=utf-8";
                case ".json": return "application/json";
                case ".png": return "image/png";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".svg": return "image/svg+xml";
                case ".wasm": return "application/wasm";
                default: return "application/octet-stream";
            }
        }

        void WritePortFile()
        {
            var dir = Path.Combine(Directory.GetParent(Application.dataPath)?.FullName ?? "", "Temp", "GamePushPlay2Web");
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "port.txt"), Port.ToString());
        }

        static bool IsPortFree(int port)
        {
            try
            {
                var listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();
                listener.Stop();
                return true;
            }
            catch
            {
                return false;
            }
        }

        static int FindFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        sealed class SseClient
        {
            readonly HttpListenerResponse _response;
            readonly object _writeLock = new object();
            volatile bool _dead;

            public SseClient(HttpListenerResponse response) => _response = response;

            public bool IsDead => _dead;

            public void Send(string json)
            {
                WriteRaw("data: " + json + "\n\n");
            }

            public void Ping()
            {
                WriteRaw(": ping\n\n");
            }

            public void Close()
            {
                _dead = true;
                try { _response.Close(); } catch { /* ignored */ }
            }

            void WriteRaw(string text)
            {
                if (_dead)
                    return;
                lock (_writeLock)
                {
                    if (_dead)
                        return;
                    try
                    {
                        var bytes = Encoding.UTF8.GetBytes(text);
                        _response.OutputStream.Write(bytes, 0, bytes.Length);
                        _response.OutputStream.Flush();
                    }
                    catch
                    {
                        _dead = true;
                    }
                }
            }
        }
    }
}
