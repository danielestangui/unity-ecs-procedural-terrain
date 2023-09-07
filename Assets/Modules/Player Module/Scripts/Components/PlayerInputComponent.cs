using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Destrom.Player
{
    public struct PlayerMoveInput : IComponentData
    {
        public float2 Value;
    }

    public struct PlayerMoveSpeed : IComponentData 
    {
        public float Value;
    }

    public struct PlayerTag : IComponentData { };
}