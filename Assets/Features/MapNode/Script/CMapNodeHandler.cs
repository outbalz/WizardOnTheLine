using System.Collections.Generic;
using UnityEngine;

public class CMapNodeHandler : MonoBehaviour
{
    #region inspector
    [SerializeField] private CMapNodeController _nodePrefab;
    [SerializeField] private CMapNodeLayer[] _layer;
    [SerializeField] private float _spaceing;
    [SerializeField] private Vector2 _startPos;
    [SerializeField] private float _maxHeight;

    [SerializeField] private SNodeIndex _nodeIdex;
    #endregion

    private void Start()
    {
        LoadMapNodeData();
    }

    private void InitializeMap()
    {
        Vector2 layerPos = _startPos;

        for (int i = 0; i < _layer.Length; i++)
        {
            int nodeNum = Random.Range(_layer[i].NodeNumMin, _layer[i].NodeNumMax + 1);
            _layer[i].Node.Clear();

            Vector2[] nodePos = new Vector2[nodeNum];
            float nodeY = _startPos.y + _maxHeight;

            float yDistanceMax = (_maxHeight *2)/nodeNum; 

            for (int j = 0; j < nodeNum; j++)
            {
                nodePos[j].x = Random.Range(layerPos.x, layerPos.x + _layer[i].With);
                nodePos[j].y = nodeY;
                nodeY -= Random.Range(1.5f, yDistanceMax);
            }

            float layerHeight = nodePos[0].y - nodePos[nodePos.Length-1].y;
            float spawnRange = (_maxHeight *2) - layerHeight;
            float yOffset = Random.Range(0f, spawnRange);

            for (int j = 0; j < nodeNum; j++)
            {
                nodePos[j].y -= yOffset;
            }

            if(i == 0 || i == _layer.Length-1)
            {
                nodePos[0].y = _startPos.y + Random.Range(-1, +1);
            }

            for (int j = 0; j < nodeNum; j++)
            {
                CMapNodeController node = Instantiate<CMapNodeController>(_nodePrefab);
                SNodeIndex index = new SNodeIndex();
                index.Stage = _nodeIdex.Stage;
                index.Layer = i;
                index.Node = j;

                node.Index = index;

                node.transform.position = nodePos[j];
                
                if (i > 0)
                {
                    float closistDistance = float.PositiveInfinity;
                    float secondeDistance = float.PositiveInfinity;
                    CMapNodeController prevNode = null;
                    CMapNodeController prevNode2 = null;

                    for (int k = 0; k < _layer[i-1].Node.Count; k++)
                    {  
                       float distance = Mathf.Abs(_layer[i-1].Node[k].transform.position.y -  node.transform.position.y);

                        if(distance < closistDistance)
                        {
                            closistDistance = distance;
                            prevNode = _layer[i-1].Node[k];
                        }

                        else if(distance < secondeDistance) 
                        {
                            if(j != nodeNum -1)
                            {
                                if(_layer[i-1].Node[k].transform.position.y < nodePos[j + 1].y)
                                {
                                    continue;
                                }
                            }

                            secondeDistance = distance;
                            prevNode2 = _layer[i-1].Node[k];
                        }
                    }

                    if(prevNode != null)
                    {
                        node.AddPrevNode(prevNode);
                        prevNode.NextNode.Add(node);
                    }

                    if (prevNode2 != null && Random.Range(0,2) == 0)
                    {
                        node.AddPrevNode(prevNode2);
                        prevNode2.NextNode.Add(node);
                    }
                }

                //TODO 노드타입 확정

                _layer[i].Node.Add(node);
            }

            layerPos.x = layerPos.x + _layer[i].With + _spaceing;
        }

        for (int i = 1; i < _layer.Length -1; i++)
        {
            for (int j = 0; j < _layer[i].Node.Count; j++)
            {
                CMapNodeController node = _layer[i].Node[j];

                if (node.NextNode.Count > 0)
                {
                    continue;
                }

                float closistDistance = float.PositiveInfinity;
                CMapNodeController nextNode = null;

                for (int k = 0; k < _layer[i+1].Node.Count; k++)
                {
                    float distance = Mathf.Abs(_layer[i+1].Node[k].transform.position.y - node.transform.position.y);

                    if (distance < closistDistance)
                    {
                        closistDistance = distance;
                        nextNode = _layer[i+1].Node[k];
                    }
                }

                if (nextNode != null)
                {
                    node.NextNode.Add(nextNode);
                    nextNode.AddPrevNode(node);
                }

            }
        }

        SaveMapNodeData();
    }

    private void LoadMapNodeData()
    {
        List<List<CNodeData>> nodeData = CGameProgressManager.Instance.MapNodeData;
        
        if(nodeData == null)
        {
            InitializeMap();
            return;
        }

        if(_layer.Length != nodeData.Count)
        {
            _layer = new CMapNodeLayer[nodeData.Count];
        }

        for (int i = 0; i < nodeData.Count; i++)
        {
            for (int j = 0; j < nodeData[i].Count; j++)
            {
                CMapNodeController node = Instantiate<CMapNodeController>(_nodePrefab);
                node.transform.position = nodeData[i][j].Pos;

                SNodeIndex index = new SNodeIndex();

                index.Stage = _nodeIdex.Stage;
                index.Layer = i;
                index.Node = j;

                node.Index = index;
                node.IsVisited = nodeData[i][j].IsVisited;

                _layer[i].Node.Add(node);

                for (int k = 0; k < nodeData[i][j].PrevNode.Count; k++)
                {
                    CMapNodeController prevNode = _layer[i-1].Node[nodeData[i][j].PrevNode[k]];

                    node.AddPrevNode(prevNode);
                    prevNode.NextNode.Add(node);
                }
            }
        }
    }

    private void SaveMapNodeData()
    {
        List<List<CNodeData>> nodeDatas = new List<List<CNodeData>>();

        for (int i = 0; i < _layer.Length; i++)
        {
            List<CNodeData> nodes = new List<CNodeData>();

            for (int j = 0; j < _layer[i].Node.Count; j++)
            {
                CNodeData data = new CNodeData(_layer[i].Node[j]);
                nodes.Add(data);
            }

            nodeDatas.Add(nodes);
        }

        CGameProgressManager.Instance.MapNodeData = nodeDatas;
    }
}

[System.Serializable]
public class CMapNodeLayer
{
    [SerializeField] private List<CMapNodeController> _node;
    [SerializeField] private int _nodeNumMin;
    [SerializeField] private int _nodeNumMax;
    [SerializeField] private float _with;

    public List<CMapNodeController> Node => _node;
    public int NodeNumMin => _nodeNumMin;
    public int NodeNumMax => _nodeNumMax;
    public float With => _with;
}

[System.Serializable]
public struct SNodeIndex
{
    public int Stage;
    public int Layer;
    public int Node;
}

public class CNodeData
{
    public Vector2 Pos;
    public int Index;
    public List<int> PrevNode;
    public List<int> NextNode;
    public bool IsVisited;

    public CNodeData(CMapNodeController node)
    {
        Pos = node.transform.position;
        Index = node.Index.Node;

        PrevNode = new List<int>();
        NextNode = new List<int>();

        for (int i = 0; i < node.PreviousNode.Count; i++)
        {
            PrevNode.Add(node.PreviousNode[i].Index.Node);
        }

        for (int i = 0; i < node.NextNode.Count; i++)
        {
            NextNode.Add(node.NextNode[i].Index.Node);
        }
    }
}