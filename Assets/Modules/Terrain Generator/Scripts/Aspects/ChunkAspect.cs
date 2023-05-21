using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace TerrainGenerator
{
    public readonly partial struct ChunkAspect : IAspect
    {
        readonly RefRO<LocalTransform> transform;
        readonly RefRW<ChunkComponent> chunk;

        /*        public ComponentType[] GetWritableTypes()
                {
                    //return new ComponentType[] { typeof(MyComponent) };
                }*/
    }
}