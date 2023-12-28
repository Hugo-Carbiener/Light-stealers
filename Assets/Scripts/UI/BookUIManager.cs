using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BookUIManager : UIManager, ActiveUIInterface
{
    private static readonly string BOOK_BUTTON_KEY = "BookButton";
    private static readonly string BOOK_MENU_ELEMENT_KEY = "BookContainer";
    private static readonly string BOOK_SPRITE_CONTAINER_ELEMENT_KEY = "Book";
    private static readonly string BOOKMARK_RETURN_BUTTON_KEY = "ReturnBookmark";
    private Button bookButton;
    private VisualElement bookMenu;
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
    void Start()
    {
        root = document.rootVisualElement;
        bookButton = root.Q<Button>(BOOK_BUTTON_KEY);
        bookMenu = root.Q<VisualElement>(BOOK_MENU_ELEMENT_KEY);
        bookSpriteContainer = root.Q<VisualElement>(BOOK_SPRITE_CONTAINER_ELEMENT_KEY);
        InitButton();
        InitBookmarks();
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
