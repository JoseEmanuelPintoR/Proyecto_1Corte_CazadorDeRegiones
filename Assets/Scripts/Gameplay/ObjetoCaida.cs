using UnityEngine;

public class ObjetoCaida : MonoBehaviour
{
    public enum TipoObjeto
    {
        Correcto,
        Incorrecto
    }

    [Header("Tipo de objeto")]
    [SerializeField] private TipoObjeto tipoObjeto;

    private Rigidbody rb;

    [Header("Power Up")]
    [SerializeField] private float velocidadMaximaPowerUp = 2f;
    [SerializeField] private float factorGravedadPowerUp = 0.25f;

    [Header("Límites horizontales")]
    [Tooltip("Radio del objeto, para que no quede medio afuera al llegar al borde.")]
    [SerializeField] private float radioObjeto = 0.5f;

    private Camera camara;
    private Vector2Int ultimaPantalla;
    private float limiteHorizontal;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        camara = Camera.main;

        CalcularLimite();
    }

    private void CalcularLimite()
    {
        ultimaPantalla = new Vector2Int(Screen.width, Screen.height);

        if (camara == null)
        {
            camara = Camera.main;
        }

        float mitadAncho = LimitesCamara.MitadAncho(camara, transform.position.z);

        if (mitadAncho <= 0f)
        {
            return;
        }

        limiteHorizontal = Mathf.Max(0f, mitadAncho - radioObjeto);
    }

    private void SujetarDentroDePantalla()
    {
        if (limiteHorizontal <= 0f)
        {
            return;
        }

        Vector3 posicion = transform.position;

        if (posicion.x >= -limiteHorizontal && posicion.x <= limiteHorizontal)
        {
            return;
        }

        posicion.x = Mathf.Clamp(posicion.x, -limiteHorizontal, limiteHorizontal);
        transform.position = posicion;

        Vector3 velocidadActual = rb.linearVelocity;
        velocidadActual.x = 0f;
        rb.linearVelocity = velocidadActual;
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        if (Screen.width != ultimaPantalla.x || Screen.height != ultimaPantalla.y)
        {
            CalcularLimite();
        }

        SujetarDentroDePantalla();

        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.PowerUpActivo)
        {

            Vector3 fuerzaContraria =
                -Physics.gravity *
                rb.mass *
                (1f - factorGravedadPowerUp);

            rb.AddForce(fuerzaContraria, ForceMode.Force);

            Vector3 velocidadActual = rb.linearVelocity;

            if (velocidadActual.y < -velocidadMaximaPowerUp)
            {
                velocidadActual.y = -velocidadMaximaPowerUp;
                rb.linearVelocity = velocidadActual;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RecogerObjeto();
        }
    }

    private void RecogerObjeto()
    {
        if (GameManager.Instance == null)
            return;

        if (tipoObjeto == TipoObjeto.Correcto)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ReproducirCorrecto();
            }

            GameManager.Instance.SumarPuntos(200);
            GameManager.Instance.AgregarCargaPowerUp();

            Debug.Log("Objeto correcto: +200 puntos");
        }
        else
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ReproducirIncorrecto();
            }

            GameManager.Instance.RestarPuntos(100);
            GameManager.Instance.PerderVida();

            Debug.Log("Objeto incorrecto: -100 puntos y -1 vida");
        }

        Destroy(gameObject);
    }

    public void ObjetoPerdido()
    {
        if (GameManager.Instance == null)
            return;

        if (tipoObjeto == TipoObjeto.Correcto)
        {
            GameManager.Instance.RestarPuntos(100);

            Debug.Log("Objeto correcto perdido: -100 puntos");
        }

        Destroy(gameObject);
    }

}
