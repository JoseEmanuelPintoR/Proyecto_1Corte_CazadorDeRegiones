using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TMP_Text textoPuntos;
    [SerializeField] private TMP_Text textoVidas;
    [SerializeField] private TMP_Text textoTiempo;
    [SerializeField] private TMP_Text textoPowerUp;

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        textoPuntos.text =
            "Puntos: " + GameManager.Instance.Puntos;

        textoVidas.text =
            "Vidas: " + GameManager.Instance.Vidas;

        textoTiempo.text =
            "Tiempo: " +
            Mathf.CeilToInt(GameManager.Instance.TiempoRestante);

        if (GameManager.Instance.CargaPowerUp >=
    GameManager.Instance.CargaMaxima)
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