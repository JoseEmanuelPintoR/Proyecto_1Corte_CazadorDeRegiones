using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Puntaje")]
    [SerializeField] private int puntos = 0;
    [SerializeField] private int puntosObjetivo = 1000;

    [Header("Vidas")]
    [SerializeField] private int vidas = 3;

    [Header("Power Up")]
    [SerializeField] private int cargaPowerUp = 0;
    [SerializeField] private int cargaMaxima = 3;
    [SerializeField] private float duracionPowerUp = 5f;

    private bool powerUpActivo = false;

    public int CargaPowerUp => cargaPowerUp;
    public int CargaMaxima => cargaMaxima;
    public bool PowerUpActivo => powerUpActivo;

    [Header("Tiempo")]
    [SerializeField] private float tiempoRestante = 60f;

    [Header("Pantallas")]
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private GameObject panelDerrota;

    private bool juegoFinalizado = false;

    public int Puntos => puntos;
    public int Vidas => vidas;
    public int PuntosObjetivo => puntosObjetivo;
    public float TiempoRestante => tiempoRestante;

    [Header("Pausa")]
    [SerializeField] private GameObject panelPausa;

    private bool juegoPausado = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (juegoFinalizado)
            return;

        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            PerderNivel();
        }
    }

    public void SumarPuntos(int cantidad)
    {
        if (juegoFinalizado)
            return;

        puntos += cantidad;

        Debug.Log("Puntos: " + puntos);

        if (puntos >= puntosObjetivo)
        {
            GanarNivel();
        }
    }

    public void RestarPuntos(int cantidad)
    {
        if (juegoFinalizado)
            return;

        puntos -= cantidad;

        if (puntos < 0)
        {
            puntos = 0;
        }

        Debug.Log("Puntos: " + puntos);
    }

    public void PerderVida()
    {
        if (juegoFinalizado)
            return;

        vidas--;

        Debug.Log("Vidas: " + vidas);

        if (vidas <= 0)
        {
            PerderNivel();
        }
    }

    void GanarNivel()
    {
        juegoFinalizado = true;

        Debug.Log("�NIVEL COMPLETADO!");

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirVictoria();
        }

        Time.timeScale = 0f;
    }
    void PerderNivel()
    {
        juegoFinalizado = true;

        Debug.Log("�NIVEL PERDIDO!");

        if (panelDerrota != null)
        {
            panelDerrota.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirDerrota();
        }

        Time.timeScale = 0f;
    }

    public void AgregarCargaPowerUp()
    {
        if (juegoFinalizado)
            return;

        if (powerUpActivo)
            return;

        if (cargaPowerUp < cargaMaxima)
        {
            cargaPowerUp++;

            Debug.Log(
                "Power Up: " +
                cargaPowerUp +
                "/" +
                cargaMaxima
            );
        }
    }

    public void ActivarPowerUp()
    {
        if (juegoFinalizado)
            return;

        if (cargaPowerUp < cargaMaxima)
        {
            Debug.Log("Power Up todav�a no est� listo");
            return;
        }

        if (powerUpActivo)
            return;

        StartCoroutine(PowerUpTemporal());
    }

    private IEnumerator PowerUpTemporal()
    {
        powerUpActivo = true;
        cargaPowerUp = 0;

        Debug.Log("�POWER UP ACTIVADO!");

        yield return new WaitForSeconds(duracionPowerUp);

        powerUpActivo = false;

        Debug.Log("Power Up terminado");
    }

    public void VolverSeleccionNiveles()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SeleccionNiveles");
    }

    public void ReintentarNivel()
    {
        Time.timeScale = 1f;

        Scene escenaActual = SceneManager.GetActiveScene();

        SceneManager.LoadScene(escenaActual.name);
    }

    public void PausarJuego()
    {
        if (juegoFinalizado || juegoPausado)
            return;

        juegoPausado = true;
        panelPausa.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinuarJuego()
    {
        if (!juegoPausado)
            return;

        juegoPausado = false;
        panelPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    public void VolverMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}
