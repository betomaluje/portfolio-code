using DG.Tweening;
using UnityEngine;

namespace EditorTool.PlayerEditor
{
    public class PlayerEditorFX : MonoBehaviour
    {
        [SerializeField] 
        private ParticleSystem _particleSystem;

        [SerializeField]
        private ToolsUI _toolsUI;

        private void Awake() {
            _particleSystem.transform.parent = null;
        }

        public void OnDelete(Vector3Int position) {
            _particleSystem.transform.position = position;
            _particleSystem.Play();
        }

        public void OnNotEnoughResources(Tool tool) {
            _toolsUI.NotEnough(tool);
        }

        public void OnNotProperPlace(SpriteRenderer toolImage) {
            toolImage.transform.DOShakePosition(.5f, .5f);
        }
    }
}