using UnityEngine;
using UnityEngine.UI;

public class MedidorPowerUp : MonoBehaviour
{
    [SerializeField] private Image relleno;
    [SerializeField] private Image halo;

    [Header("Llenado")]
    [SerializeField] private float velocidadLlenado = 3f;

    [Header("Luz cuando esta listo")]
    [SerializeField] private float brilloMinimo = 0.3f;
    [SerializeField] private float brilloMaximo = 0.95f;
    [SerializeField] private float velocidadBrillo = 1.6f;
    [SerializeField] private float crecimiento = 0.18f;

    private Vector3 escalaHalo = Vector3.one;

    void Awake()
    {
        if (halo != null)
        {
            escalaHalo = halo.rectTransform.localScale;
        }

        if (relleno != null)
        {
            relleno.fillAmount = 0f;
        }

        Apagar();
    }

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        int maxima = Mathf.Max(1, GameManager.Instance.CargaMaxima);
        float objetivo = Mathf.Clamp01((float)GameManager.Instance.CargaPowerUp / maxima);

        if (relleno != null)
        {
            relleno.fillAmount = Mathf.MoveTowards(
                relleno.fillAmount, objetivo, velocidadLlenado * Time.unscaledDeltaTime);
        }

        bool listo = GameManager.Instance.CargaPowerUp >= GameManager.Instance.CargaMaxima;

        if (listo || GameManager.Instance.PowerUpActivo)
        {
            Brillar();
        }
        else
        {
            Apagar();
        }
    }

    private void Brillar()
    {
        if (halo == null)
            return;

        float pulso = Mathf.PingPong(Time.unscaledTime * velocidadBrillo, 1f);

        halo.enabled = true;
        halo.color = ConAlfa(halo.color, Mathf.Lerp(brilloMinimo, brilloMaximo, pulso));
        halo.rectTransform.localScale = escalaHalo * (1f + crecimiento * pulso);
    }

    private void Apagar()
    {
        if (halo == null)
            return;

        halo.color = ConAlfa(halo.color, 0f);
        halo.rectTransform.localScale = escalaHalo;
    }

    private static Color ConAlfa(Color color, float alfa)
    {
        return new Color(color.r, color.g, color.b, alfa);
    }
}
