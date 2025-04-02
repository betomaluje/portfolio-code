using System;
using UnityEngine;

namespace Utils {
    public abstract class Timer {
        protected float initialTime;
        public float Time { get; set; }
        public bool IsRunning { get; protected set; }
        public float Duration => initialTime;

        public float Progress => Mathf.Clamp01(Time / initialTime);

        public event Action OnTimerStart = delegate { };
        public event Action OnTimerStop = delegate { };

        protected Timer(float value) {
            initialTime = value;
            IsRunning = false;
        }

        public virtual void Start() {
            Time = initialTime;
            if (!IsRunning) {
                IsRunning = true;
                OnTimerStart.Invoke();
            }
        }

        public void Stop() {
            if (IsRunning) {
                IsRunning = false;
                OnTimerStop.Invoke();
            }
        }

        public abstract void Tick(float deltaTime);
        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public virtual void Reset() => Time = initialTime;

        public virtual void Reset(float newTime) {
            initialTime = newTime;
            Reset();
        }
    }

    public class CountdownTimer : Timer {
        public CountdownTimer(float value) : base(value) { }

        public override void Tick(float deltaTime) {
            if (IsRunning && Time > 0) {
                Time -= deltaTime;
            }

            if (IsRunning && Time <= 0) {
                Stop();
            }
        }

        public bool IsFinished => Time <= 0;
    }

    public class NotifyingCountdownTimer : Timer {
        protected float _interval = 1f;
        private float _time;
        public event Action OnInterval = delegate { };

        public NotifyingCountdownTimer(float value, float interval) : base(value) {
            // interval can't be bigger than value
            if (interval > value) {
                interval = value;
            }
            _interval = interval;
        }

        public override void Start() {
            base.Start();
            _time = 0;
        }

        public override void Tick(float deltaTime) {
            if (IsRunning && Time > 0) {
                Time -= deltaTime;
                _time += deltaTime;

                if (_time >= _interval) {
                    OnInterval?.Invoke();
                    _time = 0;
                }
            }

            if (IsRunning && Time <= 0) {
                Stop();
            }
        }
    }

    public class StopwatchTimer : Timer {
        public StopwatchTimer() : base(0) { }

        public override void Tick(float deltaTime) {
            if (IsRunning) {
                Time += deltaTime;
            }
        }

        public float GetTime() => Time;
    }
}