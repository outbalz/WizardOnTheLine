using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CMapNodeController : MonoBehaviour
{
    #region inspector
    [SerializeField] private LineRenderer _lineRendererPrefab;
    [SerializeField] private Image _image;
    #endregion

    #region private var
    private List<CMapNodeController> _previousNode = new List<CMapNodeController>();
    private List<CMapNodeController> _nextNode = new List<CMapNodeController>();
    private SONodeTypeData _typeData;
    private SNodeIndex _index; 
    private bool _isVisited = false;
    private bool _isActive = false;
    #endregion


    #region property
    public List<CMapNodeController> PreviousNode => _previousNode;
    public List<CMapNodeController> NextNode => _nextNode;
    public SONodeTypeData TypeData => _typeData;
    public SNodeIndex Index { get { return _index; } set { _index = value; } }
    public bool IsVisited
    { 
        get { return _isVisited; 
    } 
        set 
        { 
            _isVisited = value;

            if (_typeData != null)
            {
                _image.sprite = value ? _typeData.NodeVisitedSprite : _typeData.NodeSpite;
            }
        } 
    }
    public bool IsActive { get { return _isActive; }  set { _isActive = value; } }
    #endregion

    public void AddPrevNode(CMapNodeController prevNode)
    {
        if(prevNode == null || _lineRendererPrefab == null)
        {
            Debug.LogWarning("one of node element is null.");
            return;
        }

        if (prevNode.IsVisited && prevNode.IsActive)
        {
            _isActive = true;
        }

        _previousNode.Add(prevNode);
        
        LineRenderer line = Instantiate<LineRenderer>(_lineRendererPrefab);

        line.positionCount = 2;
        line.SetPosition(0, prevNode.transform.position);
        line.SetPosition(1, transform.position);
    }

    public void SetNodeType(SONodeTypeData data)
    {
        _typeData = data;
        _image.sprite = _typeData.NodeSpite;

        if(data.Type == SONodeTypeData.ENodeType.Start)
        {
            IsVisited = true;
            IsActive = true;
        }
    }

    public void OnClick()
    {

        if (!_isActive)
        {
            return;
        }

        if (_isVisited)
        {
            return;
        }

        if(CGameProgressManager.Instance.MapNodeIndex.Layer != _index.Layer - 1)
        {
            return;
        }

        if(_typeData == null)
        {
            Debug.LogWarning("_typeData is null.");
            return;
        }

        _isVisited = true;

        for (int i = 0; i < _previousNode.Count; i++)
        {
            _previousNode[i].IsActive = false;
        }

        CGameProgressManager.Instance.MapNodeIndex = _index;
        CMapNodeHandler.Instance.SaveMapNodeData();
        _typeData.OnNodeClick();

    }
}
