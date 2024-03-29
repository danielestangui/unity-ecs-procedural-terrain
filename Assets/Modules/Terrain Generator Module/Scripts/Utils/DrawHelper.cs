using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using System;
using Shapes;

namespace TerrainGenerator.Utils
{
    public static class DrawHelper
    {
        public static bool dirtyFlag = false;
        private static ArrayList onDrawGizmoActions = new ArrayList();

        private enum Plane { XZ, XY, YZ }

        public static void DrawCube(float3 center, float side, float thickness, Color color) 
        {
            float halfside = side * 0.5f;

            float3[] cornners = new float3[]
            {
               center + new float3(-1,-1,-1) * halfside,
               center + new float3(1,-1,-1) * halfside,
               center + new float3(1,1,-1) * halfside,
               center + new float3(-1,1,-1) * halfside,
               center + new float3(-1,-1,1) * halfside,
               center + new float3(1,-1,1) * halfside,
               center + new float3(1,1,1) * halfside,
               center + new float3(-1,1,1) * halfside
            };

            onDrawGizmoActions.Add(new Action(() =>
                {
                    Draw.Thickness = thickness;
                    Draw.ThicknessSpace = ThicknessSpace.Pixels;
                    Draw.LineGeometry = LineGeometry.Volumetric3D;

                    Draw.Line(cornners[0], cornners[1], color);
                    Draw.Line(cornners[1], cornners[2], color);
                    Draw.Line(cornners[2], cornners[3], color);
                    Draw.Line(cornners[3], cornners[0], color);

                    Draw.Line(cornners[4], cornners[5], color);
                    Draw.Line(cornners[5], cornners[6], color);
                    Draw.Line(cornners[6], cornners[7], color);
                    Draw.Line(cornners[7], cornners[4], color);

                    Draw.Line(cornners[0], cornners[4], color);
                    Draw.Line(cornners[1], cornners[5], color);
                    Draw.Line(cornners[2], cornners[6], color);
                    Draw.Line(cornners[3], cornners[7], color);

                }));
        }

        private static void DrawCircle(float3 center, float radius, Color color, Plane plane = Plane.XZ)
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

        public static void DrawLine(Vector3 from, Vector3 to, Color color) 
        {
            onDrawGizmoActions.Add(new Action(() => Draw.Line(from, to, color)));
        }

        public static void DrawSphere(float3 position, float radius, Color color) 
        {
            //onDrawGizmoActions.Add(new Action(() => Gizmos.color = color));
            //onDrawGizmoActions.Add(new Action(() => Gizmos.DrawSphere(position,radius)));

            onDrawGizmoActions.Add(new Action(() => 
            {
                Draw.Sphere(position, radius, color);
            }));
        }

        public static void DrawCircleSphere(float3 position, float radius, Color color)
        {
            DrawCircle(position, radius, color, Plane.XZ);
            DrawCircle(position, radius, color, Plane.XY);
            DrawCircle(position, radius, color, Plane.YZ);
        }

        public static void DrawText(float3 position, string text) 
        {
            onDrawGizmoActions.Add(new Action(() => Handles.Label(position, text)));
        }
        public static ArrayList GetOnDrawGizmoActions()
        {
            return onDrawGizmoActions;
        }

        public static void ClearOnDrawGizmoActions()
        {
            onDrawGizmoActions.Clear();
        }

    }
}