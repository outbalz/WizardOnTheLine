

using UnityEngine;

public class CUnitPool : CObjectPool<CUnitPool,CUnitController>
{
    public void ReturnAllUnit()
    {
        for (int i = _liveList.Count -1; i >= 0; i--)
        {
            _liveList[i].KillUnit();
        }
    }
}
