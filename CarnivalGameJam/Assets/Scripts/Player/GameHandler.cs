using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameHandler : SingletonBase<GameHandler>
{
    [Header("Game UI")]
    public TextMeshProUGUI m_HighScoreText;
    public int m_HighScoreCharacters = 6;
    public HappinessMeter m_HappinessMeter = new HappinessMeter();
    public DifficultyMeter m_DifficultyMeter = new DifficultyMeter();

    //difficulty is determined by the number of customers successfully served
    //formula is = m_MaxDifficultyIncrease * ((curr customer / m_AddDifficultyCustomerInterval) / m_MaxDifficultyCustomer)
    [Header("DifficultyModifier")]
    public float m_MaxDifficultyIncrease = 3.0f; // x + 1 times the amount of difficulty
    public int m_AddDifficultyCustomerInterval = 1; // For x number of customer difficulty will increase
    public int m_MaxDifficultyInterval = 20;
    private float m_CurrModifierValue = 1.0f;

    [Header("Lose Conditions")]
    public int m_FailedCustomerBuffer = 5; //after reaching below the threshold, a small buffer
    public float m_FailurePercentage = 0.2f;
    public LoseScene m_LoseScene = new LoseScene();
    [HideInInspector] public bool m_Lose = false;

    [Header("Stroop Handler")]
    public StroopColorTest m_StroopTest = new StroopColorTest();

    [Header("HighScore")]
    public int m_DefaultScoreAdded = 20;
    public int m_MaxScoreAdded = 40;
    private int m_CurrHighScore = 0;
    private int m_UIHighScore = 0;

    //checked how many of them were happy/unhappy
    private int m_TotalCustomersQueued = 0;
    private int m_TotalCustomersHappy = 0;

    //when modifier gets updated
    public delegate void ModifierUpdated(float currModifier);
    public ModifierUpdated ModifierUpdatedCallback;

    // Start is called before the first frame update
    override public void Awake()
    {
        base.Awake();

        m_TotalCustomersQueued = 0;
        m_TotalCustomersHappy = 0;

        m_CurrModifierValue = 1.0f;

        ModifierUpdatedCallback += m_StroopTest.StroopModifierUpdate;

        m_HappinessMeter.Init();
        m_DifficultyMeter.Init();

        m_Lose = false;
    }

    public void UpdateCustomerCounter(bool customerHappy)
    {
        ++m_TotalCustomersQueued;

        if (customerHappy)
        {
            ++m_TotalCustomersHappy;

            //update difficulty modifier
            int difficultyInterval = (int)((float)m_TotalCustomersHappy / (float)m_AddDifficultyCustomerInterval);
            float difficultyPercentage = (float)difficultyInterval / (float)m_MaxDifficultyInterval;
            m_CurrModifierValue = 1.0f + difficultyPercentage * m_MaxDifficultyIncrease;

            if (ModifierUpdatedCallback != null)
                ModifierUpdatedCallback.Invoke(m_CurrModifierValue); //invoke the deletgate

            m_DifficultyMeter.UpdateDifficultyMeter(difficultyPercentage);

            UpdateAndAddHighScore();
        }

        UpdateHappinessLevels();
    }

    public void UpdateAndAddHighScore()
    {
        int scoreGained = (int)(m_DefaultScoreAdded * m_CurrModifierValue);
        scoreGained = Mathf.Clamp(scoreGained, m_DefaultScoreAdded, m_MaxScoreAdded);
        m_CurrHighScore += scoreGained;

        if (m_HighScoreText == null)
            return;
    }

    public void Update()
    {   
        if (m_UIHighScore < m_CurrHighScore)
        {
            m_UIHighScore += 1;
            string highScoreText = m_UIHighScore.ToString();
            string highScoreTextPadding = "";

            if (highScoreText.Length < m_HighScoreCharacters)
            {
                int charactersToPad = m_HighScoreCharacters - highScoreText.Length;

                for (int i = 0; i < charactersToPad; ++i)
                {
                    highScoreTextPadding += "0";
                }
            }
            else if (highScoreText.Length > m_HighScoreCharacters)
            {
                //if players are insane enough to get more than the limit
                for (int i = 0; i < m_HighScoreCharacters; ++i)
                {
                    highScoreTextPadding += "X";
                }

                highScoreText = "";
            }

            if (m_HighScoreText != null)
                m_HighScoreText.SetText(highScoreTextPadding + highScoreText);
        }
        else if (m_UIHighScore > m_CurrHighScore) //a just in case, but this should never happen
        {
            m_UIHighScore = m_CurrHighScore;
        }

    }

    public void UpdateHappinessLevels()
    {
        int maxFailures = (int)((float)m_TotalCustomersQueued * m_FailurePercentage) + m_FailedCustomerBuffer;
        int numberOfFailures = m_TotalCustomersQueued - m_TotalCustomersHappy;

        float successRate = 1.0f - ((float)numberOfFailures / (float)maxFailures);
        m_HappinessMeter.UpdateHappinessMeter(successRate); //update UI

        if (numberOfFailures >= maxFailures)
        {
            LoseGame();
        }
    }

    //when unhapiness level reach a certain threshold
    void LoseGame()
    {
        m_Lose = true;
        m_LoseScene.SetUpLose(m_CurrHighScore);
    }

    //functions belonging to the stroop color test
    public StroopTestTypes RandomizeStroopType()
    {
        return m_StroopTest.RandomizeStroopType();
    }

    public Color StroopDefaultColor()
    {
        return m_StroopTest.m_StroopDefaultColor;
    }
}
