using System.Collections.Generic;
using System.Linq;
using Extensions;
using Player.Input;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Options {
    public class OptionsMenuUI : MonoBehaviour {
        [SerializeField]
        private List<OptionsPanel> _panels;

        private PlayerUIInput _playerInput;

        private int _currentIndex = 0;

        private void Awake() {
            _playerInput = FindFirstObjectByType<PlayerUIInput>();
            if (_playerInput == null) {
                _playerInput = gameObject.GetOrAdd<PlayerUIInput>();
            }

            _currentIndex = 0;
        }

        private void OnEnable() {
            _playerInput.NextViewEvent += NextPanel;
            _playerInput.PreviousViewEvent += PreviousPanel;
        }

        public void ActivateFirstTime() {
            for (int i = 0; i < _panels.Count; i++) {
                if (i == 0) {
                    _panels[i].Activate();
                }
                else {
                    _panels[i].Deactivate();
                }
            }
        }

        private void OnDisable() {
            _playerInput.NextViewEvent -= NextPanel;
            _playerInput.PreviousViewEvent -= PreviousPanel;

            EventSystem.current.SetSelectedGameObject(null);
        }

        [Button]
        private void NextPanel() {
            if (_panels.Any()) {
                _panels[_currentIndex].Deactivate();

                _currentIndex = (_currentIndex + 1) % _panels.Count;

                _panels[_currentIndex].Activate();
            }
        }

        [Button]
        private void PreviousPanel() {
            if (_panels.Any()) {
                _panels[_currentIndex].Deactivate();

                _currentIndex--;
                if (_currentIndex < 0) {
                    _currentIndex = _panels.Count - 1;
                }

                _panels[_currentIndex].Activate();
            }
        }

        public void GoToPanel(int index) {
            if (index >= 0 && index < _panels.Count && _panels.Any()) {
                _panels[_currentIndex].Deactivate();

                _currentIndex = index;

                _panels[_currentIndex].Activate();
            }
        }


    }
}