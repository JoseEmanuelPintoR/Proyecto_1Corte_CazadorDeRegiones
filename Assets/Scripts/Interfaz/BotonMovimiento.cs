using UnityEngine;
using UnityEngine.EventSystems;

public class BotonMovimiento : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Direccion
    {
        Izquierda,
        Derecha,
        Salto
    }

    [SerializeField] private Direccion direccion;
    [SerializeField] private PlayerController jugador;

    public void OnPointerDown(PointerEventData eventData)
    {
        Avisar(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Avisar(false);
    }

    private void Avisar(bool presionado)
    {
        if (jugador == null)
        {
            return;
        }

        switch (direccion)
        {
            case Direccion.Izquierda:
                jugador.MoverIzquierda(presionado);
                break;

            case Direccion.Derecha:
                jugador.MoverDerecha(presionado);
                break;

            case Direccion.Salto:
                jugador.MantenerSalto(presionado);
                break;
        }
    }
}
