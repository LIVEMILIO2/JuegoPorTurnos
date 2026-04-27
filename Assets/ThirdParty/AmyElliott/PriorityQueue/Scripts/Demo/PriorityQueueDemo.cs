namespace AE
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PriorityQueueDemo : MonoBehaviour
    {
        [SerializeField] private Cube[] _cubes;
        [SerializeField] private Cube2[] _cubes2;
        [SerializeField] private GameObject _cubePrefab;
        [SerializeField] private GameObject _cubePrefab2;
        private PriorityQueue<Cube> _queue;
        private float _timer = 0;
        private float _dequeueInterval = 2.0f;

        void Start()
        {
            _queue = new PriorityQueue<Cube>();

            for (int i = 0; i < 2; i++)
            {
                GameObject temp = Instantiate(_cubePrefab, new Vector3(i * 2.0f, 0, 0), Quaternion.identity);
                GameObject temp2 = Instantiate(_cubePrefab, new Vector3(i * -2.0f, 0, 2.0f), Quaternion.identity);
                temp.GetComponent<Cube>().Size = Random.Range(1, 100);
                temp2.GetComponent<Cube2>().Size = Random.Range(1, 100);
                _queue.Enqueue(temp.GetComponent<Cube>(), temp.GetComponent<Cube>().Size);
            }
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