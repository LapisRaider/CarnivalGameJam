using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBalloon : MonoBehaviour
{
    //TODO:: should have a ref to the balloon
    //change the color

    //default balloon color


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
            //hysics.Raycast(ray, out hit);
            //Debug.Log("This hit at " + hit.point);
        }

        //HOW DO I ART THIS MYSELF
    }

    private void PopBalloon()
    {
        //TODO:: play the popping balloon animation
        //after finish popping balloon, reset balloon

        ResetBalloon();
    }

    private void ResetBalloon()
    {
        //TODO::  replay the balloon BLOWING animation
        //RESET THE BALLOON MATERIAL AND STUFF

        m_CurrentColor = ColorVariants.COLORLESS;

        m_CurrentMix.m_Blue = false;
        m_CurrentMix.m_Red = false;
        m_CurrentMix.m_Yellow = false;
    }

    private void ChangeBalloonColor()
    {
        m_CurrentColor = ColorData.Instance.AddColor(m_CurrentMix);

        //TODO:: change balloon color and do all the special effects
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
