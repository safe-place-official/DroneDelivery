using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuControl : MonoBehaviour
{
    public GameObject PauseMenu;

    bool isPaused = false;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame))
        {
            OpenOrClosePauseMenu();
        }
    }

    public void OpenOrClosePauseMenu()
    {
        if (PauseMenu == null)
        {
            Debug.LogError("PauseMenu reference is not set!");
            return;
        }

        isPaused = !PauseMenu.activeSelf;
        PauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        AudioListener.pause = isPaused;
    }
}
