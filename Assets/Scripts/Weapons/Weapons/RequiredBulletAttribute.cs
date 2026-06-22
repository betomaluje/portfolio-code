using System;

namespace Weapons {
    /// <summary>
    /// An attribute used to specify which specialized bullet script a weapon requires.
    /// This is used by the Weapon Creator Wizard to automate bullet generation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class RequiredBulletAttribute : Attribute {
        public Type BulletType { get; }

        public RequiredBulletAttribute(Type bulletType) {
            BulletType = bulletType;
        }
    }
}
