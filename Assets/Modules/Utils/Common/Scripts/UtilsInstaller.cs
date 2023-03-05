using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.GizmosECS;

namespace Utils 
{
    public class UtilsInstaller : MonoBehaviour
    {
        [ContextMenu("Install utils services")]
        private void InstallUtilsServices()
        {
            Debug.Log("Installing utils services...");
            UtilsServerLocator.Instance.RegisterService<GizmoECS>(new GizmoECS());
        }
    }
}
