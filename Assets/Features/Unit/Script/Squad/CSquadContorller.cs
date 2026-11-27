using System.Collections.Generic;
using UnityEngine;

public struct SOderContext
{
    public enum EOderType
    {
        None,
        Move,
        AttackMove,
        Stop,
        Retreat
    }

    public EOderType oderType;
    public Vector3 targetPos;
}
public class CSquadContorller : MonoBehaviour
{

    #region inspector
    [SerializeField] private CSquadMarkerController _markerController;
    [SerializeField] private float _squadRadiusX;
    [SerializeField] private CSquadSkillCasterController _skillCasterController;
    [SerializeField] private CModifierHandler _squadModifierHandler;
	[SerializeField] private List<CUnitController> _squadMembers = new List<CUnitController>();
    [SerializeField] private bool _usePreAssginedSquad = false;
	#endregion

	#region private var
    private SOPlayerData _playerData;
    private ISquadReferance _squadReferance;
    #endregion

    #region property
    public SOPlayerData PlayerData => _playerData;
    public float SquadRadiusX => _squadRadiusX;
    public List<CUnitController> SquadMembers => _squadMembers;
    public CSquadSkillCasterController SkillCaster => _skillCasterController;
    public CModifierHandler SquadModifierHandler => _squadModifierHandler;
    public ISquadReferance SquadReferance => _squadReferance;
    public bool IsSelectable = true;
    public event System.Action OnSquadDeath;
    #endregion

    private void Awake()
    {
        ExceptionCheck();
    }

    private void ExceptionCheck()
    {
        if( _markerController == null )
        {
            if(!TryGetComponent<CSquadMarkerController>(out _markerController))
            {
                Debug.LogWarning($"{this}/ _markerController is not assigned and not found on the GameObject {gameObject.name}.");
                enabled = false;
                return;
            }
        }

        if( _skillCasterController == null )
        {
            if(!TryGetComponent<CSquadSkillCasterController>(out _skillCasterController))
            {
                Debug.LogWarning($"{this}/ _skillCasterController is not assigned and not found on the GameObject {gameObject.name}.");
                enabled = false;
                return;
            }
        }

        if( _squadModifierHandler == null )
        {
            if(!TryGetComponent<CModifierHandler>(out _squadModifierHandler))
            {
                Debug.LogWarning($"{this}/ _squadModifierHandler is not assigned and not found on the GameObject {gameObject.name}.");
                enabled = false;
                return;
            }
        }
    }

    public void InitializeSquad(ISquadReferance squadData, SOPlayerData playerData)
    {
        if (squadData == null)
        {
            Debug.LogWarning("squadDataPreset is null.");
            return;
        }

        if (playerData == null)
        {
            Debug.LogWarning("playerData is null.");
            enabled = false;
            return;
        }
        _playerData = playerData;

        if (!_usePreAssginedSquad)
        {
            _squadMembers.Clear();
            float spawnPointY = CUtil.MAP_HIGHT_MAX;

            Vector2[] spawnPos = new Vector2[squadData.SquadSize];

            for (int i = 0; i < squadData.SquadSize; i++)
            {
                float spawnPointX = transform.position.x;
                spawnPointX += Random.Range(-_squadRadiusX, _squadRadiusX);
                spawnPointY -= Random.Range(0.3f, 0.7f);

                spawnPos[i] = new Vector2(spawnPointX, spawnPointY);
            }

            float spawnHeigt = spawnPos[0].y - spawnPointY;
            float mapHeight = CUtil.MAP_HIGHT_MAX - CUtil.MAP_HIGHT_MIN;

            float spawnRange = mapHeight - spawnHeigt;
            float spawnPontYOffset = Random.Range(0, spawnRange - 0.7f);

            for (int i = 0; i < squadData.SquadSize; i++)
            {
                spawnPos[i].y -= spawnPontYOffset;
            }


            for (int i = 0; i < squadData.SquadSize; i++)
            {
                CUnitController unit = CUnitFactory.Instance.Creat(this, squadData.UnitDatas[i], spawnPos[i]);

                if (squadData.UnitModifier != null && squadData.UnitModifier.Count > 0)
                {
                    for (int j = 0; j < squadData.UnitModifier.Count; j++)
                    {
                        unit.Modifier.ApplyModifierWithCheck(squadData.UnitModifier[j]);
                        unit.HP.InitializeHP();
                    }
                }

                _squadMembers.Add(unit);

            }
        }

        else
        {
            for(int i = 0;i < _squadMembers.Count; i++)
            {
                CUnitController unit = _squadMembers[i];

                if (squadData.UnitDatas[i] != null)
                {
                    unit.InitializeUnit(this, squadData.UnitDatas[i]);
                }

                else if (squadData.UnitDatas[0] != null)
                {
                    unit.InitializeUnit(this, squadData.UnitDatas[0]);
                }
                else
                {
                    Debug.LogWarning("UnitDatas is null.");
                }

                if (squadData.UnitModifier != null && squadData.UnitModifier.Count > 0)
                {
                    for (int j = 0; j < squadData.UnitModifier.Count; j++)
                    {
                        unit.Modifier.ApplyModifierWithCheck(squadData.UnitModifier[j]);
                        unit.HP.InitializeHP();
                    }
                }

                unit.gameObject.SetActive(true);
            }
        }

        ExceptionCheck();

        _markerController.InitializeMarker(squadData);

        SetSquadMaxHP();
        SquadHpUpdate();

        if(squadData.SquadModifier != null && squadData.SquadModifier.Count > 0)
        {
            for (int i = 0; i < squadData.SquadModifier.Count; i++)
            {
                _squadModifierHandler.ApplyModifierWithCheck(squadData.SquadModifier[i]);
            }
        }

        _skillCasterController.enabled = true;
        _skillCasterController.InitializeSquadSkill(squadData);

        _squadReferance = squadData;
        IsSelectable = true;
    }

