using DG.Tweening;
using Player.Input;
using Scene_Management;
using Sounds;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Gems {
    public class GemsFoundController : MonoBehaviour {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        private GemsController _gemsController;

        private void Awake() {
            _gemsController = GetComponentInChildren<GemsController>();
            _gemsController.enabled = false;
            _canvasGroup.alpha = 0;
        }

        public void DoGemFX() {
            _gemsController.enabled = true;
            _gemsController.GetProgress();
        }

        public void ShowUI() {
            _canvasGroup.DOFade(1, 1).SetUpdate(true).OnComplete(() => {
                InputSystem.onAnyButtonPress.CallOnce(ctrl => {
                    DebugTools.DebugLog.Log($"Returning to previous scene");
                    SoundManager.instance.Play("button_click");

                    OnFxDone();
                });
            });
        }

        private void OnFxDone() {
            SceneLoader.Instance.UnloadScene("GemFoundScene", () => {
                Time.timeScale = 1;
                var uiLoader = FindFirstObjectByType<InGameUILoader>();
                if (uiLoader != null) {
                    uiLoader.LoadUI();
                }

                var player = FindFirstObjectByType<PlayerBattleInput>();
                if (player != null) {
                    player.enabled = true;
                }
            });
        }
    }
}