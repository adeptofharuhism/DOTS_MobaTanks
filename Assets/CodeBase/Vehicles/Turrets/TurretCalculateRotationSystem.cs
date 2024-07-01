using Assets.CodeBase.Targeting;
using Assets.CodeBase.Weapon;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Turrets
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(WeaponCooldownSystem))]
    [UpdateBefore(typeof(WeaponProjectileSpawnSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct TurretCalculateRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (slot, target, parent)
                in SystemAPI.Query<WeaponSlot, CurrentTarget, WeaponParent>()
                .WithAll<WeaponHasTurret, WeaponReadyToFireTag>()) {

                if (target.Value == Entity.Null) {
                    ecb.SetComponent(parent.Value, new TurretRotationAngle { Value = 0 });
                    continue;
                }

                RefRO<LocalToWorld> slotTransform = SystemAPI.GetComponentRO<LocalToWorld>(slot.Value);
                RefRO<LocalToWorld> targetTransform = SystemAPI.GetComponentRO<LocalToWorld>(target.Value);

                ecb.SetComponent(parent.Value, new TurretRotationAngle { 
                    Value = CalculateLookAngle(
                        slotTransform,
                        targetTransform.ValueRO.Position)
                });
            }

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        private float CalculateLookAngle(RefRO<LocalToWorld> slotTransform, float3 targetPosition) {
            float3 slotForward = slotTransform.ValueRO.Forward;
            float3 slotRight = slotTransform.ValueRO.Right;

            float3 slotPoint0 = slotTransform.ValueRO.Position;
            float3 slotPoint1 = slotTransform.ValueRO.Position + slotForward;
            float3 slotPoint2 = slotTransform.ValueRO.Position + slotRight;

            float d21 = slotPoint1.x - slotPoint0.x;
            float d31 = slotPoint2.x - slotPoint0.x;
            float d22 = slotPoint1.y - slotPoint0.y;
            float d32 = slotPoint2.y - slotPoint0.y;
            float d23 = slotPoint1.z - slotPoint0.z;
            float d33 = slotPoint2.z - slotPoint0.z;

            float A = d22 * d33 - d23 * d32;
            float B = d23 * d31 - d21 * d33;
            float C = d21 * d32 - d22 * d31;
            float D = -(A * slotPoint0.x + B * slotPoint0.y + C * slotPoint0.z);

            float3 P = targetPosition;

            float T = -(A * P.x + B * P.y + C * P.z + D) / (A * A + B * B + C * C);

            float3 H = new float3(
                A * T + P.x,
                B * T + P.y,
                C * T + P.z);

            float3 lookVector = H - slotPoint0;
            lookVector = math.normalize(lookVector);
            float lookAndForwardDot = math.dot(lookVector, slotForward);
            float lookAngle = math.acos(lookAndForwardDot);

            float rightAndLookDot = math.dot(lookVector, slotRight);
            float rightAndLookAngle = math.acos(rightAndLookDot);
            if (rightAndLookAngle > math.PIHALF)
                lookAngle *= -1;

            return lookAngle;
        }

        /* Quick introduction to symbols
         * _____________________________
         * 1) Searching for a plane equation:
         * |x-x0 x1-x0 x2-x0|
         * |y-y0 y1-y0 y2-y0| = 0
         * |z-z0 z1-z0 z2-z0|
         * 
         * Equal to
         * |x-x0 d21 d31|
         * |y-y0 d22 d32| = 0
         * |z-z0 d23 d33|
         * 
         * Equation is in form: Ax + By + Cz + D = 0
         * A = d22d33 - d23d32
         * B = d23d31 - d21d33
         * C = d21d32 - d22d31
         * D = - (Ax0 + By0 + Cz0)
         * 
         * 2) Search for a normal vector to plane:
         * N = (A; B; C)
         * 
         * 3) Perpendicular is going to be built with a vector N and a point P
         * P - is a position of targetTransform
         * P = (Px, Py, Pz)
         * 
         * 4) Equation of the perpendicular:
         * (x-Px)/A = (y-Py)/B = (z-Pz)/C
         * 
         * 5) Perpendicular as a parametric (T) system:
         * (x = AT + Px
         * {y = BT + Py
         * (z = CT + Pz
         * 
         * 6) Put parametric coordinates into plane equation and express T
         * 
         * T = -(APx + BPy + CPz + D)/(AA + BB + CC)
         * 
         * 7) Put T into parametric system and interpret results as point H
         * 
         * Point H is a height's base on a targeting plane
         * Gun is pointing at H point really...
         */
    }
}
