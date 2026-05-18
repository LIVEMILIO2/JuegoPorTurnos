using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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