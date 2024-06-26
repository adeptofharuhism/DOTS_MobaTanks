﻿using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;

namespace Assets.CodeBase.GameEntry
{
    public struct ConnectionRequestData : IComponentData
    {
        public FixedString64Bytes PlayerName;
    }

    public struct SetNewPlayerDataRequest : IRpcCommand
    {
        public FixedString64Bytes PlayerName;
    }
}
