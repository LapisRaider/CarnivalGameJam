using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("Object models")]
    public Texture[] m_CustomerMaterial;
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
    private NPC[] m_NPCsWaitingRef; //ref to which NPC is in the waiting spawn pos

    private NPC[] m_CustomersInQueue; //for customers actually ordering
    private float m_CurrCustomerQueuing = 0;
    private float m_SpawnIntervalTracker = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_CustomersInQueue = new NPC[m_QueuePositions.Length];
        for (int i = 0; i < m_QueuePositions.Length; ++i)
        {
            m_CustomersInQueue[i] = null;
        }

        m_NPCsWaitingRef = new NPC[m_SpawnPos.Length];
        for (int i = 0; i < m_SpawnPos.Length; ++i)
        {
            m_NPCsWaitingRef[i] = null;
        }

        m_NPCObjPooler.AddNPCInPooler();
        m_NPCManagerData.Init();

        m_SpawnIntervalTracker = m_NPCManagerData.m_DefaultSpawnInterval;

        GameHandler.Instance.ModifierUpdatedCallback += UpdateNPCDataModifiers;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_SpawnIntervalTracker >= 0.0f)
        {
            m_SpawnIntervalTracker -= Time.deltaTime;

        }
        else
        {
            //spawn NPCs into waiting list and positions
            if (m_WaitingNPCs.Count < m_SpawnPos.Length)
            {
                float randomRate = Random.Range(0.0f, 1.0f);
                if (randomRate <= m_NPCManagerData.m_CurrSpawnRate)
                {
                    //need check if got space first and wait
                    NPC npc = SpawnInNPC(true);
                    if (npc != null)
                    {
                        m_WaitingNPCs.Enqueue(npc);
                    }
                    else
                    {
                        Debug.LogWarning("Something wrong with the waiting position tracker");
                    }
                }

                m_SpawnIntervalTracker = m_NPCManagerData.m_CurrSpawnInterval; //reset spawn interval
            }
        }

        //grab from the waiting list and put to queue
        if (m_CurrCustomerQueuing < m_CustomersInQueue.Length)
        {
            if (m_WaitingNPCs.Count != 0)
            {
                NPC npc = m_WaitingNPCs.Dequeue();

                //remove them from the waiting reference list
                for (int i = 0; i < m_NPCsWaitingRef.Length; ++i)
                {
                    if (m_NPCsWaitingRef[i] == npc)
                    {
                        m_NPCsWaitingRef[i] = null;
                        break;
                    }
                }

                GetNPCToQueue(npc);
            }
        }

        //if not enough customers ordering the balloon, and none in waiting list, spawn some immediately
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

            npc.InitNPCToQueue(m_NPCManagerData.m_CurrNPCPatienceTime, m_NPCManagerData.m_CurrNPCMoveSpeed,
                                m_NPCManagerData.m_CurrNPCRotationSpeed, m_QueuePositions[i], leavePos);
            npc.OnLeftQueueCallback += NPCLeftQueuePos; //set the callback

            m_CustomersInQueue[i] = npc;

            ++m_CurrCustomerQueuing;

            break;
        }
    }

    private NPC SpawnInNPC(bool isWaiting = false)
    {
        if (m_SpawnPos.Length == 0)
        {
            Debug.LogWarning("SpawnPos Empty");
            return null;
        }

        //search for an empty waiting position
        for (int i = 0; i < m_NPCsWaitingRef.Length; ++i)
        {
            if (m_NPCsWaitingRef[i] != null)
                continue;

            NPC npc = m_NPCObjPooler.GetNPC();
            if (npc == null)
            {
                Debug.LogWarning("No NPC being spawned from pooler");
                return null;
            }

            GameObject npcObj = npc.gameObject;
            npcObj.transform.position = m_SpawnPos[i].position;
            npcObj.SetActive(true);

            //waiting to be queued, so no npc should spawn at this position anymore
            if (isWaiting)
                m_NPCsWaitingRef[i] = npc;

            GameObject prop = null;
            if (m_Props.Length != 0)
            {
                prop = m_Props[Random.Range(0, m_Props.Length)];
                if (prop.activeInHierarchy)
                    prop = null;
            }

            //properly set the props and material
            npc.StartNPCAppear(prop, m_CustomerMaterial[Random.Range(0,8)]);

            return npc;
        }

        return null;
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