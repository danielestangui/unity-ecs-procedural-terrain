using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using TerrainGenerator.Utils;
using System;
using Unity.Collections;

namespace TerrainGenerator
{
    public static class DualContouring
    {
        public static int AXIS_X = 0;
        public static int AXIS_Y = 1;
        public static int AXIS_Z = 2;

        public static int MATERIAL_AIR = 0;
        public static int MATERIAL_SOLID = 1;

        public static float QEF_ERROR = 1e-6f;
        public static int QEF_SWEEPS = 4;

        public static readonly int[][] edgevmap = new int[12][]
        {
	        new int[2]{0,4},new int[2]{1,5},new int[2]{2,6},new int[2]{3,7},	// z-axis 
	        new int[2]{0,2},new int[2]{1,3},new int[2]{4,6},new int[2]{5,7},	// y-axis
	        new int[2]{0,1},new int[2]{2,3},new int[2]{4,5},new int[2]{6,7}		// x-axis
        };  

        public static readonly Vector3[] CHILD_MIN_OFFSETS =
        {   
	    // needs to match the vertMap from Dual Contouring impl
	    new Vector3( 0, 0, 0 ),
        new Vector3( 0, 0, 1 ),
        new Vector3( 0, 1, 0 ),
        new Vector3( 0, 1, 1 ),
        new Vector3( 1, 0, 0 ),
        new Vector3( 1, 0, 1 ),
        new Vector3( 1, 1, 0 ),
        new Vector3( 1, 1, 1 ),
        };

