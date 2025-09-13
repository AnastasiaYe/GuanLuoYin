using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public bool enableCutscenes = true;
    public float cutsceneDuration = 5f; // Duration of the cutscene
    
    [Header("UI References")]
    public Canvas cutsceneCanvas;
    public Image backgroundImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button skipButton;
    
    [Header("Cutscene Content")]
    public string cutsceneTitle = "Stage Complete!";
    public string cutsceneDescription = "All stages have been completed. Returning to the beginning...";
    public Sprite cutsceneBackground;
    
    [Header("Audio")]
    public AudioClip cutsceneMusic;
    public AudioClip cutsceneSoundEffect;
    
    private AudioSource audioSource;
    private bool isCutsceneActive = false;
    private Coroutine cutsceneCoroutine;
    
    // Events
    public System.Action OnCutsceneStarted;
    public System.Action OnCutsceneCompleted;
    
    // Singleton pattern for easy access
    private static CutsceneManager _instance;
    public static CutsceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CutsceneManager>();
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Subscribe to stage cycle completion
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageCycleCompleted += OnStageCycleCompleted;
        }
        
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        
        // Setup skip button
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipCutscene);
        }
        
        // Initially hide cutscene UI
        if (cutsceneCanvas != null)
        {
            cutsceneCanvas.gameObject.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from stage cycle completion
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageCycleCompleted -= OnStageCycleCompleted;
        }
    }
    
    private void OnStageCycleCompleted()
    {
        // Trigger cutscene when completing a full stage cycle (stage 4 -> stage 0)
        if (enableCutscenes && !isCutsceneActive)
        {
            StartCutscene();
        }
    }
    
    /// <summary>
    /// Start the cutscene sequence
    /// </summary>
    public void StartCutscene()
    {
        if (isCutsceneActive || cutsceneCanvas == null) return;
        
        isCutsceneActive = true;
        
        // Setup cutscene content
        SetupCutsceneContent();
        
        // Show cutscene UI
        cutsceneCanvas.gameObject.SetActive(true);
        
        // Play audio
        PlayCutsceneAudio();
        
        // Start cutscene sequence
        cutsceneCoroutine = StartCoroutine(CutsceneSequence());
        
        OnCutsceneStarted?.Invoke();
    }
    
    /// <summary>
    /// Skip the cutscene and go directly to stage 0
    /// </summary>
    public void SkipCutscene()
    {
        if (!isCutsceneActive) return;
        
        if (cutsceneCoroutine != null)
        {
            StopCoroutine(cutsceneCoroutine);
            cutsceneCoroutine = null;
        }
        
        EndCutscene();
    }
    
    /// <summary>
    /// End the cutscene and return to normal gameplay
    /// </summary>
    private void EndCutscene()
    {
        isCutsceneActive = false;
        
        // Hide cutscene UI
        if (cutsceneCanvas != null)
        {
            cutsceneCanvas.gameObject.SetActive(false);
        }
        
        // Stop audio
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        OnCutsceneCompleted?.Invoke();
    }
    
    /// <summary>
    /// Setup cutscene content (text, images, etc.)
    /// </summary>
    private void SetupCutsceneContent()
    {
        if (titleText != null)
        {
            titleText.text = cutsceneTitle;
        }
        
        if (descriptionText != null)
        {
            descriptionText.text = cutsceneDescription;
        }
        
        if (backgroundImage != null && cutsceneBackground != null)
        {
            backgroundImage.sprite = cutsceneBackground;
        }
    }
    
    /// <summary>
    /// Play cutscene audio
    /// </summary>
    private void PlayCutsceneAudio()
    {
        if (audioSource == null) return;
        
        if (cutsceneMusic != null)
        {
            audioSource.clip = cutsceneMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (cutsceneSoundEffect != null)
        {
            audioSource.PlayOneShot(cutsceneSoundEffect);
        }
    }
    
    /// <summary>
    /// Main cutscene sequence coroutine
    /// </summary>
    private IEnumerator CutsceneSequence()
    {
        // Wait for the cutscene duration
        yield return new WaitForSeconds(cutsceneDuration);
        
        // End the cutscene
        EndCutscene();
    }
    
    /// <summary>
    /// Check if cutscene is currently active
    /// </summary>
    public bool IsCutsceneActive()
    {
        return isCutsceneActive;
    }
    
    /// <summary>
    /// Set cutscene content dynamically
    /// </summary>
    public void SetCutsceneContent(string title, string description, Sprite background = null)
    {
        cutsceneTitle = title;
        cutsceneDescription = description;
        if (background != null)
        {
            cutsceneBackground = background;
        }
    }
    
    /// <summary>
    /// Enable or disable cutscenes
    /// </summary>
    public void SetCutscenesEnabled(bool enabled)
    {
        enableCutscenes = enabled;
    }
    
    #region Debug Methods
    
    [ContextMenu("Test Cutscene")]
    public void TestCutscene()
    {
        StartCutscene();
    }
    
    [ContextMenu("Skip Current Cutscene")]
    public void TestSkipCutscene()
    {
        SkipCutscene();
    }
    
    #endregion
}
