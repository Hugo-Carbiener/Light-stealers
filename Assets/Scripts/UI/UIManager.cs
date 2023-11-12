using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIManager : MonoBehaviour
{
    [SerializeField] protected UIDocument document;
    protected VisualElement root;

    protected void updateText(Label text, string content)
    {
        text.text = content;
    }

    public void setVisibility(DisplayStyle displayStyle)
    {
        root.style.display = displayStyle;
    }

    public void setPosition(Vector2 worldPosition)
    {
        Vector2 screenPoint = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, worldPosition, Camera.main);
        root.transform.position = screenPoint;
    }
}
