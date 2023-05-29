using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIManager : MonoBehaviour
{
    protected void updateText(Text text, string content)
    {
        text.text = content;
    }

    public abstract void updateUIComponent();
}
