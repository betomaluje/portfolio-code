using BerserkPixel.Utils;
using Player.Input;
using Scene_Management;
using UI;
using UnityEngine;

namespace Gems {
    public class Gem : MonoBehaviour {
        [SerializeField]
        private LayerMask _targetMask;

        [SerializeField]
        private SceneField _gemFoundScene;

        private void OnTriggerEnter2D(Collider2D collision) {
            if (_targetMask.LayerMatchesObject(collision)) {
                DebugTools.DebugLog.Log($"Gem found!");
                if (_gemFoundScene != null) {
                    var uiLoader = FindFirstObjectByType<InGameUILoader>();
                    if (uiLoader != null) {
                        uiLoader.UnloadUI();
                    }

                    var player = FindFirstObjectByType<PlayerBattleInput>();
                    if (player != null) {
                        player.enabled = false;
                    }

                    SceneLoader.Instance.LoadSceneAdditive(_gemFoundScene, onSceneStartLoad: OnSceneStartLoading, onSceneLoaded: OnSceneLoaded);
                }
            }
        }

        private void OnSceneStartLoading() {
            // finally we destroy the gem
            Destroy(gameObject);
        }

        private void OnSceneLoaded() {
            Time.timeScale = 0;
        }
    }
}