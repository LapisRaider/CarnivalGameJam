using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    public bool test;

    [Header("UI info")]
    public GameObject m_SpeechBubble;
    public TextMeshProUGUI m_ColorText;

    [Header("Npc Behavior")]
    private float m_PatienceTime = 0.0f; //in seconds

    private ColorVariants m_ColorWanted = ColorVariants.COLORLESS;

    // Start is called before the first frame update
    void Start()
    {
        //set the UI inactive
        if (m_SpeechBubble != null)
            m_SpeechBubble.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //reduce the patienceTime, if patience tim e = 0, quit
        //m_PatienceTime -= Time.deltaTime;
        if (test)
        {
            AskForBalloon();
            test = false;
        }
            
    }

    void InitNPC(float patienceTime, Vector3 queueDestination)
    {
        m_PatienceTime = patienceTime;
        //should walk to destination
        //after walking to destination call the askForBalloon

        //should init the patience timing here
        //if patience
    }

    public void AskForBalloon()
    {
        //set the UI to true
        if (m_SpeechBubble != null)
            m_SpeechBubble.SetActive(false);

        //randomize the color
        m_ColorWanted = (ColorVariants)Random.Range((int)(ColorVariants.RED), (int)(ColorVariants.COLORLESS));

        Debug.Log("Color wanted " + m_ColorWanted);
        //must first check which mode to go for
        //need see whether to go for the different color
        // or the normal version for text, if this version show the text, text make black
        //or the normal just color version, if this version, show the square, change color

        if (m_ColorText == null)
            return;

        m_ColorText.color = ColorData.Instance.GetColor(m_ColorWanted);
        m_ColorText.text = ColorData.Instance.GetColorName(m_ColorWanted);
    }

    public void TakeBalloon(ColorVariants colorGiven)
    {
        //give balloon to child
        //walk away

        if (colorGiven == m_ColorWanted)
        {
            //the NPC walks away happy
            //play some happy effects
            //add to happiness levels i guess
            Debug.Log("Correct");
        }
        else
        {
            Debug.Log("WRONG");
            //NPC not happy, cry BITCH
            //play some sad effects
            //decrease happiness level or counter or whatever
        }
    }
}
