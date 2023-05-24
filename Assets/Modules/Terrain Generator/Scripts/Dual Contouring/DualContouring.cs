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
	        new int[2]{0,4},new int[2]{1,5},new int[2]{2,6},new int[2]{3,7},	// x-axis 
	        new int[2]{0,2},new int[2]{1,3},new int[2]{4,6},new int[2]{5,7},	// y-axis
	        new int[2]{0,1},new int[2]{2,3},new int[2]{4,5},new int[2]{6,7}		// z-axis
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

        public static VerticeElement CalculatePoint(int index, GridVertex[] vertices, Cell[] cells, int resolution, ref List<IntersectingEdgesElement> edges)
        {
            int corners = 0;

            int[] cornersArray = 
                {
                    cells[index].corner0,
                    cells[index].corner1,
                    cells[index].corner2,
                    cells[index].corner3,
                    cells[index].corner4,
                    cells[index].corner5,
                    cells[index].corner6,
                    cells[index].corner7
                };

            corners |= (vertices[cells[index].corner0].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 0;
            corners |= (vertices[cells[index].corner1].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 1;
            corners |= (vertices[cells[index].corner2].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 2;
            corners |= (vertices[cells[index].corner3].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 3;
            corners |= (vertices[cells[index].corner4].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 4;
            corners |= (vertices[cells[index].corner5].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 5;
            corners |= (vertices[cells[index].corner6].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 6;
            corners |= (vertices[cells[index].corner7].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 7;

            if (corners == 0 || corners == 255)
            {
                return new VerticeElement 
                {
                    position = Vector3.zero,
                    normal = Vector3.zero
                };
            }

            //Debug.Log($"Conners binary: {Convert.ToString(corners, 2)}");


            const int MAX_CROSSINGS = 6;
            int edgeCount = 0;
            float3 averageNormal = Vector3.zero;
            QefSolver qef = new QefSolver();

            for (int i = 0; i < 12 && edgeCount < MAX_CROSSINGS; i++)
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

                float3 p1 = vertices[cornersArray[c1]].position;
                float3 p2 = vertices[cornersArray[c2]].position;

                float3 p = ApproximateZeroCrossingPosition(p1, p2);

                //Debug.Log($"Para {edgevmap[i][0]} y {edgevmap[i][1]} pinta.");

                //Draw.DrawSphere(p, 0.1f, Color.red);

                float3 n = CalculateSurfaceNormal(p);
                qef.add(p.x, p.y, p.z, n.x, n.y, n.z);

                averageNormal += n;

                Cell[] surrondingCells = GetSurrondingCells(cornersArray[c1], cornersArray[c2],cells);

                IntersectingEdgesElement edge = new IntersectingEdgesElement
                {
                    vertexIndex0 = cornersArray[c1],
                    vertexIndex1 = cornersArray[c2],
                    axis = GetAxis(c1, c2),
                    sharedCells00 = surrondingCells[0],
                    sharedCells01 = surrondingCells[1],
                    sharedCells10 = surrondingCells[2],
                    sharedCells11 = surrondingCells[3]
                };

                edges.Add(edge);
                edgeCount++;
            }

            Vector3 qefPosition = Vector3.zero;
            qef.solve(ref qefPosition, QEF_ERROR, QEF_SWEEPS, QEF_ERROR);

            float3 position = new float3(qefPosition.x, qefPosition.y, qefPosition.z);

            VerticeElement vertice = new VerticeElement
            {
                position = new float3(qefPosition.x, qefPosition.y, qefPosition.z),
                normal = averageNormal / edgeCount,
                cell = cells[index]
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

        private static Cell[] GetSurrondingCells(int index0, int index1 ,Cell[] cells) 
        {
            Cell[] surrondigCells = new Cell[8];

            int cellIndex = 0;

            foreach (Cell cell in cells) 
            {
                if (CellContainsVecrtiece(index0, cell) && CellContainsVecrtiece(index1, cell)) 
                {
                    if (cellIndex > surrondigCells.Length) 
                    {
                        Debug.Log("Error ");
                        continue;
                    }
                    surrondigCells[cellIndex] = cell;
                    cellIndex++;
                }
            }

            return surrondigCells;
        }

        private static bool CellContainsVecrtiece(int vertice,  Cell  cell) 
        {
            bool control = false;

            control |= cell.corner0 == vertice;
            control |= cell.corner1 == vertice;
            control |= cell.corner2 == vertice;
            control |= cell.corner3 == vertice;
            control |= cell.corner4 == vertice;
            control |= cell.corner5 == vertice;
            control |= cell.corner6 == vertice;
            control |= cell.corner7 == vertice;

            return control;
        }

        private static int GetAxis(int c1, int c2) 
        {
            //Debug.Log($"Puntos a evaluar; {c1}, {c2}");

            return AXIS_X;
        }
    }
}