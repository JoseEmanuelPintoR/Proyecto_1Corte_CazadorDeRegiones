using UnityEngine;

public class MovimientoDiagonal : MonoBehaviour
{
    [Header("Movimiento lateral")]
    [SerializeField] private float fuerzaHorizontal = 1.5f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (rb == null)
            return;

        float direccion = Random.Range(0, 2) == 0 ? -1f : 1f;

        rb.AddForce(
            Vector3.right * direccion * fuerzaHorizontal,
            ForceMode.Impulse
        );
    }
}
