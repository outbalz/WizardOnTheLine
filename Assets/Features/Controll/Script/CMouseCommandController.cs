using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CMouseCommandController : MonoBehaviour
{
    public enum EMouseConnadState
    {
        Universal,
        Move,
        AttackMove,
        Skill
    }

    #region inspector
    [SerializeField] private Camera _camera;
    [SerializeField] private string _mousePositionActionName = "MousePosition";
    [SerializeField] private string _mouseClickActionName = "MouseClick";
    [SerializeField] private string _mouseRightClickActionName = "MouseRigtCilck";
    #endregion

    #region private var
    private EMouseConnadState _connadState = EMouseConnadState.Universal;
    private CInputVectorActionData _mousePositionAction;
    private CInputActionData _mouseClickAction;
    private CInputActionData _mouseRightClickAction;
    private bool _isBinded = false;
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

    private void OnDestroy()
    {
        UnBindAction();
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
    }

    private void ChangeCommandState(EMouseConnadState state)
    {
        _connadState = state;
    }

    private void OnLeftClickCommand(InputAction.CallbackContext context)
    {
        switch (_connadState)
        {
            case EMouseConnadState.Universal:
                return;
            case EMouseConnadState.Move:
                break;
            case EMouseConnadState.AttackMove:
                break;
            case EMouseConnadState.Skill:
                break;
            default:
                break;
        }
    }

    private void OnRightClickCommand(InputAction.CallbackContext context)
    {
        if (_connadState != EMouseConnadState.Universal)
        {
            ChangeCommandState(EMouseConnadState.Universal);
            return;
        }

        IReadOnlyList<CUnitController> selectedUnit = CUnitSelectionController.Instance.SelectedUnits;
        Vector2 mousePosition = _mousePositionAction.Value;
        Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
        worldPosition.z = 0f;

        for (int i = 0; i < selectedUnit.Count; i++)
        {
            CUnitController unit = selectedUnit[i];

            if (unit == null)
            {
                continue;
            }

            unit.Movement.GiveMoveOder(worldPosition.x);
        }
    }

}
