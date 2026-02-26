using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 5f;
    public float altura = 0.5f;

    public float Vida = 100;

    public int enemyMoveRange = 3;

    public float enemyAtackRange = 1.5f;

    public float damage = 25f;

    List<Vector3> path = new List<Vector3>();

    int index = 0;

    bool moving = false;

    PlayerScript objetivoActual;


    void Update()
    {
        if (moving)
            Mover();
    }


    public void SetPath(List<Vector3> nuevoPath)
    {
        Debug.Log("Enemy recibio path");

        path = nuevoPath;

        index = 0;

        objetivoActual = ObtenerObjetivo();

        if (path.Count == 0)
        {
            Debug.Log("Enemy ya esta en rango, ataca directamente");

            IntentarAtacar();

            GameManager.Instance.SiguienteTurno();

            return;
        }

        moving = true;
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
            transform.position = target;

            index++;

            if (index >= path.Count)
            {
                moving = false;

                IntentarAtacar();

                GameManager.Instance.SiguienteTurno();
            }
        }
    }


    void IntentarAtacar()
    {
        if (objetivoActual == null)
            objetivoActual = ObtenerObjetivo();

        float distancia =
            Vector3.Distance(
                transform.position,
                objetivoActual.transform.position
            );

        Debug.Log("Enemy intenta atacar");
        Debug.Log("Distancia: " + distancia);

        if (distancia <= enemyAtackRange)
        {
            Debug.Log("ENEMY ATACA");

            objetivoActual.RecibirDamage(damage);
        }
        else
        {
            Debug.Log("Enemy fuera de rango");
        }
    }


    PlayerScript ObtenerObjetivo()
    {
        float d1 =
            Vector3.Distance(
                transform.position,
                GameManager.Instance.player1.transform.position
            );

        float d2 =
            Vector3.Distance(
                transform.position,
                GameManager.Instance.player2.transform.position
            );

        if (d1 <= d2)
            return GameManager.Instance.player1;
        else
            return GameManager.Instance.player2;
    }


    public void RecibirDamage(float cantidad)
    {
        Vida -= cantidad;

        Debug.Log("Enemy vida: " + Vida);

        if (Vida <= 0)
            Die();
    }


    void Die()
    {
        Debug.Log("Enemy murio");

        Destroy(gameObject);
    }


    public bool Moving()
    {
        return moving;
    }
}