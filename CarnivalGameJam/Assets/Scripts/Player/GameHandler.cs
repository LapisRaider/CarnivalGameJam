using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : SingletonBase<GameHandler>
{
    //should deal with 

    //should check the current state of the game
    //should have a happiness modifier
    //check how much customers had in total
    //checked how many of them were happy/unhappy

    private float m_CurrModifierValue = 1.0f;

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
        // i need do the modifier
        //best to do based on max number of customers
    }

    void UpdateCustomerCounter(bool customerHappy)
    {
        ++m_TotalCustomersQueued;

        if (customerHappy)
        {
            ++m_TotalCustomersHappy;
            //TODO:: update highscore
        }


        //TODO:: update happiness UI here

        //update modifier here
        //invoke the deletgate
        if (ModifierUpdatedCallback != null)
            ModifierUpdatedCallback.Invoke(m_CurrModifierValue);
    }
}
