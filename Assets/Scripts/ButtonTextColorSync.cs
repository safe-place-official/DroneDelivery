using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonTextColorSync : MonoBehaviour,
    ISelectHandler,
    IDeselectHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Settings")]
    public Text text;
    public float colorTransitionSpeed = 10f;

    [Header("Debug")]
    [SerializeField] private bool _isPressed;
    [SerializeField] private bool _isSelected;

    private Button _button;
    private Color _targetColor;
    private bool _componentsCached;

    void Awake()
    {
        CacheComponents();
        _targetColor = text != null ? text.color : Color.white;
    }

    void OnEnable()
    {
        CacheComponents();
        UpdateColors();
    }

    void Update()
    {
        if (!_componentsCached) return;

        if (text.color != _targetColor)
        {
            text.color = Color.Lerp(text.color, _targetColor, colorTransitionSpeed);
        }
    }

    private void CacheComponents()
    {
        _button = GetComponent<Button>();
        _componentsCached = _button != null && text != null;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!_componentsCached) return;
        _isSelected = true;
        UpdateColors();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!_componentsCached) return;
        _isSelected = false;
        UpdateColors();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_componentsCached) return;
        _isPressed = true;
        UpdateColors();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_componentsCached) return;
        _isPressed = false;
        UpdateColors();
    }

    private void UpdateColors()
    {
        if (!_componentsCached) return;

        _targetColor = _button.interactable
            ? GetActiveColorState()
            : _button.colors.disabledColor;
    }

    private Color GetActiveColorState()
    {
        if (_isPressed) return _button.colors.pressedColor;
        if (_isSelected) return _button.colors.selectedColor;
        return _button.colors.normalColor;
    }

    public void ForceSelectState(bool selected)
    {
        if (!_componentsCached) return;
        _isSelected = selected;
        UpdateColors();
    }

    void OnDisable()
    {
        // Сбрасываем состояния при деактивации
        _isPressed = false;
        _isSelected = false;
        _componentsCached = false;
    }
}