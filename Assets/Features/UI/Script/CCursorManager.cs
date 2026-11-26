using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class CCursorManager : CSingletonDontDstroyAtLoad<CCursorManager>
{
    public enum ECursorType
    {
        Universal,
        Attack,
        Move,
        Skill
    }
    
    [SerializeField] private Texture2D _universalCursor;
    [SerializeField] private Texture2D _attckCursor;
    [SerializeField] private Texture2D _moveCursor;
    [SerializeField] private Texture2D _skillCursor;


    private void Start()
    {
        SetCursorType(ECursorType.Universal);
    }

    private void SetCursorType(ECursorType cursorType)
    {
        switch (cursorType)
        {
            case ECursorType.Universal:
                Cursor.SetCursor(_universalCursor, Vector2.zero, CursorMode.Auto);
                break;
            case ECursorType.Attack:
                Cursor.SetCursor(_attckCursor, new Vector2(_attckCursor.width * 0.5f, _attckCursor.height * 0.5f), CursorMode.Auto);
                break;
            case ECursorType.Move:
                Cursor.SetCursor(_moveCursor, new Vector2(_attckCursor.width * 0.5f, _attckCursor.height * 0.5f), CursorMode.Auto);
                break;
            case ECursorType.Skill:
                Cursor.SetCursor(_skillCursor, new Vector2(_attckCursor.width * 0.5f, _attckCursor.height * 0.5f), CursorMode.Auto);
                break;
            default:
                break;
        }
    }

    public static void SetCursor(ECursorType cursorType)
    {
        Instance.SetCursorType(cursorType);
    }

}
