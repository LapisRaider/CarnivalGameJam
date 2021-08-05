using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHover : MonoBehaviour
{
    public float m_UpOffset = 1.0f;
    public float m_InterpolationTime = 0.2f;

    private RectTransform m_RectTransform;
    private Vector3 m_OriginalPos;

    private bool m_Hovering = false;

    public void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_OriginalPos = m_RectTransform.position;

        m_Hovering = false;
    }

    public void Update()
    {
        if (m_Hovering)
        {
            if (m_RectTransform.position.y < m_OriginalPos.y + m_UpOffset)
            {
                float newYPos = m_RectTransform.position.y + m_InterpolationTime * Time.deltaTime;
                m_RectTransform.position = new Vector3(m_RectTransform.position.x, newYPos, m_RectTransform.position.z);
            }
        }
        else
        {
            if (m_RectTransform.position.y > m_OriginalPos.y)
            {
                float newYPos = m_RectTransform.position.y - m_InterpolationTime * Time.deltaTime;
                m_RectTransform.position = new Vector3(m_RectTransform.position.x, newYPos, m_RectTransform.position.z);
            }
        }
    }

    public void MouseHoverButton()
    {
        if (m_RectTransform == null)
            return;

        m_Hovering = true;
    }

    public void MouseExitButton()
    {
        if (m_RectTransform == null)
            return;

        m_Hovering = false;
    }
}
