using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto = 9.5f;

    [Tooltip("Gravedad extra mientras cae. 1 = gravedad normal; mas alto = cae mas rapido.")]
    [SerializeField] private float multiplicadorCaida = 2.5f;

    [Tooltip("Gravedad extra mientras sube pero ya se solto el boton, para dosificar la altura.")]
    [SerializeField] private float multiplicadorSaltoCorto = 2f;

    [Header("Limites")]
    [Tooltip("Calcula los limites con lo que la camara ve de verdad. Al desmarcarlo se usan " +
             "los valores fijos de abajo.")]
    [SerializeField] private bool limitesDesdeCamara = true;

    [Tooltip("Aire que se deja entre el ala del condor y el borde de la pantalla.")]
    [SerializeField] private float margenBorde = 0.02f;

    [Tooltip("Que parte del dibujo se respeta al calcular el borde. El sprite trae alas y aire " +
             "transparente, asi que con 1 el condor se queda corto y no alcanza lo que cae en la orilla.")]
    [Range(0f, 1f)]
    [SerializeField] private float porcionFigura = 0.55f;

    [SerializeField] private float limiteIzquierdo = -4f;
    [SerializeField] private float limiteDerecho = 4f;

    private Rigidbody rb;
    private float movimientoHorizontal;

    private Camera camara;
    private float mediaFigura;
    private Vector2Int ultimaPantalla;

    private bool estaEnSuelo = true;

    private bool botonIzquierdaPresionado = false;
    private bool botonDerechaPresionado = false;
    private bool botonSaltoPresionado = false;

    private bool saltoMantenido = false;

    public float MovimientoHorizontal => movimientoHorizontal;

    public bool EstaEnSuelo => estaEnSuelo;

    public float LimiteIzquierdo => limiteIzquierdo;

    public float LimiteDerecho => limiteDerecho;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        camara = Camera.main;

        SpriteRenderer dibujo = GetComponentInChildren<SpriteRenderer>();
        mediaFigura = dibujo != null ? dibujo.bounds.extents.x : 0.5f;

        CalcularLimites();
    }

    private void CalcularLimites()
    {
        ultimaPantalla = new Vector2Int(Screen.width, Screen.height);

        if (!limitesDesdeCamara)
        {
            return;
        }

        if (camara == null)
        {
            camara = Camera.main;
        }

        float mitadAncho = LimitesCamara.MitadAncho(camara, transform.position.z);

        if (mitadAncho <= 0f)
        {
            return;
        }

        limiteDerecho = Mathf.Max(0f, mitadAncho - mediaFigura * porcionFigura - margenBorde);
        limiteIzquierdo = -limiteDerecho;
    }

    void Update()
    {
        if (Screen.width != ultimaPantalla.x || Screen.height != ultimaPantalla.y)
        {
            CalcularLimites();
        }

        movimientoHorizontal = 0f;

        bool izquierdaTeclado = false;
        bool derechaTeclado = false;
        bool saltoTeclado = false;

        if (Keyboard.current != null)
        {
            saltoTeclado = Keyboard.current.spaceKey.isPressed;

            izquierdaTeclado =
                Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed;

            derechaTeclado =
                Keyboard.current.dKey.isPressed ||
                Keyboard.current.rightArrowKey.isPressed;

            if (Keyboard.current.spaceKey.wasPressedThisFrame &&
                estaEnSuelo)
            {
                Saltar();
            }
        }

        bool moverIzquierda =
            izquierdaTeclado || botonIzquierdaPresionado;

        bool moverDerecha =
            derechaTeclado || botonDerechaPresionado;

        saltoMantenido = saltoTeclado || botonSaltoPresionado;

        if (moverIzquierda && !moverDerecha)
        {
            movimientoHorizontal = -1f;
        }
        else if (moverDerecha && !moverIzquierda)
        {
            movimientoHorizontal = 1f;
        }
    }

    void FixedUpdate()
    {
        Vector3 velocidadActual = rb.linearVelocity;

        velocidadActual.x = movimientoHorizontal * velocidad;

        velocidadActual.y += GravedadExtra(velocidadActual.y);

        rb.linearVelocity = velocidadActual;

        Vector3 posicionActual = transform.position;

        posicionActual.x = Mathf.Clamp(
            posicionActual.x,
            limiteIzquierdo,
            limiteDerecho
        );

        transform.position = posicionActual;
    }

    private float GravedadExtra(float velocidadVertical)
    {
        float multiplicador;

        if (velocidadVertical < 0f)
        {
            multiplicador = multiplicadorCaida;
        }
        else if (velocidadVertical > 0f && !saltoMantenido)
        {
            multiplicador = multiplicadorSaltoCorto;
        }
        else
        {
            return 0f;
        }

        return Physics.gravity.y * (multiplicador - 1f) * Time.fixedDeltaTime;
    }

    public void MoverIzquierda(bool presionado)
    {
        botonIzquierdaPresionado = presionado;
    }

    public void MoverDerecha(bool presionado)
    {
        botonDerechaPresionado = presionado;
    }

    public void MantenerSalto(bool presionado)
    {
        botonSaltoPresionado = presionado;

        if (presionado)
        {
            Saltar();
        }
    }

    public void Saltar()
    {
        if (!estaEnSuelo)
            return;

        rb.AddForce(
            Vector3.up * fuerzaSalto,
            ForceMode.Impulse
        );

        estaEnSuelo = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            estaEnSuelo = true;
        }
    }
}
