using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using TerrainGenerator.Utils;
using Unity.Collections;
using Unity.Rendering;

namespace TerrainGenerator 
{
    [UpdateInGroup(typeof(TerrainGeneratorSystemGroup))]
    [UpdateAfter(typeof(DualCounturingSystem))]
    public partial struct GenerateMeshSystem : ISystem
    {
    }
}
