using UnityEngine;

[System.Serializable]
public class StroopColorTest
{
    [Header("Chances")]
    public int m_DefaultTextChance = 33;
    public int m_DefaultColorChance = 33;

    [Header("Chances diff text color")]
    public int m_DefaultDiffTextColorChance = 10;
    public int m_MaxDiffTextColorChance = 70;
    private int m_CurrDiffTextColorChance = 0;

    [Header("UI Font")]
    public Color m_StroopDefaultColor = Color.black;

    public void Init()
    {
        m_CurrDiffTextColorChance = m_DefaultDiffTextColorChance;
    }

    public void StroopModifierUpdate(float modifier)
    {
        //should increase
        int modifiedChance = (int)(m_DefaultDiffTextColorChance * modifier);
        m_CurrDiffTextColorChance = Mathf.Clamp(modifiedChance, m_DefaultDiffTextColorChance, m_MaxDiffTextColorChance);
    }

    public StroopTestTypes RandomizeStroopType()
    {
        int totalChance = m_DefaultColorChance + m_DefaultTextChance + m_CurrDiffTextColorChance;
        int randomized = Random.Range(0, totalChance);

        if (randomized <= m_DefaultTextChance) //default text
        {
            return StroopTestTypes.DEFAULT_TEXT;
        }
        else if (randomized <= m_DefaultTextChance + m_DefaultColorChance) //default color
        {
            return StroopTestTypes.DEFAULT_COLOR;
        }
        else
        {
            return StroopTestTypes.DIFF_TEXT_COLOR;
        }
    }
}

public enum StroopTestTypes
{
    DEFAULT_TEXT, //text of color is just displayed
    DEFAULT_COLOR, //default color, just show the color
    DIFF_TEXT_COLOR, //randomize the color and text
}