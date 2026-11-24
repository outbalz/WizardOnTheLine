using UnityEngine;

public abstract class SONodeTypeData : SOGameData
{
    public enum ENodeType
    {
        Start,
        Battle,
        Safe,
        RandomIncounter,
        Boss
    }

    [SerializeField] private Sprite _nodeSpite;
    [SerializeField] private Sprite _nodeVisitedSpite;
    [SerializeField] private ENodeType _type;

    public ENodeType Type => _type;
    public Sprite NodeSpite => _nodeSpite;
    public Sprite NodeVisitedSprite => _nodeVisitedSpite;

    public abstract void OnNodeClick();
}