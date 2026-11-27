using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CBackgroundLayer
{
    [SerializeField, Range(0f, 1.5f)] private float _speedMutiplier;
    [SerializeField] private List<Transform> _segments = new List<Transform>();

    public float HalfWitdth = 0f;
    public float SegmentWitdth = 0f;

    public List<Transform> Segments => _segments;
    public float SpeedMutiplier => _speedMutiplier;
}

public class CBackgroundController : MonoBehaviour
{

    #region inspector
    [SerializeField] private Camera _cam;

    [SerializeField] private List<CBackgroundLayer> _backgroundLayers = new List<CBackgroundLayer>();

    [SerializeField] private float _extraBuffer = 1.0f;
    [SerializeField] private float _overlap = 0.05f;
    #endregion

    private void Awake()
    {

        if (_cam == null && Camera.main != null)
        {
            _cam = Camera.main;
        }
         
        if(_cam == null)
        {
            Debug.LogWarning("carmera is not assigned in the inspector and no main camera found.");
            enabled = false;
            return;
        }


        if (_backgroundLayers == null || _backgroundLayers.Count == 0)
        {
            Debug.LogWarning("backgroundLayer is not assigned.");
            enabled = false;
            return;
        }

        for (int i = 0; i < _backgroundLayers.Count; i++)
        {
            _backgroundLayers[i].SegmentWitdth = MeasureSegmenWidth(_backgroundLayers[i].Segments[0]);
            _backgroundLayers[i].HalfWitdth = _backgroundLayers[i].SegmentWitdth * 0.5f;

        }

        if (_overlap < 0f)
        {
            _overlap = 0f;
        }

    }

    private float MeasureSegmenWidth(Transform seg)
    {
        SpriteRenderer sr = seg.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            return sr.bounds.size.x;
        }

        Debug.LogWarning("SpriteRenderer is not assigned.");
        return 0f;
    }

    private float GetCamLeftEdgeX()
    {

        float z = Mathf.Abs(_cam.transform.position.z - transform.position.z);

        Vector3 w = _cam.ViewportToWorldPoint(new(0f, 0.5f, z));

        return w.x;

    }

    private float GetCamRightEdgeX()
    {

        float z = Mathf.Abs(_cam.transform.position.z - transform.position.z);

        Vector3 w = _cam.ViewportToWorldPoint(new(1f, 0.5f, z));

        return w.x;

    }

    private float GetRightMostEdgeX(CBackgroundLayer backgroundLayer)
    {
        float rightMost = float.NegativeInfinity;
        for (int i = 0; i < backgroundLayer.Segments.Count; i++)
        {
            float rightEdge = backgroundLayer.Segments[i].position.x + backgroundLayer.HalfWitdth;

            if (rightMost < rightEdge)
            {
                rightMost = rightEdge;
            }
        }

        return rightMost;
    }

    private float GetLeftMostEdgeX(CBackgroundLayer backgroundLayer)
    {
        float leftMost = float.PositiveInfinity;
        for (int i = 0; i < backgroundLayer.Segments.Count; i++)
        {
            float leftEdge = backgroundLayer.Segments[i].position.x - backgroundLayer.HalfWitdth;

            if (leftMost > leftEdge)
            {
                leftMost = leftEdge;
            }
        }

        return leftMost;
    }

    private void UpdateLoop(float moveAmount)
    {
        float camLeftX = GetCamLeftEdgeX();
        float camRightX = GetCamRightEdgeX();

        float LeftLimit = camLeftX - _extraBuffer;
        float RightLimit = camRightX + _extraBuffer;

        for (int i = 0; i < _backgroundLayers.Count; i++)
        {

            float relativeMoveAount = moveAmount * (_backgroundLayers[i].SpeedMutiplier -1);

            if (Mathf.Abs(moveAmount) < 0.00001f)
            {
                continue;
            }

            for (int j = 0; j < _backgroundLayers[i].Segments.Count; j++)
            {
                Transform seg = _backgroundLayers[i].Segments[j];

                float segRightEdgeX = seg.position.x + _backgroundLayers[i].HalfWitdth;
                float segLeftEdgeX = seg.position.x - _backgroundLayers[i].HalfWitdth;

                if (relativeMoveAount < 0 && segRightEdgeX < LeftLimit)
                {
                    float rightMosEdgeX = GetRightMostEdgeX(_backgroundLayers[i]);
                    Vector3 p = seg.position;

                    p.x = rightMosEdgeX + _backgroundLayers[i].HalfWitdth - _overlap;

                    seg.position = p;

                    rightMosEdgeX = p.x + _backgroundLayers[i].HalfWitdth;
                    //leftMostEdgeX = GetLeftMostEdgeX(_backgroundLayers[i]);
                }

                if (relativeMoveAount > 0 && segLeftEdgeX > RightLimit)
                {
                    float leftMostEdgeX = GetLeftMostEdgeX(_backgroundLayers[i]);
                    Vector3 p = seg.position;

                    p.x = leftMostEdgeX - _backgroundLayers[i].HalfWitdth + _overlap;

                    seg.position = p;

                    leftMostEdgeX = p.x - _backgroundLayers[i].HalfWitdth;
                }
            }
        }

    }

    public void OnCameraMove(float amount)
    {

        for (int i = 0; i < _backgroundLayers.Count; i++)
        {

            for (int j = 0; j < _backgroundLayers[i].Segments.Count; j++)
            {
                Vector3 p = _backgroundLayers[i].Segments[j].position;

                p.x += amount * _backgroundLayers[i].SpeedMutiplier;

                _backgroundLayers[i].Segments[j].position = p;
            }
        }

        UpdateLoop(amount);
    }
}
