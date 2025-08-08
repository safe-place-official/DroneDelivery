using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(EventSystem))]
public class GamepadMenuController : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    private int currentIndex = 0;
    private EventSystem eventSystem;
    private float lastInputTime;

    void Start()
    {
        eventSystem = EventSystem.current;
        SelectFirstActiveButton();

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            EventTrigger trigger = buttons[i].gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => OnButtonHover(index));
            trigger.triggers.Add(entry);
        }
    }


    void Update()
    {
        if (eventSystem == null || buttons == null || buttons.Length == 0) return;

        if (IsAnyButtonActive(buttons))
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    SelectFirstActiveButton();
                }
            }
            else if (Gamepad.current != null)
            {
                HandleNavigation();
            }
            HandleSubmit();
        }
    }

    bool IsAnyButtonActive(Button[] buttons)
    {
        foreach (Button button in buttons)
        {
            if (button != null && button.gameObject.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }




    private void HandleNavigation()
    {
        var gamepad = Gamepad.current;

        Vector2 leftStickInput = gamepad.leftStick.value;
        float leftStickHorizontal = leftStickInput.x;
        float leftStickVertical = leftStickInput.y;

        Vector2 rightStickInput = gamepad.rightStick.value;
        float rightStickHorizontal = rightStickInput.x;
        float rightStickVertical = rightStickInput.y;

        if (Time.time - lastInputTime < 0.2f) return;

        int direction = 0;

        // Используем вертикальное движение с левого стика или правого стика
        if (Mathf.Abs(leftStickVertical) > 0.5f || Mathf.Abs(rightStickVertical) > 0.5f)
        {
            direction = (leftStickVertical > 0 || rightStickVertical > 0) ? -1 : 1;
        }
        // Используем горизонтальное движение с левого стика или правого стика
        else if (Mathf.Abs(leftStickHorizontal) > 0.5f || Mathf.Abs(rightStickHorizontal) > 0.5f)
        {
            direction = (leftStickHorizontal > 0 || rightStickHorizontal > 0) ? 1 : -1;
        }

        if (direction != 0)
        {
            MoveToNextActiveButton(direction);
            lastInputTime = Time.time;
        }
    }

    private void MoveToNextActiveButton(int direction)
    {
        int startIndex = currentIndex;
        do
        {
            currentIndex += direction;
            if (currentIndex < 0) currentIndex = buttons.Length - 1;
            if (currentIndex >= buttons.Length) currentIndex = 0;
        }
        while (!buttons[currentIndex].gameObject.activeInHierarchy || !buttons[currentIndex].interactable);

        eventSystem.SetSelectedGameObject(buttons[currentIndex].gameObject);
    }

    private void HandleSubmit()
    {
        if ((Keyboard.current.enterKey.isPressed) &&
            buttons[currentIndex].interactable && buttons[currentIndex].gameObject.activeInHierarchy)
        {
            buttons[currentIndex].onClick.Invoke();
        }

        if (Gamepad.current != null)
        {
            if ((Gamepad.current.buttonNorth.isPressed) &&
        buttons[currentIndex].interactable && buttons[currentIndex].gameObject.activeInHierarchy)
            {
                buttons[currentIndex].onClick.Invoke();
            }
        }
    }

    private void OnButtonHover(int index)
    {
        if (buttons[index].gameObject.activeInHierarchy && buttons[index].interactable)
        {
            currentIndex = index;
            eventSystem.SetSelectedGameObject(buttons[currentIndex].gameObject);
        }
    }

    private void SelectFirstActiveButton()
    {
        currentIndex = 0;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject.activeInHierarchy && buttons[i].interactable)
            {
                currentIndex = i;
                eventSystem.SetSelectedGameObject(buttons[currentIndex].gameObject);
                return;
            }
        }
    }
}
