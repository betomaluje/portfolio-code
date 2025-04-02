using System.Collections;
using BerserkPixel.Health;
using Camera;
using Level;
using NPCs;
using Player.Input;
using Scene_Management;
using Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

namespace UI {
    public class PlayerDeathUI : MonoBehaviour {
        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private TextMeshProUGUI _messageText;

        [SerializeField]
        private SceneField _villageScene;

        [SerializeField]
        private GameObject[] _otherPanels;

        private CharacterHealth _playerHealth;
        private PlayerBattleInput _player;
        private NPCSwarm _npcSwarm;
        private DisablePanelComponent _disablePanelComponent;

        private void Awake() {
            _player = FindFirstObjectByType<PlayerBattleInput>();
            if (_player != null) {
                _playerHealth = _player.GetComponent<CharacterHealth>();
            }
            _npcSwarm = FindFirstObjectByType<NPCSwarm>();

            _disablePanelComponent = new DisablePanelComponent(_rectTransform);

            _rectTransform.gameObject.SetActive(false);
        }

        private void Start() {
            if (_player == null || _playerHealth == null) {
                _disablePanelComponent.Hide();
            }
        }

        private void OnEnable() {
            if (_playerHealth != null) {
                _playerHealth.OnDie += Display;
            }
            _npcSwarm?.OnAllNPCDefeated.AddListener(LostByNPC);
        }

        private void OnDisable() {
            if (_playerHealth != null) {
                _playerHealth.OnDie -= Display;
            }
            _npcSwarm?.OnAllNPCDefeated.RemoveListener(LostByNPC);

            Time.timeScale = 1f;
        }

        private void LostByNPC() {
            _messageText.text = "All villagers were killed";
            Display();
        }

        private void Display() {
            _rectTransform.gameObject.SetActive(true);
            SoundManager.instance.StopAll();
            SoundManager.instance.Play("defeat");

            foreach (GameObject panel in _otherPanels) {
                panel.SetActive(false);
            }

            StartCoroutine(BeginGameOverSequence());

            // wait until any button pressed
            InputSystem.onAnyButtonPress.CallOnce(ctrl => {
                SoundManager.instance.Play("button_click");
                var currentScene = SceneManager.GetActiveScene();
                if (currentScene.name.Equals("Intro Dungeon")) {
                    SceneLoader.Instance.ReloadScene();
                }
                else {
                    SceneLoader.Instance.LoadScene(_villageScene);
                }
            });
        }

        private IEnumerator BeginGameOverSequence() {
            PostProcessingManager.Instance.SetProfile("GameOver");
            yield return CinemachineCameraHighlight.Instance.HighlightAsync(_player.transform);

            Time.timeScale = 0f;
        }
    }
}