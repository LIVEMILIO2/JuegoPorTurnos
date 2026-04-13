using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed = 5f;
    public float altura = 0.5f;

    public float damage = 50f;
    public float health = 100f;
    public float rangoAtaque = 1.5f;
    public int playerMoveRange = 3;
    public string playerStats = "Warrior";
    List<Vector3> path = new List<Vector3>();
    int index = 0;
    bool moviendose = false;
    bool yaSeMovio = false;
    bool yaAtaco = false;

    void Update()
    {
        if (moviendose)
            Mover();

        if (EsMiTurno())
        {
            if (!yaAtaco && Input.GetKeyDown(KeyCode.F))
                IntentarAtacar();

            if (Input.GetKeyDown(KeyCode.Space))
                GameManager.Instance.SiguienteTurno();
        }
    }

    void Mover()
    {
        Vector3 target = path[index];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            transform.position = target;
            index++;

            if (index >= path.Count)
            {
                moviendose = false;
                yaSeMovio = true;
                VerificarFinTurno();
            }
        }
    }

    public void SetPath(List<Vector3> nuevoPath)
    {
        if (yaSeMovio) return;
        path = nuevoPath;
        index = 0;
        if (path.Count == 0) return;
        moviendose = true;
    }

    void IntentarAtacar()
    {
        if (GameManager.Instance.enemies.Count == 0) return;

        EnemyScript objetivo = null;
        float distanciaMinima = Mathf.Infinity;

        var enemigosActuales = new List<EnemyScript>(GameManager.Instance.enemies);
        foreach (EnemyScript enemy in enemigosActuales)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= rangoAtaque && dist < distanciaMinima)
            {
                distanciaMinima = dist;
                objetivo = enemy;
            }
        }

        if (objetivo != null)
        {
            Debug.Log("PLAYER ATACA");
            objetivo.RecibirDamage(damage);
            yaAtaco = true;
            VerificarFinTurno();
        }
        else
        {
            Debug.Log("No hay enemigos en rango");
        }
    }

    void VerificarFinTurno()
    {
        if (yaSeMovio && yaAtaco)
        {
            Debug.Log("Jugador termino turno");
            GameManager.Instance.SiguienteTurno();
        }
    }

    public void ReiniciarTurno()
    {
        yaSeMovio = false;
        yaAtaco = false;
    }

    bool EsMiTurno()
    {
        return GameManager.Instance.JugadorActual() == this;
    }

    public bool EstaMoviendose()
    {
        return moviendose;
    }

    public void RecibirDamage(float cantidad)
    {
        health -= cantidad;
        Debug.Log($"{name} recibe daño: {cantidad}. Vida restante: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    // NUEVO: Método de muerte
    void Die()
    {
        Debug.Log($"{name} ha muerto.");
        if (GameManager.Instance != null)
            GameManager.Instance.RemovePlayer(this);

        Destroy(gameObject);
    }
}