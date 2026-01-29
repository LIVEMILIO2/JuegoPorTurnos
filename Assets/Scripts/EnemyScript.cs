using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 5f;
    public float altura = 0.5f;
    public float vida = 100;

    private Vector3 target;

    private bool moving = false;
    void Start()
    {
        if (moving)
            Mover();
        
        
    }
    void Update()
    {
        
    }
    public void Mover()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            transform.position = target;
            moving = false;
        }
    }
    public void SetTarget(Vector3 destino)
    {
        destino.y = altura;
        target = destino;
        moving = true;
    }
    public bool Moving()
    {
        return moving;
    }
    public void Die()
    {
        if(vida  < 0)
        {
            Destroy(gameObject);
        }
    }
}
