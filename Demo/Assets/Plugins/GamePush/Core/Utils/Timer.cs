namespace GamePush.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class Timer
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly int _delayMilliseconds;
        private readonly Action _callback;

        public Timer(int delayMilliseconds, Action callback)
        {
            _delayMilliseconds = delayMilliseconds;
            _callback = callback;
        }

        public void Start()
        {
            Stop();
            _cancellationTokenSource = new CancellationTokenSource();

            _ = RunTimerAsync(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
        }

        private async Task RunTimerAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(_delayMilliseconds, token);
                if (token.IsCancellationRequested) return;

                _callback?.Invoke();
            }
            catch (TaskCanceledException)
            {
               //
            }
        }
    }
}