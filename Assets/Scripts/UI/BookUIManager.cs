using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BookUIManager : UIManager, ActiveUIInterface
{
    private static readonly string BOOK_BUTTON_KEY = "BookButton";
    private static readonly string BOOK_MENU_ELEMENT_KEY = "BookContainer";
    private Button bookButton;
    private VisualElement bookMenu;

    [Header("Background colors")]
    [SerializeField] private Color openedMenuColor;
    [SerializeField] private Color closedMenuColor;

    private static BookUIManager _instance;
    public static BookUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BookUIManager>();
            }

            return _instance;
        }
    }
    void Start()
    {
        root = document.rootVisualElement;
        bookButton = root.Q<Button>(BOOK_BUTTON_KEY);
        bookMenu = root.Q<VisualElement>(BOOK_MENU_ELEMENT_KEY);
        InitButton();
        SetVisibility(bookMenu, DisplayStyle.None);
    }

    private void InitButton()
    {
        if (bookButton == null)
        {
            Debug.LogError("Could not find button element in Book menu.");
            return;
        }
        bookButton.clickable.clicked += delegate { OpenUIComponent(); };
    }

    public void CloseUIComponent()
    {
        SetVisibility(bookMenu, DisplayStyle.None);
        SetVisibility(bookButton, DisplayStyle.Flex);
        root.style.backgroundColor = closedMenuColor;
        SetEnabled(bookButton, true);
    }

    public void OpenUIComponent()
    {
        ResourceUIManager.Instance.UpdateUIComponent();
        HousingUIManager.Instance.UpdateUIComponent();
        TileSelectionManager.Instance.UnselectCell();
        SetEnabled(bookButton, false);
        root.style.backgroundColor = openedMenuColor;
        SetVisibility(bookMenu, DisplayStyle.Flex);
        SetVisibility(bookButton, DisplayStyle.None);
    }

    public void ResetUIComponent()
    {
        ResourceUIManager.Instance.UpdateUIComponent();
        HousingUIManager.Instance.UpdateUIComponent();
        SetEnabled(bookButton, true);
        SetVisibility(bookMenu, DisplayStyle.None);
    }
}
