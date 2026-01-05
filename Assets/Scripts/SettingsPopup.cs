using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPopup : MonoBehaviour
{
    [Header("Root")]
    public GameObject popupRoot;       // SettingsPopup
    public GameObject mainMenuPanel;   // MainMenuPanel

    [Header("Tabs")]
    public GameObject graphicsPanel;
    public GameObject audioPanel;
    public GameObject gameplayPanel;
    public GameObject controlsPanel;

    [Header("Graphics UI")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;
    public Toggle vSyncToggle;

    [Header("Audio UI")]
    public Slider masterVolumeSlider;

    private SettingsData _current;
    private SettingsData _snapshot;

    private Resolution[] _resolutions;

    [System.Serializable]
    private struct SettingsData
    {
        public int resolutionIndex;
        public int qualityIndex;
        public bool fullscreen;
        public bool vSync;
        public float masterVolume;
    }

    // ----------------- LIFECYCLE -----------------
    private void Awake()
    {
        // На старте попап скрыт
        if (popupRoot) popupRoot.SetActive(false);
    }

    private void Start()
    {
        // Заполняем UI списками (если UI назначен)
        SetupResolutions();
        SetupQuality();

        // Загружаем сохранённые настройки → применяем в UI и в игру
        _current = Load();
        ApplyToUI(_current);

        // ApplyToGame требует _resolutions (и иногда UI), но мы уже сделали SetupResolutions()
        ApplyToGame(_current);
    }

    // ----------------- OPEN/CLOSE -----------------
    public void Open()
    {
        // Запомнить текущее состояние UI для Cancel
        _snapshot = ReadFromUI();

        // Скрыть главное меню
        if (mainMenuPanel) mainMenuPanel.SetActive(false);

        // Показать попап
        if (popupRoot) popupRoot.SetActive(true);

        // Открыть вкладку по умолчанию
        ShowTab("graphics");
    }

    public void Close()
    {
        // Скрыть попап
        if (popupRoot) popupRoot.SetActive(false);

        // Вернуть главное меню
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    // ----------------- TABS -----------------
    public void ShowGraphics() => ShowTab("graphics");
    public void ShowAudio()    => ShowTab("audio");
    public void ShowGameplay() => ShowTab("gameplay");
    public void ShowControls() => ShowTab("controls");

    // Можно вызывать и из кнопок: ShowTab("audio")
    public void ShowTab(string tab)
    {
        if (graphicsPanel) graphicsPanel.SetActive(tab == "graphics");
        if (audioPanel) audioPanel.SetActive(tab == "audio");
        if (gameplayPanel) gameplayPanel.SetActive(tab == "gameplay");
        if (controlsPanel) controlsPanel.SetActive(tab == "controls");
    }

    // ----------------- BUTTONS -----------------
    public void OnApply()
    {
        _current = ReadFromUI();
        Save(_current);
        ApplyToGame(_current);
        Close();
    }

    public void OnCancel()
    {
        // Откатить UI (и ничего не применять)
        ApplyToUI(_snapshot);
        Close();
    }

    public void OnDefaults()
    {
        var def = GetDefaults();
        ApplyToUI(def);
        // НЕ применяем сразу — пользователь может нажать Cancel
    }

    // ----------------- SETUP -----------------
    private void SetupResolutions()
    {
        if (!resolutionDropdown) return;

        _resolutions = Screen.resolutions;

        var options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            var r = _resolutions[i];

#if UNITY_2022_2_OR_NEWER
            options.Add($"{r.width}x{r.height} @{r.refreshRateRatio.value:0.#}Hz");
#else
            options.Add($"{r.width}x{r.height} @{r.refreshRate}Hz");
#endif

            if (r.width == Screen.currentResolution.width &&
                r.height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void SetupQuality()
    {
        if (!qualityDropdown) return;

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.RefreshShownValue();
    }

    // ----------------- APPLY -----------------
    private void ApplyToGame(SettingsData s)
    {
        // Graphics: Resolution + Fullscreen
        if (_resolutions != null && _resolutions.Length > 0)
        {
            int resIndex = Mathf.Clamp(s.resolutionIndex, 0, _resolutions.Length - 1);
            Resolution r = _resolutions[resIndex];
            Screen.SetResolution(r.width, r.height, s.fullscreen);
        }

        // Quality
        if (QualitySettings.names != null && QualitySettings.names.Length > 0)
        {
            int q = Mathf.Clamp(s.qualityIndex, 0, QualitySettings.names.Length - 1);
            QualitySettings.SetQualityLevel(q, true);
        }

        // VSync
        QualitySettings.vSyncCount = s.vSync ? 1 : 0;

        // Audio
        AudioListener.volume = Mathf.Clamp01(s.masterVolume);
    }

    // ----------------- UI <-> DATA -----------------
    private SettingsData ReadFromUI()
    {
        return new SettingsData
        {
            resolutionIndex = resolutionDropdown ? resolutionDropdown.value : 0,
            qualityIndex = qualityDropdown ? qualityDropdown.value : QualitySettings.GetQualityLevel(),
            fullscreen = fullscreenToggle && fullscreenToggle.isOn,
            vSync = vSyncToggle && vSyncToggle.isOn,
            masterVolume = masterVolumeSlider ? masterVolumeSlider.value : AudioListener.volume
        };
    }

    private void ApplyToUI(SettingsData s)
    {
        if (resolutionDropdown && resolutionDropdown.options.Count > 0)
        {
            resolutionDropdown.value = Mathf.Clamp(s.resolutionIndex, 0, resolutionDropdown.options.Count - 1);
            resolutionDropdown.RefreshShownValue();
        }

        if (qualityDropdown && qualityDropdown.options.Count > 0)
        {
            qualityDropdown.value = Mathf.Clamp(s.qualityIndex, 0, qualityDropdown.options.Count - 1);
            qualityDropdown.RefreshShownValue();
        }

        if (fullscreenToggle) fullscreenToggle.isOn = s.fullscreen;
        if (vSyncToggle) vSyncToggle.isOn = s.vSync;

        if (masterVolumeSlider) masterVolumeSlider.value = Mathf.Clamp01(s.masterVolume);
    }

    // ----------------- SAVE/LOAD -----------------
    private void Save(SettingsData s)
    {
        PlayerPrefs.SetInt("set_res", s.resolutionIndex);
        PlayerPrefs.SetInt("set_quality", s.qualityIndex);
        PlayerPrefs.SetInt("set_fullscreen", s.fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("set_vsync", s.vSync ? 1 : 0);
        PlayerPrefs.SetFloat("set_master", s.masterVolume);
        PlayerPrefs.Save();
    }

    private SettingsData Load()
    {
        var def = GetDefaults();

        return new SettingsData
        {
            resolutionIndex = PlayerPrefs.GetInt("set_res", def.resolutionIndex),
            qualityIndex = PlayerPrefs.GetInt("set_quality", def.qualityIndex),
            fullscreen = PlayerPrefs.GetInt("set_fullscreen", def.fullscreen ? 1 : 0) == 1,
            vSync = PlayerPrefs.GetInt("set_vsync", def.vSync ? 1 : 0) == 1,
            masterVolume = PlayerPrefs.GetFloat("set_master", def.masterVolume)
        };
    }

    private SettingsData GetDefaults()
    {
        return new SettingsData
        {
            resolutionIndex = 0,
            qualityIndex = QualitySettings.GetQualityLevel(),
            fullscreen = true,
            vSync = false,
            masterVolume = 1f
        };
    }
}
