using UnityEngine;

public class CUnitFactory : CObjectPoolFactory<CUnitFactory, CUnitPool, CUnitController>
{
    public CUnitController Creat(CSquadContorller squad, SOUnitData data, Vector3 pos)
    {
        CUnitController unit = Creat(squad.transform);
        unit.transform.position = pos;
        unit.InitializeUnit(squad, data);
        unit.gameObject.SetActive(true);

        return unit;
    }
}
