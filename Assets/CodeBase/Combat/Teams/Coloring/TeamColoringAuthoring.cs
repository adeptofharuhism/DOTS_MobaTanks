using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Combat.Teams.Coloring
{
    [RequireComponent(typeof(TeamAuthoring))]
    public class TeamColoringAuthoring : MonoBehaviour
    {
        public class TeamColoringBaker : Baker<TeamColoringAuthoring>
        {
            public override void Bake(TeamColoringAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<InitialTeamColoringTag>(entity);

                DynamicBuffer<EntiенWithRendererElement> entitiesWithRendererBuffer = 
                    AddBuffer<EntiенWithRendererElement>(entity);
                foreach (MeshRenderer renderer in authoring.GetComponentsInChildren<MeshRenderer>())
                    entitiesWithRendererBuffer.Add(new EntiенWithRendererElement {
                        Value = GetEntity(renderer.gameObject, TransformUsageFlags.Dynamic)
                    });
            }
        }
    }
}
