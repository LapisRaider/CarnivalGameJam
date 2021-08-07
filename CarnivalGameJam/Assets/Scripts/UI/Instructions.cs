using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
 
public class Instructions : MonoBehaviour
{
    [Header("UI in game")]
    public Image m_InstructionImage;
    public Button m_RightButton;
    public Button m_LeftButton;
    public TextMeshProUGUI m_PageNumber;

    [Header("Instructions")]
    public Sprite[] m_Instructions;

    private int m_CurrPageNo = 0;

    // Start is called before the first frame update
    void Start()
    {
        ResetInstructions();
    }

    private void OnEnable()
    {
        ResetInstructions();
    }

    public void ResetInstructions()
    {
        m_CurrPageNo = 0;

        if (m_InstructionImage != null)
            m_InstructionImage.sprite = m_Instructions[m_CurrPageNo];

        if (m_LeftButton != null)
            m_LeftButton.interactable = false;

        if (m_RightButton != null)
            m_RightButton.interactable = true;

        
        string pageNumber = m_CurrPageNo + "/" + m_Instructions.Length.ToString();
        if (m_PageNumber != null)
            m_PageNumber.text = pageNumber;
    }

    public void TurnPage(bool next) //if false its prev page
    {
        if (next)
            ++m_CurrPageNo;
        else
            --m_CurrPageNo;

        m_CurrPageNo = Mathf.Clamp(m_CurrPageNo, 0, m_Instructions.Length - 1);

        if (m_InstructionImage != null)
            m_InstructionImage.sprite = m_Instructions[m_CurrPageNo];

        if (m_LeftButton != null)
        {
            if (m_CurrPageNo == 0)
                m_LeftButton.interactable = false;
            else
                m_LeftButton.interactable = true;
        }

        if (m_RightButton != null)
        {
            if (m_CurrPageNo >= m_Instructions.Length - 1)
                m_RightButton.interactable = false;
            else
                m_RightButton.interactable = true;
        }

        string pageNumber = (m_CurrPageNo + 1) + "/" + m_Instructions.Length.ToString();
        if (m_PageNumber != null)
            m_PageNumber.text = pageNumber;

        EventSystem.current.SetSelectedGameObject(null);
    }
}
