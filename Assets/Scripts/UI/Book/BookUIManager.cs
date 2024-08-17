using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BookUIManager : UIManager, IActiveUI
{
    private static readonly string BOOK_SPRITE_CONTAINER_ELEMENT_KEY = "Book";
    private static readonly string BOOKMARK_RETURN_BUTTON_KEY = "ReturnBookmark";
    private VisualElement bookSpriteContainer;

    [Header("Background colors")]
    [SerializeField] private Color openedMenuColor;
    [SerializeField] private Color closedMenuColor;
    [Header("Bookmark highlights sprite database")]
    [SerializeField] private SerializableDictionary<string, Sprite> bookmarkHighlights;
    [SerializeField] private Sprite baseBookSprite;

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

    private void Awake()
    {
        root = document.rootVisualElement;
        bookSpriteContainer = root.Q<VisualElement>(BOOK_SPRITE_CONTAINER_ELEMENT_KEY);
    }

    void Start()
    {
        InitBookmarks();
        SetVisibility(root, DisplayStyle.None);
    }

    public void CloseUIComponent()
    {
        SetVisibility(root, DisplayStyle.None);
        root.style.backgroundColor = closedMenuColor;
        ResetUIComponent();
    }

    public void OpenUIComponent()
    {
        ResourceUIManager.Instance.UpdateUIComponent();
        HousingUIManager.Instance.UpdateUIComponent();
        MainMenuUIManager.Instance.UpdateUIComponent();
        root.style.backgroundColor = openedMenuColor;
        SetVisibility(root, DisplayStyle.Flex);
    }

    public void ToggleUIComponent()
    {
        if (IsVisible())
        {
            CloseUIComponent();
        } else
        {
            OpenUIComponent();
        }
    }

    public void UpdateVisibility() { }

    public void ResetUIComponent()
    {
        ResourceUIManager.Instance.UpdateUIComponent();
        HousingUIManager.Instance.UpdateUIComponent();
        SetEnabled(MainMenuUIManager.Instance.bookButton, true);
        SetVisibility(root, DisplayStyle.None);
        MainMenuUIManager.Instance.UpdateUIComponent();
    }

    public bool CanBeOpened()
    {
        return !BuildingUIManager.Instance.IsVisible()
            && !BattleReportUIManager.Instance.IsVisible()
            && !BattlefieldUIManager.Instance.IsVisible();
    }
    private void InitBookmarks()
    {
        InitBookmarkActions();
        InitBookmarkHighlights();
    }

    private void InitBookmarkHighlights()
    {
        if (bookmarkHighlights == null || bookmarkHighlights.Count() == 0) return;
        foreach(var bookmarkHighlight in bookmarkHighlights)
        {
            Button bookmark = root.Q<Button>(bookmarkHighlight.Key);
            if (bookmark == null)
            {
                Debug.LogError("Could not find bookmark " + bookmarkHighlight.Key + " when loading bookmark highlights.");
                continue;
            }
            bookmark.RegisterCallback<MouseEnterEvent>(delegate { bookSpriteContainer.style.backgroundImage = new StyleBackground(bookmarkHighlight.Value); });
            bookmark.RegisterCallback<MouseLeaveEvent>(delegate { bookSpriteContainer.style.backgroundImage = new StyleBackground(baseBookSprite); });
        }
    }

    private void InitBookmarkActions()
    {
        Button bookmark = root.Q<Button>(BOOKMARK_RETURN_BUTTON_KEY);
        if (bookmark == null)
        {
            Debug.LogError("Could not find bookmark button " + BOOKMARK_RETURN_BUTTON_KEY + ".");
            return;
        }
        bookmark.clickable.clicked += delegate { CloseUIComponent(); };
    }
}
