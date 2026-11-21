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
	#endregion

	#region private var
    private SOPlayerData _playerData;
	private List<CUnitController> _squadMembers = new List<CUnitController>();
    #endregion

    #region property
    public SOPlayerData PlayerData => _playerData;
    public float SquadRadiusX => _squadRadiusX;
    public List<CUnitController> SquadMembers => _squadMembers;
    public CSquadSkillCasterController SkillCaster => _skillCasterController;
    public CModifierHandler SquadModifierHandler => _squadModifierHandler;
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

    public void InitializeSquad(SOSquadDataPreset squadDataPreset, SOPlayerData playerData)
    {
        if (squadDataPreset == null)
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

        _squadMembers.Clear();

        float spawnPointY = CUtil.MAP_HIGHT_MAX - Random.Range(0f, 1.8f);

        for (int i = 0; i < squadDataPreset.SquadSize; i++)
        {
            CUnitController unit = CUnitFactory.Instance.Creat(this, squadDataPreset.UnitDatas[i]);

            float spawnPointX = unit.transform.position.x;
            spawnPointX += Random.Range(-_squadRadiusX, _squadRadiusX);

            unit.transform.position = new Vector3(spawnPointX, spawnPointY, unit.transform.position.z);

            _squadMembers.Add(unit);

            spawnPointY -= Random.Range(0.3f, 0.7f);
        }

        ExceptionCheck();

        _markerController.InitializeMarker(squadDataPreset);

        SetSquadMaxHP();
        SquadHpUpdate();

        _skillCasterController.enabled = true;
        _skillCasterController.InitializeSquadSkill(squadDataPreset);
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
                break;
            case SOderContext.EOderType.Retreat:
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
        CSquadPool.Instance.ReturnToPool(this);

        CGameManager.Instance.SquadListMap[_playerData].Remove(this);

        OnSquadDeath?.Invoke();
        OnSquadDeath = null;
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

