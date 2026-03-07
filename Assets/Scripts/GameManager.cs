using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<PlayerScript> players = new List<PlayerScript>();
    public List<EnemyScript> enemies = new List<EnemyScript>();

    int turnoIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (players.Count > 0)
            players[0].ReiniciarTurno();
    }

    void Update()
    {
        if (EsTurnoPlayer())
            DetectarClickMovimiento();
    }

    void DetectarClickMovimiento()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        PlayerScript player = JugadorActual();

        if (player == null)
            return;

        if (player.EstaMoviendose())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 destino = hit.point;

            Vector2Int start =
                GraphCreator.Instance.WorldToGrid(
                    player.transform.position
                );

            Vector2Int goal =
                GraphCreator.Instance.WorldToGrid(
                    destino
                );

            GraphCreator.Instance.CalcularCamino(
                start,
                goal,
                player
            );
        }
    }

    public PlayerScript JugadorActual()
    {
        if (turnoIndex < players.Count)
            return players[turnoIndex];

        return null;
    }

    public EnemyScript EnemyActual()
    {
        int index = turnoIndex - players.Count;

        if (index >= 0 && index < enemies.Count)
            return enemies[index];

        return null;
    }

    public bool EsTurnoPlayer()
    {
        return turnoIndex < players.Count;
    }

    public bool EsTurnoEnemy()
    {
        return turnoIndex >= players.Count;
    }

    public void SiguienteTurno()
    {
        turnoIndex++;

        if (turnoIndex >= players.Count + enemies.Count)
            turnoIndex = 0;

        if (EsTurnoPlayer())
        {
            JugadorActual().ReiniciarTurno();
        }
        else
        {
            EnemyActual().TomarTurno();
        }
    }
    public void BotonPasarTurno()
    {
        Debug.Log("BOTON PASAR TURNO PRESIONADO");

        SiguienteTurno();
        //graphCreator.ResetVisual();
    }
}