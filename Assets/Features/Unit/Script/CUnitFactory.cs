using UnityEngine;

public class CUnitFactory : CObjectPoolFactory<CUnitFactory, CUnitPool, CUnitController>
{
    public CUnitController Creat(CSquadContorller squad, SOUnitData data)
    {
        CUnitController unit = Creat(squad.transform);
        unit.transform.position = squad.transform.position;
        unit.InitializeUnit(squad, data);
        unit.gameObject.SetActive(true);

        return unit;
    }
}
