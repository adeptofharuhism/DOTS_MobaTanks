using Assets.CodeBase.Teams;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Targeting
{
    [RequireComponent(typeof(TeamAuthoring))]
    public class TargetableAuthoring : MonoBehaviour
    {
        public class TargetableBaker : Baker<TargetableAuthoring>
        {
            public override void Bake(TargetableAuthoring authoring) {
                Entity targetable = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<Targetable>(targetable);
            }
        }
    }
}