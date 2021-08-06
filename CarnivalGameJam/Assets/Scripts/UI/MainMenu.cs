using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject m_QuitButton;

    public void Start()
    {
        if (m_QuitButton != null)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                m_QuitButton.SetActive(false);
        }
    }

    public void QuitGame()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            Application.Quit();
    }

    public void TransitionScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
