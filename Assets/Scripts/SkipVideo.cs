using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class VideoSkipper : MonoBehaviour
{
    [SerializeField] private float skipDelay = 10f;
    [SerializeField] private TextMeshProUGUI skipText;
    private VideoPlayer videoPlayer;
    private bool canSkip = false;
    public int SkipSceneIndex = 3;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        Invoke("EnableSkip", skipDelay);
        skipText.text = "";
    }

    void Update()
    {
        if(canSkip && (Keyboard.current.anyKey.wasPressedThisFrame ||
                       Mouse.current.leftButton.wasPressedThisFrame ||
                       Mouse.current.rightButton.wasPressedThisFrame))
        {
            SkipVideo();
        }
    }

    private void EnableSkip()
    {
        canSkip = true;
        Debug.Log("can skip now");
        skipText.text = "Press any key to skip...";
    }

    private void SkipVideo()
    {
        videoPlayer.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - SkipSceneIndex);
    }
}