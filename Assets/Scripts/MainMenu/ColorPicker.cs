using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorPicker : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Color Picker")]
    [SerializeField] private RawImage colorPalette;
    [SerializeField] private RectTransform hueCursor;

    [Header("Brightness Slider")]
    [SerializeField] private RawImage brightnessSlider;
    [SerializeField] private RectTransform brightnessHandle;

    [Header("Target Material")]
    [SerializeField] private Material targetMaterial;
    [SerializeField] private int materialIndex = 0;

    private Texture2D hueTexture;
    private Texture2D brightnessTexture;
    private RectTransform hueRect;
    private RectTransform brightnessRect;
    private float currentHue = 0f;
    private float currentBrightness = 1f;
    private float currentSaturation = 1f;
    private bool isDraggingHue = false;
    private bool isDraggingBrightness = false;

    private const string COLOR_PREFIX = "MaterialColor_";
    private const string INDEX_KEY = "ColorPicker_MaxIndex";

    private static List<ColorPicker> _instances = new List<ColorPicker>();

    private void Awake()
    {
        _instances.Add(this);
        hueRect = colorPalette.GetComponent<RectTransform>();
        brightnessRect = brightnessSlider.GetComponent<RectTransform>();

        CreateHueTexture();
        CreateBrightnessTexture();
        LoadColor();
    }

    private void OnDestroy()
    {
        _instances.Remove(this);
    }

    // === Color Texture Generation ===
    private void CreateHueTexture()
    {
        int width = 256;
        int height = 256;

        hueTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        hueTexture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < height; y++)
        {
            float saturation = (float)y / (height - 1);
            for (int x = 0; x < width; x++)
            {
                float hue = (float)x / (width - 1);
                hueTexture.SetPixel(x, y, Color.HSVToRGB(hue, saturation, 1f));
            }
        }
        hueTexture.Apply();
        colorPalette.texture = hueTexture;
    }

    private void CreateBrightnessTexture()
    {
        brightnessTexture = new Texture2D(32, 256);
        for (int y = 0; y < brightnessTexture.height; y++)
        {
            float brightness = 1f - (float)y / brightnessTexture.height;
            Color color = new Color(brightness, brightness, brightness);
            for (int x = 0; x < brightnessTexture.width; x++)
            {
                brightnessTexture.SetPixel(x, y, color);
            }
        }
        brightnessTexture.Apply();
        brightnessSlider.texture = brightnessTexture;
    }

    // === Color Loading/Saving ===
    public void LoadColor()
    {
        if (targetMaterial == null) return;

        string colorKey = COLOR_PREFIX + materialIndex;
        if (PlayerPrefs.HasKey(colorKey) && ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(colorKey), out Color savedColor))
        {
            targetMaterial.color = savedColor;
            Color.RGBToHSV(savedColor, out currentHue, out currentSaturation, out currentBrightness);
            UpdateVisuals();
        }
    }

    private void SaveCurrentColor()
    {
        if (targetMaterial == null) return;

        string colorKey = COLOR_PREFIX + materialIndex;
        PlayerPrefs.SetString(colorKey, "#" + ColorUtility.ToHtmlStringRGB(targetMaterial.color));

        if (materialIndex > PlayerPrefs.GetInt(INDEX_KEY, 0))
            PlayerPrefs.SetInt(INDEX_KEY, materialIndex);

        PlayerPrefs.Save();
    }

    // === Static Methods (Global Control) ===
    public static void LoadAllColors()
    {
        foreach (var picker in _instances)
            picker.LoadColor();
    }

    public static void LoadColorsByIndex(int index)
    {
        foreach (var picker in _instances)
            if (picker.materialIndex == index)
                picker.LoadColor();
    }

    public static void ResetAllSavedColors()
    {
        int maxIndex = PlayerPrefs.GetInt(INDEX_KEY, 0);
        for (int i = 0; i <= maxIndex; i++)
            PlayerPrefs.DeleteKey(COLOR_PREFIX + i);

        PlayerPrefs.DeleteKey(INDEX_KEY);
        PlayerPrefs.Save();
    }

    // === UI Interaction ===
    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(hueRect, eventData.position, eventData.pressEventCamera))
        {
            isDraggingHue = true;
            UpdateHueSelection(eventData);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(brightnessRect, eventData.position, eventData.pressEventCamera))
        {
            isDraggingBrightness = true;
            UpdateBrightnessSelection(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDraggingHue) UpdateHueSelection(eventData);
        else if (isDraggingBrightness) UpdateBrightnessSelection(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDraggingHue = false;
        isDraggingBrightness = false;
        SaveCurrentColor();
    }

    private void UpdateHueSelection(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(hueRect, eventData.position, eventData.pressEventCamera, out Vector2 localPos))
            return;

        Vector2 clampedPos = new Vector2(
            Mathf.Clamp(localPos.x, -hueRect.rect.width / 2, hueRect.rect.width / 2),
            Mathf.Clamp(localPos.y, -hueRect.rect.height / 2, hueRect.rect.height / 2)
        );

        hueCursor.localPosition = clampedPos;
        currentHue = (clampedPos.x + hueRect.rect.width / 2) / hueRect.rect.width;
        currentSaturation = (clampedPos.y + hueRect.rect.height / 2) / hueRect.rect.height;
        UpdateColor();
    }

    private void UpdateBrightnessSelection(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(brightnessRect, eventData.position, eventData.pressEventCamera, out Vector2 localPos))
            return;

        float clampedY = Mathf.Clamp(localPos.y, -brightnessRect.rect.height / 2, brightnessRect.rect.height / 2);
        brightnessHandle.localPosition = new Vector2(0, clampedY);
        currentBrightness = 1f - (clampedY + brightnessRect.rect.height / 2) / brightnessRect.rect.height;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (targetMaterial != null)
            targetMaterial.color = Color.HSVToRGB(currentHue, currentSaturation, currentBrightness);
    }

    private void UpdateVisuals()
    {
        hueCursor.localPosition = new Vector2(
            currentHue * hueRect.rect.width - hueRect.rect.width / 2,
            currentSaturation * hueRect.rect.height - hueRect.rect.height / 2
        );

        brightnessHandle.localPosition = new Vector2(
            0,
            (1f - currentBrightness) * brightnessRect.rect.height - brightnessRect.rect.height / 2
        );
    }

    private void Update()
    {
        if ((isDraggingHue || isDraggingBrightness) && Mouse.current.leftButton.isPressed)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Mouse.current.position.ReadValue();

            if (isDraggingHue) UpdateHueSelection(eventData);
            else if (isDraggingBrightness) UpdateBrightnessSelection(eventData);
        }
    }
}