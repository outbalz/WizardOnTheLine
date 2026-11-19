using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;

public class CMouseCommandController : CSingleton<CMouseCommandController>
{
    public enum EMouseCommadState
    {
        Universal,
        Move,
        AttackMove,
        Skill
    }

    #region inspector
    [SerializeField] private Camera _camera;
    #endregion

    #region private var
    private readonly string _mousePositionActionName = "MousePosition";
    private readonly string _mouseClickActionName = "MouseClick";
    private readonly string _mouseRightClickActionName = "MouseRigtCilck";
    private readonly string _AKeyActionName = "A";

    private EMouseCommadState _commadState = EMouseCommadState.Universal;
    private CInputVectorActionData _mousePositionAction;
    private CInputActionData _mouseClickAction;
    private CInputActionData _mouseRightClickAction;
    private CInputActionData _AKeyAction;
    private bool _isBinded = false;
    #endregion

    #region property
    public EMouseCommadState CommadState => _commadState;
    public event Action<Vector2> OnClickEvent;
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
            enabled = false;
            return;
        }

        if (CInputDispatcher.Instance.InputActionMap.TryGetValue(_mousePositionActionName, out CInputActionData mousePositionAction))
        {
            if (mousePositionAction is CInputVectorActionData vectorAction)
            {
                _mousePositionAction = vectorAction;
            }

            else
            {
                Debug.LogError($"Mouse position action '{_mousePositionActionName}' is not of type CInputVectorActionData.");
                enabled = false;
                return;
            }
        }

        else
        {
            Debug.LogError($"Mouse position action '{_mousePositionActionName}' not found in InputActionMap.");
            enabled = false;
            return;
        }

        if (!(CInputDispatcher.Instance.InputActionMap.TryGetValue(_mouseClickActionName, out _mouseClickAction)))
        {
            Debug.LogError($"Mouse click action '{_mouseClickActionName}' not found in InputActionMap.");
            enabled = false;
            return;
        }

        if (!(CInputDispatcher.Instance.InputActionMap.TryGetValue(_mouseRightClickActionName, out _mouseRightClickAction)))
        {
            Debug.LogError($"Mouse click action '{_mouseRightClickActionName}' not found in InputActionMap.");
            enabled = false;
            return;
        }

        if (!(CInputDispatcher.Instance.InputActionMap.TryGetValue(_AKeyActionName, out _AKeyAction)))
        {
            Debug.LogError($"Mouse click action '{_AKeyAction}' not found in InputActionMap.");
            enabled = false;
            return;
        }

        BindAction();
    }

    private void OnEnable()
    {
        BindAction();
    }

    private void OnDisable()
    {
        UnBindAction();
    }

    protected override void OnDestroy()
    {
        UnBindAction();
        OnClickEvent = null;
        base.OnDestroy();
    }

    private void BindAction()
    {
        if (_isBinded)
        {
            return;
        }

        if(_mouseClickAction == null || _mouseRightClickAction == null)
        {
            return;
        }

        _mouseClickAction.OnPerformedAction += OnLeftClickCommand;
        _mouseRightClickAction.OnPerformedAction += OnRightClickCommand;
        _AKeyAction.OnPerformedAction += OnAKeyPerfomed;

        _isBinded = true;
    }

    private void UnBindAction()
    {
        if(!_isBinded)
        {
            return;
        }

        if(_mouseClickAction == null || _mouseRightClickAction == null)
        {
            return;
        }

        _mouseClickAction.OnPerformedAction -= OnLeftClickCommand;
        _mouseRightClickAction.OnPerformedAction -= OnRightClickCommand;
        _AKeyAction.OnPerformedAction -= OnAKeyPerfomed;

        _isBinded = false;
    }

    private void ChangeCommandState(EMouseCommadState state)
    {
        _commadState = state;
    }

    private void OnLeftClickCommand()
    {
        if(_commadState == EMouseCommadState.Universal)
        {
            OnClickEvent = null;
            return;
        }

        Vector2 mousePosition = _mousePositionAction.Value;
        Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
        worldPosition.z = 0f;

        SOderContext oder = new SOderContext();

        switch (_commadState)
        {
            case EMouseCommadState.Move:
                break;
            case EMouseCommadState.AttackMove:

                oder.oderType = SOderContext.EOderType.AttackMove;
                oder.targetPos = worldPosition;

                GiveOderToSelectedSquad(oder);
                break;
            case EMouseCommadState.Skill:
                break;
            default:
                break;
        }

        OnClickEvent?.Invoke(_mousePositionAction.Value);
    }

    private void OnRightClickCommand()
    {
        if (_commadState != EMouseCommadState.Universal)
        {
            ChangeCommandState(EMouseCommadState.Universal);
            return;
        }

        Vector2 mousePosition = _mousePositionAction.Value;
        Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
        worldPosition.z = 0f;

        SOderContext oder = new SOderContext();

        oder.oderType = SOderContext.EOderType.Move;
        oder.targetPos = worldPosition;

        GiveOderToSelectedSquad(oder);

    }

    private void GiveOderToSelectedSquad(SOderContext oder)
    {
        IReadOnlyList<CSquadContorller> selecedSquad = CUnitSelectionController.Instance.SelectedSquad;

        for (int i = 0; i < selecedSquad.Count; i++)
        {
            selecedSquad[i].GiveOder(oder);
        }
    }

    private void OnAKeyPerfomed()
    {
        _commadState = EMouseCommadState.AttackMove;
    }

    public void SetCommanState(EMouseCommadState state)
    {
        _commadState = state;
    }
}
