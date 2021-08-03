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
    public Animator m_Animator;
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
        //TODO:: should decide based on the difficulty
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
        if (colorGiven == m_ColorWanted)
        {
            Debug.Log(m_ColorWanted);
            Happy();
        }
        else
        {
            Debug.Log("WRONG");
            Sad();
        }

        //TODO:: show the balloon rising up
    }

    //walk sadly away
    public void Sad()
    {
        //NPC not happy, cry BITCH
        //play some sad effects
        //decrease happiness level or counter or whatever
        //make sure to rotate towards the direction

        Leave();
    }

    //walk happ
    public void Happy()
    {
        //the NPC walks away happy
        //play some happy effects
        //add to happiness levels i guess

        Leave();
    }

    //leave the qeueing position
    public void Leave()
    {

    }
}
