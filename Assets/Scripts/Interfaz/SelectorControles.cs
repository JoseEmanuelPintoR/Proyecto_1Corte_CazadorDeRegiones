using UnityEngine;

public class SelectorControles : MonoBehaviour
{

    public const string ClaveControl = "ControlSeleccionado";

    public const int Botones = 0;
    public const int Joystick = 1;

    [Header("Controles de la escena")]
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject botonIzquierda;
    [SerializeField] private GameObject botonDerecha;

    public static int ControlGuardado
    {
        get { return PlayerPrefs.GetInt(ClaveControl, Botones) == Joystick ? Joystick : Botones; }
    }

    public static void Guardar(int control)
    {
        PlayerPrefs.SetInt(ClaveControl, control == Joystick ? Joystick : Botones);
        PlayerPrefs.Save();
    }

    public static int Alternar()
    {
        int nuevo = ControlGuardado == Joystick ? Botones : Joystick;
        Guardar(nuevo);
        return nuevo;
    }

    void Awake()
    {
        Aplicar();
    }

    public void Aplicar()
    {
        bool conJoystick = ControlGuardado == Joystick;

        if (joystick != null)
        {
            joystick.SetActive(conJoystick);
        }

        if (botonIzquierda != null)
        {
            botonIzquierda.SetActive(!conJoystick);
        }

        if (botonDerecha != null)
        {
            botonDerecha.SetActive(!conJoystick);
        }
    }
}
