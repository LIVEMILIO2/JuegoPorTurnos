using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed = 5f;
    public float altura = 0.5f;
    public float damage = 50f;
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
        transform.position = Vector3.MoveTowards(
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
        EnemyScript enemy = GameManager.Instance.enemy;

        float distancia = Vector3.Distance(
            transform.position,
            enemy.transform.position
        );

        if (distancia <= rangoAtaque)
        {
            enemy.RecibirDamage(damage);
            yaAtaco = true;
            Debug.Log("Enemy golpeado!");
            Debug.Log(enemy.Vida);
            VerificarFinTurno();
        }
        else
        {
            Debug.Log("Enemy fuera de rango");
        }
    }

    void VerificarFinTurno()
    {
        if (yaSeMovio || yaAtaco)
        {
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
        if (GameManager.Instance.turnoActual == Turno.Player1 &&
            GameManager.Instance.player1 == this)
            return true;

        if (GameManager.Instance.turnoActual == Turno.Player2 &&
            GameManager.Instance.player2 == this)
            return true;

        return false;
    }

    public bool EstaMoviendose()
    {
        return moviendose;
    }
}
