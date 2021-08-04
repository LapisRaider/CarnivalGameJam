using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("Object models")]
    public Material[] m_CustomerMaterial;
    public GameObject[] m_Props; //toys and stuff to attach

    [Header("Queue Positioning")]
    public Transform[] m_QueuePositions; //determines the max customer queing at once
    public Transform[] m_LeavePos;
    public Transform[] m_SpawnPos; //places to spawn the NPC at

    [Header("Customer spawning Rates")]
    public NPCObjectPooler m_NPCObjPooler = new NPCObjectPooler();
    public float m_MinCustomersQueuing = 1; //if less than this number spawn in immediately
    public NPCManagerData m_NPCManagerData;

    private Queue<NPC> m_WaitingNPCs = new Queue<NPC>(); //for npcs waiting to get into queue
    private float m_CurrCustomerQueuing = 0;
    private NPC[] m_CustomersInQueue; //for customers actually ordering

    // Start is called before the first frame update
    void Start()
    {
        m_CustomersInQueue = new NPC[m_QueuePositions.Length];
        for (int i = 0; i < m_QueuePositions.Length; ++i)
        {
            m_CustomersInQueue[i] = null;
        }

        m_NPCObjPooler.AddNPCInPooler();
        m_NPCManagerData.Init();

        GameHandler.Instance.ModifierUpdatedCallback += UpdateNPCDataModifiers;
    }

    // Update is called once per frame
    void Update()
    {
        
        
        //TODO:: MAKE SURE TO ADD SOME SORT OF TIMER HERE for spawning
        if (m_WaitingNPCs.Count < m_SpawnPos.Length)
        {
            float randomRate = Random.Range(0.0f, 1.0f);
            if (randomRate <= m_NPCManagerData.m_CurrSpawnRate)
            {
                m_WaitingNPCs.Enqueue(SpawnInNPC());
            }
        }

        if (m_CurrCustomerQueuing < m_CustomersInQueue.Length)
        {
            if (m_WaitingNPCs.Count != 0)
            {
                NPC npc = m_WaitingNPCs.Dequeue(); //grab from the queue
                GetNPCToQueue(npc);
            }
        }

        //if not enough customers ordering the balloon, spawn some immediately
        if (m_CurrCustomerQueuing < m_MinCustomersQueuing)
        {
            NPC npc = SpawnInNPC();
            GetNPCToQueue(npc);
        }
    }

    private void GetNPCToQueue(NPC npc)
    {
        //add NPC to queue and get the queue position
        for (int i = 0; i < m_CustomersInQueue.Length; ++i)
        {
            if (m_CustomersInQueue[i] != null)
                continue;

            Vector3 leavePos = m_LeavePos[Random.Range(0, m_LeavePos.Length)].position;
            Vector3 queuePos = m_QueuePositions[i].position;

            npc.InitNPCToQueue(m_NPCManagerData.m_CurrNPCPatienceTime, m_NPCManagerData.m_CurrNPCMoveSpeed,
                                m_NPCManagerData.m_CurrNPCRotationSpeed, queuePos, leavePos);
            npc.OnLeftQueueCallback += NPCLeftQueuePos; //set the callback

            m_CustomersInQueue[i] = npc;

            ++m_CurrCustomerQueuing;

            break;
        }
    }

    private NPC SpawnInNPC()
    {
        if (m_SpawnPos.Length == 0)
        {
            Debug.LogWarning("SpawnPos Empty");
            return null;
        }

        NPC npc = m_NPCObjPooler.GetNPC();
        if (npc == null)
        {
            Debug.LogWarning("No NPC being spawned from pooler");
            return null;
        }

        GameObject npcObj = npc.gameObject;
        npcObj.transform.position = m_SpawnPos[Random.Range(0, m_SpawnPos.Length)].position;

        npcObj.SetActive(true);

        //TODO:: should have some sort of spawning animation, like phase in kind
        //npc.spawnIN() //something like that here
        //TODO:: change the material of the NPC and also attach some objects onto the npc

        return npc;
    }

    void NPCLeftQueuePos(NPC npc)
    {
        npc.OnLeftQueueCallback -= NPCLeftQueuePos; //remove the callback
        --m_CurrCustomerQueuing;

        for (int i = 0; i < m_CustomersInQueue.Length; ++i)
        {
            if (m_CustomersInQueue[i] == null)
                continue;

            if (npc != m_CustomersInQueue[i])
                continue;

            m_CustomersInQueue[i] = null;
            break;
        }
    }

    private void UpdateNPCDataModifiers(float currModifier)
    {
        m_NPCManagerData.UpdateBasedOnModifier(currModifier);
    }
}

