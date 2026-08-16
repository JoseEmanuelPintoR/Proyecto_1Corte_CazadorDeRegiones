using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalController : MonoBehaviour
{
    public void IrASeleccionNiveles()
    {
        SceneManager.LoadScene("SeleccionNiveles");
    }

    public void IrAPersonalizacion()
    {
        SceneManager.LoadScene("Personalizacion");
    }

    public void IrACreditos()
    {
        SceneManager.LoadScene("Creditos");
    }
}