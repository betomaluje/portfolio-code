using DG.Tweening;
using NPCs;
using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform))]
    public class PlayerNpcUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI[] _npcAmountTexts;

        private NPCSwarm _npcSwarm;
        private DisablePanelComponent _disablePanelComponent;

        private void Awake() {
            _npcSwarm = FindFirstObjectByType<NPCSwarm>();
            _disablePanelComponent = new DisablePanelComponent(GetComponent<RectTransform>());
        }

        private void OnEnable() {
            if (_npcSwarm != null) {
                _npcSwarm.OnNPCAmountChange.AddListener(UpdateNpcCount);
            }
        }

        private void OnDisable() {
            if (_npcSwarm != null) {
                _npcSwarm.OnNPCAmountChange.RemoveListener(UpdateNpcCount);
            }
        }

        private void Start() {
            if (_npcSwarm == null) {
                // hide all children
                _disablePanelComponent.Hide();
            }
        }

        private void UpdateNpcCount(string npcsOnScene) {
            foreach (var npcAmountText in _npcAmountTexts) {
                npcAmountText.DOText(npcsOnScene, 1f);
            }
        }
    }
}