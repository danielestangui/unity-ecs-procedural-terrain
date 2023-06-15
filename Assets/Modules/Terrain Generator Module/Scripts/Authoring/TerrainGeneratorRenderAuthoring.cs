using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace TerrainGenerator
{
    [AddComponentMenu("Terrain Generator/Terrain Generator Render Authoring")]
    public class TerrainGeneratorRenderAuthoring : MonoBehaviour
    {
        [SerializeField]
        private TerrainGeneratorRenderData data;

        public class Baker : Baker<TerrainGeneratorRenderAuthoring>
        {
            static ComponentTypeSet componentsToAdd = new(new ComponentType[]
            {
                typeof(TerrainGeneratorRenderComponent)
            });

            public override void Bake(TerrainGeneratorRenderAuthoring authoring)
            {
                // Add Components
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, componentsToAdd);

                // Set Components
                TerrainGeneratorRenderComponent terrainGeneratorRenderComponent = new TerrainGeneratorRenderComponent
                {
                    showTerrainGeneratorBoundingBox = authoring.data.showTerrainGeneratorBoundingBox,
                };

                SetComponent(entity, terrainGeneratorRenderComponent);
            }
        }
    }
    public struct TerrainGeneratorRenderComponent : IComponentData
    {
        public bool showTerrainGeneratorBoundingBox;
    }
}
