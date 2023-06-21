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
                // Baking dependencies
                DependsOn(authoring.data);

                if (authoring.data == null) return;

                // Add Components
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, componentsToAdd);

                // Set Components
                TerrainGeneratorRenderComponent terrainGeneratorRenderComponent = new TerrainGeneratorRenderComponent
                {
                    // Leaf Bounding Box
                    showLeafBoundingBoxEnable = authoring.data.showLeafBoundingBox.enable,
                    showLeafBoundingBoxThickness = authoring.data.showLeafBoundingBox.thickness,
                    showLeafBoundingBoxColor = authoring.data.showLeafBoundingBox.color,

                    // Grid Vertex
                    showGridVertexEnable = authoring.data.showGridVertex.enable,
                    showGridVertexRadius = authoring.data.showGridVertex.radius,
                    showGridVertexGradientColor = authoring.data.showGridVertex.gradientColor,
                    showGridVertexAirColor = authoring.data.showGridVertex.airColor,
                    showGridVertexGroundColor = authoring.data.showGridVertex.groundColor,

                    // Cell
                    showCellEnable = authoring.data.showCell.enable,
                    showOnlyCellWithVertex = authoring.data.showCell.onlyCellWithVertex,
                    showCellThickness = authoring.data.showCell.thickness,
                    showCellColor = authoring.data.showCell.color,

                    // Vertex
                    showVertexEnable = authoring.data.showVertex.enable,
                    showVertexRadius = authoring.data.showVertex.radius,
                    showVertexColor = authoring.data.showVertex.color,
                    showVertexEnableNormals = authoring.data.showVertex.enableNormals,
                    showVertexNormalLenght = authoring.data.showVertex.normalLenght,
                    showVertexNormalColor = authoring.data.showVertex.normalColor,

                    //Edges
                    showEdgesEnable = authoring.data.showEdge.enable,
                    showEdgesThickness = authoring.data.showEdge.thickness,
                    showEdgesInteriorColor = authoring.data.showEdge.interiorColor,
                    showEdgesBorderColor = authoring.data.showEdge.borderColor

                };

                SetComponent(entity, terrainGeneratorRenderComponent);
            }
        }
    }
    public struct TerrainGeneratorRenderComponent : IComponentData
    {
        public bool showLeafBoundingBoxEnable;
        public float showLeafBoundingBoxThickness;
        public Color showLeafBoundingBoxColor;

        public bool showGridVertexEnable;
        public float showGridVertexRadius;
        public bool showGridVertexGradientColor;
        public Color showGridVertexAirColor;
        public Color showGridVertexGroundColor;

        public bool showCellEnable;
        public bool showOnlyCellWithVertex;
        public float showCellThickness;
        public Color showCellColor;

        public bool showVertexEnable;
        public float showVertexRadius;
        public Color showVertexColor;
        public bool showVertexEnableNormals;
        public float showVertexNormalLenght;
        public Color showVertexNormalColor;

        public bool showEdgesEnable;
        public float showEdgesThickness;
        public Color showEdgesInteriorColor;
        public Color showEdgesBorderColor;
    }
}
