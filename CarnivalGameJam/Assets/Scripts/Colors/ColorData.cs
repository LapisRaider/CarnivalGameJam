using System.Collections.Generic;
using UnityEngine;

public class ColorData : SingletonBase<ColorData>
{
    [Header("Color Info")]
    public ColorInfoData[] m_ColorInfoData;

    private Dictionary<ColorVariants, ColorInfo> m_AvilableColorList = new Dictionary<ColorVariants, ColorInfo>(); // allow for easy access to color UI data

    // Start is called before the first frame update
    override public void Awake()
    {
        //transfer the color info to the dictionary
        foreach (ColorInfoData colorInfoData in m_ColorInfoData)
        {
            colorInfoData.m_ColorInfo.m_ColorName = colorInfoData.m_Name;
            m_AvilableColorList.Add(colorInfoData.m_ColorEnumType, colorInfoData.m_ColorInfo);
        }
    }

    //get the color after mixing
    public ColorVariants AddColor(ColorMixes currentColorMix)
    {
        //there should be a fuckin variant, if dont have just return the same current color
        foreach (KeyValuePair<ColorVariants, ColorInfo> availableColor in m_AvilableColorList)
        {
            ColorMixes colorMixes = availableColor.Value.m_ColorToMix;

            //check if have all the correct color type
            if (currentColorMix.m_Red != colorMixes.m_Red)
                continue;

            if (currentColorMix.m_Blue != colorMixes.m_Blue)
                continue;

            if (currentColorMix.m_Yellow != colorMixes.m_Yellow)
                continue;

            return availableColor.Key;
        }

        return ColorVariants.COLORLESS;
    }

    public string GetColorName(ColorVariants colorType)
    {
        if (!m_AvilableColorList.ContainsKey(colorType))
            return "";

        return m_AvilableColorList[colorType].m_ColorName;
    }

    public Color GetColor(ColorVariants colorType)
    {
        if (!m_AvilableColorList.ContainsKey(colorType))
            return Color.white;

        return m_AvilableColorList[colorType].m_Color;
    }
}