using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CEnemyAIManager : MonoBehaviour
{
    private enum EAITactic
    {
        Idle,
        Defence,
        FoceRecon,
        FullOffence,
    }

    #region const
    private const float MAP_WITH_MIN = -10f;
    private const float MAP_WITH_MAX = 50f;
    #endregion

    #region inspector
    //[SerializeField] private SOPlayerData _aiPlayer;
    [SerializeField] private List<SOSquadDataPreset> _squadPrest;
    [SerializeField] private CSquadSpawner _spawner;
    #endregion

    #region private var
    private CancellationTokenSource _SpawnCTS;
    private CancellationTokenSource _tacticCTS;
    #endregion


    private void Start()
    {
        ExceptionCheck();

        _SpawnCTS = new CancellationTokenSource();
        SquadSpawnCheck(_SpawnCTS.Token).Forget();

        _tacticCTS = new CancellationTokenSource();
        TacticsCheck(_tacticCTS.Token).Forget();
    }

    private void OnDisable()
    {
        if (_SpawnCTS != null)
        {
            _SpawnCTS.Cancel();
            _SpawnCTS.Dispose();
            _SpawnCTS = null;
        }

        if(_tacticCTS != null)
        {
            _tacticCTS.Cancel();
            _tacticCTS.Dispose();
            _tacticCTS = null;
        }
    }

    private void OnDestroy()
    {
        if (_SpawnCTS != null)
        {
            _SpawnCTS.Cancel();
            _SpawnCTS.Dispose();
            _SpawnCTS = null;
        }

        if(_tacticCTS != null)
        {
            _tacticCTS.Cancel();
            _tacticCTS.Dispose();
            _tacticCTS = null;
        }        
    }

    private void ExceptionCheck()
    {
        if(_squadPrest.Count == 0)
        {
            Debug.LogWarning("_squadPrest List is empty.");
            enabled = false;
            return;
        }
        /*
        if(_aiPlayer == null)
        {
            Debug.LogWarning("_aiPlayer is not assigned.");
            enabled = false;
            return;
        }
        */
        if (_spawner == null)
        {
            Debug.LogWarning("_spawner is not assigned.");
            enabled = false;
            return;
        }
    }

    private async UniTask SquadSpawnCheck(CancellationToken ct)
    {

        while (!ct.IsCancellationRequested)
        {
            int randomIndex = Random.Range(0, _squadPrest.Count);

            if(CGameManager.Instance.SpendEnemyCMP(_squadPrest[randomIndex].Cost))
            {
                _spawner.SpawnSquad(_squadPrest[randomIndex], CGameManager.Instance.EnemyPlayerData);
            }

            int nextSec = Random.Range(5, 15);
            await UniTask.WaitForSeconds(nextSec);
        }
        
    }

    private async UniTask TacticsCheck(CancellationToken ct)
    {
        await UniTask.WaitForSeconds(20);

        while (!ct.IsCancellationRequested)
        {
            /*
            System.Array values = System.Enum.GetValues(typeof(EAITactic));
            int randomIndex = Random.Range(0, values.Length);
            EAITactic tactic = (EAITactic)values.GetValue(randomIndex);
            */

            EAITactic tactic = (EAITactic)Random.Range(0, System.Enum.GetValues(typeof(EAITactic)).Length);

            IReadOnlyList<CSquadContorller> squads = CGameManager.Instance.SquadListMap[CGameManager.Instance.EnemyPlayerData];

            float frontLinPos = MAP_WITH_MIN;
            float fallbackPos = MAP_WITH_MAX;
            SOderContext moveOder = new SOderContext();

            int nextSec = Random.Range(10, 31);

            if(squads.Count <= 0)
            {
                await UniTask.WaitForSeconds(nextSec);
                continue;
            }

            switch (tactic)
            {
                case EAITactic.Idle:
                    Debug.Log("Enemy is doing nothing");
                    break;
                case EAITactic.Defence:
                    Debug.Log("Enemy is taking Defecive postion.");

                    frontLinPos = FindFrontLine();

                    if(fallbackPos - 15f > frontLinPos)
                    {
                        fallbackPos = fallbackPos - 15f;
                        frontLinPos = frontLinPos + 15f;
                    }

                    for (int i = 0; i < squads.Count; i++)
                    {
                        float defencsPos = Random.Range(frontLinPos, fallbackPos);

                        moveOder.oderType = SOderContext.EOderType.Move;
                        moveOder.targetPos = _spawner.transform.position;
                        moveOder.targetPos.x = defencsPos;

                        squads[i].GiveOder(moveOder);
                    }


                    break;
                case EAITactic.FoceRecon:
                    Debug.Log("Enemy is sending few unit forward");

                    frontLinPos = FindFrontLine();

                    int randomIndex = Random.Range(0, squads.Count);

                    moveOder.oderType = SOderContext.EOderType.AttackMove;
                    moveOder.targetPos = _spawner.transform.position;
                    moveOder.targetPos.x = frontLinPos;

                    CSquadContorller reconSquad = squads[randomIndex];

                    reconSquad.GiveOder(moveOder);
                    
                    int randomIndex2 =  -1;

                    CSquadContorller reconSquad2 = null;

                    if(squads.Count > 4)
                    {
                        randomIndex2 =  Random.Range(0, squads.Count);

                        while(randomIndex == randomIndex2)
                        {
                            randomIndex2 =  Random.Range(0, squads.Count);
                        }
                        
                        reconSquad2 = squads[randomIndex2];

                        reconSquad2.GiveOder(moveOder);
                    }

                    int reconTime = Random.Range(10, 20);

                    await UniTask.WaitForSeconds(reconTime);

                    moveOder.oderType = SOderContext.EOderType.Move;

                    if (fallbackPos - 15f > frontLinPos)
                    {
                        fallbackPos = fallbackPos - 15f;
                        frontLinPos = frontLinPos + 15f;
                    }

                    moveOder.targetPos.x = Random.Range(frontLinPos, fallbackPos);

                    if (CGameManager.Instance.SquadListMap[CGameManager.Instance.EnemyPlayerData].Contains(reconSquad))
                    {
                        reconSquad.GiveOder(moveOder);
                    }

                    if(reconSquad2 != null && CGameManager.Instance.SquadListMap[CGameManager.Instance.EnemyPlayerData].Contains(reconSquad2))
                    {
                        reconSquad2.GiveOder(moveOder);
                    }

                    break;
                case EAITactic.FullOffence:
                    Debug.Log("Enemy is prepare for Full offence");

                    for (int i = 0; i < squads.Count; i++)
                    {
                        moveOder.oderType = SOderContext.EOderType.AttackMove;
                        moveOder.targetPos = _spawner.transform.position;
                        moveOder.targetPos.x = MAP_WITH_MIN;

                        squads[i].GiveOder(moveOder);
                    }

                    break;
                default:
                    break;
            }

            await UniTask.WaitForSeconds(nextSec);
        }
    }

    private float FindFrontLine()
    {
        float frontLinePos = MAP_WITH_MIN;

        IReadOnlyList<CSquadContorller> playerSquads = CGameManager.Instance.SquadListMap[CGameManager.Instance.PlayerData];

        if (playerSquads == null || playerSquads.Count <= 0)
        {
            return frontLinePos;
        }

        for (int i = 0; i < playerSquads.Count; i++)
        {
            if (playerSquads[i].SquadMembers[0].transform.position.x > frontLinePos)
            {
                frontLinePos = playerSquads[i].SquadMembers[0].transform.position.x;
            }
        }

        return frontLinePos;
    }
}
