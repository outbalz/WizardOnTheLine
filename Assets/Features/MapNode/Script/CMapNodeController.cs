using System.Collections.Generic;
using UnityEngine;

public class CMapNodeController : MonoBehaviour
{
    #region inspector
    [SerializeField] private LineRenderer _lineRendererPrefab;
    #endregion

    #region private var
    private List<CMapNodeController> _previousNode = new List<CMapNodeController>();
    private List<CMapNodeController> _nextNode = new List<CMapNodeController>(); 
    private SNodeIndex _index; 
    private bool _isVisited = false;
    #endregion


    #region property
    public List<CMapNodeController> PreviousNode => _previousNode;
    public List<CMapNodeController> NextNode => _nextNode;
    public SNodeIndex Index { get { return _index; } set { value = _index; } }
    public bool IsVisited { get { return _isVisited; } set { value = _isVisited; } }
    #endregion

    public void AddPrevNode(CMapNodeController prevNode)
    {
        if(prevNode == null || _lineRendererPrefab == null)
        {
            Debug.LogWarning("one of node element is null.");
            return;
        }

        _previousNode.Add(prevNode);
        
        LineRenderer line = Instantiate<LineRenderer>(_lineRendererPrefab);

        line.positionCount = 2;
        line.SetPosition(0, prevNode.transform.position);
        line.SetPosition(1, transform.position);
    }
}
