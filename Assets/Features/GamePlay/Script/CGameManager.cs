using TMPro;
using UnityEngine;

public class CGameManager : CSingleton<CGameManager>
{
	#region inspector
	[SerializeField] private float _CMPperSec;
	[SerializeField] private SOPlayerData _playerData;
	[SerializeField] private SOPlayerData _enemyPlayerData;

	[Header("UI")]
	[SerializeField] private TMP_Text _CMPtext;
	#endregion

	#region private var
	private float _playerCMP = 200f;
	private float _enemyCMP = 200f;
    #endregion

    private void Start()
    {
        if(_CMPtext == null)
		{
			Debug.LogWarning("_CMPtext is not assigned in the inspector.");
			enabled = false;
			return;
		}
    }

    private void Update()
    {
		_playerCMP += _CMPperSec * Time.deltaTime;
		_enemyCMP += _CMPperSec * Time.deltaTime;

		_CMPtext.SetText($"{_playerCMP:0}");
    }

	public bool SpendPlayerCMP(float amount)
	{
		if (_playerCMP < amount)
		{
			return false;
		}

		_playerCMP -= amount;
		return true;
	}
}
