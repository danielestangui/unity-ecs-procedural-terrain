using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Utils;
using Utils.GizmosECS;
using Unity.Entities;

namespace TerrainGenerator 
{
    [UpdateAfter(typeof(TerrainGeneratorSystem))]
    public partial class TerrainMarchingCubesSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            
        }
    }
}