using UnityEngine;
using UnityEngine.UI;

public class CSquadMarkerController : MonoBehaviour
{
	#region inspector
	//[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private Image _image;
	[SerializeField] private Color32 _color;
	[SerializeField] private CSquadContorller _squad;
	[SerializeField] private float _offsetY;
	[SerializeField] private Slider _hpSlider;
	#endregion

	#region private var
	private CUnitController _squadLeader;
	#endregion

	#region property
	public CSquadContorller Squad => _squad;
	#endregion

	private void Awake()
    {
		ExceptionCheck();
    }

    private void ExceptionCheck()
	{
		/*
		if(_renderer == null)
		{
			if(!TryGetComponent<SpriteRenderer>(out _renderer))
			{
                Debug.LogWarning($"{this}/ SpriteRenderer is not assigned and not found on the GameObject {gameObject.name}.");
                enabled = false;
                return;
            }
		}
		*/
		if (_image == null)
		{
			Debug.LogWarning($"{this}/ _image is not assigned.");
			enabled = false;
			return;
		}

		if(_squad == null)
		{
			Debug.LogWarning($"{this}/ SpriteRenderer is not assigned.");
			enabled = false;
			return;
		}

		if( _hpSlider == null)
		{
			Debug.LogWarning($"{this}/ _hpSlider is not assigned.");
			return;
		}
	}

    public void InitializeMarker(SOSquadDataPreset preset)
    {
		enabled = true;
        ExceptionCheck();
		AssignSquadLeader();

        gameObject.layer = _squad.PlayerData.UnitLayer;
		//_renderer.sprite = preset.MarkerSprite;
		_image.sprite = preset.MarkerSprite;
        _color = _squad.PlayerData.PlayerColor;

		_hpSlider.fillRect.GetComponent<Image>().color = _color;
		SelectMarker(false);
    }

    public void AssignSquadLeader()
	{
		if (_squad.SquadMembers.Count <= 0)
		{
			_squad.KillSquad();
			enabled = false;
			return;
		}

		//int ranNum = Random.Range(0, _squad.SquadMembers.Count);

		_squadLeader = _squad.SquadMembers[0];
		enabled = true;
	}

    private void Update()
    {
        if(_squadLeader == null || _squadLeader.gameObject.activeSelf == false)
		{
			AssignSquadLeader();
			return;
		}

		Vector3 leaderPos = _squadLeader.transform.position;
		leaderPos.y += _offsetY;

		transform.position = leaderPos;
    }

	public void SelectMarker(bool isSelected)
	{
		Color32 color = _color;

		if (isSelected)
		{
			//_renderer.color = color;
			_image.color = color;
		}

		else
		{
			color.a = 150;
			//_renderer.color = color;
			_image.color = color;
		}
	}

	public void UpdateHPSlider(float hp)
	{
		_hpSlider.SetValueWithoutNotify(hp);
	}

	public void SetMaxHP( float maxHp)
	{
		_hpSlider.maxValue = maxHp;
	}
}
