using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CBattleUIManager : MonoBehaviour
{
    #region inspector
    [SerializeField] private TMP_Text _CMPtext;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _defeatedPanel;
    [SerializeField] private Transform _buttonLayout;
    [SerializeField] private CUnitSpawnButtonController _spawnButtonPrefab;
    #endregion

    private void Awake()
    {
        if (_CMPtext == null)
        {
            Debug.LogWarning("_CMPtext is not assigned in the inspector.");
            enabled = false;
            return;
        }

        if(_buttonLayout == null)
        {
            Debug.LogWarning("_buttonLayout is not assigned in the inspector.");
            enabled = false;
            return;
        }

        if (_victoryPanel == null)
        {
            Debug.LogWarning("_victoryPanel is not assigned in the inspector.");
        }

        if ( _defeatedPanel == null)
        {
            Debug.LogWarning("_defeatedPanel is not assigned in the inspector.");
        }
    }

    public void UpdateText(float cmp)
    {
        _CMPtext.SetText($"{cmp:0}");
    }


    public void GameOver(bool isVictory)
    {
        Cursor.lockState = CursorLockMode.None;

        if (isVictory)
        {
            _victoryPanel.SetActive(true);
        }

        else
        {
            _defeatedPanel.SetActive(true);
        }
    }

    public void InitializeButtons()
    {
        IReadOnlyList<SOSquadDataPreset> squadDatas = CGameProgressManager.Instance.UnlockSquadList;
        IReadOnlyDictionary<SOSquadDataPreset, List<CSquadData>> squadMap = CGameProgressManager.Instance.SquadMap;

        for (int i = squadDatas.Count -1; i >= 0; i--)
        {
            CUnitSpawnButtonController button = Instantiate(_spawnButtonPrefab, _buttonLayout);
            button.Initialize(squadMap[squadDatas[i]]);
        }
    }
}
