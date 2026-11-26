using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CUnitSelectionController : CSingleton<CUnitSelectionController>
{
    #region inspector
    [SerializeField] private Camera _camera;
    [SerializeField] private SpriteRenderer _selectBox;
    [SerializeField] private LayerMask _selectableLayer;
    [SerializeField] private CMouseCommandController _mouseCommand;
    [SerializeField] private float _doubleClickInterval = 0.4f;
    #endregion

    #region private var
    private readonly string _mousePositionActionName = "MousePosition";
    private readonly string _mouseClickActionName = "MouseClick";
    private readonly string _unitTag = "Unit";
    private readonly string _makerTag = "SquadMarker";

    private CInputVectorActionData _mousePositionAction;
    private CInputActionData _mouseClickAction;
    private Vector2 _mouseStartPosition;
    private Vector2 _mousePosition;
    private bool _isBinded = false;

    private List<CSquadContorller> _selectedSquad = new List<CSquadContorller>();
    private HashSet<CSquadContorller> _selectedSquadHash = new HashSet<CSquadContorller>();
    private float _lastClickTime;
    #endregion

    #region property
    public IReadOnlyList<CSquadContorller> SelectedSquad => _selectedSquad;
    public bool IsMouseOverUi = false;
    #endregion

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        if (_camera == null)
        {
            Debug.LogError("Camera is not assigned in the inspector and no main camera found.");
        }

        if (_selectBox == null)
        {
            Debug.LogError("SelectBox is not assigned in the inspector.");
        }

        if (_selectBox != null)
        {
            _selectBox.gameObject.SetActive(false);
        }
        
        if(_mouseCommand == null)
        {
            if(!TryGetComponent<CMouseCommandController>(out _mouseCommand))
            {
                Debug.LogWarning($"{this}/ _mouseCommand is not assigned and not found on the GameObject {gameObject.name}");
                enabled = false;
                return;
            }
        }

        if(CInputDispatcher.Instance.InputActionMap.TryGetValue(_mousePositionActionName, out CInputActionData mousePositionAction))
        {
            if(mousePositionAction is CInputVectorActionData vectorAction)
            {
                _mousePositionAction = vectorAction;
            }

            else
            {
                Debug.LogError($"Mouse position action '{_mousePositionActionName}' is not of type CInputVectorActionData.");
            }
        }

        else
        {
            Debug.LogError($"Mouse position action '{_mousePositionActionName}' not found in InputActionMap.");
        }

        if(CInputDispatcher.Instance.InputActionMap.TryGetValue(_mouseClickActionName, out _mouseClickAction))
        {
            BindAction();
        }

        else
        {
            Debug.LogError($"Mouse click action '{_mouseClickActionName}' not found in InputActionMap.");
        }
    }

    private void OnEnable()
    {
        BindAction();
    }

    private void OnDisable()
    {
        UnbindAction();
    }

    protected override void OnDestroy()
    {
        UnbindAction();
        base.OnDestroy();
    }

    private void BindAction()
    {
        if(_isBinded)
        {
            return;
        }

        if (_mouseClickAction == null)
        {
            return;
        }

        _mouseClickAction.OnStartedAction += OnClickStarted;
        _mouseClickAction.OnCanceledAction += OnClickCanceled;

        _isBinded = true;
    }

    private void UnbindAction()
    {
        if (!_isBinded)
        {
            return;
        }

        if (_mouseClickAction == null)
        {
            Debug.LogError("Mouse click action is not assigned.");
            return;
        }
        
        _mouseClickAction.OnStartedAction -= OnClickStarted;
        _mouseClickAction.OnCanceledAction -= OnClickCanceled;

        _isBinded = false;
    }

    private void OnClickStarted()
    {
        if (_mousePositionAction == null)
        {
            Debug.LogError("Mouse position action is not assigned.");
            return;
        }

        if(_mouseCommand.CommadState != CMouseCommandController.EMouseCommadState.Universal)
        {
            return;
        }

        if (IsMouseOverUi)
        {
            return;
        }

        _mousePositionAction.OnPerformedAction += OnMousePosPerfomed;
        
        Vector2 mousePosition = _mousePositionAction.Value;
        Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
        worldPosition.z = 0f;

        _selectBox.gameObject.SetActive(true);
        _selectBox.transform.position = worldPosition;
        _selectBox.size = Vector2.zero;

        _mouseStartPosition = worldPosition;
    }

    private void OnClickCanceled()
    {
        if (_mouseCommand.CommadState != CMouseCommandController.EMouseCommadState.Universal)
        {
            _mouseCommand.SetCommanState(CMouseCommandController.EMouseCommadState.Universal);
            return;
        }

        if (IsMouseOverUi && !_selectBox.gameObject.activeSelf)
        {
            return;
        }
        float selectionSize = _selectBox.size.magnitude;

        _selectBox.gameObject.SetActive(false);
        _selectBox.size = Vector2.zero;

        if (_mousePositionAction == null)
        {
            Debug.LogError("Mouse position action is not assigned.");
            return;
        }

        _mousePositionAction.OnPerformedAction -= OnMousePosPerfomed;

        for (int i = 0; i < _selectedSquad.Count; i++)
        {
            _selectedSquad[i].SelectSquad(false);
        }

        Collider2D[] selectedColliders;

        if(Time.time - _lastClickTime < _doubleClickInterval)
        {
            if(_selectedSquad.Count <= 0)
            {
                return;
            }

            ISquadReferance referance = _selectedSquad[0].SquadReferance;

            Vector2 pointA = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector2 pointB = _camera.ViewportToWorldPoint(new Vector3(1, 1, 0));

            selectedColliders = Physics2D.OverlapAreaAll(pointA, pointB, _selectableLayer);

            _selectedSquad.Clear();
            _selectedSquadHash.Clear();

            for (int i = 0; i < selectedColliders.Length; i++)
            {
                if (selectedColliders[i].CompareTag(_unitTag))
                {
                    CUnitController unit = selectedColliders[i].GetComponent<CUnitController>();

                    if (unit == null)
                    {
                        continue;
                    }

                    if (_selectedSquadHash.Contains(unit.Squad))
                    {
                        continue;
                    }

                    if (!unit.Squad.IsSelectable)
                    {
                        continue;
                    }

                    if(unit.Squad.SquadReferance != referance)
                    {
                        continue;
                    }

                    unit.Squad.SelectSquad(true);
                    _selectedSquad.Add(unit.Squad);
                    _selectedSquadHash.Add(unit.Squad);
                }

                else if (selectedColliders[i].CompareTag(_makerTag))
                {
                    CSquadMarkerController marker = selectedColliders[i].GetComponent<CSquadMarkerController>();

                    if (marker == null)
                    {
                        continue;
                    }

                    if (_selectedSquadHash.Contains(marker.Squad))
                    {
                        continue;
                    }

                    if (!marker.Squad.IsSelectable)
                    {
                        continue;
                    }

                    if(marker.Squad.SquadReferance != referance)
                    {
                        continue;
                    }

                    marker.Squad.SelectSquad(true);
                    _selectedSquad.Add(marker.Squad);
                    _selectedSquadHash.Add(marker.Squad);

                }
            }

            return;
        }


        _lastClickTime = Time.time;

        _selectedSquad.Clear();
        _selectedSquadHash.Clear();


        if (selectionSize < 0.1f)
        {
            selectedColliders = Physics2D.OverlapPointAll(_mouseStartPosition, _selectableLayer);

            float minY = float.MaxValue;
            CSquadContorller minYSquad = null;
            bool isSquadMarkerSelected = false;

            for (int i = 0; i < selectedColliders.Length; i++)
            {

                if (selectedColliders[i] == null)
                {
                    continue;
                }

                if (selectedColliders[i].transform.position.y < minY)
                {
                    if (selectedColliders[i].CompareTag(_makerTag))
                    {
                        CSquadMarkerController marker = selectedColliders[i].GetComponent<CSquadMarkerController>();

                        if (marker != null)
                        {
                            if (!marker.Squad.IsSelectable)
                            {
                                continue;
                            }
                            minY = marker.transform.position.y;
                            minYSquad = marker.Squad;
                            isSquadMarkerSelected = true;
                        }
                    }


                }
            }

            if (!isSquadMarkerSelected)
            {
                for (int i = 0; i < selectedColliders.Length; i++)
                {

                    if (selectedColliders[i] == null)
                    {
                        continue;
                    }

                    if (selectedColliders[i].transform.position.y < minY)
                    {
                        if (selectedColliders[i].CompareTag(_unitTag))
                        {
                            CUnitController unit = selectedColliders[i].GetComponent<CUnitController>();

                            if (unit != null)
                            {
                                if (!unit.Squad.IsSelectable) 
                                { 
                                    continue; 
                                }
                                minY = unit.transform.position.y;
                                minYSquad = unit.Squad;
                            }
                        }

                    }
                }
            }

            if (minYSquad != null)
            {
                minYSquad.SelectSquad(true);
                _selectedSquad.Add(minYSquad);
                _selectedSquadHash.Add(minYSquad);
            }

            //Debug.Log($"Selected {_selectedSquad.Count} Squads.");
            return;
        }

        selectedColliders = Physics2D.OverlapAreaAll(_mouseStartPosition, _mousePosition, _selectableLayer);
        
        for (int i = 0; i < selectedColliders.Length; i++)
        {
            if (selectedColliders[i].CompareTag(_unitTag))
            {
                CUnitController unit = selectedColliders[i].GetComponent<CUnitController>();

                if (unit == null)
                {
                    continue;
                }

                if(_selectedSquadHash.Contains(unit.Squad))
                {
                    continue;
                }

                if (!unit.Squad.IsSelectable)
                {
                    continue;
                }

                unit.Squad.SelectSquad(true);
                _selectedSquad.Add(unit.Squad);
                _selectedSquadHash.Add(unit.Squad);
            }

            else if(selectedColliders[i].CompareTag(_makerTag))
            {
                CSquadMarkerController marker = selectedColliders[i].GetComponent<CSquadMarkerController>();

                if(marker == null)
                {
                    continue;
                }

                if(_selectedSquadHash.Contains(marker.Squad))
                {
                    continue;
                }

                if (!marker.Squad.IsSelectable)
                {
                    continue;
                }

                marker.Squad.SelectSquad(true);
                _selectedSquad.Add(marker.Squad);
                _selectedSquadHash.Add(marker.Squad);

            }
        }

        //Debug.Log($"Selected {_selectedSquad.Count} Squads.");
        return;
    }

    private void OnMousePosPerfomed()
    {
        Vector2 mousePosition = _mousePositionAction.Value;
        Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
        worldPosition.z = 0f;

        _mousePosition = worldPosition;
        
        Vector2 loweLeft = new Vector2(Mathf.Min(_mouseStartPosition.x, _mousePosition.x), Mathf.Min(_mouseStartPosition.y, _mousePosition.y));
        Vector2 upperRight = new Vector2(Mathf.Max(_mouseStartPosition.x, _mousePosition.x), Mathf.Max(_mouseStartPosition.y, _mousePosition.y));

        Vector2 size = upperRight - loweLeft;

        _selectBox.size = size;

        Vector2 center = loweLeft + size / 2f;
        _selectBox.transform.position = center;
    }

}
