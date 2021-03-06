using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class NPC : MonoBehaviour
{
    [Header("NPC Texture")]
    private Material m_Material;
    public Transform m_AllPartsParent;

    [Header("UI info")]
    public GameObject m_SpeechBubble;
    public TextMeshProUGUI m_ColorText;
    public Image m_ColorDefaultImage;

    [Header("Npc Effects")]
    public Animator m_Animator;
    public Transform m_PropParentTransform;
    public AudioSource m_AudioSource;

    public float m_MaxShakeAmt = 1.0f;
    public float m_MaxShakeFrequency = 0.8f;
    public float m_DefaultShakeFrequency = 0.0f;
    public float m_StartShakingPercentage = 0.7f;
    private float m_CurrentShakeProbability = 0.0f;

    public GameObject m_BalloonObj;
    public MeshRenderer m_BalloonRenderer;
    private Material m_BalloonMaterial;

    [Header("Npc Behavior")]
    private bool m_IsWaiting = false;
    private float m_PatienceTime = 0.0f; //in seconds
    private float m_CurrPatienceTime = 0.0f;
    private ColorVariants m_ColorWanted = ColorVariants.COLORLESS;

    public delegate void OnLeavingQueue(NPC npc);
    public OnLeavingQueue OnLeftQueueCallback;

    [Header("Npc Movement")]
    public float m_StopThreshold = 1.0f; //threshold to stop
    public float m_RotationStopThreshold = 0.98f;
    private BoxCollider m_BoxCollider;

    // set by npcManager
    private float m_WalkSpeed = 1.0f;
    private float m_RotationSpeed = 1.0f;
    private float m_StartTiming = 0.0f;

    //for positions
    private Transform m_OriginalTransform;
    private Transform m_QueueTransform;
    private Vector3 m_NextPos;
    private Vector3 m_LeaveDestination;

    // Start is called before the first frame update
    void Start()
    {
        ResetNPCUI();
        m_OriginalTransform = transform;
        m_IsWaiting = false;
        m_StartTiming = 0.0f;

        m_BoxCollider = GetComponent<BoxCollider>();
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }

    public void OnMouseOver()
    {
        if (m_Animator)
            m_Animator.SetBool("Hovering", true);
    }

    public void OnMouseExit()
    {
        if (m_Animator)
            m_Animator.SetBool("Hovering", false);
    }

    public void CreateMaterial(Material material, Material balloonMaterial)
    {
        m_Material = new Material(material);

        foreach (Transform child in m_AllPartsParent)
        {
            SkinnedMeshRenderer meshRenderer = child.GetComponent<SkinnedMeshRenderer>();
            if (meshRenderer == null)
                continue;

            meshRenderer.material = m_Material;
        }

        m_BalloonMaterial = new Material(balloonMaterial);
        if (m_BalloonRenderer != null)
        {
            m_BalloonRenderer.material = m_BalloonMaterial;
        }
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
        if (!m_IsWaiting)
            return;

        m_CurrPatienceTime -= Time.deltaTime; //reduce the patienceTime

        //to show they are losing patience
        float patiencePercentage = 1.0f - (m_CurrPatienceTime / m_PatienceTime);
        if (patiencePercentage > m_StartShakingPercentage)
        {
            m_CurrentShakeProbability = (m_MaxShakeFrequency - m_DefaultShakeFrequency) * patiencePercentage;

            float shakeChance = Random.Range(m_DefaultShakeFrequency, m_MaxShakeFrequency);

            if (shakeChance < m_CurrentShakeProbability)
            {
                if (m_QueueTransform != null)
                {
                    Vector3 shakePos = new Vector3(m_QueueTransform.position.x + Random.Range(-m_MaxShakeAmt, m_MaxShakeAmt), m_QueueTransform.position.y, m_QueueTransform.position.z + Random.Range(-m_MaxShakeAmt, m_MaxShakeAmt));
                    transform.position = shakePos;
                }
            }
        }

        if (m_CurrPatienceTime <= 0.0f)
        {
            NPCPlaySound("NPCLeave");
            Sad(); //Leave and sad
        }
    }

    public void StartNPCAppear(GameObject prop = null, Texture texture = null)
    {
        if (m_Animator != null)
        {
            m_Animator.SetBool("Sad", false);
            m_Animator.SetBool("Walking", false);
        }

        if (texture != null)
        {
            m_Material.mainTexture = texture;
        }

        if (prop != null)
        {
            //attach prop to hand
            if (m_PropParentTransform != null)
            {
                if (m_PropParentTransform.childCount == 0)
                {
                    prop.transform.parent = m_PropParentTransform;
                    prop.transform.localPosition = Vector3.zero;
                    prop.transform.localRotation = Quaternion.identity;
                }
            }
        }

        if (m_BalloonObj != null)
            m_BalloonObj.SetActive(false);

        if (m_BoxCollider != null)
            m_BoxCollider.enabled = false;
    }

    public void InitNPCToQueue(float patienceTime, float walkSpeed, float rotationSpeed, Transform queueTransform, Vector3 leavePos)
    {
        m_PatienceTime = patienceTime;
        m_CurrPatienceTime = patienceTime;
        m_LeaveDestination = leavePos;
        m_LeaveDestination.y = 0.0f;

        m_QueueTransform = queueTransform;

        m_NextPos = queueTransform.position;
        m_NextPos.y = 0.0f;

        m_WalkSpeed = walkSpeed;
        m_RotationSpeed = rotationSpeed;

        m_OriginalTransform = transform;
        m_IsWaiting = false;

        m_StartTiming = 0.0f;

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
        transform.rotation = Quaternion.Lerp(m_OriginalTransform.rotation, nextRotation, (Time.time - m_StartTiming) * m_RotationSpeed);

        return Mathf.Abs(Quaternion.Dot(transform.rotation, nextRotation)) >= m_RotationStopThreshold;
    }

    public bool RotateTowardsRotation()
    {
        transform.rotation = Quaternion.Lerp(m_OriginalTransform.rotation, m_QueueTransform.rotation, (Time.time - m_StartTiming) * m_RotationSpeed);

        return Mathf.Abs(Quaternion.Dot(transform.rotation, m_QueueTransform.rotation)) >= m_RotationStopThreshold;
    }

    IEnumerator StartQueue()
    {
        //while havent walk finish to destination
        m_Animator.SetBool("Walking", true);

        m_StartTiming = Time.time;
        while (!WalkToDestination())
        {
            RotateTowardsLocation(); //rotate towards the direction it is walking to

            yield return null;
        }

        //reach queue, face the player
        m_OriginalTransform = gameObject.transform;
        m_StartTiming = Time.time;
        while (!RotateTowardsRotation())
        {
            yield return null;
        }

        if (m_BoxCollider != null)
            m_BoxCollider.enabled = true;

        m_Animator.SetBool("Walking", false);
        AskForBalloon();

        yield return null;
    }

    public void AskForBalloon()
    {
        m_IsWaiting = true; //start waiting

        //set the UI to true
        if (m_SpeechBubble != null)
        {
            m_SpeechBubble.SetActive(true);

            Transform speechBubbleTransform = m_SpeechBubble.transform;
            speechBubbleTransform.LookAt(Camera.main.transform);

            m_SpeechBubble.transform.rotation = Quaternion.Euler(speechBubbleTransform.rotation.eulerAngles.x,
                speechBubbleTransform.rotation.eulerAngles.y + 180.0f, speechBubbleTransform.rotation.eulerAngles.z);
        }

        //randomize the color
        m_ColorWanted = (ColorVariants)Random.Range((int)(ColorVariants.RED), (int)(ColorVariants.COLORLESS));

        StroopTestTypes stroopMode = GameHandler.Instance.RandomizeStroopType();

        if (m_SpeechBubble != null)
            m_SpeechBubble.SetActive(true);

        switch (stroopMode)
        {
            case StroopTestTypes.DEFAULT_TEXT: //normal version for text
                {
                    if (m_ColorText == null)
                        return;

                    m_ColorText.enabled = true;
                    m_ColorText.color = GameHandler.Instance.StroopDefaultColor();
                    m_ColorText.text = ColorData.Instance.GetColorName(m_ColorWanted);
                }
                break;
            case StroopTestTypes.DEFAULT_COLOR: //just show a color instead
                {
                    if (m_ColorDefaultImage == null)
                        return;

                    m_ColorDefaultImage.enabled = true;
                    m_ColorDefaultImage.color = ColorData.Instance.GetColor(m_ColorWanted);
                    m_ColorText.text = ColorData.Instance.GetColorName(m_ColorWanted);
                }
                break;
            case StroopTestTypes.SAME_COLOR_TEXT:
                {
                    if (m_ColorDefaultImage == null)
                        return;

                    m_ColorText.enabled = true;
                    m_ColorText.text = ColorData.Instance.GetColorName(m_ColorWanted);
                    m_ColorText.color = ColorData.Instance.GetColor(m_ColorWanted);
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

        NPCPlaySound("NPCArrive");
    }

    public bool TakeBalloon(ColorVariants colorGiven)
    {
        if (!m_IsWaiting)
            return false;

        bool customerHappy = colorGiven == m_ColorWanted;
        
        if (customerHappy)
        {
            Happy();
        }
        else
        {
            NPCPlaySound("NPCWrong");
            Sad();
        }

        return true;
    }

    //walk sadly away
    public void Sad()
    {
        if (m_Animator != null)
            m_Animator.SetBool("Sad", true);

        GameHandler.Instance.UpdateCustomerCounter(false);

        Leave();
    }

    //walk away happy
    public void Happy()
    {
        if (m_BalloonObj != null)
            m_BalloonObj.SetActive(true);

        if (m_BalloonMaterial != null)
            m_BalloonMaterial.color = ColorData.Instance.GetColor(m_ColorWanted);

        GameHandler.Instance.UpdateCustomerCounter(true);

        NPCPlaySound("NPCHappy");

        Leave();
    }

    //leave the qeueing position
    public void Leave()
    {
        m_IsWaiting = false;
        ResetNPCUI();

        if (OnLeftQueueCallback != null)
            OnLeftQueueCallback.Invoke(this);

        if (gameObject.activeInHierarchy)
            StartCoroutine(StartLeaving());
    }

    public void NPCPlaySound(string sound)
    {
        if (m_AudioSource == null)
            return;

        SoundManager.Instance.Play(sound, m_AudioSource);
    }

    IEnumerator StartLeaving()
    {
        if (m_BoxCollider != null)
            m_BoxCollider.enabled = false;

        //set the animation
        m_Animator.SetBool("Walking", true);

        m_OriginalTransform = gameObject.transform;
        m_NextPos = m_LeaveDestination;
        m_StartTiming = Time.time;
        while (!WalkToDestination())
        {
            RotateTowardsLocation(); //rotate towards the direction it is walking to

            yield return null;
        }

        gameObject.SetActive(false); //out of screen

        yield return null;
    }
}
