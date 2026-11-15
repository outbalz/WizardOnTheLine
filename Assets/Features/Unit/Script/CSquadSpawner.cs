using UnityEngine;

public class CSquadSpawner : MonoBehaviour
{

    public void SpawnSquad(SOSquadDataPreset data)
    {
        CSquadFactory.Instance.Creat(transform, data);
    }

    //for test====================
    #region debug
    [SerializeField] private SOSquadDataPreset _debugPrest;
    #endregion

    [ContextMenu("SpawnSquad")]
    private void SpawnTest()
    {
        SpawnSquad(_debugPrest);
    }
    //=============================
}
