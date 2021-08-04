﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class NPC : MonoBehaviour
{
    public bool test;

    [Header("UI info")]
    public GameObject m_SpeechBubble;
    public TextMeshProUGUI m_ColorText;
    public Image m_ColorDefaultImage;
    public Color m_StroopTextDefaultColor; //TODO:: TEMP VAR PUT IT IN STROOP COLOR TEST

    [Header("Npc Effects")]
    public Animator m_Animator;

    [Header("Npc Behavior")]
    private bool m_IsWaiting = false;
    private float m_PatienceTime = 0.0f; //in seconds
    private ColorVariants m_ColorWanted = ColorVariants.COLORLESS;

    public delegate void OnLeavingQueue(NPC npc);
    public OnLeavingQueue OnLeftQueueCallback;

    [Header("Npc Movement")]
    public float m_StopThreshold = 1.0f; //threshold to stop
    public float m_RotationStopThreshold = 0.98f;

    // set by npcManager
    private float m_WalkSpeed = 1.0f;
    private float m_RotationSpeed = 1.0f;

    //for positions
    private Transform m_OriginalTransform;
    private Vector3 m_NextPos;
    private Vector3 m_LeaveDestination;

    // Start is called before the first frame update
    void Start()
    {
        ResetNPCUI();
        m_OriginalTransform = transform;
        m_IsWaiting = false;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, m_StopThreshold);
    }

    public void ResetNPCUI()
    {
        //set the UI inactive
        if (m_ColorText != null)
            m_ColorText.enabled = false;

        if (m_ColorDefaultImage != null)
            m_ColorDefaultImage.enabled = false;

        if (m_SpeechBubble != null)
            m_SpeechBubble.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (test)
        {
            //AskForBalloon();
            StartCoroutine(StartQueue());

            test = false;
        }


        if (!m_IsWaiting)
            return;

        m_PatienceTime -= Time.deltaTime; //reduce the patienceTime
        //TODO:: UPDATE SOME UI TO SHOW THE TIME LEFT
        if (m_PatienceTime <= 0.0f)
        {
            Sad(); //Leave and sad
        }
    }


    public void InitNPCToQueue(float patienceTime, float walkSpeed, float rotationSpeed, Vector3 queuePos, Vector3 leavePos)
    {
        m_PatienceTime = patienceTime;
        m_LeaveDestination = leavePos;
        m_LeaveDestination.y = 0.0f;
        m_NextPos = queuePos;
        m_NextPos.y = 0.0f;

        m_WalkSpeed = walkSpeed;
        m_RotationSpeed = rotationSpeed;

        m_OriginalTransform = transform;
        m_IsWaiting = false;

        StartCoroutine(StartQueue());
    }

    //returns true if destination reached
    public bool WalkToDestination()
    {
        Vector3 currPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 dir = m_NextPos - currPos;
        dir.Normalize();

        transform.position += dir * Time.deltaTime * m_WalkSpeed;

        return Vector2.Distance(m_NextPos, currPos) <= m_StopThreshold;
    }

    //returns true if facing the direction
    public bool RotateTowardsLocation()
    {
        Vector3 currPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 dir = m_NextPos - currPos;
        dir.Normalize();

        if (dir == Vector3.zero)
            return true;

        Quaternion nextRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Lerp(m_OriginalTransform.rotation, nextRotation, Time.time * m_RotationSpeed);

        return Mathf.Abs(Quaternion.Dot(transform.rotation, nextRotation)) >= m_RotationStopThreshold;
    }

    IEnumerator StartQueue()
    {
        //while havent walk finish to destination
        while(!WalkToDestination())
        {
            RotateTowardsLocation(); //rotate towards the direction it is walking to

            yield return null;
        }

        //reach queue, face the player
        while (!RotateTowardsLocation())
        {
            yield return null;
        }

        AskForBalloon();

        yield return null;
    }

    public void AskForBalloon()
    {
        m_IsWaiting = true; //start waiting

        //set the UI to true
        if (m_SpeechBubble != null)
            m_SpeechBubble.SetActive(false);

        //randomize the color
        m_ColorWanted = (ColorVariants)Random.Range((int)(ColorVariants.RED), (int)(ColorVariants.COLORLESS));

        Debug.Log("Color wanted " + m_ColorWanted);

        //TODO:: should decide based on the difficulty
        StroopTestTypes stroopMode = StroopTestTypes.DIFF_TEXT_COLOR;

        if (m_SpeechBubble != null)
            m_SpeechBubble.SetActive(true);

        switch (stroopMode)
        {
            case StroopTestTypes.DEFAULT_TEXT: //normal version for text
                {
                    if (m_ColorText == null)
                        return;

                    m_ColorText.color = m_StroopTextDefaultColor;
                    m_ColorText.text = ColorData.Instance.GetColorName(m_ColorWanted);
                }
                break;
            case StroopTestTypes.DEFAULT_COLOR: //just show a color instead
                {
                    if (m_ColorDefaultImage == null)
                        return;

                    m_ColorDefaultImage.enabled = true;
                    m_ColorDefaultImage.color = ColorData.Instance.GetColor(m_ColorWanted);
                }
                break;
            case StroopTestTypes.DIFF_TEXT_COLOR: //show correct color diff text
                {
                    if (m_ColorText == null)
                        return;

                    m_ColorText.enabled = true;
                    m_ColorText.color = ColorData.Instance.GetColor(m_ColorWanted);

                    //randomize the text
                    ColorVariants randomColor = (ColorVariants)Random.Range((int)(ColorVariants.RED), (int)(ColorVariants.COLORLESS));
                    m_ColorText.text = ColorData.Instance.GetColorName(randomColor);
                }
                break;
            default:
                break;
        }
    }

    public void TakeBalloon(ColorVariants colorGiven)
    {
        if (!m_IsWaiting)
            return;

        //TODO:: update UI accordingly

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
        //put a sad emjoi

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
        m_IsWaiting = false;

        if (OnLeftQueueCallback != null)
            OnLeftQueueCallback.Invoke(this);

        StartCoroutine(StartLeaving());
    }

    IEnumerator StartLeaving()
    {
        //set the animation


        m_NextPos = m_LeaveDestination;
        while (!WalkToDestination())
        {
            RotateTowardsLocation(); //rotate towards the direction it is walking to

            yield return null;
        }

        gameObject.SetActive(false); //out of screen

        yield return null;
    }
}
