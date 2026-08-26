using UnityEngine;
using System.Collections;

public class RafagaHorizontal : MonoBehaviour
{
    [Header("Ráfaga")]
    [SerializeField] private float velocidadRafaga = 2.5f;

    [Header("Tiempo entre ráfagas")]
    [SerializeField] private float tiempoMinimo = 0.6f;
    [SerializeField] private float tiempoMaximo = 1.2f;

    [Header("Duración de la ráfaga")]
    [SerializeField] private float duracionRafaga = 0.35f;

    private Rigidbody rb;
    private bool rafagaActiva = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        StartCoroutine(CicloRafagas());
    }

    private IEnumerator CicloRafagas()
    {
        while (true)
        {
            float espera = Random.Range(
                tiempoMinimo,
                tiempoMaximo
            );

            yield return new WaitForSeconds(espera);

            if (rb == null)
                yield break;

            yield return StartCoroutine(AplicarRafaga());
        }
    }

    private IEnumerator AplicarRafaga()
    {
        if (rafagaActiva)
            yield break;

        rafagaActiva = true;

        float direccion =
            Random.Range(0, 2) == 0 ? -1f : 1f;

        float tiempo = 0f;

        while (tiempo < duracionRafaga)
        {
            Vector3 velocidadActual = rb.linearVelocity;

            velocidadActual.x =
                direccion * velocidadRafaga;

            rb.linearVelocity = velocidadActual;

            tiempo += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        Vector3 velocidadFinal = rb.linearVelocity;
        velocidadFinal.x = 0f;
        rb.linearVelocity = velocidadFinal;

        rafagaActiva = false;
    }
}
