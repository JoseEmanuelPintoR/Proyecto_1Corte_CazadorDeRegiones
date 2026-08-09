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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        movimientoHorizontal = 0f;

        if (Keyboard.current == null)
            return;

        // Movimiento izquierda
        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            movimientoHorizontal = -1f;
        }

        // Movimiento derecha
        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            movimientoHorizontal = 1f;
        }

        // Salto
        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            estaEnSuelo)
        {
            Saltar();
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

    void Saltar()
    {
        rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);

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