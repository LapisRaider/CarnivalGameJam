using System.Collections;
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


    //checked how many of them were happy/unhappy

    private int m_TotalCustomersQueued = 0;
    private int m_TotalCustomersHappy = 0;

    //TODO:: the score should be added based on the current state of the game
    //when u successfully serve a customer ++ to it
    //private int m_HighScore = 0;

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

    void UpdateCustomerCounter(bool customerHappy)
    {
        ++m_TotalCustomersQueued;

        if (customerHappy)
        {
            ++m_TotalCustomersHappy;
            //TODO:: update highscore and UI

            //TODO:: IF GOT TIME, DO A DIFFICULTY UI

            int difficultyInterval = m_TotalCustomersHappy / m_AddDifficultyCustomerInterval;
            m_CurrModifierValue = 1.0f + (difficultyInterval / m_MaxDifficultyInterval) * m_MaxDifficultyIncrease;
            
            if (ModifierUpdatedCallback != null)
                ModifierUpdatedCallback.Invoke(m_CurrModifierValue); //invoke the deletgate
        }


        //TODO:: update happiness UI here
    }

    void LoseGame()
    {
        //when unhapiness level reach a certain threshold
    }
}
