// ClickSoundManager_NewInput.cs
using UnityEngine;
using UnityEngine.InputSystem; // 新输入系统

[RequireComponent(typeof(AudioSource))]
public class ClickSound : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlayClickSound();
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            PlayClickSound();
        }
    }

    private void PlayClickSound()
    {
        if (clickSound != null) audioSource.PlayOneShot(clickSound);
        else Debug.LogWarning("Click sound not assigned!");
    }
}
