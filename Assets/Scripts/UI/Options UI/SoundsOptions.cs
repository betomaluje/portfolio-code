using Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Options {
    public class SoundsOptions : OptionsPanel {
        [SerializeField]
        private Slider _masterSlider;
        [SerializeField]
        private Slider _bgSlider;
        [SerializeField]
        private Slider _sfxSlider;

        private void OnEnable() {
            _masterSlider.onValueChanged.AddListener(HandleMaster);
            _bgSlider.onValueChanged.AddListener(HandleBG);
            _sfxSlider.onValueChanged.AddListener(HandleSFX);

            var masterSavedPrefs = SoundManager.instance.GetMasterVolume();
            var songSavedPrefs = SoundManager.instance.GetMusicVolume();
            var sfxSavedPrefs = SoundManager.instance.GetSFXVolume();

            _masterSlider.value = masterSavedPrefs;
            _bgSlider.value = songSavedPrefs;
            _sfxSlider.value = sfxSavedPrefs;
        }

        private void OnDisable() {
            _masterSlider.onValueChanged.RemoveListener(HandleMaster);
            _bgSlider.onValueChanged.RemoveListener(HandleBG);
            _sfxSlider.onValueChanged.RemoveListener(HandleSFX);
        }

        private void HandleMaster(float volume) {
            SoundManager.instance.SetMasterVolume(volume);
        }

        private void HandleBG(float volume) {
            SoundManager.instance.SetMusicVolume(volume);
        }

        private void HandleSFX(float volume) {
            SoundManager.instance.SetSFXVolume(volume);
        }
    }
}