using Input;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCursorUI : MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursorTexture;
    [SerializeField] private Vector2 defaultCursorSize = new Vector2(32f, 32f);
    private RawImage cursorImage;
    private RectTransform rectTransform;
    private PlayerInteractionHandler interactionHandler;

    private Texture2D overrideCursorTexture;

    void Start()
    {
        Cursor.visible = false;
        cursorImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
        GameManager gameManager = GameManager.Instance;
        Player player = gameManager.Player;
        interactionHandler = player.InteractionHandler;
        SetCursorSize(defaultCursorSize);
    }

    void Update()
    {
        Vector3 mousePos = InputRegistry.Player.Mouse.action.ReadValue<Vector2>();
        rectTransform.position = mousePos;
    }

    public void SetCursorTexture(Texture2D texture)
    {
        if (!texture)
        {
            return;
        }
        overrideCursorTexture = texture;
        cursorImage.texture = texture;
    }

    public void ClearCursorTexture()
    {
        overrideCursorTexture = null;
        SetCursorTexture(defaultCursorTexture);
        SetCursorSize(defaultCursorSize);
    }

    public void SetCursorSize(Vector2 size)
    {
        rectTransform.sizeDelta = size;
    }

    public void Show()
    {
        if (cursorImage)
        {
            cursorImage.enabled = true;
        }
    }

    public void Hide()
    {
        if (cursorImage)
        {
            cursorImage.enabled = false;
        }
    }

    void OnEnable()
    {
        Show();
    }

    void OnDisable()
    {
        Hide();
    }
}
