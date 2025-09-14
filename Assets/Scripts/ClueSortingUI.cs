using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClueSortingUI : MonoBehaviour
{
    [Header("UI References")]
    public Button sortByCategoryButton;
    public Button sortByTitleButton;
    public Button sortByTimeButton;
    public TextMeshProUGUI currentSortLabel;
    
    [Header("Button Colors")]
    public Color activeButtonColor = Color.green;
    public Color inactiveButtonColor = Color.white;
    
    private ClueManager clueManager;
    
    private void Start()
    {
        clueManager = FindObjectOfType<ClueManager>();
        
        if (clueManager == null)
        {
            Debug.LogError("ClueSortingUI: ClueManager not found!");
            return;
        }
        
        // Setup button listeners
        if (sortByCategoryButton != null)
            sortByCategoryButton.onClick.AddListener(() => SetSortingMethod(ClueSortingMethod.ByCategory));
        
        if (sortByTitleButton != null)
            sortByTitleButton.onClick.AddListener(() => SetSortingMethod(ClueSortingMethod.ByTitle));
        
        if (sortByTimeButton != null)
            sortByTimeButton.onClick.AddListener(() => SetSortingMethod(ClueSortingMethod.ByEarnedTime));
        
        // Update UI to show current sorting method
        UpdateSortingUI();
    }
    
    private void SetSortingMethod(ClueSortingMethod method)
    {
        if (clueManager != null)
        {
            clueManager.SetSortingMethod(method);
            UpdateSortingUI();
        }
    }
    
    private void UpdateSortingUI()
    {
        if (clueManager == null) return;
        
        // Update button colors
        UpdateButtonColor(sortByCategoryButton, clueManager.sortingMethod == ClueSortingMethod.ByCategory);
        UpdateButtonColor(sortByTitleButton, clueManager.sortingMethod == ClueSortingMethod.ByTitle);
        UpdateButtonColor(sortByTimeButton, clueManager.sortingMethod == ClueSortingMethod.ByEarnedTime);
        
        // Update current sort label
        if (currentSortLabel != null)
        {
            string sortText = GetSortingMethodText(clueManager.sortingMethod);
            currentSortLabel.text = $"Sorting: {sortText}";
        }
    }
    
    private void UpdateButtonColor(Button button, bool isActive)
    {
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = isActive ? activeButtonColor : inactiveButtonColor;
            button.colors = colors;
        }
    }
    
    private string GetSortingMethodText(ClueSortingMethod method)
    {
        switch (method)
        {
            case ClueSortingMethod.ByCategory:
                return "By Category";
            case ClueSortingMethod.ByTitle:
                return "By Title";
            case ClueSortingMethod.ByEarnedTime:
                return "By Time";
            default:
                return "Unknown";
        }
    }
    
    [ContextMenu("Test Sort By Category")]
    public void TestSortByCategory()
    {
        SetSortingMethod(ClueSortingMethod.ByCategory);
    }
    
    [ContextMenu("Test Sort By Title")]
    public void TestSortByTitle()
    {
        SetSortingMethod(ClueSortingMethod.ByTitle);
    }
    
    [ContextMenu("Test Sort By Time")]
    public void TestSortByTime()
    {
        SetSortingMethod(ClueSortingMethod.ByEarnedTime);
    }
}
