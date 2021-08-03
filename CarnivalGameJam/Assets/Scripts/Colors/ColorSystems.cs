using UnityEngine;
using System;

// keeps track of the color UI info and enum type, this is meant for inspector data
[Serializable]
public class ColorInfoData
{
    public string m_Name; // to display at inspector

    public ColorVariants m_ColorEnumType;
    public ColorInfo m_ColorInfo;
}

[Serializable]
public struct ColorInfo
{
    [Header("Color info")]
    public Color m_Color;
    [HideInInspector] public string m_ColorName;

    [Header("Color Combo Mixes")]
    public ColorMixes m_ColorToMix;
}

[Serializable]
public struct ColorMixes //since we're only having the primary colors in the game
{
    public bool m_Red;
    public bool m_Blue;
    public bool m_Yellow;
}

public enum ColorVariants
{
    RED,
    YELLOW,
    BLUE,
    PURPLE, //red + blue
    ORANGE, //yellow + red
    GREEN, //blue + yellow
    BROWN, //red + blue + yellow
    COLORLESS, //no color
}
