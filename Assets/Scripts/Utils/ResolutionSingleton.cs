using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils {
    public class ResolutionSingleton : PersistentSingleton<ResolutionSingleton> {
        private const int MIN_RESOLUTION_WIDTH = 800;

        public static readonly string MIDDLE_SUFFIX = " x ";

        public HashSet<Resolution> AvailableResolutions { get; private set; }

        public List<string> Resolutions => AvailableResolutions
                                            .Select(resolution => resolution.width + MIDDLE_SUFFIX + resolution.height)
                                            .Distinct()
                                            .ToList();

        private readonly PreferencesStorage _preferencesStorage = new();

        public bool GetPrefsWindowed() => _preferencesStorage.GetWindowed();

        public (int, int) GetPrefsResolution() => _preferencesStorage.GetResolution();

        protected override void Awake() {
            base.Awake();
            AvailableResolutions = Screen.resolutions
                .Where(resolution => resolution.width >= MIN_RESOLUTION_WIDTH &&
                        resolution.width % 16 == 0 && resolution.height % 9 == 0)
                .ToHashSet();
        }

        public int FindIndex(Vector2Int lastResolution) {
            return AvailableResolutions.ToList().FindIndex(resolution =>
                resolution.width == lastResolution.x && resolution.height == lastResolution.y);
        }

        private FullScreenMode GetFullScreen(bool windowedToggled) => windowedToggled ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;

        public void SetWindowed(bool windowedToggled) {
            Screen.fullScreenMode = GetFullScreen(windowedToggled);
            _preferencesStorage.SetWindowed(windowedToggled);
        }

        public void SetResolution(int width, int height, bool windowedToggled) {
            Screen.SetResolution(width, height, GetFullScreen(windowedToggled));
            _preferencesStorage.SetResolution(width, height);
        }
    }
}