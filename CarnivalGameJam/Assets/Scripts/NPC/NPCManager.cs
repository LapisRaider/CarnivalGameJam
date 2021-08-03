using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("Object models")]
    public Material[] m_CustomerMaterial;
    public GameObject[] m_Props; //toys and stuff to attach

    [Header("Queue Positioning")]
    public Transform[] m_QueuePositions; 

    NPCObjectPooler m_NPCObjPooler = new NPCObjectPooler();

    //array positions
    //if there is a position in the array that is available


    // Start is called before the first frame update
    void Start()
    {
        m_NPCObjPooler.SpawnNPC();
    }

    // Update is called once per frame
    void Update()
    {
        //when there is an avilable spot in the queue, grab NPC and put it there
        //there should some NPCs at the back just chilling

        //NPC npc = m_NPCObjPooler.GetNPC();

        //
    }
}
