using Timer = System.Timers.Timer;

namespace HRtoVRChat_OSC
{
    public class CustomTimer
    {
        public bool IsRunning { get; private set; }
        private Timer _timer;

        public CustomTimer(int ms, Action<CustomTimer> callback)
        {
            if(_timer != null)
                Close();
            _timer = new Timer(ms);
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, args) => { callback.Invoke(this); };
            _timer.Start();
            IsRunning = true;
        }

        public void Close()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Close();
            }
            IsRunning = false;
        }
    }

    public class ExecuteInTime
    {
        public bool IsWaiting { get; private set; }
        private Timer _timer;

        public ExecuteInTime(int ms, Action<ExecuteInTime> callback)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Close();
            }
            _timer = new Timer(ms);
            _timer.AutoReset = false;
            _timer.Elapsed += (sender, args) =>
            {
                callback.Invoke(this);
                IsWaiting = false;
                _timer.Stop();
                _timer.Close();
            };
            _timer.Start();
            IsWaiting = true;
        }
    }
}