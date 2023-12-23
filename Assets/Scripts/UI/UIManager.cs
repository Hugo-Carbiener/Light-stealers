using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIManager : MonoBehaviour
{
    [SerializeField] protected UIDocument document;
    protected VisualElement root;

    protected void UpdateText(Label text, string content)
    {
        text.text = content;
    }

    protected void SetVisibility(DisplayStyle displayStyle)
    {
        root.style.display = displayStyle;
    }

    protected void SetVisibility(VisualElement element, DisplayStyle displayStyle)
    {
        element.style.display = displayStyle;
    }

    protected void SetPosition(Vector2 worldPosition)
    {
        Vector2 screenPoint = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, worldPosition, Camera.main);
        root.transform.position = screenPoint;
    }

    protected void SetEnabled(VisualElement element, bool enabled)
    {
        if (element == null)
        {
            Debug.LogError("Could not element to enable.");
            return;
        }
        element.SetEnabled(enabled);
    }
}
