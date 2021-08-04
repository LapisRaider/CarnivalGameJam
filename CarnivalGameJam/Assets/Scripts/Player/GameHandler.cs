﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameHandler : SingletonBase<GameHandler>
{
    [Header("Game UI")]
    public TextMeshProUGUI m_HighScoreText;

    //difficulty is determined by the number of customers successfully served
    //formula is = m_MaxDifficultyIncrease * ((curr customer / m_AddDifficultyCustomerInterval) / m_MaxDifficultyCustomer)
    [Header("DifficultyModifier")]
    public float m_MaxDifficultyIncrease = 3.0f; // x + 1 times the amount of difficulty
    public int m_AddDifficultyCustomerInterval = 1; // For x number of customer difficulty will increase
    public int m_MaxDifficultyInterval = 20;
    private float m_CurrModifierValue = 1.0f;

    [Header("HighScore")]
    public int m_DefaultScoreAdded = 20;
    public int m_MaxScoreAdded = 40;
    private int m_CurrHighScore = 0;

    //checked how many of them were happy/unhappy
    private int m_TotalCustomersQueued = 0;
    private int m_TotalCustomersHappy = 0;


    public delegate void ModifierUpdated(float currModifier);
    public ModifierUpdated ModifierUpdatedCallback;

    // Start is called before the first frame update
    override public void Awake()
    {
        base.Awake();

        m_TotalCustomersQueued = 0;
        m_TotalCustomersHappy = 0;

        m_CurrModifierValue = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateCustomerCounter(bool customerHappy)
    {
        ++m_TotalCustomersQueued;

        if (customerHappy)
        {
            ++m_TotalCustomersHappy;

            //update difficulty modifier
            int difficultyInterval = (int)((float)m_TotalCustomersHappy / (float)m_AddDifficultyCustomerInterval);
            m_CurrModifierValue = 1.0f + ((float)difficultyInterval / (float)m_MaxDifficultyInterval) * m_MaxDifficultyIncrease;
            
            if (ModifierUpdatedCallback != null)
                ModifierUpdatedCallback.Invoke(m_CurrModifierValue); //invoke the deletgate

            //TODO:: IF GOT TIME, DO A DIFFICULTY UI

            UpdateAndAddHighScore();
        }

        UpdateHappinessLevels();
    }

    public void UpdateAndAddHighScore()
    {
        int scoreGained = (int)(m_DefaultScoreAdded * m_CurrModifierValue);
        scoreGained = Mathf.Clamp(scoreGained, m_DefaultScoreAdded, m_MaxScoreAdded);
        m_CurrHighScore += scoreGained;

        //TODO:: update highscore and UI
        if (m_HighScoreText == null)
            return;

        //TODO:: maybe can lerp the text and do a + add score thing
        m_HighScoreText.SetText(m_CurrHighScore.ToString());
    }

    public void UpdateHappinessLevels()
    {
        //TODO:: update happiness UI here and check if below threshold

    }

    void LoseGame()
    {
        //when unhapiness level reach a certain threshold
        //stop game and show gameover screen, or just transition to gameover screen
    }
}
