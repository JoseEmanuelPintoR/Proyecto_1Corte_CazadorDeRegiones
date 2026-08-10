using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto = 7f;

    [Header("Limites")]
    [SerializeField] private float limiteIzquierdo = -4f;
    [SerializeField] private float limiteDerecho = 4f;

    private Rigidbody rb;
    private float movimientoHorizontal;

    private bool estaEnSuelo = true;

    // Controles táctiles
    private bool botonIzquierdaPresionado = false;
    private bool botonDerechaPresionado = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        movimientoHorizontal = 0f;

        bool izquierdaTeclado = false;
        bool derechaTeclado = false;

        // Controles de teclado para pruebas en PC
        if (Keyboard.current != null)
        {
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

        // Teclado + botones táctiles
        bool moverIzquierda =
            izquierdaTeclado || botonIzquierdaPresionado;

        bool moverDerecha =
            derechaTeclado || botonDerechaPresionado;

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

        rb.linearVelocity = velocidadActual;

        Vector3 posicionActual = transform.position;

        posicionActual.x = Mathf.Clamp(
            posicionActual.x,
            limiteIzquierdo,
            limiteDerecho
        );

        transform.position = posicionActual;
    }

    public void MoverIzquierda(bool presionado)
    {
        botonIzquierdaPresionado = presionado;
    }

    public void MoverDerecha(bool presionado)
    {
        botonDerechaPresionado = presionado;
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