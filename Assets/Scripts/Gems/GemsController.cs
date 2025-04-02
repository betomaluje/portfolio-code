using Dungeon;
using Scene_Management;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gems {
    [RequireComponent(typeof(Animator))]
    public class GemsController : MonoBehaviour {

        private readonly int ANIM_GEMS_IDLE = Animator.StringToHash("Gems_Idle");
        private readonly int ANIM_GEMS_FOREST = Animator.StringToHash("Gems_Forest");
        private readonly int ANIM_GEMS_DESERT = Animator.StringToHash("Gems_Desert");
        private readonly int ANIM_GEMS_SKY = Animator.StringToHash("Gems_Sky");

        [Header("Forest")]
        [SerializeField]
        private SceneField _boss1Scene;
        [SerializeField]
        private GameObject _forestGemsParticles;

        [Header("Desert")]
        [SerializeField]
        private SceneField _boss2Scene;
        [SerializeField]
        private GameObject _desertGemsParticles;

        [Header("Sky")]
        [SerializeField]
        private SceneField _boss3Scene;
        [SerializeField]
        private GameObject _skyGemsParticles;

        private Animator _animator;

        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable() {
            DisableParticles();
            GetProgress();
        }

        private void DisableParticles() {
            _forestGemsParticles.SetActive(false);
            _desertGemsParticles.SetActive(false);
            _skyGemsParticles.SetActive(false);
        }

        public void GetProgress() {
            var prefsForest = DungeonWinsUtils.GetWinsPrefsName(_boss1Scene);
            var forestWins = PlayerPrefs.GetInt(prefsForest, 0);

            if (forestWins <= 0) {
                // idle
                _animator.CrossFadeInFixedTime(ANIM_GEMS_IDLE, .1f);
                return;
            }

            _forestGemsParticles.SetActive(true);

            var prefsDesert = DungeonWinsUtils.GetWinsPrefsName(_boss2Scene);
            var desertWins = PlayerPrefs.GetInt(prefsDesert, 0);

            if (desertWins <= 0) {
                // forest
                DoForestAnimation();
                return;
            }

            _desertGemsParticles.SetActive(true);

            var prefsSky = DungeonWinsUtils.GetWinsPrefsName(_boss3Scene);
            var skyWins = PlayerPrefs.GetInt(prefsSky, 0);

            if (skyWins <= 0) {
                // desert
                DoDesertAnimation();
                return;
            }

            _skyGemsParticles.SetActive(true);

            // sky
            DoSkyAnimation();
        }

        [Button]
        private void DoForestAnimation() {
            _animator.CrossFadeInFixedTime(ANIM_GEMS_FOREST, .1f);
        }

        [Button]
        private void DoDesertAnimation() {
            _animator.CrossFadeInFixedTime(ANIM_GEMS_DESERT, .1f);
        }

        [Button]
        private void DoSkyAnimation() {
            _animator.CrossFadeInFixedTime(ANIM_GEMS_SKY, .1f);
        }
    }
}