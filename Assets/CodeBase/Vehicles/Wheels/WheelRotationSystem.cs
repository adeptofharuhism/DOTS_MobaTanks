﻿using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(WheelSetRotationInputSystem))]
    public partial struct WheelRotationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (rotationInput, rotationParameters, forceCastPoint)
                in SystemAPI.Query<WheelRotationInput, WheelRotationParameters, WheelForceCastPoint>()
                .WithAll<WheelHasRotationTag, WheelInitializedTag, Simulate>()) {

                RefRW<LocalTransform> forceCastTransform = SystemAPI.GetComponentRW<LocalTransform>(forceCastPoint.Value);

                int clockwiseMultiplier = rotationParameters.RotatesClockwise ? 1 : -1;
                float rotationAngle = rotationInput.Value * clockwiseMultiplier * rotationParameters.MaxRotationAngle;

                forceCastTransform.ValueRW.Rotation = quaternion.Euler(0, math.radians(rotationAngle), 0);
            }
        }
    }
}