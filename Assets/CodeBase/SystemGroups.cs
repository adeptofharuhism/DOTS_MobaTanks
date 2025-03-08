using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GhostSimulationSystemGroup))]
    public partial class RelevancySystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RelevancySystemGroup))]
    public partial class CameraSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CameraSystemGroup))]
    public partial class PlayerCountSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerCountSystemGroup))]
    public partial class GameStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    public partial class PrepareForGameStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateAfter(typeof(PrepareForGameStateSystemGroup))]
    public partial class ReportInGameStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateAfter(typeof(ReportInGameStateSystemGroup))]
    public partial class InGameStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateAfter(typeof(InGameStateSystemGroup))]
    public partial class ReportEndGameStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateAfter(typeof(ReportEndGameStateSystemGroup))]
    public partial class EndGameStateSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GameStateSystemGroup))]
    public partial class PlayerSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    public partial class BeginRespawnSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    [UpdateAfter(typeof(BeginRespawnSystemGroup))]
    public partial class InventorySystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    [UpdateAfter(typeof(InventorySystemGroup))]
    public partial class ShopSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    [UpdateAfter(typeof(ShopSystemGroup))]
    public partial class VehicleSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    [UpdateAfter(typeof(VehicleSystemGroup))]
    public partial class TurretSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    [UpdateAfter(typeof(TurretSystemGroup))]
    public partial class WheelSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    [UpdateAfter(typeof(WheelSystemGroup))]
    public partial class EndRespawnSystemGroup: ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerSystemGroup))]
    public partial class WeaponSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(WeaponSystemGroup))]
    public partial class MobUpdateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(MobUpdateSystemGroup))]
    public partial class CreationSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(CreationSystemGroup))]
    public partial class SpawnInitializationSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(CreationSystemGroup))]
    [UpdateAfter(typeof(SpawnInitializationSystemGroup))]
    public partial class SpawnSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(MobUpdateSystemGroup))]
    [UpdateAfter(typeof(CreationSystemGroup))]
    public partial class LogicSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(LogicSystemGroup))]
    public partial class MoveToPointStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(LogicSystemGroup))]
    [UpdateAfter(typeof(MoveToPointStateSystemGroup))]
    public partial class MoveToTargetStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(LogicSystemGroup))]
    [UpdateAfter(typeof(MoveToTargetStateSystemGroup))]
    public partial class PreAttackStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(LogicSystemGroup))]
    [UpdateAfter(typeof(PreAttackStateSystemGroup))]
    public partial class AttackStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(LogicSystemGroup))]
    [UpdateAfter(typeof(AttackStateSystemGroup))]
    public partial class PostAttackStateSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MobUpdateSystemGroup))]
    public partial class StructuresSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(StructuresSystemGroup))]
    public partial class BaseStructureSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MobUpdateSystemGroup))]
    public partial class CombatSimulationSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(CombatSimulationSystemGroup))]
    public partial class DamageSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(CombatSimulationSystemGroup))]
    [UpdateAfter(typeof(DamageSystemGroup))]
    public partial class HealthSystemGroup : ComponentSystemGroup { }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(HealthSystemGroup))]
    public partial class HealthBarClientSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CombatSimulationSystemGroup))]
    public partial class EffectsSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(EffectsSystemGroup))]
    public partial class TeamColoringSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(TeamColoringSystemGroup))]
    public partial class PostDeathEffectsSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EffectsSystemGroup))]
    public partial class FinancesSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FinancesSystemGroup))]
    public partial class DestructionSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DestructionSystemGroup))]
    public partial class UiSystemGroup : ComponentSystemGroup { }
}
