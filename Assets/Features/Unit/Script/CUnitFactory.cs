using UnityEngine;

public class CUnitFactory : CObjectPoolFactory<CUnitFactory, CUnitPool, CUnitController>
{
    public CUnitController Creat(CSquadContorller squad, SOUnitStatusData data)
    {
        CUnitController unit = Creat(squad.transform);
        unit.transform.position = squad.transform.position;
        unit.InitializeUnit(squad, data);

        return unit;
    }
}
