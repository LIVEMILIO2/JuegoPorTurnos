namespace AE
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PriorityQueueDemo : MonoBehaviour
    {
        [SerializeField] public Cube[] _cubes;
        [SerializeField] private GameObject _cubePrefab;
        private PriorityQueue<Cube> _queue;
        private float _timer = 0;
        private float _dequeueInterval = 2.0f;

        void Start()
        {
            _queue = new PriorityQueue<Cube>();

            for (int i = 0; i < 5; i++)
            {
                GameObject temp = Instantiate(_cubePrefab, new Vector3(i * 2.0f, 0, 0), Quaternion.identity);
                temp.GetComponent<Cube>().Size = Random.Range(1, 100); // Assign a random size as priority
                _queue.Enqueue(temp.GetComponent<Cube>(), temp.GetComponent<Cube>().Size);
            }

            // Enqueue objects with their priorities
            //foreach (Cube obj in _cubes)
            //{
            //    int priority = Random.Range(1, 100); // Assign a random priority
            //    _queue.Enqueue(obj, priority);
            //}
        }
            

        void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= _dequeueInterval)
            {
                if (!_queue.IsEmpty())
                {
                    Cube cube = _queue.Dequeue();
                    cube.ProcessCube();
                    Debug.Log($"Dequeued cube with priority: {cube.Size}");

                }
                _timer = 0;
            }
        }

        
    }
}