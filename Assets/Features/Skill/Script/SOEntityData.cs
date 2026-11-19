using UnityEngine;

[CreateAssetMenu(fileName = "EntityDataSO_", menuName = "Scriptable Objects/EntityDataSO")]
public class SOEntityData : ScriptableObject
{
    #region inspector
    [SerializeField] private CSkillEntityController _prefab;

    [Header("EffectAreaSize")]
    [SerializeField] private float _height;
    [SerializeField] private float _width;

    [Space]
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _effectTime;

    [SerializeField] private bool _isEffectContinuous;

    [Space]
    [SerializeReference, SubclassSelector] private CEntityEffect _effect;
    #endregion

    #region property
    public float Height => _height;
    public float Width => _width;
    public float LifeTime => _lifeTime;
    public float EffectTime => _effectTime;
    public bool IsEffectContinuous => _isEffectContinuous;
    public CEntityEffect Effect => _effect;
    #endregion

    public CSkillEntityController GetFromPool()
    {
        return CEntityPool.Instance.GetFromPool(_prefab);
    }

    public void ReturnToPool(CSkillEntityController entity)
    {
        CEntityPool.Instance.ReturnToPool(entity, _prefab);
    }
}

[System.Serializable]
public abstract class CEntityEffect
{
    public abstract void ApplyEffect(CSkillEntityController entity);
}