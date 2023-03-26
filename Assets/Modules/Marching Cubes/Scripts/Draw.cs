using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGenerator 
{
    public static class Draw
    {
        public enum Plane { XZ, XY, YZ }

        public static void DrawCircle(float3 center, float radius, Color color, Plane plane = Plane.XZ)
        {
            const int numSegments = 10;
            float3 prevPos = float3.zero;

            // Debug rendering (the white circle).
            for (float segment = 0; segment <= numSegments; segment ++)
            {
                float angle = segment * math.PI * 2f / numSegments;
                float a = math.sin(angle) * radius;
                float b = math.cos(angle) * radius;
                float3 pos = float3.zero;

                switch (plane)
                {
                    case Plane.XZ:
                        pos = new float3(center.x + a, center.y, center.z + b);
                        break;

                    case Plane.XY:
                        pos = new float3(center.x + a, center.y + b, center.z);
                        break;

                    case Plane.YZ:
                        pos = new float3(center.x, center.y + a, center.z + b);
                        break;
                }

                if (segment > 0) 
                {
                    Debug.DrawLine(prevPos, pos, color);
                }

                prevPos = pos;
            }
        }


        public static void DrawSphere(float3 position, float radius, Color color)
        {
            DrawCircle(position, radius, color, Plane.XZ);
            DrawCircle(position, radius, color, Plane.XY);
            DrawCircle(position, radius, color, Plane.YZ);
        }
    }
}