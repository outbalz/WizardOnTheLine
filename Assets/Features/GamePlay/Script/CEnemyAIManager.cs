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


    #region inspector
    [SerializeField] private SOPlayerData _aiPlayer;
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
        
        if(_aiPlayer == null)
        {
            Debug.LogWarning("_aiPlayer is not assigned.");
            enabled = false;
            return;
        }

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
                _spawner.SpawnSquad(_squadPrest[randomIndex], _aiPlayer);
            }

            int nextSec = Random.Range(5, 15);
            await UniTask.WaitForSeconds(nextSec);
        }
        
    }

    private async UniTask TacticsCheck(CancellationToken ct)
    {  
        while (!ct.IsCancellationRequested)
        {
            /*
            System.Array values = System.Enum.GetValues(typeof(EAITactic));
            int randomIndex = Random.Range(0, values.Length);
            EAITactic tactic = (EAITactic)values.GetValue(randomIndex);
            */

            EAITactic tactic = (EAITactic)Random.Range(0, System.Enum.GetValues(typeof(EAITactic)).Length);

            switch (tactic)
            {
                case EAITactic.Idle:
                    Debug.Log("Enemy is doing nothing");
                    break;
                case EAITactic.Defence:
                    Debug.Log("Enemy is taking Defecive postion.");
                    break;
                case EAITactic.FoceRecon:
                    Debug.Log("Enemy is sending few unit forward");
                    break;
                case EAITactic.FullOffence:
                    Debug.Log("Enemy is prepare for Full offence");
                    break;
                default:
                    break;
            }

            int nextSec = Random.Range(20, 50);
            await UniTask.WaitForSeconds(nextSec);
        }
    }
}
