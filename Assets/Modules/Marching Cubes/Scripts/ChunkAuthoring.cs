using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[AddComponentMenu("Terrain Generator/Chunk Authoring")]
public class ChunkAuthoring : MonoBehaviour
{
    [SerializeField, Range(2,5)]
    private int resolution;

    [Header("Noise Settings")]
    [SerializeField]
    private float frequency;
    [SerializeField, Range(0,1)]
    private float threshold;

    public class Baker : Baker<ChunkAuthoring>
    {
        public override void Bake(ChunkAuthoring authoring)
        {
            int resolution3 = authoring.resolution * authoring.resolution * authoring.resolution;

            var data = new ChunkComponent
            {
                resolution = authoring.resolution,
                vertices = new NativeArray<Vertex>(resolution3, Allocator.Persistent),
                frequency = authoring.threshold,
                threshold = authoring.frequency
            };
            AddComponent(data);
        }
    }
}

struct ChunkComponent : IComponentData
{
    public int resolution;
    public NativeArray<Vertex> vertices; 

    public float frequency;
    public float threshold;
}

struct Vertex 
{
    public float3 position;
    public bool value;
}
