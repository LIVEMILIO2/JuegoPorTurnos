using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed = 5f;     
    public float altura = 0.5f;  

    private Vector3 target;
    private bool moviendose = false;

    void Update()
    {
        if (moviendose)
            Mover();
    }

    public void SetTarget(Vector3 destino)
    {
        destino.y = altura; 
        target = destino;
        moviendose = true;
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
    public bool EstaMoviendose()
    {
        return moviendose;
    }
}
