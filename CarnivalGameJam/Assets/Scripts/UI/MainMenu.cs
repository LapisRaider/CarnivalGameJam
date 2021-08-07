using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject m_QuitButton;
    public Animator m_TransitionAnimation;
    public float m_TransitionTime = 1.0f;

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
        if (m_TransitionAnimation != null)
            m_TransitionAnimation.SetTrigger("FadeOut");

        StartCoroutine(TransitionNextScene(sceneName));
    }

    IEnumerator TransitionNextScene(string sceneName)
    {
        yield return new WaitForSeconds(m_TransitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
