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

    public float m_CurrSpawnInterval = 10.0f;
    private float m_CurrSpawnRate = 0.5f;
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
    }

    // Update is called once per frame
    void Update()
    {

        //TODO CHECK THE CURRENT GAME STATE AND GET A MULTIPLIER FROM IT
        //m_CurrSpawnRate UPDATE THIS


        //TODO:: MAKE SURE TO ADD SOME SORT OF TIMER HERE for spawning
        if (m_WaitingNPCs.Count < m_SpawnPos.Length)
        {
            float randomRate = Random.Range(0.0f, 1.0f);
            if (randomRate <= m_CurrSpawnRate)
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

            //TODO:: the speed, patience time and rotation time 
            //should be determined based on the current difficulty modifier
            float patienceTime = 1.0f;
            float walkSpeed = 1.0f;
            float rotationSpeed = 1.0f;

            npc.InitNPCToQueue(patienceTime, walkSpeed, rotationSpeed, queuePos, leavePos);
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
}

[System.Serializable]
public class NPCManagerData
{
    [Header("Customer spawning Rates")]
    public float m_MaxSpawnRate = 0.75f;
    public float m_MinSpawnRate = 0.5f;

    public float m_MinSpawnInterval = 5.0f;
    public float m_MaxSpawnInterval = 10.0f;

    [Header("NPC data")]
    //will increase as time goes
    public float m_NPCMaxSpeed = 1.0f;
    public float m_NPCMinSpeed = 1.0f;

    public float m_NPCMaxRotationSpeed = 1.0f;
    public float m_NPCMinRotationSpeed = 1.0f;

    //will decrease as time goes
    public float m_NPCMaxPatienceTime = 2.0f;
    public float m_NPCMinPatienceTime = 2.0f;
}