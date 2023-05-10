using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;

namespace Utils.GizmosECS 
{
    public class GizmoMonobehabiour : MonoBehaviour
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

    public class GizmoECS 
    {
        private static GizmoMonobehabiour Handler => _handler != null ? _handler : (_handler = CreateHandler());
        private static GizmoMonobehabiour _handler;

        public void OnDrawGizmos(Action action) 
        {
            Handler.DrawGizmos += action;
        }

        public void OnDrawGizmosSelected(Action action) 
        {
            Handler.DrawGizmosSelected += action;
        }

        private static GizmoMonobehabiour CreateHandler()
        {
            var go = new GameObject("Gizmo Handler");
            go.hideFlags = HideFlags.DontSave;

            return go.AddComponent<GizmoMonobehabiour>();
        }
    }
}