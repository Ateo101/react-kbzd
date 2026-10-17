using System;
using CrimeEmpire.Bosses;
using CrimeEmpire.Economy;
using CrimeEmpire.Save;
using UnityEngine;

namespace CrimeEmpire.Core
{
    /// <summary>
    /// Game-wide state: wallet, progress, bosses, save. Survives scene loads.
    /// Created on first access, so any scene can be played straight from the editor.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public sealed class GameServices : MonoBehaviour
    {
        [SerializeField] float autosaveInterval = 30f;
        [SerializeField] long startingCoins = 500;
        [SerializeField] long startingContrabandCap = 500;

        static GameServices _instance;
        static bool _quitting;

        public static GameServices Instance
        {
            get
            {
                if (_instance == null && !_quitting)
                    new GameObject(nameof(GameServices)).AddComponent<GameServices>();
                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        public Wallet Wallet { get; private set; }
        public ProgressState Progress { get; private set; }
        public BossRoster Bosses { get; private set; }

        /// <summary>Fires when the app comes back after being away (cold start or resume). Offline income listens.</summary>
        public event Action<TimeSpan> ReturnedAfterAway;

        SaveSystem _save;
        TimeSpan _pendingAway;
        DateTime _pausedAtUtc;
        float _nextAutosave;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            _instance = null;
            _quitting = false;
        }

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;

            _save = new SaveSystem("save.json");
            Wallet = new Wallet();
            Progress = new ProgressState();
            Bosses = new BossRoster();
            Wallet.SetCap(CurrencyType.Contraband, startingContrabandCap);

            var data = _save.Load();
            if (data != null)
            {
                data.ApplyTo(Wallet, Progress, Bosses);
                _pendingAway = ClampAway(DateTime.UtcNow - new DateTime(data.lastSeenUtcTicks, DateTimeKind.Utc));
            }
            else
            {
                Wallet.Add(CurrencyType.Coins, startingCoins);
            }

            _nextAutosave = Time.unscaledTime + autosaveInterval;
        }

        /// <summary>Returns time away since the last session and clears it, so it is paid out only once.</summary>
        public TimeSpan ConsumeTimeAway()
        {
            var away = _pendingAway;
            _pendingAway = TimeSpan.Zero;
            return away;
        }

        public void Save() => _save.Write(SaveData.Capture(Wallet, Progress, Bosses, DateTime.UtcNow));

        void Update()
        {
            if (Time.unscaledTime < _nextAutosave) return;
            _nextAutosave = Time.unscaledTime + autosaveInterval;
            Save();
        }

        void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                _pausedAtUtc = DateTime.UtcNow;
                Save();
                return;
            }

            if (_pausedAtUtc == default) return;
            _pendingAway += ClampAway(DateTime.UtcNow - _pausedAtUtc);
            _pausedAtUtc = default;
            ReturnedAfterAway?.Invoke(_pendingAway);
        }

        void OnApplicationQuit()
        {
            _quitting = true;
            Save();
        }

        // A device clock moved backwards gives negative time; never pay for that.
        static TimeSpan ClampAway(TimeSpan away) => away < TimeSpan.Zero ? TimeSpan.Zero : away;
    }
}
