using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("VFX")]
    public GameObject prefabAtaque; // arrastra tu prefab de particle system aquí

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Instancia el VFX en la posición del objetivo y lo destruye al terminar.
    /// </summary>
    public void ReproducirAtaque(Vector3 posicion)
    {
        if (prefabAtaque == null) return;

        GameObject vfx = Instantiate(prefabAtaque, posicion, Quaternion.identity);

        // Obtener duración del particle system y destruir al terminar
        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
            Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
        else
            Destroy(vfx, 2f); // fallback si no encuentra el ParticleSystem
    }
}