    public bool GiveOder(SOderContext context)
    {

        switch (context.oderType)
        {
            case SOderContext.EOderType.Move:
            case SOderContext.EOderType.AttackMove:
                for (int i = 0; i < _squadMembers.Count; i++)
                {
                    _squadMembers[i].StateContoller.GiveOder(context);
                }

                return true;
            case SOderContext.EOderType.Stop:
                for (int i = 0; i < _squadMembers.Count; i++)
                {
                    context.targetPos = _squadMembers[i].transform.position;
                    _squadMembers[i].StateContoller.GiveOder(context);
                }

                break;
            case SOderContext.EOderType.Retreat:
                IsSelectable = false;
                for (int i = 0; i < _squadMembers.Count; i++)
                {
                    context.targetPos = _squadMembers[i].transform.position;
                    context.targetPos.x = CUtil.MAP_HIGHT_MIN;

                    CMultiflyModifier retreatModifier = new CMultiflyModifier("MovementSpeed", 1.5f);

                    _squadMembers[i].Modifier.ApplyModifierWithCheck(retreatModifier);

                    _squadMembers[i].StateContoller.GiveOder(context);
                    _squadMembers[i].SelectUint(false);
                }

                IsSelectable = false;

                SelectSquad(false);
                _markerController.gameObject.SetActive(false);

                break;
            default:
                break;
        }

        return false;
    }

    public void SelectSquad(bool isSelected)
    {
        for (int i = 0; i < _squadMembers.Count; i++)
        {
            _squadMembers[i].SelectUint(isSelected);
        }

        _markerController.SelectMarker(isSelected);
    }

    public void KillSquad()
    {
        _skillCasterController.enabled = false;

        CGameManager.Instance.SquadListMap[_playerData].Remove(this);

        OnSquadDeath?.Invoke();
        OnSquadDeath = null;
        _squadReferance = null;

        if (!_usePreAssginedSquad)
        {
            CSquadPool.Instance.ReturnToPool(this);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void SetSquadMaxHP()
    {
        float maxHP = 0;

        for (int i = 0; i < _squadMembers.Count; i++)
        {
            maxHP += _squadMembers[i].Status.MaxHP;
        }

        _markerController.SetMaxHP(maxHP);
    }

    public void SquadHpUpdate()
    {
        float hp = 0;

        for (int i = 0; i < _squadMembers.Count; i++)
        {
            hp += _squadMembers[i].HP.HP;
        }

        _markerController.UpdateHPSlider(hp);
    }
}

