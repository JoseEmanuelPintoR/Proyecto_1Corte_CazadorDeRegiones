using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PersonalizacionController : MonoBehaviour
{
    [Header("Nombre del jugador")]
    [SerializeField] private TMP_InputField campoNombre;

    private const string CLAVE_NOMBRE = "NombreCazador";

    private const string CLAVE_ACCESORIO = "AccesorioSeleccionado";

    private string accesorioSeleccionado = "Predeterminado";

    [Header("Botones de accesorios")]
    [SerializeField] private Button botonPredeterminado;
    [SerializeField] private Button botonPoncho;
    [SerializeField] private Button botonSombreroVueltiao;
    [SerializeField] private Button botonVestidoDanza;
    [SerializeField] private Button botonSombreroLlanero;
    [SerializeField] private Button botonPlumas;

    [Header("Colores de seleccion")]
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorSeleccionado = Color.yellow;

    [Header("Vista previa del condor")]
    [SerializeField] private Image vistaPrevia;
    [SerializeField] private AccesorioCondor[] accesorios;

    public void VolverMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    void Start()
    {
        // Si ya existe un nombre guardado, lo mostramos
        if (PlayerPrefs.HasKey(CLAVE_NOMBRE))
        {
            campoNombre.text = PlayerPrefs.GetString(CLAVE_NOMBRE);
        }
        if (PlayerPrefs.HasKey(CLAVE_ACCESORIO))
        {
            accesorioSeleccionado =
                PlayerPrefs.GetString(CLAVE_ACCESORIO);
        }
        ActualizarBotones();
    }

    public void GuardarPersonalizacion()
    {
        string nombre = campoNombre.text.Trim();

        if (nombre == "")
        {
            Debug.Log("Debes escribir un nombre.");
            return;
        }

        PlayerPrefs.SetString(CLAVE_NOMBRE, nombre);
        PlayerPrefs.SetString(CLAVE_ACCESORIO, accesorioSeleccionado);

        PlayerPrefs.Save();

        Debug.Log("Nombre guardado: " + nombre);
        Debug.Log("Accesorio guardado: " + accesorioSeleccionado);
    }

    public void SeleccionarPredeterminado()
    {
        accesorioSeleccionado = "Predeterminado";
        ActualizarBotones();
        Debug.Log("Accesorio seleccionado: Predeterminado");
    }

    public void SeleccionarPoncho()
    {
        accesorioSeleccionado = "Poncho";
        ActualizarBotones();
        Debug.Log("Accesorio seleccionado: Poncho");
    }

    public void SeleccionarSombreroVueltiao()
    {
        accesorioSeleccionado = "SombreroVueltiao";
        ActualizarBotones();
        Debug.Log("Accesorio seleccionado: Sombrero Vueltiao");
    }

    public void SeleccionarVestidoDanza()
    {
        accesorioSeleccionado = "VestidoDanza";
        ActualizarBotones();
        Debug.Log("Accesorio seleccionado: Vestido de danza");
    }

    public void SeleccionarSombreroLlanero()
    {
        accesorioSeleccionado = "SombreroLlanero";
        ActualizarBotones();
        Debug.Log("Accesorio seleccionado: Sombrero Llanero");
    }

    public void SeleccionarPlumas()
    {
        accesorioSeleccionado = "Plumas";
        ActualizarBotones();
        Debug.Log("Accesorio seleccionado: Adorno de plumas");
    }

    private void ActualizarBotones()
    {
        botonPredeterminado.image.color =
            accesorioSeleccionado == "Predeterminado"
            ? colorSeleccionado : colorNormal;

        botonPoncho.image.color =
            accesorioSeleccionado == "Poncho"
            ? colorSeleccionado : colorNormal;

        botonSombreroVueltiao.image.color =
            accesorioSeleccionado == "SombreroVueltiao"
            ? colorSeleccionado : colorNormal;

        botonVestidoDanza.image.color =
            accesorioSeleccionado == "VestidoDanza"
            ? colorSeleccionado : colorNormal;

        botonSombreroLlanero.image.color =
            accesorioSeleccionado == "SombreroLlanero"
            ? colorSeleccionado : colorNormal;

        botonPlumas.image.color =
            accesorioSeleccionado == "Plumas"
            ? colorSeleccionado : colorNormal;

        ActualizarVistaPrevia();
    }

    private void ActualizarVistaPrevia()
    {
        if (vistaPrevia == null || accesorios == null)
        {
            return;
        }

        foreach (AccesorioCondor accesorio in accesorios)
        {
            if (accesorio != null && accesorio.clave == accesorioSeleccionado)
            {
                vistaPrevia.sprite = accesorio.sprite;
                vistaPrevia.enabled = accesorio.sprite != null;
                return;
            }
        }
    }


}