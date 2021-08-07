using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystems : MonoBehaviour
{
    [Header("Transitions")]
    public Animator m_TransitionAnimation;
    public float m_TransitionTime = 1.0f;

    [Header("UI")]
    public GameObject[] m_ObjectsToBeInactive;
    public GameObject m_PauseMenu;

    bool m_Pause = false;

    public void Start()
    {
        m_Pause = false;
        m_PauseMenu.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameHandler.Instance.m_Lose)
                return;

            m_Pause = !m_Pause; //toggle pause
            m_PauseMenu.SetActive(m_Pause);

            for (int i =0; i < m_ObjectsToBeInactive.Length; ++i)
            {
                m_ObjectsToBeInactive[i].SetActive(!m_Pause);
            }

            if (m_Pause)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
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
