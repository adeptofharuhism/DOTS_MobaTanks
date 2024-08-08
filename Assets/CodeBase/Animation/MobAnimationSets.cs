using UnityEngine;

namespace Assets.CodeBase.Animation
{
    public class MobAnimationSet : ScriptableObject
    {
        public virtual byte Idle { get; }
        public virtual byte Attack { get; }
        public virtual byte Run { get; }
        public virtual byte Death { get; }
    }

    [CreateAssetMenu(menuName = Constants.ScriptableObjectsNames.AnimationSets.MeleeMob)]
    public class MeleeMobAnimationSet : MobAnimationSet
    {
        [SerializeField] private MeleeMobAnimated _idle;
        [SerializeField] private MeleeMobAnimated _attack;
        [SerializeField] private MeleeMobAnimated _run;
        [SerializeField] private MeleeMobAnimated _death;

        public override byte Idle => (byte)_idle;
        public override byte Attack => (byte)_attack;
        public override byte Run => (byte)_run;
        public override byte Death => (byte)_death;
    }
}
