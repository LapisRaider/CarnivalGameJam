using UnityEngine;

[System.Serializable]
public class HappinessMeter
{
    [Header("UI for meter")]
    public RectTransform m_Needle;
    public float m_TotalRotationValue = 180.0f;
    public float m_StartingRotation = -90.0f;

    //SHOW DANGER SIGN WHEN reach below threshold
    [Header("Warning")]
    public GameObject m_DangerText;
    public float m_PercentageToDanger;

    public void Init()
    {
        m_Needle.rotation = Quaternion.Euler(0.0f, 0.0f, m_StartingRotation);
    }

    public void UpdateHappinessMeter(float successRate)
    {
        if (successRate <= m_PercentageToDanger)
        {
            if (m_DangerText != null)
                m_DangerText.SetActive(true);
        }
        else
        {
            if (m_DangerText != null)
                m_DangerText.SetActive(false);
        }

        if (m_Needle == null)
            return;

        float newRotationValue = m_TotalRotationValue * successRate;
        newRotationValue = Mathf.Clamp(newRotationValue, 0.0f, m_TotalRotationValue);
        if (newRotationValue > 90.0f)
        {
            newRotationValue = -(newRotationValue - 90.0f);
        }
        else
        {
            newRotationValue = 90.0f - newRotationValue;
        }
           
        m_Needle.rotation = Quaternion.Euler(0.0f, 0.0f, newRotationValue);
    }
}
