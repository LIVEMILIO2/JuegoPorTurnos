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
    /// Instancia el VFX en la posición del objetivo, orientado hacia el atacante.
    /// </summary>
    public void ReproducirAtaque(Vector3 posicion, Vector3? posicionAtacante = null)
    {
        if (prefabAtaque == null) return;

        Quaternion rotacion = Quaternion.identity;
        if (posicionAtacante.HasValue)
        {
            Vector3 dir = posicionAtacante.Value - posicion;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                rotacion = Quaternion.LookRotation(dir);
        }

        GameObject vfx = Instantiate(prefabAtaque, posicion, rotacion);

        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
            Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
        else
            Destroy(vfx, 2f);
    }
}