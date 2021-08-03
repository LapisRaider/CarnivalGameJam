using System.Collections.Generic;
using UnityEngine;

public class NPCObjectPooler
{
    public GameObject m_NPCPrefab;
    public GameObject m_Parent;
    public int m_InitialSpawnAmt;

    List<NPC> m_NPCList = new List<NPC>();

    // Start is called before the first frame update
    void SpawnNPC()
    {
        for (int i = 0; i < m_InitialSpawnAmt; ++i)
        {
            if (m_NPCPrefab == null)
                return;

            GameObject npcObj = GameObject.Instantiate(m_NPCPrefab, new Vector3(0.0f, 0, 0), Quaternion.identity);
            NPC npc = npcObj.GetComponent<NPC>();

            m_NPCList.Add(npc);
        }
    }

    public NPC GetNPC()
    {
        foreach (NPC npc in m_NPCList)
        {
            if (!npc.gameObject.activeSelf)
                return npc;
        }

        SpawnNPC();

        return GetNPC();
    }
}
