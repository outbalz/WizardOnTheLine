using UnityEngine;

public class CCursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D _cursorTexture;

    private Vector2 _clickPos;

    private void Start()
    {
        Cursor.SetCursor(_cursorTexture, _clickPos, CursorMode.Auto);
    }
}
