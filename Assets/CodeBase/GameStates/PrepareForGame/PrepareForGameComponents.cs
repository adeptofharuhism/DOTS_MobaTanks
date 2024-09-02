using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.GameStates.PrepareForGame
{
    public struct ConnectionRequestData : IComponentData
    {
        public FixedString64Bytes PlayerName;
    }

    public struct SetNewPlayerDataRpc : IRpcCommand
    {
        public FixedString64Bytes PlayerName;
    }
}