        public static VerticeElement CalculatePoint(int cellIndex, int vertexIndex, GridVertex[] gridVertex, Cell[] cells, int resolution, ref List<IntersectingEdgesElement> edges)
        {
            int corners = 0;

            if (cellIndex == 10) 
            {
                Debug.Log("");
            }
            int[] cornersArray = 
                {
                    cells[cellIndex].corner0,
                    cells[cellIndex].corner1,
                    cells[cellIndex].corner2,
                    cells[cellIndex].corner3,
                    cells[cellIndex].corner4,
                    cells[cellIndex].corner5,
                    cells[cellIndex].corner6,
                    cells[cellIndex].corner7
                };

            corners |= (gridVertex[cells[cellIndex].corner0].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 0;
            corners |= (gridVertex[cells[cellIndex].corner1].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 1;
            corners |= (gridVertex[cells[cellIndex].corner2].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 2;
            corners |= (gridVertex[cells[cellIndex].corner3].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 3;
            corners |= (gridVertex[cells[cellIndex].corner4].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 4;
            corners |= (gridVertex[cells[cellIndex].corner5].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 5;
            corners |= (gridVertex[cells[cellIndex].corner6].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 6;
            corners |= (gridVertex[cells[cellIndex].corner7].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 7;

            if (corners == 0 || corners == 255)
            {
                return new VerticeElement 
                {
                    index = -1,
                    position = Vector3.zero,
                    normal = Vector3.zero
                };
            }

            //Debug.Log($"Conners binary: {Convert.ToString(corners, 2)}");


            //const int MAX_CROSSINGS = 6;
            int edgeCount = 0;
            float3 averageNormal = Vector3.zero;
            QefSolver qef = new QefSolver();

            //for (int i = 0; i < 12 && edgeCount < MAX_CROSSINGS; i++)
            for (int i = 0; i < 12; i++)
            {
                int c1 = edgevmap[i][0];
                int c2 = edgevmap[i][1];

                // Extrae el valor de coners en la posción c1 y lo asigna a m1
                int m1 = (corners >> c1) & 1;
                int m2 = (corners >> c2) & 1;

                // Si en ambos puntos (corners) se da la circustancia de tener el mismo material queire decir que no hay puntos de corte
                if ((m1 == MATERIAL_AIR && m2 == MATERIAL_AIR) || (m1 == MATERIAL_SOLID && m2 == MATERIAL_SOLID))
                {
                    // salta el ciclo del bucle
                    //Debug.Log($"Para {edgevmap[i][0]} y {edgevmap[i][1]} salta el bucle.");
                    continue;
                }

                float3 p1 = gridVertex[cornersArray[c1]].position;
                float3 p2 = gridVertex[cornersArray[c2]].position;

                float3 p = ApproximateZeroCrossingPosition(p1, p2);

                //Debug.Log($"Para {edgevmap[i][0]} y {edgevmap[i][1]} pinta.");

                //Draw.DrawSphere(p, 0.1f, Color.red);

                float3 n = CalculateSurfaceNormal(p);
                qef.add(p.x, p.y, p.z, n.x, n.y, n.z);

                averageNormal += n;

                // Calcualate Edge

                Cell[] surrondingCells = GetSurrondingCells(cornersArray[c1], cornersArray[c2],cells, gridVertex, resolution);

                IntersectingEdgesElement edge = new IntersectingEdgesElement
                {
                    index = edges.Count,
                    vertexIndex0 = gridVertex[cornersArray[c1]].index,
                    vertexIndex1 = gridVertex[cornersArray[c2]].index,
                    axis = GetAxis(gridVertex[cornersArray[c1]].index, gridVertex[cornersArray[c2]].index, resolution, gridVertex),
                    sharedCells00 = surrondingCells[0],
                    sharedCells01 = surrondingCells[1],
                    sharedCells10 = surrondingCells[2],
                    sharedCells11 = surrondingCells[3]
                };

                // Contiene ya este edge
                if (!ContainsEdge(edge, edges))
                {
                    //Debug.Log($"[DualContoruing]Edge({edges.Count}) {edge.vertexIndex0}, {edge.vertexIndex1}: {edge.axis}");
                    edges.Add(edge);
                    edgeCount++;
                }
            }

            Vector3 qefPosition = Vector3.zero;
            qef.solve(ref qefPosition, QEF_ERROR, QEF_SWEEPS, QEF_ERROR);

            float3 position = new float3(qefPosition.x, qefPosition.y, qefPosition.z);

            VerticeElement vertice = new VerticeElement
            {
                index = vertexIndex,
                position = new float3(qefPosition.x, qefPosition.y, qefPosition.z),
                normal = Vector3.Normalize(averageNormal),
                cell = cells[cellIndex]
            };

            return vertice;
        }

        /// <summary>
        /// Divide en cuatro segementos (steps) la linea que hay entre los puntos de corte, donde obtenga el minimo valor será el punto más 
        /// cercano de interseccion
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <returns> Devuelve un valora aproximado de donde se encuentra la interseccion </returns>
        public static float3 ApproximateZeroCrossingPosition(float3 p0, float3 p1)
        {
            // approximate the zero crossing by finding the min value along the edge
            float minValue = 100000f;
            float t = 0f;
            float currentT = 0f;
            const int steps = 8;
            const float increment = 1f / (float)steps;
            while (currentT <= 1.0f)
            {
                float3 p = p0 + ((p1 - p0) * currentT);

                // Utiliza el valor absoluto de tal forma que el valor mas cercano a 0 es aquel que separa los dos tipos de materiales
                float density = Mathf.Abs(MyNoise.PerlinNoise3D.DensityFunction(p));
                if (density < minValue)
                {
                    minValue = density;
                    t = currentT;
                }

                currentT += increment;
            }

            return p0 + ((p1 - p0) * t);
        }

        public static float3 CalculateSurfaceNormal(Vector3 p)
        {
            float H = 0.001f;
            float dx = MyNoise.PerlinNoise3D.DensityFunction(p + new Vector3(H, 0.0f, 0.0f)) - MyNoise.PerlinNoise3D.DensityFunction(p - new Vector3(H, 0.0f, 0.0f));
            float dy = MyNoise.PerlinNoise3D.DensityFunction(p + new Vector3(0.0f, H, 0.0f)) - MyNoise.PerlinNoise3D.DensityFunction(p - new Vector3(0.0f, H, 0.0f));
            float dz = MyNoise.PerlinNoise3D.DensityFunction(p + new Vector3(0.0f, 0.0f, H)) - MyNoise.PerlinNoise3D.DensityFunction(p - new Vector3(0.0f, 0.0f, H));

            return  (float3) new Vector3(dx, dy, dz).normalized;
        }

        private static Cell[] GetSurrondingCells(int index0, int index1 ,Cell[] cells, GridVertex[] gridVertex, int resolution) 
        {

            Cell[] surrondigCells = new Cell[4];

            int surrondigCellsCount = 0;

            for (int i = 0; i < cells.Length; i++)
            {
                if (CellContainsVecrtiece(index0, cells[i]) && CellContainsVecrtiece(index1, cells[i])) 
                {
                    //Debug.Log($"[DualContoruing]Celda {cells[i].index} contiene a {index0}, {index1} ");
                    surrondigCells[surrondigCellsCount] = cells[i];
                    surrondigCellsCount++;
                }
            }

            return surrondigCells;
        }

        private static bool CellContainsVecrtiece(int vertice,  Cell  cell) 
        {
            bool control = false;

            control = control || (cell.corner0 == vertice);
            control = control || (cell.corner1 == vertice);
            control = control || (cell.corner2 == vertice);
            control = control || (cell.corner3 == vertice);
            control = control || (cell.corner4 == vertice);
            control = control || (cell.corner5 == vertice);
            control = control || (cell.corner6 == vertice);
            control = control || (cell.corner7 == vertice);

            return control;
        }

        private static int GetAxis(int v1, int v2, int resolution, GridVertex[] gridVertex) 
        {
            int diference = Math.Abs(v2 - v1);

            if (diference == 1)
            {
                return AXIS_X;
            }
            else if (diference == resolution)
            {
                return AXIS_Y;
            }
            else if (diference == resolution * resolution)
            {
                return AXIS_Z;
            }
            else 
                return -1;
        }

        private static bool ContainsEdge(IntersectingEdgesElement edge, List<IntersectingEdgesElement> edges) 
        {
            foreach (IntersectingEdgesElement element in edges) 
            {
                if (edge.vertexIndex0 == element.vertexIndex0 && edge.vertexIndex1 == element.vertexIndex1) 
                {
                    return true;
                }
            }

            return false;
        }
    } 
}