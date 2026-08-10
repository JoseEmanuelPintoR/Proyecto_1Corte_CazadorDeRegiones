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
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb == null || GameManager.Instance == null)
            return;

        if (GameManager.Instance.PowerUpActivo)
        {
            // Reduce la gravedad efectiva al 25 %
            Vector3 fuerzaContraria =
                -Physics.gravity *
                rb.mass *
                (1f - factorGravedadPowerUp);

            rb.AddForce(fuerzaContraria, ForceMode.Force);

            // Evita que un objeto que ya venía rápido siga cayendo demasiado rápido
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
            GameManager.Instance.SumarPuntos(200);
            GameManager.Instance.AgregarCargaPowerUp();

            Debug.Log("Objeto correcto: +200 puntos");
        }
        else
        {
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