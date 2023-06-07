using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctreeModule 
{
    public static class OctreeUtils
    {

        private static Color[] colors =
            {
        Color.white,
        Color.yellow,
        Color.red,
        Color.green,
        Color.blue,
        Color.cyan,
        Color.magenta,
        Color.gray,
        Color.black
    };

        public static Color GetColor(int index)
        {
            return colors[index % colors.Length];
        }
    }

}