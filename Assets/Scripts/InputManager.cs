using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour {
    // Delegates
    public delegate void moveInputHandler(Vector2 moveVector);
    public delegate void zoomInputHandler(float zoomAmount);
    public delegate void selectionInputHandler(Vector2 mousePosition);
    public delegate void menuInputHandler();

    public static event moveInputHandler onMoveInput;
    public static event zoomInputHandler onZoomInput;
    public static event selectionInputHandler onSelectInput;
    public static event menuInputHandler onRightClick;

    void Update()
    {
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W))
        {
            onMoveInput?.Invoke(Vector2.up);
        }
        if (Input.GetKey(KeyCode.S))
        {
            onMoveInput?.Invoke(Vector2.down);
        }
        if (Input.GetKey(KeyCode.D))
        {
            onMoveInput?.Invoke(Vector2.right);
        }
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A))
        {
            onMoveInput?.Invoke(Vector2.left);
        }

        if (Input.GetMouseButtonDown(0))
        {
            // if the mouse is not over UI 
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                onSelectInput?.Invoke(Input.mousePosition);
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            onRightClick?.Invoke();
        }
    }
}
