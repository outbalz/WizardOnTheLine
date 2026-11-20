
using UnityEngine;

public class CSquadFactory : CObjectPoolFactory<CSquadFactory, CSquadPool, CSquadContorller>
{
    public CSquadContorller Creat(Transform tr, SOSquadDataPreset data, SOPlayerData playerData)
    {
        CSquadContorller squad = Creat();
        squad.transform.position = tr.position;
        squad.InitializeSquad(data, playerData);
        squad.gameObject.SetActive(true);

        return squad;
    }

    public CSquadContorller Creat(Vector3 pos, SOSquadDataPreset data, SOPlayerData playerData)
    {
        CSquadContorller squad = Creat();
        squad.transform.position = pos;
        squad.InitializeSquad(data, playerData);
        squad.gameObject.SetActive(true);

        return squad;
    }
}
