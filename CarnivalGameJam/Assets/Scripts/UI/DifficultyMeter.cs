using UnityEngine;
[System.Serializable]
public class DifficultyMeter
{
    [Header("Difficulty Meter UI")]
    public float m_StartPosX = 90.0f;
    public float m_FinalPosX = 90.0f;
    public RectTransform m_DifficultyMeterRect;

    public void Init()
    {
        if (m_DifficultyMeterRect == null)
            return;

        m_DifficultyMeterRect.anchoredPosition = new Vector2(m_StartPosX, m_DifficultyMeterRect.anchoredPosition.y);
    }

    public void UpdateDifficultyMeter(float difficultyMultiplier)
    {
        if (m_DifficultyMeterRect == null)
            return;

        float difference = m_FinalPosX - m_StartPosX;
        float nextXPos = nextXPos = m_StartPosX + difference * difficultyMultiplier;

        nextXPos = Mathf.Clamp(nextXPos, m_FinalPosX, m_StartPosX);
        m_DifficultyMeterRect.anchoredPosition = new Vector2(nextXPos, m_DifficultyMeterRect.anchoredPosition.y);
    }
}
