using Extensions;
using UnityEngine;

namespace Buildings {
    public class BuildingBlueprint : MonoBehaviour {
        [SerializeField]
        private SpriteRenderer _image;

        [Header("Collision")]
        [SerializeField]
        private BoxCollider2D _collider;

        public void Setup(Building building, BoxCollider2D ghostCollider) {
            _image.sprite = building.Sprite;

            // update colliders bounds
            _collider.size = ghostCollider.size;
            _collider.offset = ghostCollider.offset;

            BuildingPersistence.Instance.AddBuilding(transform, building);
        }

        public void RestoreFromRepo(Building building) {
            _image.sprite = building.Sprite;

            // update colliders bounds
            _image.UpdateColliderSize(_collider, new Vector2(.25f, -.5f));
        }
    }
}