using Assets.CodeBase.Teams;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Effects.Coloring
{
    [RequireComponent(typeof(TeamAuthoring))]
    public class TeamColoringAuthoring : MonoBehaviour
    {
        public class TeamColoringBaker : Baker<TeamColoringAuthoring>
        {
            public override void Bake(TeamColoringAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<InitialTeamColoringTag>(entity);

                DynamicBuffer<EntitiesWithRendererElement> entitiesWithRendererBuffer = 
                    AddBuffer<EntitiesWithRendererElement>(entity);
                foreach (MeshRenderer renderer in authoring.GetComponentsInChildren<MeshRenderer>())
                    entitiesWithRendererBuffer.Add(new EntitiesWithRendererElement {
                        Value = GetEntity(renderer.gameObject, TransformUsageFlags.Dynamic)
                    });
            }
        }
    }
}
