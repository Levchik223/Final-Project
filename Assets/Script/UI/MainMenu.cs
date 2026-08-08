using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsController;

    public void OpenSettings()
    {
        settingsController.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsController.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Warrior Hunt");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
