using UnityEngine;

public class InitialVolumeSetter : MonoBehaviour
{
    public AudioSource audioSource;  // Источник звука
    public string saveKey;           // Ключ сохранения (например, "MusicVolume")
    public bool ignorePause;

    private void Start()
    {
        if (audioSource != null)
        {
            float savedVolume = PlayerPrefs.GetFloat(saveKey, 1f);
            audioSource.volume = savedVolume;
            audioSource.ignoreListenerPause = ignorePause;

            // Добавь эту строку для автоматического запуска
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}