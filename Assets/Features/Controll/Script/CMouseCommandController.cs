using UnityEngine;

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
    [SerializeField] private string _mousePositionActionName = "MousePosition";
    [SerializeField] private string _mouseClickActionName = "MouseClick";
    [SerializeField] private string _mouseRightClickActionName = "MouseRigtCilck";
    #endregion

    #region private var
    private EMouseConnadState _connadState = EMouseConnadState.Universal;
    private CInputVectorActionData _mousePositionAction;
    private CInputActionData _mouseClickAction;
    private CInputActionData _mouseRightClickAction;
    #endregion

    private void Start()
    {

        if (CInputDispatcher.Instance.InputActionMap.TryGetValue(_mousePositionActionName, out CInputActionData mousePositionAction))
        {
            if (mousePositionAction is CInputVectorActionData vectorAction)
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

        if (!(CInputDispatcher.Instance.InputActionMap.TryGetValue(_mouseClickActionName, out _mouseClickAction)))
        {
            Debug.LogError($"Mouse click action '{_mouseClickActionName}' not found.");
        }

        if (!(CInputDispatcher.Instance.InputActionMap.TryGetValue(_mouseRightClickActionName, out _mouseRightClickAction)))
        {
            Debug.LogError($"Mouse click action '{_mouseRightClickActionName}' not found.");
        }
    }

    private void ChangeCommandState(EMouseConnadState state)
    {
        _connadState = state;
    }

    private void OnLeftClickCommand()
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

    private void OnRightClickCommand()
    {
        if (_connadState != EMouseConnadState.Universal)
        {
            ChangeCommandState(EMouseConnadState.Universal);
            return;
        }
    }

}
