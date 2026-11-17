using UnityEngine;
using UnityEngine.InputSystem;

public class CCameraContoller : MonoBehaviour
{
	#region inspector
	[SerializeField] private Camera _camera;
	[SerializeField] private float _senstivity = 1.0f;
	[SerializeField] private float _penArea = 1.0f;
    [SerializeField] private string _mousePositionActionName = "MousePosition";
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    #endregion

    #region private var
    private CInputVectorActionData _mousePositionAction;
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
        }
        /*
        */
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

        //BindAction();

    }

    /*
    private void OnEnable()
    {
        BindAction();
    }

    private void OnDisable()
    {
        UnbindAction();
    }

    private void OnDestroy()
    {
        UnbindAction();
    }

    private void BindAction()
    {
        if (_isBinded)
        {
            return;
        }

        if (_mousePositionAction == null)
        {
            return;
        }

        _mousePositionAction.OnPerformedAction += OnMousePosPerfomed;

        _isBinded = true;
    }

    private void UnbindAction()
    {
        if (!_isBinded)
        {
            return;
        }

        if (_mousePositionAction == null)
        {
            Debug.LogError("Mouse click action is not assigned.");
            return;
        }

        _mousePositionAction.OnPerformedAction -= OnMousePosPerfomed;

        _isBinded = false;
    }

    private void OnMousePosPerfomed(InputAction.CallbackContext context)
    {
        Vector2 mousePos = context.ReadValue<Vector2>();
        Debug.Log(mousePos);
    }
    */

    private void Update()
    {

        Vector2 mousePos = _mousePositionAction.Value;
        Vector3 pos = _camera.transform.position;

        if(mousePos.x >= Screen.width - _penArea)
        {
            pos.x += _senstivity * Time.deltaTime;
        }

        else if (mousePos.x <= _penArea)
        {
            pos.x -= _senstivity * Time.deltaTime;
        }

        pos.x =Mathf.Clamp(pos.x, _minX, _maxX);

        _camera.transform.position = pos;
    }
}
