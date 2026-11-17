using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CUnitSelectionController : CSingleton<CUnitSelectionController>
{
    #region inspector
    [SerializeField] private Camera _camera;
    [SerializeField] private SpriteRenderer _selectBox;
    [SerializeField] private LayerMask _selectableLayer;
    [SerializeField] private string _mousePositionActionName = "MousePosition";
    [SerializeField] private string _mouseClickActionName = "MouseClick";
    [SerializeField] private string _unitTag = "Unit";
    [SerializeField] private string _makerTag = "SquadMarker";
    #endregion

    #region private var
    private CInputVectorActionData _mousePositionAction;
    private CInputActionData _mouseClickAction;
    private Vector2 _mouseStartPosition;
    private Vector2 _mousePosition;
    private bool _isBinded = false;

    private List<CSquadContorller> _selectedSquad = new List<CSquadContorller>();
    private HashSet<CSquadContorller> _selectedSquadHash = new HashSet<CSquadContorller>();
    #endregion

    #region property
    public IReadOnlyList<CSquadContorller> SelectedSquad => _selectedSquad;
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

    private void OnClickStarted(InputAction.CallbackContext context)
    {
        if (_mousePositionAction == null)
        {
            Debug.LogError("Mouse position action is not assigned.");
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

    private void OnClickCanceled(InputAction.CallbackContext context)
    {
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


        _selectedSquad.Clear();
        _selectedSquadHash.Clear();

        Collider2D[] selectedColliders;

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

                marker.Squad.SelectSquad(true);
                _selectedSquad.Add(marker.Squad);
                _selectedSquadHash.Add(marker.Squad);

            }
        }

        //Debug.Log($"Selected {_selectedSquad.Count} Squads.");
        return;
    }

    private void OnMousePosPerfomed(InputAction.CallbackContext context)
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
