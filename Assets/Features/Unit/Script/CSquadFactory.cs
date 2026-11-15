
using UnityEngine;

public class CSquadFactory : CObjectPoolFactory<CSquadFactory, CSquadPool, CSquadContorller>
{
    public CSquadContorller Creat(Transform tr, SOSquadDataPreset data)
    {
        CSquadContorller squad = Creat();
        squad.transform.position = tr.position;
        squad.InitializeSquad(data);

        return squad;
    }

    public CSquadContorller Creat(Vector3 pos, SOSquadDataPreset data)
    {
        CSquadContorller squad = Creat();
        squad.transform.position = pos;
        squad.InitializeSquad(data);

        return squad;
    }
}
