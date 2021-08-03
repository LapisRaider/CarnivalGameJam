using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBalloon : MonoBehaviour
{
    //TODO:: should have a ref to the balloon
    [Header("Balloon Object")]
    public Animator m_BalloonAnimator;
    public SpriteRenderer m_BalloonSprite;
    public Color m_DefaultBalloonColor; //default balloon color

    private ColorVariants m_CurrentColor = ColorVariants.COLORLESS;
    private ColorMixes m_CurrentMix;

    // Start is called before the first frame update
    void Awake()
    {
        ResetBalloon();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO:: check whether i clicked on an NPC
        //if yes, give Balloon and reset balloon
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Casts the ray and get the first game object hit
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "NPC")
                {
                    //check if got the component
                    //get the NPC component

                    //give balloon
                    ResetBalloon();
                }
            }
        }

        //HOW DO I ART THIS MYSELF
    }

    private void ResetBalloon()
    {
        //TODO::  replay the balloon BLOWING animation
        //RESET THE BALLOON MATERIAL AND STUFF
        if (m_BalloonSprite == null)
        {
            Debug.LogWarning("NO BALLOON OBJ REF");
            return;
        }

        m_BalloonSprite.color = m_DefaultBalloonColor;

        m_CurrentColor = ColorVariants.COLORLESS;

        m_CurrentMix.m_Blue = false;
        m_CurrentMix.m_Red = false;
        m_CurrentMix.m_Yellow = false;
    }

    //for the player to reset the balloon by popping it
    public void PopBalloon()
    {
        //TODO:: play the popping balloon animation
        //after finish popping balloon, reset balloon

        ResetBalloon();
    }

    private void ChangeBalloonColor()
    {
        m_CurrentColor = ColorData.Instance.AddColor(m_CurrentMix);

        //TODO:: change balloon color and do all the special effects

        if (m_BalloonSprite == null)
        {
            Debug.LogWarning("NO BALLOON OBJ REF");
            return;
        }
            
        m_BalloonSprite.color = ColorData.Instance.GetColor(m_CurrentColor);
    }

    public void AddBlue()
    {
        //if already true ignore
        if (m_CurrentMix.m_Blue)
            return;

        m_CurrentMix.m_Blue = true;
        ChangeBalloonColor();
    }

    public void AddRed()
    {
        //if already true ignore
        if (m_CurrentMix.m_Red)
            return;

        m_CurrentMix.m_Red = true;
        ChangeBalloonColor();
    }

    public void AddYellow()
    {
        //if already true ignore
        if (m_CurrentMix.m_Yellow)
            return;

        m_CurrentMix.m_Yellow = true;
        ChangeBalloonColor();
    }

}
