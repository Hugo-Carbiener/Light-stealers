using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleReportUIManager : UIManager, ActiveUIInterface
{
    private VisualElement battleReport;

    private static BattleReportUIManager _instance;
    public static BattleReportUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BattleReportUIManager>();
            }

            return _instance;
        }
    }



    void Start()
    {
        root = document.rootVisualElement;
        SetVisibility(battleReport, DisplayStyle.None);
    }

    public void CloseUIComponent()
    {
        SetVisibility(battleReport, DisplayStyle.None);
    }

    public void OpenUIComponent()
    {
        SetVisibility(battleReport, DisplayStyle.Flex);
    }

    public void ResetUIComponent()
    {
        throw new System.NotImplementedException();
    }
}
