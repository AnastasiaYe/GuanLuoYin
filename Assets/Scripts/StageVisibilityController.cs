using UnityEngine;
using System.Collections.Generic;

public class StageVisibilityController : MonoBehaviour
{
    [Header("Stage Visibility Settings")]
    [Tooltip("Stages where this object should be visible (0-4)")]
    public List<int> visibleStages = new List<int> { 0 };

    [Tooltip("How to handle stages not in the visible list")]
    public VisibilityMode visibilityMode = VisibilityMode.ShowInListedStages;

    [Header("Animation Settings")]
    [Tooltip("Animate visibility changes (fade in/out)")]
    public bool animateTransitions = false;
    [Tooltip("Duration of fade in/out animation")]
    public float animationDuration = 1f;
    [Tooltip("Animation curve for fade transitions")]
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Debug")]
    public bool enableDebugLogging = false;

    public enum VisibilityMode
    {
        ShowInListedStages,    // Only show in stages listed in visibleStages
        HideInListedStages     // Hide in stages listed in visibleStages, show in others
    }

    private Renderer objectRenderer;
    private CanvasGroup canvasGroup;
    private SpriteRenderer spriteRenderer;
    private bool isAnimating = false;
    private bool currentVisibility = true;
    private bool componentsInitialized = false;

    private void Start()
    {
        InitializeComponents();
        SubscribeToStageManager();

        if (enableDebugLogging)
        {
            Debug.Log($"StageVisibilityController on {name}: Initialized for stages {string.Join(", ", visibleStages)}");
        }
    }

    /// <summary>
    /// Initialize renderer components once
    /// </summary>
    private void InitializeComponents()
    {
        if (componentsInitialized) return;

        objectRenderer = GetComponent<Renderer>();
        canvasGroup = GetComponent<CanvasGroup>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        componentsInitialized = true;
    }

    /// <summary>
    /// Subscribe to stage manager events
    /// </summary>
    private void SubscribeToStageManager()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageChanged += OnStageChanged;

