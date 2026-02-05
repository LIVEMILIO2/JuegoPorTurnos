using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 5f;
    public float altura = 0.5f;
    public float Vida = 100;
    public int enemyMoveRange = 3;

    List<Vector3> path = new List<Vector3>();
    int index = 0;
    bool moving = false;

    void Update()
    {
        if (moving)
            Mover();
        if (CompareTag("Player"))
        {
            Vida -= 50;
            
        }
        if (Vida < 0)
            die();
    }

    public void SetPath(List<Vector3> nuevoPath)
    {
        path = nuevoPath;
        index = 0;
        moving = path.Count > 0;
    }

    void Mover()
    {
        Vector3 target = path[index];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            index++;

            if (index >= path.Count)
            {
                moving = false;
                GameManager.Instance.SiguienteTurno();
            }
        }
    }
    void die()
    {   
            Destroy(gameObject);
        
    }
    public bool Moving()
    {
        return moving;
    }
}
