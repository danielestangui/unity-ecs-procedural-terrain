using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;

namespace GizmosNameSpace 
{
    public class GizmoMono : MonoBehaviour
    {
        public Action DrawGizmos;
        public Action DrawGizmosSelected;

        private void OnDrawGizmos()
        {
            DrawGizmos?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmosSelected?.Invoke();
        }
    }

    public static class MyGizmo 
    {
        private static GizmoMono Handler => _handler != null ? _handler : (_handler = CreateHandler());
        private static GizmoMono _handler;

        public static void OnDrawGizmos(Action action) 
        {
            Handler.DrawGizmos += action;
        }

        private static GizmoMono CreateHandler()
        {
            var go = new GameObject("Gizmo Handler");
            go.hideFlags = HideFlags.DontSave;

            return go.AddComponent<GizmoMono>();
        }
    }
}