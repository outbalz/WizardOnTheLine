using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CGameManager : CSingleton<CGameManager>
{
	#region inspector
	[SerializeField] private float _CMPperSec;
	[SerializeField] private SOPlayerData _playerData;
	[SerializeField] private SOPlayerData _enemyPlayerData;
	[SerializeField] private CEnemyAIManager _enemyAIManager;

	[Space]
	[SerializeField] private float _difficultyMultiplyer;

	[Header("UI")]
	[SerializeField] private TMP_Text _CMPtext;
	[SerializeField] private GameObject _victoryPanel;
	[SerializeField] private GameObject _defeatedPanel;
	[SerializeField] private Camera _NoPostCam;
	#endregion

	#region private var
	private float _playerCMP = 200f;
	private float _enemyCMP = 200f;
	private float _numHolder = 0;
	private Dictionary<SOPlayerData, List<CSquadContorller>> _squadListMap = new Dictionary<SOPlayerData, List<CSquadContorller>>();
	#endregion

	#region property
	public SOPlayerData PlayerData => _playerData;
	public SOPlayerData EnemyPlayerData => _enemyPlayerData;
	public Dictionary<SOPlayerData, List<CSquadContorller>> SquadListMap => _squadListMap;
	public float PlayerCMP => _playerCMP;
    #endregion

    protected override void Awake()
    {
        if(_CMPtext == null)
		{
			Debug.LogWarning("_CMPtext is not assigned in the inspector.");
			enabled = false;
			return;
		}

		if (_playerData == null || _enemyPlayerData == null)
		{
			Debug.LogWarning("playerData is null");
			enabled = false;
			return;
		}

		_squadListMap.Add(_playerData, new List<CSquadContorller>());
		_squadListMap.Add(_enemyPlayerData, new List<CSquadContorller>());

		base.Awake();
    }

    private void Update()
    {
		_numHolder += _CMPperSec * Time.deltaTime;
		_enemyCMP += _CMPperSec * _difficultyMultiplyer * Time.deltaTime;

		if (_numHolder >= 1)
		{
			float num = Mathf.Floor(_numHolder);

			_numHolder -= num;
			_playerCMP += num;

			UpdateText();
		}
    }

	public bool SpendPlayerCMP(float amount)
	{
		if (_playerCMP < amount)
		{
			return false;
		}

		_playerCMP -= amount;
		UpdateText();

        return true;
	}

	public bool SpendEnemyCMP(float amount)
	{
		if (_enemyCMP < amount)
		{
			return false;
		}
		
		_enemyCMP -= amount;
		return true;
	}

	private void UpdateText()
	{
        _CMPtext.SetText($"{_playerCMP:0}");
    }

	public void GameOver(bool isVictory)
	{
		Time.timeScale = 0;
		_enemyAIManager.enabled = false;
		_NoPostCam.gameObject.SetActive(false);

		if (isVictory)
		{
			_victoryPanel.SetActive(true);
		}

		else
		{
            _defeatedPanel.SetActive(true);
        }
	}
}
