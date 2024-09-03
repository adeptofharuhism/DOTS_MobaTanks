using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GhostSimulationSystemGroup))]
    [UpdateBefore(typeof(CameraSystemGroup))]
    public partial class RelevancySystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RelevancySystemGroup))]
    [UpdateBefore(typeof(PlayerCountSystemGroup))]
    public partial class CameraSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CameraSystemGroup))]
    [UpdateBefore(typeof(GameStateSystemGroup))]
    public partial class PlayerCountSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerCountSystemGroup))]
    [UpdateBefore(typeof(PlayerSystemGroup))]
    public partial class GameStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateBefore(typeof(ReportInGameStateSystemGroup))]
    public partial class PrepareForGameStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateAfter(typeof(PrepareForGameStateSystemGroup))]
    [UpdateBefore(typeof(InGameStateSystemGroup))]
    public partial class ReportInGameStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateAfter(typeof(ReportInGameStateSystemGroup))]
    [UpdateBefore(typeof(ReportEndGameStateSystemGroup))]
    public partial class InGameStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateAfter(typeof(InGameStateSystemGroup))]
    [UpdateBefore(typeof(EndGameStateSystemGroup))]
    public partial class ReportEndGameStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateAfter(typeof(ReportEndGameStateSystemGroup))]
    public partial class EndGameStateSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GameStateSystemGroup))]
    [UpdateBefore(typeof(WeaponSystemGroup))]
    public partial class PlayerSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    [UpdateBefore(typeof(InventorySystemGroup))]
    public partial class BeginRespawnSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    [UpdateAfter(typeof(BeginRespawnSystemGroup))]
    [UpdateBefore(typeof(VehicleSystemGroup))]
    public partial class InventorySystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    [UpdateAfter(typeof(InventorySystemGroup))]
    [UpdateBefore(typeof(TurretSystemGroup))]
    public partial class VehicleSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    [UpdateAfter(typeof(VehicleSystemGroup))]
    [UpdateBefore(typeof(WheelSystemGroup))]
    public partial class TurretSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    [UpdateAfter(typeof(TurretSystemGroup))]
    [UpdateBefore(typeof(EndRespawnSystemGroup))]
    public partial class WheelSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PlayerSystemGroup))]
    [UpdateAfter(typeof(WheelSystemGroup))]
    public partial class EndRespawnSystemGroup: ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerSystemGroup))]
    [UpdateBefore(typeof(MobUpdateSystemGroup))]
    public partial class WeaponSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(WeaponSystemGroup))]
    [UpdateBefore(typeof(CombatSimulationSystemGroup))]
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
    [UpdateBefore(typeof(PreAttackStateSystemGroup))]
    public partial class MoveToTargetStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(LogicSystemGroup))]
    [UpdateAfter(typeof(MoveToTargetStateSystemGroup))]
    [UpdateBefore(typeof(AttackStateSystemGroup))]
    public partial class PreAttackStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(LogicSystemGroup))]
    [UpdateAfter(typeof(PreAttackStateSystemGroup))]
    [UpdateBefore(typeof(PostAttackStateSystemGroup))]
    public partial class AttackStateSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(LogicSystemGroup))]
    [UpdateAfter(typeof(AttackStateSystemGroup))]
    public partial class PostAttackStateSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MobUpdateSystemGroup))]
    [UpdateBefore(typeof(CombatSimulationSystemGroup))]
    public partial class StructuresSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(StructuresSystemGroup))]
    public partial class BaseStructureSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MobUpdateSystemGroup))]
    [UpdateBefore(typeof(EffectsSystemGroup))]
    public partial class CombatSimulationSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(CombatSimulationSystemGroup))]
    [UpdateBefore(typeof(HealthSystemGroup))]
    public partial class DamageSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(CombatSimulationSystemGroup))]
    [UpdateAfter(typeof(DamageSystemGroup))]
    public partial class HealthSystemGroup : ComponentSystemGroup { }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(HealthSystemGroup))]
    public partial class HealthBarClientSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CombatSimulationSystemGroup))]
    [UpdateBefore(typeof(FinancesSystemGroup))]
    public partial class EffectsSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateBefore(typeof(PostDeathEffectsSystemGroup))]
    public partial class TeamColoringSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(TeamColoringSystemGroup))]
    public partial class PostDeathEffectsSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EffectsSystemGroup))]
    [UpdateBefore(typeof(ShopSystemGroup))]
    public partial class FinancesSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FinancesSystemGroup))]
    [UpdateBefore(typeof(DestructionSystemGroup))]
    public partial class ShopSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ShopSystemGroup))]
    [UpdateBefore(typeof(UiSystemGroup))]
    public partial class DestructionSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DestructionSystemGroup))]
    public partial class UiSystemGroup : ComponentSystemGroup { }
}
