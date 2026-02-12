using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 5f;
    public float altura = 0.5f;
    public float Vida = 100;
    public int enemyMoveRange = 3;
    public float enemyAtackRange = 1.5f;

    List<Vector3> path = new List<Vector3>();
    int index = 0;
    bool moving = false;

    void Update()
    {
        if (moving)
            Mover();
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
    public void CheckDistance()
    {
        PlayerScript player1 = GameManager.Instance.player1;
        PlayerScript player2 = GameManager.Instance.player2;
        float dist1 = Vector3.Distance(transform.position, player1.transform.position);
        float dist2 = Vector3.Distance(transform.position, player2.transform.position);
        if (enemyAtackRange >= dist1)
        {
            GraphCreator.Instance.StopEnemy();
        }
        if (enemyAtackRange >= dist2)
        {
            GraphCreator.Instance.StopEnemy();
        }
    }

    public void RecibirDamage(float cantidad)
    {
        Vida -= cantidad;
        Debug.Log("Vida enemy: " + Vida);

        if (Vida <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public bool Moving()
    {
        return moving;
    }
}
