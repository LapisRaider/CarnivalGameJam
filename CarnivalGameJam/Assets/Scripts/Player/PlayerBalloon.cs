using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBalloon : MonoBehaviour
{
    //have a ref to the balloon
    [Header("Balloon Object")]
    public Animator m_BalloonAnimator;
    public Material m_BalloonMaterial;
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
        //check whether i clicked on an NPC
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Casts the ray and get the first game object hit
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("NPCs"))
                {
                    //check if got the NPC component
                    NPC npc = hit.transform.GetComponent<NPC>();
                    if (npc == null)
                        return;

                    npc.TakeBalloon(m_CurrentColor); //give balloon to child
                    ResetBalloon();
                }
            }
        }

        //HOW DO I ART THIS MYSELF
    }

    //reset the balloon and material
    private void ResetBalloon()
    {
        //TODO::  replay the balloon BLOWING animation
        
        if (m_BalloonMaterial == null)
        {
            Debug.LogWarning("NO BALLOON OBJ REF");
            return;
        }

        m_BalloonMaterial.color = m_DefaultBalloonColor;

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

        if (m_BalloonMaterial == null)
        {
            Debug.LogWarning("NO BALLOON OBJ REF");
            return;
        }

        m_BalloonMaterial.color = ColorData.Instance.GetColor(m_CurrentColor);
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
