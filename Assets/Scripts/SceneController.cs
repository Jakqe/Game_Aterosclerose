using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadDoctorsOffice1()
    {
        SceneManager.LoadScene("DoctorsOffice1");
    }

    public void LoadArtery()
    {
        SceneManager.LoadScene("Artery");
    }

    public void LoadDoctorsOffice2()
    {
        SceneManager.LoadScene("DoctorsOffice2");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
