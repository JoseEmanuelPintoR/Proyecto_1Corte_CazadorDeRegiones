using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Marcador")]
    [SerializeField] private TMP_Text textoPuntos;
    [SerializeField] private TMP_Text textoTiempo;
    [SerializeField] private TMP_Text textoPowerUp;

    [Header("Nombre del cazador")]
    [SerializeField] private TMP_Text textoNombre;

    [Header("Vidas")]
    [SerializeField] private Image[] corazones;
    [SerializeField] private Sprite corazonLleno;
    [SerializeField] private Sprite corazonVacio;

    private const string ClaveNombre = "NombreCazador";

    void Start()
    {
        if (textoNombre != null)
        {
            textoNombre.text = PlayerPrefs.GetString(ClaveNombre, "Cazador");
        }
    }

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (textoPuntos != null)
        {
            textoPuntos.text = "Puntos: " + GameManager.Instance.Puntos;
        }

        if (textoTiempo != null)
        {
            textoTiempo.text = TiempoEnReloj(GameManager.Instance.TiempoRestante);
        }

        ActualizarCorazones();
        ActualizarPowerUp();
    }

    private static string TiempoEnReloj(float segundosRestantes)
    {
        int segundos = Mathf.Max(0, Mathf.CeilToInt(segundosRestantes));

        return (segundos / 60).ToString("00") + ":" + (segundos % 60).ToString("00");
    }

    private void ActualizarCorazones()
    {
        if (corazones == null)
            return;

        int vidas = GameManager.Instance.Vidas;

        for (int i = 0; i < corazones.Length; i++)
        {
            if (corazones[i] == null)
                continue;

            corazones[i].sprite = i < vidas ? corazonLleno : corazonVacio;
        }
    }

    private void ActualizarPowerUp()
    {
        if (textoPowerUp == null)
            return;

        if (GameManager.Instance.CargaPowerUp >= GameManager.Instance.CargaMaxima)
        {
            textoPowerUp.text = "LISTO";
        }
        else
        {
            textoPowerUp.text =
                GameManager.Instance.CargaPowerUp +
                "/" +
                GameManager.Instance.CargaMaxima;
        }
    }
}
