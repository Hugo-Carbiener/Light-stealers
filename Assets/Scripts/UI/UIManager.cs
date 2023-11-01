using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIManager : MonoBehaviour
{
    protected void updateText(Label text, string content)
    {
        text.text = content;
    }

    public abstract void updateUIComponent();
}
