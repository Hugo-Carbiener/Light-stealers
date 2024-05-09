using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActiveUI
{
    public void OpenUIComponent();
    public void CloseUIComponent();
    public void ToggleUIComponent();
    public void UpdateVisibility();
    public void ResetUIComponent();
    public bool CanBeOpened();

}
