# 🎮 Clue System Documentation

## Overview
A comprehensive drag-and-drop clue system with category-based organization, stage visibility, and game completion detection.

## 📋 Basic Systems & Functions

### 1. ClueManager System
**What it does**: Central hub that manages all clues in your game
**Main functions**:
- Stores all available clues in a database
- Creates draggable clue tokens when clues are earned
- Handles clue sorting (by category, title, or time earned)
- Manages the scrollable UI area where clue tokens appear

**How it works**: When a clue is earned, it creates a visual token that players can drag around. It keeps track of which clues have been found and organizes them in the UI.

### 2. ClueSlot System
**What it does**: Drop zones where players place specific clues
**Main functions**:
- Accepts only the correct clue (rejects wrong ones)
- Shows the clue's title when filled
- Changes color based on the clue's category
- Provides visual and audio feedback

**How it works**: Each slot "expects" a specific clue. When you drag a clue token onto it, it checks if it's the right one. If yes, it snaps into place and shows the clue title.

### 3. ClueData System
**What it does**: Defines what each clue contains
**Main functions**:
- Stores clue information (title, description, icon, category)
- Categorizes clues (Name, Location, Object, Action)
- Provides unique identification for each clue

**How it works**: Each clue is a data structure that contains all the information needed to create and display the clue token.

### 4. ClueToken System
**What it does**: Visual representation of clues that players can interact with
**Main functions**:
- Displays clue information (icon, title, description)
- Enables drag and drop functionality
- Shows category-based colors
- Provides visual feedback during interaction

**How it works**: When a clue is earned, this creates a draggable UI element that players can move around the screen.

### 5. GameCompletionManager System
**What it does**: Monitors game progress and triggers completion
**Main functions**:
- Watches all clue slots to see if they're filled
- Triggers scene transition when all slots are complete
- Provides completion statistics and progress tracking

**How it works**: It continuously checks if all required clue slots are filled with correct clues. When they are, it automatically transitions to the end scene.

### 6. StageVisibilityController System
**What it does**: Controls when objects appear/disappear based on game stages
**Main functions**:
- Shows/hides objects during specific game stages
- Provides smooth fade animations
- Supports flexible visibility rules

**How it works**: Objects can be configured to only appear during certain stages of the game, creating dynamic environments.

---

## 🔧 How to Modify the Systems

### Modifying ClueManager

#### Basic Setup
1. Create empty GameObject → Add `ClueManager` script
2. Assign `clueScrollContent` (RectTransform of your scroll area)
3. Assign `clueTokenPrefab` (your clue token prefab)
4. Add clues to "All Clues" list in inspector

#### Adding New Clues
1. In ClueManager inspector, expand "All Clues"
2. Click "+" to add new clue
3. Fill in:
   - **ID**: Unique identifier (e.g., "clue_knife")
   - **Title**: Display name (e.g., "Bloody Knife")
   - **Description**: Detailed info (e.g., "A knife with bloodstains")
   - **Icon**: Sprite image for the clue
   - **Category**: Choose from Name/Location/Object/Action

#### Changing Sort Order
```csharp
// In inspector or code
sortingMethod = ClueSortingMethod.ByCategory; // Groups by category
sortingMethod = ClueSortingMethod.ByTitle;    // Alphabetical
sortingMethod = ClueSortingMethod.ByEarnedTime; // Order earned
```

### Modifying ClueSlots

#### Basic Setup
1. Create UI Image → Add `ClueSlot` script
2. Set `expectedClueId` to match a clue's ID or title
3. Set `expectedCategory` to the clue's category
4. Assign `slotImage` reference to the Image component

#### Adding Title Display
1. Create child TextMeshPro component
2. Position it where you want the title to appear
3. Assign it to `titleText` field in ClueSlot
4. Enable `showTitleWhenFilled`

#### Customizing Colors
**Method 1: Use Category Colors (Automatic)**
```csharp
useCategoryColors = true;  // Uses default category colors
```

**Method 2: Custom Colors**
```csharp
useCategoryColors = true;
useCustomColors = true;
customNameColor = Color.blue;
customLocationColor = Color.green;
// etc.
```

**Method 3: Default Colors**
```csharp
useCategoryColors = false;
emptyColor = Color.white;
filledColor = Color.green;
```

#### Color Intensity Control
```csharp
emptyColorIntensity = 0.3f;   // How faded when empty (0-1)
filledColorIntensity = 0.8f;  // How solid when filled (0-1)
```

### Modifying Game Completion

#### Basic Setup
1. Create empty GameObject → Add `GameCompletionManager` script
2. Set `endSceneName` to your end scene name
3. Configure `transitionDelay` (seconds before scene change)
4. Set `completionMessage` text

#### Customizing Completion
```csharp
transitionDelay = 2f;  // Wait 2 seconds before scene change
showCompletionMessage = true;  // Show completion popup
completionMessage = "Congratulations! All clues found!";
```

### Modifying Stage Visibility

#### Basic Setup
1. Add `StageVisibilityController` script to any GameObject
2. Configure `visibleStages` list (which stages to show in)
3. Set `visibilityMode` (show in listed stages or hide in listed stages)

#### Stage Configuration Examples
```csharp
// Show object only in stages 1 and 3
visibleStages = [1, 3];
visibilityMode = VisibilityMode.ShowInListedStages;

// Hide object in stages 0 and 4
visibleStages = [0, 4];
visibilityMode = VisibilityMode.HideInListedStages;
```

