using UnityEngine;

public class ZonaFallo : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ObjetoCaida objeto = other.GetComponent<ObjetoCaida>();

        if (objeto != null)
        {
            objeto.ObjetoPerdido();
        }
    }
}