[System.Serializable]
public class NPCManagerData
{
    [Header("Customer spawning Rates")]
    public float m_MaxSpawnRate = 0.75f; //spawn rate should increase
    public float m_DefaultSpawnRate = 0.5f;

    [Header("NPC spawn intervals")]
    public float m_MinSpawnInterval = 5.0f; //spawn interval should be decreasing as the time goes
    public float m_DefaultSpawnInterval = 10.0f;

    [Header("NPC Speed data")]
    //will increase as time goes
    public float m_NPCMaxSpeed = 1.0f;
    public float m_NPCDefaultSpeed = 1.0f;

    [Header("NPC Rotation Speed data")]
    //will increase as time goes
    public float m_NPCMaxRotationSpeed = 1.0f;
    public float m_NPCDefaultRotationSpeed = 1.0f;

    //will decrease as time goes
    [Header("NPC Patience Timing")]
    public float m_NPCDefaultPatienceTime = 2.0f;
    public float m_NPCMinPatienceTime = 2.0f;

    [Header("Current NPC Manager Data")]
    [HideInInspector] public float m_CurrSpawnRate = 0.5f;
    [HideInInspector] public float m_CurrSpawnInterval = 10.0f;
    [HideInInspector] public float m_CurrNPCMoveSpeed = 1.0f;
    [HideInInspector] public float m_CurrNPCRotationSpeed = 1.0f;
    [HideInInspector] public float m_CurrNPCPatienceTime = 1.0f;

    public void Init()
    {
        m_CurrSpawnRate = m_DefaultSpawnRate;
        m_CurrSpawnInterval = m_DefaultSpawnInterval;

        m_CurrNPCMoveSpeed = m_NPCDefaultSpeed;
        m_CurrNPCRotationSpeed = m_NPCDefaultRotationSpeed;
        m_CurrNPCPatienceTime = m_NPCDefaultPatienceTime;
    }

    public void UpdateBasedOnModifier(float currModifier)
    {
        float modifierCalculator = m_DefaultSpawnRate * currModifier;
        m_CurrSpawnRate = Mathf.Clamp(modifierCalculator, m_DefaultSpawnRate, m_MaxSpawnRate);

        //spawn interval
        modifierCalculator = m_DefaultSpawnInterval * (1.0f / currModifier);
        m_CurrSpawnInterval = Mathf.Clamp(modifierCalculator, m_MinSpawnInterval, m_DefaultSpawnInterval);

        //NPC move speed
        modifierCalculator = m_NPCDefaultSpeed * currModifier;
        m_CurrNPCMoveSpeed = Mathf.Clamp(modifierCalculator, m_NPCDefaultSpeed, m_NPCMaxSpeed);

        //npc rotation speed
        modifierCalculator = m_NPCDefaultRotationSpeed * currModifier;
        m_CurrNPCRotationSpeed = Mathf.Clamp(modifierCalculator, m_NPCDefaultRotationSpeed, m_NPCMaxRotationSpeed);

        //npc patience time
        modifierCalculator = m_NPCDefaultPatienceTime * (1.0f / currModifier);
        m_CurrNPCPatienceTime = Mathf.Clamp(modifierCalculator, m_NPCMinPatienceTime, m_NPCDefaultPatienceTime);
    }
}