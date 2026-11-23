using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class CCameraContoller : CSingleton<CCameraContoller>
{
	#region inspector
	[SerializeField] private Camera _camera;
	[SerializeField] private float _senstivity = 1.0f;
	[SerializeField] private float _penArea = 1.0f;
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private CBackgroundController _bgController;
    #endregion

    #region private var
    private readonly string _mousePositionActionName = "MousePosition";

    private CInputVectorActionData _mousePositionAction;
    //private bool _isBinded = false;
    #endregion

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        if (_camera == null)
        {
            Debug.LogWarning("Camera is not assigned in the inspector and no main camera found.");
        }

        if (CInputDispatcher.Instance.InputActionMap.TryGetValue(_mousePositionActionName, out CInputActionData mousePositionAction))
        {
            if (mousePositionAction is CInputVectorActionData vectorAction)
            {
                _mousePositionAction = vectorAction;
            }

            else
            {
                Debug.LogWarning($"Mouse position action '{_mousePositionActionName}' is not of type CInputVectorActionData.");
                enabled = false;
                return;
            }
        }

        else
        {
            Debug.LogWarning($"Mouse position action '{_mousePositionActionName}' not found in InputActionMap.");
            enabled = false;
            return;
        }

        if(_bgController == null)
        {
            Debug.LogWarning("_bgController is not assigned.");
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
            pos.x += _senstivity * Time.unscaledDeltaTime;
        }

        else if (mousePos.x <= _penArea)
        {
            pos.x -= _senstivity * Time.unscaledDeltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, _minX, _maxX);

        if (pos != _camera.transform.position)
        {
            float dx = pos.x - _camera.transform.position.x;
            _camera.transform.position = pos;
            _bgController.OnCameraMove(dx);
        }
    }

    public static void ShakeCamera(float strength, float duration)
    {
    }

}
