using System;

namespace Weapons {
    public interface IRangeLimitable {
        event Action OnOutOfRange;
        void SetMaxRange(float maxRange);
    }
}