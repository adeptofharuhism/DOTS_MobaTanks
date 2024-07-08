using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Assets.CodeBase.Combat.Teams;

namespace Assets.CodeBase.Network
{
    public struct ConnectionRequestData : IComponentData
    {
        public FixedString64Bytes PlayerName;
    }

    public struct SetNewPlayerDataRpc : IRpcCommand
    {
        public FixedString64Bytes PlayerName;
    }

    public struct ReadyRpc : IRpcCommand { }
    public struct PlayerReady : IComponentData { }

    public struct WinnerTeam : IComponentData
    {
        public TeamType Value;
    }
}
