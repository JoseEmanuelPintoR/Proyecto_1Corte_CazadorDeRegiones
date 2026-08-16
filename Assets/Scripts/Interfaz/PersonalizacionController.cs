using UnityEngine;
using TMPro;

public class PersonalizacionController : MonoBehaviour
{
    [Header("Nombre del jugador")]
    [SerializeField] private TMP_InputField campoNombre;

    private const string CLAVE_NOMBRE = "NombreCazador";

    void Start()
    {
        // Si ya existe un nombre guardado, lo mostramos
        if (PlayerPrefs.HasKey(CLAVE_NOMBRE))
        {
            campoNombre.text = PlayerPrefs.GetString(CLAVE_NOMBRE);
        }
    }

    public void GuardarNombre()
    {
        string nombre = campoNombre.text.Trim();

        if (nombre == "")
        {
            Debug.Log("Debes escribir un nombre.");
            return;
        }

        PlayerPrefs.SetString(CLAVE_NOMBRE, nombre);
        PlayerPrefs.Save();

        Debug.Log("Nombre guardado: " + nombre);
    }
}