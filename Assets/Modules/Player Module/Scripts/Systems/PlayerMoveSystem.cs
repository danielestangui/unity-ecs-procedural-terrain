using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Destrom.Player
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial struct PlayerMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            new PlayerMoveJob
            {
                DeltaTime = deltaTime
            }.Schedule();
        }
    }

    [BurstCompile]
    public partial struct PlayerMoveJob : IJobEntity 
    {
        public float DeltaTime;

        [BurstCompile]
        private void Execute(ref LocalTransform transform, in PlayerMoveInput moveInput, PlayerMoveSpeed moveSpeed) 
        {
            transform.Position.xz += moveInput.Value * moveSpeed.Value * DeltaTime;
        }
    }
}