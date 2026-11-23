using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CUnitSpawnButtonController : MonoBehaviour
{
	#region inspector
	[SerializeField] private CSquadSpawner _spawner;
	[SerializeField] private SOPlayerData _playerData;
	[SerializeField] private SOSquadDataPreset _preset;

	[SerializeField] private List<CSquadData> _data;
	[SerializeField] private int _index = 0;
	[SerializeField] private Dropdown _dropdown;
	[SerializeField] private Image _potrait;
	[SerializeField] private TMP_Text _costText;
    #endregion

    public void Initialize(List<CSquadData> squadDatas)
    {
        _data = squadDatas;
		_spawner = CGameManager.Instance.Spawner;

		if(_dropdown == null)
		{
			Debug.LogWarning("_dropdown is not assigned.");
			return;
		}

		_dropdown.options.Clear();

		for (int i = 0; i < squadDatas.Count; i++)
		{
			Dropdown.OptionData option = new Dropdown.OptionData(squadDatas[i].MarkerSprite);
			_dropdown.options.Add(option);
		}

		OnDropDownChanged(0);
    }

    public void OnButtonClicked()
	{
		if (!CGameManager.Instance.SpendPlayerCMP(_data[_index].Cost))
		{
			return;
		}

		_spawner.SpawnSquad(_data[_index], _playerData);
	}

	public void OnDropDownChanged(int index)
	{
		if (index > _data.Count)
		{
			Debug.LogWarning("out of index range.");
			return;
		}

		_index = index;

		_potrait.sprite = _data[index].PotraitSprite;
		_costText.SetText($"{_data[index].Cost}");
	}
}
