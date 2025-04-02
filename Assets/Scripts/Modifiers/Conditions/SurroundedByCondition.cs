using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Conditions
{
    [CreateAssetMenu(menuName = "Aurora/Modifiers/Conditions/On Sorrounded By")]
    [TypeInfoBox("Condition that triggers when the owner is surrounded by a certain amount of targets")]
    public class SurroundedByCondition : BaseCondition
    {
        [Tooltip("The amount of rolls to trigger the condition.")]
        [SerializeField]
        [Min(0)]
        private int _amountOfTargets = 1;

        [SerializeField]
        [Min(0)]
        private float _radius = 3f;

        [SerializeField]
        private LayerMask _targetMask;

        private Transform _owner;
        private bool _amountOfEnemiesReached;

        public override void Setup(Transform owner)
        {
            _owner = owner;
            _amountOfEnemiesReached = false;
        }

        public override void ResetCondition()
        {
            base.ResetCondition();
            _amountOfEnemiesReached = false;
        }

        public override bool Check(float deltaTime)
        {
            var colliders = Physics2D.OverlapCircleAll(_owner.position, _radius, _targetMask);

            if (colliders.Length >= _amountOfTargets)
            {

                if (!_amountOfEnemiesReached)
                {
                    _amountOfEnemiesReached = true;
                    return _amountOfEnemiesReached;
                }
            }

            return _amountOfEnemiesReached;
        }

        public override void Cleanup()
        {
            _owner = null;
            _amountOfEnemiesReached = false;
        }

    }
}