            // Set initial visibility based on current stage
            UpdateVisibilityForStage(StageManager.Instance.currentStage);
        }
        else
        {
            Debug.LogWarning($"StageVisibilityController on {name}: StageManager not found!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from stage changes
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageChanged -= OnStageChanged;
        }
    }

    /// <summary>
    /// Called when stage changes
    /// </summary>
    /// <param name="newStage">The new stage number</param>
    private void OnStageChanged(int newStage)
    {
        UpdateVisibilityForStage(newStage);
    }

    /// <summary>
    /// Update visibility based on current stage
    /// </summary>
    /// <param name="stage">The current stage</param>
    private void UpdateVisibilityForStage(int stage)
    {
        bool shouldBeVisible = ShouldBeVisibleInStage(stage);

        if (enableDebugLogging)
        {
            Debug.Log($"StageVisibilityController on {name}: Stage {stage} - Should be visible: {shouldBeVisible} (Current: {currentVisibility})");
        }

        if (shouldBeVisible != currentVisibility)
        {
            SetVisibility(shouldBeVisible);
        }
    }

    /// <summary>
    /// Determine if object should be visible in given stage
    /// </summary>
    /// <param name="stage">The stage to check</param>
    /// <returns>True if should be visible, false otherwise</returns>
    private bool ShouldBeVisibleInStage(int stage)
    {
        switch (visibilityMode)
        {
            case VisibilityMode.ShowInListedStages:
                return visibleStages.Contains(stage);

            case VisibilityMode.HideInListedStages:
                return !visibleStages.Contains(stage);

            default:
                return true;
        }
    }

    /// <summary>
    /// Set the visibility of the object
    /// </summary>
    /// <param name="visible">Whether the object should be visible</param>
    public void SetVisibility(bool visible)
    {
        if (isAnimating) return; // Don't interrupt ongoing animation

        currentVisibility = visible;

        if (animateTransitions)
        {
            StartCoroutine(AnimateVisibility(visible));
        }
        else
        {
            ApplyVisibility(visible);
        }
    }

    /// <summary>
    /// Apply visibility without animation
    /// </summary>
    /// <param name="visible">Whether the object should be visible</param>
    private void ApplyVisibility(bool visible)
    {
        InitializeComponents(); // Ensure components are initialized

        // Handle Canvas UI elements
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        // Handle 3D objects
        if (objectRenderer != null)
        {
            objectRenderer.enabled = visible;
        }

        // Handle 2D sprites
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = visible;
        }

        // Fallback: disable entire GameObject
        if (canvasGroup == null && objectRenderer == null && spriteRenderer == null)
        {
            gameObject.SetActive(visible);
        }

        if (enableDebugLogging)
        {
            Debug.Log($"StageVisibilityController on {name}: Visibility set to {visible}");
        }
    }

    /// <summary>
    /// Animate visibility change
    /// </summary>
    /// <param name="visible">Target visibility</param>
    private System.Collections.IEnumerator AnimateVisibility(bool visible)
    {
        isAnimating = true;

        float startAlpha = visible ? 0f : 1f;
        float endAlpha = visible ? 1f : 0f;
        float elapsed = 0f;

        // Set initial state
        if (canvasGroup != null)
        {
            canvasGroup.alpha = startAlpha;
        }

        // Animate
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;
            float curveValue = fadeCurve.Evaluate(progress);
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, curveValue);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = currentAlpha;
            }

            yield return null;
        }

        // Ensure final state
        ApplyVisibility(visible);
        isAnimating = false;

        if (enableDebugLogging)
        {
            Debug.Log($"StageVisibilityController on {name}: Animation completed - Visibility: {visible}");
        }
    }

    /// <summary>
    /// Add a stage to the visible stages list
    /// </summary>
    /// <param name="stage">Stage to add</param>
    public void AddVisibleStage(int stage)
    {
        if (!visibleStages.Contains(stage))
        {
            visibleStages.Add(stage);
            UpdateVisibilityForStage(StageManager.Instance?.currentStage ?? 0);

            if (enableDebugLogging)
            {
                Debug.Log($"StageVisibilityController on {name}: Added stage {stage} to visible stages");
            }
        }
    }

    /// <summary>
    /// Remove a stage from the visible stages list
    /// </summary>
    /// <param name="stage">Stage to remove</param>
    public void RemoveVisibleStage(int stage)
    {
        if (visibleStages.Contains(stage))
        {
            visibleStages.Remove(stage);
            UpdateVisibilityForStage(StageManager.Instance?.currentStage ?? 0);

            if (enableDebugLogging)
            {
                Debug.Log($"StageVisibilityController on {name}: Removed stage {stage} from visible stages");
            }
        }
    }

    /// <summary>
    /// Set the visible stages list
    /// </summary>
    /// <param name="stages">List of stages where object should be visible</param>
    public void SetVisibleStages(List<int> stages)
    {
        visibleStages = new List<int>(stages);
        UpdateVisibilityForStage(StageManager.Instance?.currentStage ?? 0);

        if (enableDebugLogging)
        {
            Debug.Log($"StageVisibilityController on {name}: Set visible stages to {string.Join(", ", visibleStages)}");
        }
    }

    /// <summary>
    /// Check if object is currently visible
    /// </summary>
    /// <returns>True if visible, false otherwise</returns>
    public bool IsVisible()
    {
        return currentVisibility;
    }

    /// <summary>
    /// Get the current visible stages
    /// </summary>
    /// <returns>List of visible stages</returns>
    public List<int> GetVisibleStages()
    {
        return new List<int>(visibleStages);
    }

    /// <summary>
    /// Force update visibility for current stage (useful for debugging)
    /// </summary>
    [ContextMenu("Force Update Visibility")]
    public void ForceUpdateVisibility()
    {
        if (StageManager.Instance != null)
        {
            UpdateVisibilityForStage(StageManager.Instance.currentStage);
        }
    }

    /// <summary>
    /// Test visibility in all stages
    /// </summary>
    [ContextMenu("Test All Stages")]
    public void TestAllStages()
    {
        Debug.Log($"=== StageVisibilityController Test for {name} ===");
        Debug.Log($"Visible Stages: {string.Join(", ", visibleStages)}");
        Debug.Log($"Visibility Mode: {visibilityMode}");
        Debug.Log($"Animate Transitions: {animateTransitions}");

        for (int stage = 0; stage <= 4; stage++)
        {
            bool shouldBeVisible = ShouldBeVisibleInStage(stage);
            Debug.Log($"Stage {stage}: {(shouldBeVisible ? "VISIBLE" : "HIDDEN")}");
        }
    }
}