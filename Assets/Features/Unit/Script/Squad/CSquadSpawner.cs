using UnityEngine;

public class CSquadSpawner : MonoBehaviour
{
    #region inspector
    [SerializeField] private SOPlayerData _playerData;
    [SerializeField] private float _moveOderOffset;
    #endregion


    public void SpawnSquad(ISquadReferance data, SOPlayerData playerData)
    {
        CSquadContorller squad = CSquadFactory.Instance.Creat(transform.position, data, playerData);

        SOderContext moveOderContext = new SOderContext();
        moveOderContext.oderType = SOderContext.EOderType.AttackMove;
        moveOderContext.targetPos = transform.position;
        moveOderContext.targetPos.x += _moveOderOffset;

        squad.GiveOder(moveOderContext);

        CGameManager.Instance.SquadListMap[playerData].Add(squad);
    }

    //for test====================
    #region debug
    [SerializeField] private SOSquadDataPreset _debugPrest;
    #endregion

    [ContextMenu("SpawnSquad")]
    public void SpawnTest()
    {
        SpawnSquad(_debugPrest, _playerData);
    }
    //=============================
}
