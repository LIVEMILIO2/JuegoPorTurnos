using System.Diagnostics;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed = 5f;
    public float altura = 0.5f;
    public float damage = 50;
    Vector3 target;
    bool moviendose = false;

    EnemyScript enemy;
    void Update()
    {
        if (moviendose)
            Mover();

       
    }

    void Mover()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            transform.position = target;
            moviendose = false;
            GameManager.Instance.SiguienteTurno();
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
           
            if (CompareTag("Enemy"))
            {
                enemy.Vida -= damage;
            }
        }

    }

    public void SetTarget(Vector3 destino)
    {
        destino.y = altura;
        target = destino;
        moviendose = true;
    }

    public bool EstaMoviendose()
    {
        return moviendose;
    }
}
