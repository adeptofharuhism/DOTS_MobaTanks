﻿using Assets.CodeBase.Teams;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Targeting
{
    [RequireComponent(typeof(TeamAuthoring))]
    public class TargeterAuthoring : MonoBehaviour
    {
        [SerializeField] private float _targetSearchRange = 20;
        [SerializeField] private bool _targetingOnCertainPoint;

        public float TargetSearchRange => _targetSearchRange;
        public bool TargetingOnCertainPoint => _targetingOnCertainPoint;

        public class TargeterBaker : Baker<TargeterAuthoring>
        {
            public override void Bake(TargeterAuthoring authoring) {
                Entity targeter = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<Targeter>(targeter);
                SetComponentEnabled<Targeter>(targeter, false);

                AddComponent(targeter, new TargeterRange { Value = authoring.TargetSearchRange });
                AddComponent(targeter, new CurrentTarget { Value = Entity.Null });

                if (authoring.TargetingOnCertainPoint)
                    AddComponent<TargetOnCertainPointTag>(targeter);
            }
        }
    }
}