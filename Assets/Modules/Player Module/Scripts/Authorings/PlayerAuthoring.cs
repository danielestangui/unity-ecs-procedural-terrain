using Unity.Entities;
using UnityEngine;

namespace Destrom.Player 
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public float moveSpeed;
    }

    public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity playerEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<PlayerTag>(playerEntity);
            AddComponent<PlayerMoveInput>(playerEntity);

            AddComponent(playerEntity, new PlayerMoveSpeed
            {
                Value = authoring.moveSpeed
            });
        }
    }
}