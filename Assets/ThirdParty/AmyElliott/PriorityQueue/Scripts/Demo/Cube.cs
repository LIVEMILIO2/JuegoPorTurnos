namespace AE
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Cube : MonoBehaviour
    {
        public int Size = 1;

       void newValue(int size)
        {
            Size = size;
        }

        public void ResetNode()
        {
            Size = 0;
        }
        public void ProcessCube()
        {
            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.magenta;
            }
        }
    }
}