using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // ADD THIS

public class CursorItemPreview : MonoBehaviour
{
    public static CursorItemPreview Instance { get; private set; }

    [Header("References")]
    public Image previewIcon;

    private RectTransform _rect;
    private Canvas _canvas;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        Hide();
    }

    void Update()
    {
        if (previewIcon.enabled)
            FollowMouse();
    }

    public void Show(Sprite sprite)
    {
        previewIcon.sprite = sprite;
        previewIcon.enabled = true;
    }

    public void Hide()
    {
        previewIcon.enabled = false;
    }

    private void FollowMouse()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue(); // FIXED

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            mousePos,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 localPoint
        );

        _rect.localPosition = localPoint;
    }
}