#### Adding Animations
```csharp
animateTransitions = true;     // Enable fade in/out
animationDuration = 1f;        // 1 second fade
fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);  // Smooth curve
```

---

## 🎨 Visual Customization Options

### Default Category Colors
- **Name** (Blue): `RGB(0.3, 0.3, 1.0)`
- **Location** (Green): `RGB(0.3, 1.0, 0.3)`
- **Object** (Yellow): `RGB(1.0, 1.0, 0.3)`
- **Action** (Magenta): `RGB(1.0, 0.3, 1.0)`

### Preset Color Schemes
Right-click on ClueSlot → Context Menu:
- **Apply Bright Color Scheme**: Vibrant, saturated colors
- **Apply Pastel Color Scheme**: Soft, muted colors
- **Apply Dark Color Scheme**: Dark, deep colors
- **Reset to Default Colors**: Restore original colors

---

## 🎮 Runtime Control Examples

### Granting Clues Programmatically
```csharp
ClueManager clueManager = FindObjectOfType<ClueManager>();
clueManager.GrantClue("clue_id_here", true); // true = open notebook
```

### Checking Clue Status
```csharp
bool hasClue = clueManager.HasClue("clue_id");
ClueData clueData = clueManager.GetClue("clue_id");
List<string> earnedClues = clueManager.GetEarnedClues();
```

### Modifying Slots at Runtime
```csharp
ClueSlot slot = GetComponent<ClueSlot>();
slot.SetExpectedCategory(ClueCategory.Action);
slot.SetUseCustomColors(true);
slot.SetCustomColor(ClueCategory.Name, Color.cyan);
slot.SetShowTitle(false);
```

### Stage Control
```csharp
StageManager stageManager = StageManager.Instance;
stageManager.ChangeStage(2);           // Go to stage 2
stageManager.ChangeToNextStage();      // Next stage
stageManager.PauseAutoStageChange();   // Pause auto progression
```

### Game Completion Events
```csharp
void Start()
{
    GameCompletionManager.Instance.OnGameCompleted += OnGameCompleted;
    GameCompletionManager.Instance.OnAllSlotsFilled += OnAllSlotsFilled;
}

void OnGameCompleted() => Debug.Log("Game completed!");
void OnAllSlotsFilled() => Debug.Log("All slots filled!");
```

---

## 🔍 Common Issues & Solutions

### Clues Won't Drag
- Check ClueToken script is attached
- Verify Canvas has GraphicRaycaster
- Ensure proper Canvas sorting order

### Slots Reject Clues
- Verify expectedClueId matches clue ID/title
- Check IsCorrectClue() method
- Ensure IDropHandler implementation

### Colors Not Showing
- Assign backgroundImage in ClueToken
- Enable useCategoryColors in ClueSlot
- Verify ClueCategoryColors settings

### Game Won't Complete
- Check GameCompletionManager in scene
- Verify endSceneName is correct
- Ensure all slots are filled
- Check debug logs

## Debug Tools

### Right-Click Context Menus
- **ClueManager**: Test Grant First Clue, Debug Item States
- **ClueSlot**: Debug Expected Clue, Debug Slot Colors, Apply Color Schemes
- **GameCompletionManager**: Test Complete Game, Debug All Slots
- **StageVisibilityController**: Force Update Visibility, Test All Stages

### Runtime Commands
```csharp
// Grant clue programmatically
clueManager.GrantClue("clue_id", true);

// Check completion status
bool completed = gameCompletionManager.AreAllSlotsFilled();

// Change stage
stageManager.ChangeStage(2);

// Modify slot at runtime
slot.SetExpectedCategory(ClueCategory.Action);
slot.SetUseCustomColors(true);
```

## API Reference

### ClueManager
```csharp
void GrantClue(string clueId, bool openNotebook = false)
bool HasClue(string clueId)
ClueData GetClue(string clueId)
void SetSortingMethod(ClueSortingMethod method)
```

### ClueSlot
```csharp
bool IsFilled()
bool HasCorrectClue()
void SetExpectedCategory(ClueCategory category)
void SetCustomColor(ClueCategory category, Color color)
```

### GameCompletionManager
```csharp
bool AreAllSlotsFilled()
(int filled, int correct, int total) GetCompletionStats()
void ResetCompletionState()
```

### StageVisibilityController
```csharp
void SetVisibility(bool visible)
void AddVisibleStage(int stage)
void SetVisibleStages(List<int> stages)
```

## Performance Tips

1. **Cache References**: Avoid repeated FindObjectOfType calls
2. **Batch Updates**: Group UI updates together
3. **Use Events**: Subscribe/unsubscribe properly in OnDestroy
4. **Profile Regularly**: Monitor frame rates and memory

## Best Practices

1. **Naming**: Use clear, descriptive names for clues and slots
2. **Organization**: Group related clues by category
3. **Testing**: Test thoroughly with debug tools
4. **Validation**: Always check for null references

## Support

### Debug Commands
```csharp
// Enable debug logging
component.enableDebugLogging = true;

// Force updates
component.ForceUpdateVisibility();
```

### Common Issues
1. Check component references are assigned
2. Verify scene hierarchy setup
3. Enable debug logging for detailed info
4. Use context menu debug tools

---

*Version 1.0 - Unity 2022.3+ Compatible*