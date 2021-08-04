using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCObjectPooler
{
    public GameObject m_NPCPrefab;
    public GameObject m_Parent;
    public int m_InitialSpawnAmt;

    List<NPC> m_NPCList = new List<NPC>();

    public void AddNPCInPooler()
    {
        for (int i = 0; i < m_InitialSpawnAmt; ++i)
        {
            if (m_NPCPrefab == null)
                return;

            GameObject npcObj = GameObject.Instantiate(m_NPCPrefab, new Vector3(0.0f, 0, 0), Quaternion.identity);
            NPC npc = npcObj.GetComponent<NPC>();
            npcObj.SetActive(false);

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

        AddNPCInPooler();

        return GetNPC();
    }
}
