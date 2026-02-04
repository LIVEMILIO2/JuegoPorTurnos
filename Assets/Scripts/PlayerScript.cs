using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed = 5f;
    public float altura = 0.5f;

    Vector3 target;
    bool moviendose = false;

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
