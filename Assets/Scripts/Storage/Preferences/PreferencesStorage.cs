using UnityEngine;

public class PreferencesStorage {
    private readonly string PREFS_FIRST_TIME = "first_time";

    private const int TRUE = 1;
    private const int FALSE = 0;

    #region Events

    public static readonly string EVENT_AIM = "Aim";
    public static readonly string EVENT_BLOOD = "Blood";
    public static readonly string EVENT_FOG_OF_WAR = "FoW";
    public static readonly string EVENT_MINIMAP = "Minimap";
    public static readonly string EVENT_FPS = "FPS";
    public static readonly string EVENT_VERSION = "Version";

    public static event System.Action<string, bool> OnPreferenceChanged = delegate { };

    #endregion

    public void SetFirstTime(bool firstTime) {
        PlayerPrefs.SetInt(PREFS_FIRST_TIME, firstTime ? TRUE : FALSE);
    }

    public bool GetFirstTime() => PlayerPrefs.GetInt(PREFS_FIRST_TIME, TRUE) == TRUE;

    private readonly string PREFS_MINIMAP = "prefs_show_minimap";
    private readonly string PREFS_AIM_ASSIST = "prefs_aim_assist";
    private readonly string PREFS_BLOOD = "prefs_show_blood";
    private readonly string PREFS_FOG_OF_WAR = "prefs_show_fow";
    private readonly string PREFS_FPS = "prefs_show_fps";
    private readonly string PREFS_VERSION = "prefs_show_version";

    #region Aim Assist
    public void SetAimAssist(bool active) {
        PlayerPrefs.SetInt(PREFS_AIM_ASSIST, active ? TRUE : FALSE);
        OnPreferenceChanged?.Invoke(EVENT_AIM, active);
    }

    public bool GetAimAssist() => PlayerPrefs.GetInt(PREFS_AIM_ASSIST, TRUE) == TRUE;
    #endregion

    #region Blood
    public void SetShowBlood(bool show) {
        PlayerPrefs.SetInt(PREFS_BLOOD, show ? TRUE : FALSE);
        OnPreferenceChanged?.Invoke(EVENT_BLOOD, show);
    }

    public bool GetShowBlood() => PlayerPrefs.GetInt(PREFS_BLOOD, TRUE) == TRUE;
    #endregion

    #region Fog of War
    public void SetShowFogOfWar(bool show) {
        PlayerPrefs.SetInt(PREFS_FOG_OF_WAR, show ? TRUE : FALSE);
        OnPreferenceChanged?.Invoke(EVENT_FOG_OF_WAR, show);
    }

    public bool GetShowFogOfWar() => PlayerPrefs.GetInt(PREFS_FOG_OF_WAR, TRUE) == TRUE;
    #endregion

    #region FPS
    public void SetShowFPS(bool show) {
        PlayerPrefs.SetInt(PREFS_FPS, show ? TRUE : FALSE);
        OnPreferenceChanged?.Invoke(EVENT_FPS, show);
    }

    public bool GetShowFPS() => PlayerPrefs.GetInt(PREFS_FPS, FALSE) == TRUE;
    #endregion

    #region Minimap
    public void SetShowMinimap(bool show) {
        PlayerPrefs.SetInt(PREFS_MINIMAP, show ? TRUE : FALSE);
        OnPreferenceChanged?.Invoke(EVENT_MINIMAP, show);
    }

    public bool GetShowMinimap() => PlayerPrefs.GetInt(PREFS_MINIMAP, FALSE) == TRUE;
    #endregion

    #region Version
    public void SetShowVersion(bool show) {
        PlayerPrefs.SetInt(PREFS_VERSION, show ? TRUE : FALSE);
        OnPreferenceChanged?.Invoke(EVENT_VERSION, show);
    }

    public bool GetShowVersion() => PlayerPrefs.GetInt(PREFS_VERSION, FALSE) == TRUE;
    #endregion

    #region Display Settings
    private readonly string PREFS_RESOLUTION = "prefs_resolution";
    private readonly string PREFS_WINDOWED = "prefs_windowed";

    public void SetWindowed(bool isWindowed) {
        PlayerPrefs.SetInt(PREFS_WINDOWED, isWindowed ? TRUE : FALSE);
    }

    public bool GetWindowed() => PlayerPrefs.GetInt(PREFS_WINDOWED, FALSE) == TRUE;

    public (int, int) GetResolution() {
        int width = PlayerPrefs.GetInt(PREFS_RESOLUTION + "_width", -1);
        int height = PlayerPrefs.GetInt(PREFS_RESOLUTION + "_height", -1);
        return (width, height);
    }

    public void SetResolution(int width, int height) {
        PlayerPrefs.SetInt(PREFS_RESOLUTION + "_width", width);
        PlayerPrefs.SetInt(PREFS_RESOLUTION + "_height", height);
    }
    #endregion
}