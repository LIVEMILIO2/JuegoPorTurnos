using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX")]
    public AudioClip sonidoAtaque;
    [Range(0f, 1f)] public float volumenSFX = 1f;

    [Header("Música de fondo")]
    public AudioClip musicaFondo;
    [Range(0f, 1f)] public float volumenMusica = 0.4f;

    private AudioSource fuenteSFX;
    private AudioSource fuenteMusica;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        fuenteSFX = GetComponent<AudioSource>();
        fuenteSFX.playOnAwake = false;
        fuenteSFX.spatialBlend = 0f;

        fuenteMusica = gameObject.AddComponent<AudioSource>();
        fuenteMusica.playOnAwake = false;
        fuenteMusica.loop = true;
        fuenteMusica.spatialBlend = 0f;
        fuenteMusica.volume = volumenMusica;
    }

    void Start()
    {
        if (musicaFondo != null)
        {
            fuenteMusica.clip = musicaFondo;
            fuenteMusica.Play();
        }
    }

    public void ReproducirAtaque()
    {
        if (sonidoAtaque == null || fuenteSFX == null) return;
        fuenteSFX.PlayOneShot(sonidoAtaque, volumenSFX);
    }

    public void CambiarMusica(AudioClip clip)
    {
        if (clip == null || fuenteMusica == null) return;
        fuenteMusica.clip = clip;
        fuenteMusica.Play();
    }

    public void SetVolumenMusica(float v)
    {
        volumenMusica = Mathf.Clamp01(v);
        if (fuenteMusica != null) fuenteMusica.volume = volumenMusica;
    }
}