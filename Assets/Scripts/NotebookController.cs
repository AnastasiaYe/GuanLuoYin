using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class NotebookController : MonoBehaviour
{
    [System.Serializable]
    public class Note
    {
        public string id;
        public string title;
        public string body;
        public Sprite icon;
        
        public Note(string id, string title, string body, Sprite icon = null)
        {
            this.id = id;
            this.title = title;
            this.body = body;
            this.icon = icon;
        }
    }
    
    [System.Serializable]
    public class NoteButton
    {
        public Button button;
        public TextMeshProUGUI titleText;
        public Image iconImage;
        public string noteId;
    }
    
    [Header("UI References")]
    [SerializeField] private CanvasGroup notebookPanel;
    [SerializeField] private Button toggleButton;
    [SerializeField] private Transform listContainer;
    [SerializeField] private GameObject noteButtonPrefab;
    
    
    [Header("Optional")]
    [SerializeField] private Image dimmer;
    
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    
    // Private fields
    private Dictionary<string, Note> notes = new Dictionary<string, Note>();
    private List<NoteButton> noteButtons = new List<NoteButton>();
    private string selectedNoteId;
    private bool isOpen = false;
    private Coroutine fadeCoroutine;
    private Keyboard keyboard;
    
    // Events
    public System.Action<string> OnNoteSelected;
    public System.Action OnNotebookOpened;
    public System.Action OnNotebookClosed;
    
    private void Start()
    {
        // Setup toggle button
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleNotebook);
        }
        
        // Initialize notebook as closed
        if (notebookPanel != null)
        {
            SetNotebookState(false, false);
        }
        
        // Get keyboard reference for Input System
        keyboard = Keyboard.current;
    }
    
    private void Update()
    {
        // Handle escape key using Input System
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame && isOpen)
        {
            CloseNotebook();
        }
    }
    
    #region Public API
    
    /// <summary>
    /// Toggle the notebook open/closed
    /// </summary>
    public void ToggleNotebook()
    {
        if (isOpen)
        {
            CloseNotebook();
        }
        else
        {
            OpenNotebook();
        }
    }
    
    /// <summary>
    /// Open the notebook
    /// </summary>
    public void OpenNotebook()
    {
        if (isOpen) return;
        
        isOpen = true;
        SetNotebookState(true, true);
        
        // Select most recent note if nothing is selected
        if (string.IsNullOrEmpty(selectedNoteId) && notes.Count > 0)
        {
            // Get the most recently added note (last in dictionary)
            string mostRecentId = null;
            foreach (var kvp in notes)
            {
                mostRecentId = kvp.Key;
            }
            SelectNote(mostRecentId);
        }
        
        // Start fade in
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeTo(1f));
        
        OnNotebookOpened?.Invoke();
    }
    
    /// <summary>
    /// Close the notebook
    /// </summary>
    public void CloseNotebook()
    {
        if (!isOpen) return;
        
        isOpen = false;
        
        // Start fade out
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeTo(0f, () => SetNotebookState(false, false)));
        
        OnNotebookClosed?.Invoke();
    }
    
    /// <summary>
    /// Add or update a note
    /// </summary>
    /// <param name="id">Unique identifier for the note</param>
    /// <param name="title">Note title</param>
    /// <param name="body">Note body text</param>
    /// <param name="icon">Optional icon sprite</param>
    public void AddOrUpdateNote(string id, string title, string body, Sprite icon = null)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("NotebookController: Note ID cannot be null or empty");
            return;
        }
        
        // Create or update note
        Note note = new Note(id, title, body, icon);
        notes[id] = note;
        
        // Update UI
        UpdateNoteButton(id);
        
        // If this is the first note or notebook is open, select it
        if (notes.Count == 1 || isOpen)
        {
            SelectNote(id);
        }
    }
    
    /// <summary>
    /// Remove a note by ID
    /// </summary>
    /// <param name="id">Note ID to remove</param>
    public void RemoveNote(string id)
    {
        if (notes.ContainsKey(id))
        {
            notes.Remove(id);
            RemoveNoteButton(id);
            
            // If we removed the selected note, select another one
            if (selectedNoteId == id)
            {
                selectedNoteId = null;
                if (notes.Count > 0)
                {
                    // Select the first available note
                    foreach (var kvp in notes)
                    {
                        SelectNote(kvp.Key);
                        break;
                    }
                }
                else
                {
                    selectedNoteId = null;
                }
            }
        }
    }
    
    /// <summary>
    /// Get a note by ID
    /// </summary>
    /// <param name="id">Note ID</param>
    /// <returns>Note if found, null otherwise</returns>
    public Note GetNote(string id)
    {
        return notes.ContainsKey(id) ? notes[id] : null;
    }
    
    /// <summary>
    /// Check if notebook is open
    /// </summary>
    /// <returns>True if open, false if closed</returns>
    public bool IsNotebookOpen()
    {
        return isOpen;
    }
    
    #endregion
    
    #region Private Methods
    
    private void SetNotebookState(bool visible, bool interactable)
    {
        if (notebookPanel == null)
        {
            Debug.LogWarning("NotebookController: NotebookPanel CanvasGroup not assigned");
            return;
        }
        
        notebookPanel.alpha = visible ? 1f : 0f; // 1f = fully opaque, 0f = transparent
        notebookPanel.interactable = interactable;
        notebookPanel.blocksRaycasts = interactable;
        
        // Handle dimmer
        if (dimmer != null)
        {
            dimmer.gameObject.SetActive(visible);
        }
    }
    
    private IEnumerator FadeTo(float targetAlpha, System.Action onComplete = null)
    {
        if (notebookPanel == null) yield break;
        
        float startAlpha = notebookPanel.alpha;
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            notebookPanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        
        notebookPanel.alpha = targetAlpha;
        onComplete?.Invoke();
    }
    
    private void UpdateNoteButton(string noteId)
    {
        if (!notes.ContainsKey(noteId)) return;
        
        Note note = notes[noteId];
        
        // Find existing button
        NoteButton existingButton = noteButtons.Find(b => b.noteId == noteId);
        
        if (existingButton != null)
        {
            // Update existing button
            UpdateButtonContent(existingButton, note);
        }
        else
        {
            // Create new button
            CreateNoteButton(note);
        }
    }
    
    private void CreateNoteButton(Note note)
    {
        if (noteButtonPrefab == null || listContainer == null)
        {
            Debug.LogWarning("NotebookController: NoteButtonPrefab or ListContainer not assigned");
            return;
        }
        
        GameObject buttonObj = Instantiate(noteButtonPrefab, listContainer);
        Button button = buttonObj.GetComponent<Button>();
        
        if (button == null)
        {
            Debug.LogWarning("NotebookController: NoteButtonPrefab must have a Button component");
            Destroy(buttonObj);
            return;
        }
        
        // Setup button
        button.onClick.AddListener(() => SelectNote(note.id));
        
        // Find text and icon components
        TextMeshProUGUI titleText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        Image iconImage = buttonObj.GetComponentInChildren<Image>();
        
        NoteButton noteButton = new NoteButton
        {
            button = button,
            titleText = titleText,
            iconImage = iconImage,
            noteId = note.id
        };
        
        UpdateButtonContent(noteButton, note);
        noteButtons.Add(noteButton);
    }
    
    private void UpdateButtonContent(NoteButton noteButton, Note note)
    {
        if (noteButton.titleText != null)
        {
            noteButton.titleText.text = note.title;
        }
        
        if (noteButton.iconImage != null)
        {
            if (note.icon != null)
            {
                noteButton.iconImage.sprite = note.icon;
                noteButton.iconImage.gameObject.SetActive(true);
            }
            else
            {
                noteButton.iconImage.gameObject.SetActive(false);
            }
        }
    }
    
    private void RemoveNoteButton(string noteId)
    {
        NoteButton buttonToRemove = noteButtons.Find(b => b.noteId == noteId);
        if (buttonToRemove != null)
        {
            noteButtons.Remove(buttonToRemove);
            if (buttonToRemove.button != null)
            {
                Destroy(buttonToRemove.button.gameObject);
            }
        }
    }
    
    private void SelectNote(string noteId)
    {
        if (!notes.ContainsKey(noteId)) return;
        
        selectedNoteId = noteId;
        Note note = notes[noteId];
        
        // Update button selection (optional visual feedback)
        UpdateButtonSelection();
        
        OnNoteSelected?.Invoke(noteId);
    }
    
    private void UpdateButtonSelection()
    {
        // Optional: Add visual feedback for selected button
        // This could change button colors, add selection indicators, etc.
        foreach (var noteButton in noteButtons)
        {
            if (noteButton.button != null)
            {
                // Example: Change button color based on selection
                ColorBlock colors = noteButton.button.colors;
                colors.normalColor = noteButton.noteId == selectedNoteId ? Color.yellow : Color.white;
                noteButton.button.colors = colors;
            }
        }
    }
    
    
    #endregion
    
    #region Inspector Setup Help
    
    /*
    INSPECTOR SETUP:
    
    1. Assign References:
       - NotebookPanel: The CanvasGroup on your root notebook panel
       - ToggleButton: The HUD button that opens/closes the notebook
       - ListContainer: The Content Transform inside your ScrollView
       - NoteButtonPrefab: A prefab with Button + TextMeshProUGUI + Image components
       - Dimmer: Image overlay for background dimming (optional)
    
    2. Settings:
       - FadeDuration: How long the fade animation takes (default 0.3s)
    
    3. Usage from other scripts:
       - notebookController.AddOrUpdateNote("note1", "My Note", "This is the note content");
       - notebookController.ToggleNotebook();
       - notebookController.OpenNotebook();
       - notebookController.CloseNotebook();
    */
    
    #endregion
}
