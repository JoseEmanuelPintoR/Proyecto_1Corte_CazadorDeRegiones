using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Volúmenes")]
    [SerializeField] private float volumenMusica = 0.5f;
    [SerializeField] private float volumenEfectos = 1f;

    [Header("Click")]

    [SerializeField] private float inicioClick = 0.69f;
    [SerializeField] private float duracionClick = 0.12f;
    [SerializeField] private float fundidoClick = 0.03f;
    [SerializeField] private float volumenClick = 0.35f;

    private AudioSource fuenteMusica;
    private AudioSource fuenteEfectos;
    private AudioSource fuenteClick;

    private Coroutine corteClick;

    private AudioClip musicaMenu;
    private AudioClip musicaAndina;
    private AudioClip musicaCaribe;
    private AudioClip musicaPacifica;
    private AudioClip musicaOrinoquia;
    private AudioClip musicaAmazonia;

    private AudioClip sonidoCorrecto;
    private AudioClip sonidoIncorrecto;
    private AudioClip sonidoVictoria;
    private AudioClip sonidoDerrota;
    private AudioClip sonidoClick;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CrearAutomaticamente()
    {
        if (Instance != null)
            return;

        GameObject objeto = new GameObject("AudioManager");
        objeto.AddComponent<AudioManager>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CrearFuentes();
        CargarClips();

        SceneManager.sceneLoaded += AlCargarEscena;

        ConfigurarEscena(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= AlCargarEscena;
            Instance = null;
        }
    }

    private void CrearFuentes()
    {
        fuenteMusica = gameObject.AddComponent<AudioSource>();
        fuenteMusica.playOnAwake = false;
        fuenteMusica.loop = true;
        fuenteMusica.spatialBlend = 0f;
        fuenteMusica.volume = volumenMusica;

        fuenteEfectos = gameObject.AddComponent<AudioSource>();
        fuenteEfectos.playOnAwake = false;
        fuenteEfectos.loop = false;
        fuenteEfectos.spatialBlend = 0f;
        fuenteEfectos.volume = volumenEfectos;
        fuenteClick = gameObject.AddComponent<AudioSource>();
        fuenteClick.playOnAwake = false;
        fuenteClick.loop = false;
        fuenteClick.spatialBlend = 0f;
        fuenteClick.volume = volumenClick;
    }

    private void CargarClips()
    {
        musicaMenu = Cargar("Menu");
        musicaAndina = Cargar("Region Andina");
        musicaCaribe = Cargar("Region Caribe");
        musicaPacifica = Cargar("Region Pacifica");
        musicaOrinoquia = Cargar("Region Orinoquia");
        musicaAmazonia = Cargar("Region Amazonia");

        sonidoCorrecto = Cargar("sonido-correcto");
        sonidoIncorrecto = Cargar("incorrect-answer");
        sonidoVictoria = Cargar("Ganador");
        sonidoDerrota = Cargar("Perdiste");
        sonidoClick = Cargar("Efecto Click");
    }

    private AudioClip Cargar(string nombre)
    {
        AudioClip clip = Resources.Load<AudioClip>("Music/" + nombre);

        if (clip == null)
        {
            Debug.LogWarning("No se encontró el audio: Assets/Resources/Music/" + nombre);
        }

        return clip;
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        ConfigurarEscena(escena.name);
    }

    private void ConfigurarEscena(string nombreEscena)
    {
        ReproducirMusicaDeEscena(nombreEscena);
        EngancharSonidoDeBotones();
    }

    private void ReproducirMusicaDeEscena(string nombreEscena)
    {
        AudioClip clip;

        switch (nombreEscena)
        {
            case "Nivel1_Andina":
                clip = musicaAndina;
                break;
            case "Nivel2_Caribe":
                clip = musicaCaribe;
                break;
            case "Nivel3_Pacifica":
                clip = musicaPacifica;
                break;
            case "Nivel4_Orinoquia":
                clip = musicaOrinoquia;
                break;
            case "Nivel5_Amazonia":
                clip = musicaAmazonia;
                break;
            default:

                clip = musicaMenu;
                break;
        }

        ReproducirMusica(clip);
    }

    private void ReproducirMusica(AudioClip clip)
    {
        if (clip == null)
        {
            DetenerMusica();
            return;
        }

        if (fuenteMusica.clip == clip && fuenteMusica.isPlaying)
            return;

        fuenteMusica.clip = clip;
        fuenteMusica.volume = volumenMusica;
        fuenteMusica.Play();
    }
    private void EngancharSonidoDeBotones()
    {
        Button[] botones = FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        for (int i = 0; i < botones.Length; i++)
        {
            botones[i].onClick.AddListener(ReproducirClick);
        }
    }

    public void ReproducirCorrecto()
    {
        ReproducirEfecto(sonidoCorrecto);
    }

    public void ReproducirIncorrecto()
    {
        ReproducirEfecto(sonidoIncorrecto);
    }

    public void ReproducirClick()
    {
        if (sonidoClick == null || fuenteClick == null)
            return;

        if (corteClick != null)
        {
            StopCoroutine(corteClick);
        }

        fuenteClick.clip = sonidoClick;
        fuenteClick.volume = volumenClick;

        fuenteClick.time = Mathf.Clamp(inicioClick, 0f, sonidoClick.length - 0.01f);
        fuenteClick.Play();

        corteClick = StartCoroutine(CortarClick());
    }

    private IEnumerator CortarClick()
    {
        float espera = duracionClick - fundidoClick;

        if (espera > 0f)
        {
            yield return new WaitForSecondsRealtime(espera);
        }

        float restante = fundidoClick;

        while (restante > 0f && fuenteClick.isPlaying)
        {
            restante -= Time.unscaledDeltaTime;
            fuenteClick.volume = volumenClick * Mathf.Clamp01(restante / fundidoClick);
            yield return null;
        }

        fuenteClick.Stop();
        fuenteClick.volume = volumenClick;

        corteClick = null;
    }

    public void ReproducirVictoria()
    {
        DetenerMusica();
        ReproducirEfecto(sonidoVictoria);
    }

    public void ReproducirDerrota()
    {
        DetenerMusica();
        ReproducirEfecto(sonidoDerrota);
    }

    public void DetenerMusica()
    {
        if (fuenteMusica != null)
        {
            fuenteMusica.Stop();
        }
    }

    private void ReproducirEfecto(AudioClip clip)
    {
        if (clip == null || fuenteEfectos == null)
            return;

        fuenteEfectos.PlayOneShot(clip, volumenEfectos);
    }
}
