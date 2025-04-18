﻿namespace Assets.CodeBase.Infrastructure.Services.WorldControl
{
    public interface IWorldControlService
    {
        void CreateClientWorld();
        void CreateServerWorld();
        void DisconnectFromServerWorld();
        void DisposeDefaultWorld();
        void DisposeNetworkWorlds();
        void StartWorlds();
    }
}