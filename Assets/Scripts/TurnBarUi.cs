using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barra de turnos vertical en Canvas, sin sprites.
/// 
/// Setup:
/// 1. Crea un Panel vertical en el Canvas (ancho ~70px, alto toda la pantalla)
///    Agrégale Vertical Layout Group (spacing 5, child force expand desactivado)
///    y Content Size Fitter (vertical: Preferred Size)
/// 2. Agrégale este script
/// 3. Crea el prefab TurnSlot (ver TurnSlot.cs)
/// </summary>
public class TurnBarUI : MonoBehaviour
{
    public static TurnBarUI Instance;

    [Header("Referencias")]
    public Transform container;
    public GameObject slotPrefab;

    private List<TurnSlot> slots = new List<TurnSlot>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Refrescar(List<TurnEntry> entradas, MonoBehaviour activo)
    {
        while (slots.Count < entradas.Count)
        {
            GameObject go = Instantiate(slotPrefab, container);
            slots.Add(go.GetComponent<TurnSlot>());
        }

        for (int i = 0; i < slots.Count; i++)
            slots[i].gameObject.SetActive(i < entradas.Count);

        for (int i = 0; i < entradas.Count; i++)
        {
            bool esActivo = entradas[i].entidad == activo;
            slots[i].Setup(entradas[i].esPlayer, esActivo);
        }
    }
}