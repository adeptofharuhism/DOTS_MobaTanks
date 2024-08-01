using Assets.CodeBase.GameStates;
using Unity.Entities;

namespace Assets.CodeBase.Mobs
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GameStateSystemGroup))]
    public partial class MobUpdateSystemGroup : ComponentSystemGroup { }

        [UpdateInGroup(typeof(MobUpdateSystemGroup))]
        [UpdateBefore(typeof(LogicSystemGroup))]
        public partial class CreationSystemGroup : ComponentSystemGroup { }

            [UpdateInGroup(typeof(CreationSystemGroup))]
            [UpdateBefore(typeof(SpawnSystemGroup))]
            public partial class SpawnInitializationSystemGroup : ComponentSystemGroup { }

            [UpdateInGroup(typeof(CreationSystemGroup))]
            [UpdateAfter(typeof(SpawnInitializationSystemGroup))]
            public partial class SpawnSystemGroup : ComponentSystemGroup { }

        [UpdateInGroup(typeof(MobUpdateSystemGroup))]
        [UpdateAfter(typeof(CreationSystemGroup))]
        public partial class LogicSystemGroup : ComponentSystemGroup { }

            [UpdateInGroup(typeof(LogicSystemGroup))]
            [UpdateBefore(typeof(MoveToTargetStateSystemGroup))]
            public partial class MoveToPointStateSystemGroup : ComponentSystemGroup { }

            [UpdateInGroup(typeof(LogicSystemGroup))]
            [UpdateAfter(typeof(MoveToPointStateSystemGroup))]
            [UpdateBefore(typeof(AttackStateSystemGroup))]
            public partial class MoveToTargetStateSystemGroup : ComponentSystemGroup { }

            [UpdateInGroup(typeof(LogicSystemGroup))]
            [UpdateAfter(typeof(MoveToTargetStateSystemGroup))]
            public partial class AttackStateSystemGroup : ComponentSystemGroup { }
}
