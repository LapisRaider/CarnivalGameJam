using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Cutscene : MonoBehaviour
{
    [Header("Transitions")]
    public Animator m_TransitionAnimation;
    public float m_TransitionTime = 1.0f;
    public GameObject m_CutSceneObj;
    public GameObject m_InstructionObj;

    [Header("Cutscene UI")]
    public TextMeshProUGUI m_Dialogue;
    public Material m_BossMaterial;
    public Material m_PlayerMaterial;
    public Color m_ChangeColor;

    [Header("Story Data")]
    public CutSceneDialogue[] m_CutsceneDialogue;

    public float m_CharacterTextTime = 0.2f;
    private float m_CurrentTimeTracker = 0.0f;

    private string m_CurrTextPrinted = "";
    private int m_CurrentDialogueIndex = 0;
    private int m_CurrentTextLength = 0;
    
    public void Start()
    {
        if (m_Dialogue != null)
            m_Dialogue.text = "";

        m_CurrentDialogueIndex = 0;
        m_CurrentTextLength = 0;
        m_CurrTextPrinted = "";
        m_CurrentTimeTracker = 0.0f;

        SoundManager.Instance.Play("BackgroundNoise");

        UpdateMaterials();
    }

    public void UpdateMaterials()
    {
        //set up the materials here for the boss and npc, whether to grey them out or not
        bool bossTalking = m_CutsceneDialogue[m_CurrentDialogueIndex].m_Boss;
        if (bossTalking) //if its the boss
        {
            if (m_BossMaterial != null)
                m_BossMaterial.color = Color.white;

            if (m_PlayerMaterial != null)
                m_PlayerMaterial.color = m_ChangeColor;
        }
        else
        {
            if (m_BossMaterial != null)
                m_BossMaterial.color = m_ChangeColor;

            if (m_PlayerMaterial != null)
                m_PlayerMaterial.color = Color.white;
        }
    }

    public void Update()
    {
        m_CurrentTimeTracker += Time.deltaTime;

        if (m_CurrentTimeTracker < m_CharacterTextTime)
            return;

        m_CurrentTimeTracker = 0.0f;

        string currText = m_CutsceneDialogue[m_CurrentDialogueIndex].m_Speech;
        if (m_CurrentTextLength < currText.Length)
        {
            m_CurrTextPrinted += currText[m_CurrentTextLength];
            ++m_CurrentTextLength;

            if (m_Dialogue != null)
                m_Dialogue.text = m_CurrTextPrinted;

            SoundManager.Instance.Play("Text");
        }
    }

    public void NextPart()
    {
        m_CurrTextPrinted = "";
        m_CurrentTimeTracker = 0.0f;

        //check if current dialogue printed finish
        string currText = m_CutsceneDialogue[m_CurrentDialogueIndex].m_Speech;
        if (m_CurrentTextLength < currText.Length)
        {
            if (m_Dialogue != null)
                m_Dialogue.text = currText;

            m_CurrentTextLength = currText.Length;
        }
        else
        {
            //check if finish dialogue
            if (m_CurrentDialogueIndex >= m_CutsceneDialogue.Length - 1)
            {
                if (m_CutSceneObj != null)
                    m_CutSceneObj.SetActive(false);

                if (m_InstructionObj != null)
                    m_InstructionObj.SetActive(true);
            }
            else
            {
                m_CurrentTextLength = 0;
                ++m_CurrentDialogueIndex;
                UpdateMaterials(); //change material of boss and player here
            }         
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

[System.Serializable]
public class CutSceneDialogue
{
    [TextArea(3, 5)]
    public string m_Speech = "";
    public bool m_Boss = false; // is it the boss saying or not
}