using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    [Header("UI info")]
    public GameObject m_SpeechBubble;
    public TextMeshProUGUI m_ColorText;

    private ColorVariants m_ColorWanted = ColorVariants.COLORLESS;

    // Start is called before the first frame update
    void Start()
    {
        //set the UI inactive
        if (m_SpeechBubble != null)
            m_SpeechBubble.SetActive(false);
    }

    public void AskForBalloon()
    {
        //set the UI to true
        if (m_SpeechBubble != null)
            m_SpeechBubble.SetActive(false);

        //randomize the color
        m_ColorWanted = (ColorVariants)Random.Range((int)(ColorVariants.RED), (int)(ColorVariants.COLORLESS));

        //must first check which mode to go for
        //need see whether to go for the different color
        // or the normal version for text, if this version show the text, text make black
        //or the normal just color version, if this version, show the square, change color
       
        m_ColorText.color = ColorData.Instance.GetColor(m_ColorWanted);
        m_ColorText.text = ColorData.Instance.GetColorName(m_ColorWanted);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        }
        else
        {
            //NPC not happy, cry BITCH
            //play some sad effects
            //decrease happiness level or counter or whatever
        }
    }
}
