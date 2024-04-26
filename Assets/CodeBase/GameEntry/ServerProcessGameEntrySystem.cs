using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
using UnityEngine;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.GameEntry
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEntrySystem : ISystem
    {
        private const string VehicleName = "Vehicle";

        public void OnCreate(ref SystemState state) {
            EntityQueryBuilder newPlayerDataRequestQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<SetNewPlayerDataRequest, ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(newPlayerDataRequestQuery));

            state.RequireForUpdate<GamePrefabs>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            Entity vehiclePrefab = SystemAPI.GetSingleton<GamePrefabs>().Vehicle;

            foreach (var (newPlayerData, requestSource, requestEntity)
                in SystemAPI.Query<SetNewPlayerDataRequest, ReceiveRpcCommandRequest>()
                .WithEntityAccess()) {

                ecb.DestroyEntity(requestEntity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);

                int clientId = SystemAPI.GetComponent<NetworkId>(requestSource.SourceConnection).Value;
                Debug.Log($"Connected {newPlayerData.PlayerName} with Client Id: {clientId}");

                Entity newVehicle = ecb.Instantiate(vehiclePrefab);
                ecb.SetName(newVehicle, VehicleName);

                float3 vehicleSpawnPosition = new float3(5 * clientId, 5, 0);
                LocalTransform vehicleTransform = LocalTransform.FromPosition(vehicleSpawnPosition);

                ecb.SetComponent(newVehicle, vehicleTransform);
                ecb.SetComponent(newVehicle, new GhostOwner { NetworkId = clientId });

                ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup { Value = newVehicle });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
/*
 * The ghost collection contains a ghost which does not have a valid prefab on the client! Ghost: '' ('ENTITY_NOT_FOUND').
0x00007ff730aa1d1a (Unity) DefaultBurstRuntimeLogCallback
0x00007ff7301ec96a (Unity) BurstCompilerService_CUSTOM_RuntimeLog
0x00007ffc5aa14476 (b8a6c026571eb1c80c0ca299f1b83cc) Unity.NetCode.GhostCollectionSystem.OnUpdate (at C:/Unity/Projects/DOTS_MobaTanks/Library/PackageCache/com.unity.burst@1.8.13/.Runtime/Library/PackageCache/com.unity.netcode@1.2.0/Runtime/Snapshot/GhostCollectionSystem.cs:409)
0x00007ffc5a9f83a0 (b8a6c026571eb1c80c0ca299f1b83cc) 115cf9753edb26bcb073d042ac7f2622
0x00007ffc5ab72aed (193c735218ac0afa16ae61f72974b88) Unity.Entities.WorldUnmanagedImpl.Unity.Entities.UnmanagedUpdate_0000162A$BurstDirectCall.Invoke (at C:/Unity/Projects/DOTS_MobaTanks/Library/PackageCache/com.unity.burst@1.8.13/.Runtime/unknown/unknown:0)
0x00007ffc5ab70ac9 (193c735218ac0afa16ae61f72974b88) 7bf3b3dc1c88cb657fd69b548232391d
0x00000269196ad398 (Mono JIT Code) (wrapper managed-to-native) Unity.Entities.WorldUnmanagedImpl/Unity.Entities.UnmanagedUpdate_0000162A$BurstDirectCall:wrapper_native_indirect_00000269BAC00890 (intptr&,void*)
0x00000269196acda3 (Mono JIT Code) Unity.Entities.WorldUnmanagedImpl/Unity.Entities.UnmanagedUpdate_0000162A$BurstDirectCall:Invoke (void*)
0x00000269196acd0b (Mono JIT Code) Unity.Entities.WorldUnmanagedImpl:UnmanagedUpdate (void*) (at ./Library/PackageCache/com.unity.entities@1.2.0/Unity.Entities/WorldUnmanaged.cs:828)
0x00000269196ac923 (Mono JIT Code) Unity.Entities.WorldUnmanagedImpl:UpdateSystem (Unity.Entities.SystemHandle) (at ./Library/PackageCache/com.unity.entities@1.2.0/Unity.Entities/WorldUnmanaged.cs:892)
0x00000269196aaed3 (Mono JIT Code) Unity.Entities.ComponentSystemGroup:UpdateAllSystems () (at ./Library/PackageCache/com.unity.entities@1.2.0/Unity.Entities/ComponentSystemGroup.cs:717)
0x00000269196aa953 (Mono JIT Code) Unity.Entities.ComponentSystemGroup:OnUpdate () (at ./Library/PackageCache/com.unity.entities@1.2.0/Unity.Entities/ComponentSystemGroup.cs:681)
0x00000269196a99ab (Mono JIT Code) Unity.Entities.SystemBase:Update () (at ./Library/PackageCache/com.unity.entities@1.2.0/Unity.Entities/SystemBase.cs:420)
0x00000269196aaf24 (Mono JIT Code) Unity.Entities.ComponentSystemGroup:UpdateAllSystems () (at ./Library/PackageCache/com.unity.entities@1.2.0/Unity.Entities/ComponentSystemGroup.cs:725)
0x00000269196aa96b (Mono JIT Code) Unity.Entities.ComponentSystemGroup:OnUpdate () (at ./Library/PackageCache/com.unity.entities@1.2.0/Unity.Entities/ComponentSystemGroup.cs:685)
0x00000269196a99ab (Mono JIT Code) Unity.Entities.SystemBase:Update () (at ./Library/PackageCache/com.unity.entities@1.2.0/Unity.Entities/SystemBase.cs:420)
0x00000269196a96bd (Mono JIT Code) Unity.Entities.ScriptBehaviourUpdateOrder/DummyDelegateWrapper:TriggerUpdate () (at ./Library/PackageCache/com.unity.entities@1.2.0/Unity.Entities/ScriptBehaviourUpdateOrder.cs:525)
0x0000026870ef7648 (Mono JIT Code) (wrapper runtime-invoke) object:runtime_invoke_void__this__ (object,intptr,intptr,intptr)
0x00007ffcce894c1e (mono-2.0-bdwgc) mono_jit_runtime_invoke (at C:/build/output/Unity-Technologies/mono/mono/mini/mini-runtime.c:3445)
0x00007ffcce7cd254 (mono-2.0-bdwgc) do_runtime_invoke (at C:/build/output/Unity-Technologies/mono/mono/metadata/object.c:3068)
0x00007ffcce7cd3cc (mono-2.0-bdwgc) mono_runtime_invoke (at C:/build/output/Unity-Technologies/mono/mono/metadata/object.c:3115)
0x00007ff731170ad4 (Unity) scripting_method_invoke
0x00007ff73114e844 (Unity) ScriptingInvocation::Invoke
0x00007ff730de5522 (Unity) ExecutePlayerLoop
0x00007ff730de5540 (Unity) ExecutePlayerLoop
0x00007ff730debdd5 (Unity) PlayerLoop
0x00007ff731dbef8f (Unity) PlayerLoopController::InternalUpdateScene
0x00007ff731dcbdbd (Unity) PlayerLoopController::UpdateSceneIfNeededFromMainLoop
0x00007ff731dca0a1 (Unity) Application::TickTimer
0x00007ff7322447ba (Unity) MainMessageLoop
0x00007ff732249690 (Unity) WinMain
0x00007ff73362c26e (Unity) __scrt_common_main_seh
0x00007ffd245a7344 (KERNEL32) BaseThreadInitThunk
0x00007ffd247426b1 (ntdll) RtlUserThreadStart

 */
