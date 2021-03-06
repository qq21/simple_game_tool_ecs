﻿public class TimerEvent : IPoolObject
{
    int _interval;
    int _count;
    int _delay;
    ITimerObject _tick;
    TimerCallback _callback;

    int _lastInvokeTime;

    bool _isDelayed;

    public bool IsInUse
    {
        get;
        set;
    }

    public void Clear()
    {
        _interval = 0;
        _count = 0;
        _delay = 0;
        _tick = null;
        _callback = null;

        _lastInvokeTime = 0;

        _isDelayed = false;
    }

    public void Init(int delay, int count, int interval, ITimerObject tick, TimerCallback timerCallback = null)
    {
        _delay = delay;
        _count = count;
        _interval = interval;
        _tick = tick;
        _callback = timerCallback;

        var gameSystemData = WorldManager.Instance.GameCore.GetData<Data.GameSystemData>();
        _lastInvokeTime = gameSystemData.unscaleTime;

        _isDelayed = false;
    }

    bool CanInvoke(int time)
    {
        var delta = time - _lastInvokeTime;
        if (!_isDelayed && delta >= _delay)
        {
            return true;
        }

        if (_isDelayed && delta >= _interval)
        {
            return true;
        }

        return false;
    }

    public void Invoke(int time)
    {
        if (CanInvoke(time))
        {
            if (!_isDelayed)
            {
                _isDelayed = true;
            }

            if (_tick != null)
            {
                _tick.Tick();
            }

            if (_callback != null)
            {
                _callback();
            }

            if (_count > 0)
            {
                _count--;
            }

            _lastInvokeTime = time;
        }
    }

    public bool IsFinished()
    {
        if (_count == 0)
            return true;

        return false;
    }
}
