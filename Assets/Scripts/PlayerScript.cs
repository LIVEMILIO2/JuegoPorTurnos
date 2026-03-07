using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed = 5f;
    public float altura = 0.5f;

    public float damage = 50f;
    public float health = 100f;
    public float rangoAtaque = 1.5f;

    Vector3 target;

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
        transform.position =
            Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            transform.position = target;

            moviendose = false;

            yaSeMovio = true;

            VerificarFinTurno();
        }
    }

    public void SetTarget(Vector3 destino)
    {
        if (yaSeMovio) return;

        destino.y = altura;

        target = destino;

        moviendose = true;
    }

    void IntentarAtacar()
    {
        if (GameManager.Instance.enemies.Count == 0)
            return;

        EnemyScript enemy = GameManager.Instance.enemies[0];

        float distancia =
            Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

        if (distancia <= rangoAtaque)
        {
            Debug.Log("PLAYER ATACA");

            enemy.RecibirDamage(damage);

            yaAtaco = true;

            VerificarFinTurno();
        }
        else
        {
            Debug.Log("Fuera de rango");
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
        Debug.Log(name + " recibe daño: " + cantidad);
    }
}