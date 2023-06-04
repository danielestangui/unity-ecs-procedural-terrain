using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace TerrainGenerator.Utils 
{
    public class DrawMono : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            if (Application.isPlaying) 
            {

                foreach (Action action in Draw.GetOnDrawGizmoActions())
                {
                    action();
                }

                Draw.ClearOnDrawGizmoActions();
            }

        }
    }
}