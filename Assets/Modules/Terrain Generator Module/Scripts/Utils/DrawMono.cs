using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Shapes;

namespace TerrainGenerator.Utils 
{
    [ExecuteAlways]
    public class DrawMono : ImmediateModeShapeDrawer
    {
        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam))
            {
                foreach (Action action in DrawHelper.GetOnDrawGizmoActions())
                {
                    action();
                }
            }
        }     
    }
}