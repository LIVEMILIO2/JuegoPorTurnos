namespace AE
{
    using UnityEngine;

    public class Cube2 : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
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
                renderer.material.color = Color.red;
            }
        }
    }
}
