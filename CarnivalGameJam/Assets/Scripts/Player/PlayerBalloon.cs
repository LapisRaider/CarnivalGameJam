using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBalloon : MonoBehaviour
{
    //have a ref to the balloon
    [Header("Balloon Object")]
    public Animator m_BalloonAnimator;
    public Material m_BalloonMaterial;
    public Color m_DefaultBalloonColor; //default balloon color

    [Header("Balloon Effects")]
    public ParticleSystem m_BurstBalloonParticles;

    [Header("Paint Particle Effects")]
    public ParticleSystem m_RedSpray;
    public ParticleSystem m_BlueSpray;
    public ParticleSystem m_YellowSpray;

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

                    if (npc.TakeBalloon(m_CurrentColor)) //give balloon to child
                        ResetBalloon();
                }
            }
        }
    }

    //reset the balloon and material
    private void ResetBalloon()
    {
        //replay the balloon BLOWING animation
        if (m_BalloonAnimator != null)
            m_BalloonAnimator.SetTrigger("BlowBalloon");

        SoundManager.Instance.Play("BalloonInflate");
        
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
        if (m_BurstBalloonParticles != null)
        {
            m_BurstBalloonParticles.Play();
            var particleSettings = m_BurstBalloonParticles.main;
            particleSettings.startColor = ColorData.Instance.GetColor(m_CurrentColor);
        }

        SoundManager.Instance.Play("BalloonPop");
        EventSystem.current.SetSelectedGameObject(null);

        ResetBalloon();
    }

    private void ChangeBalloonColor()
    {
        m_CurrentColor = ColorData.Instance.AddColor(m_CurrentMix);

        if (m_BalloonMaterial == null)
        {
            Debug.LogWarning("NO BALLOON OBJ REF");
            return;
        }

        m_BalloonMaterial.color = ColorData.Instance.GetColor(m_CurrentColor);
    }

    public void AddBlue()
    {
        if (m_BlueSpray != null)
            m_BlueSpray.Play();

        SoundManager.Instance.Play("SprayPaint");
        EventSystem.current.SetSelectedGameObject(null);

        //if already true ignore
        if (m_CurrentMix.m_Blue)
            return;

        m_CurrentMix.m_Blue = true;
        ChangeBalloonColor();
    }

    public void AddRed()
    {
        if (m_RedSpray != null)
            m_RedSpray.Play();

        SoundManager.Instance.Play("SprayPaint");
        EventSystem.current.SetSelectedGameObject(null);

        //if already true ignore
        if (m_CurrentMix.m_Red)
            return;

        m_CurrentMix.m_Red = true;
        ChangeBalloonColor();
    }

    public void AddYellow()
    {
        if (m_YellowSpray != null)
            m_YellowSpray.Play();

        SoundManager.Instance.Play("SprayPaint");
        EventSystem.current.SetSelectedGameObject(null);

        //if already true ignore
        if (m_CurrentMix.m_Yellow)
            return;

        m_CurrentMix.m_Yellow = true;
        ChangeBalloonColor();
    }

}
