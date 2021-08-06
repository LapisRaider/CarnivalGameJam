using UnityEngine;
using TMPro;

[System.Serializable]
public class LoseScene
{
    [Header("Set Up Scene")]
    public GameObject m_LoseSceneEnvironment;
    public GameObject[] m_SetInactiveWhenLose;
    public Transform m_LoseScreenCameraTransform;

    [Header("Lose UI")]
    public GameObject m_LoseUI;
    public TextMeshProUGUI m_HighScoreText;

    // Start is called before the first frame update
    void Init()
    {
        if (m_LoseSceneEnvironment != null)
            m_LoseSceneEnvironment.SetActive(false);

        if (m_LoseUI != null)
            m_LoseUI.SetActive(false);
    }

    public void SetUpLose(int currentScore)
    {
        //set up scene
        for (int i =0; i < m_SetInactiveWhenLose.Length; ++i)
        {
            m_SetInactiveWhenLose[i].SetActive(false);
        }

        if (m_LoseSceneEnvironment != null)
            m_LoseSceneEnvironment.SetActive(true);

        if (m_LoseScreenCameraTransform != null)
        {
            Camera.main.transform.position = m_LoseScreenCameraTransform.position;
            Camera.main.transform.rotation = m_LoseScreenCameraTransform.rotation;
        }

        //set up UI
        if (m_LoseUI != null)
            m_LoseUI.SetActive(true);

        if (m_HighScoreText != null)
            m_HighScoreText.SetText(currentScore.ToString());
    }
}
