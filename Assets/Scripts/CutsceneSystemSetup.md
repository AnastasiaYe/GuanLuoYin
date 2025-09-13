# Cutscene System Setup Guide

## Overview
A cutscene system that automatically triggers after completing all 5 stages (stage 4 -> stage 0) and then returns to stage 1.

## Components

### 1. CutsceneManager.cs
- Manages cutscene playback and timing
- Automatically triggers after stage cycle completion
- Handles UI, audio, and skip functionality

### 2. Enhanced StageManager.cs
- Added `OnStageCycleCompleted` event
- Detects when completing full stage cycle (4 -> 0)

## Setup Instructions

### Step 1: Create Cutscene Manager
1. **Create empty GameObject** named "CutsceneManager"
2. **Add `CutsceneManager` component**
3. **Configure settings:**
   - `Enable Cutscenes`: ✅ (enabled by default)
   - `Cutscene Duration`: 5 seconds (adjustable)
   - `Cutscene Title`: "Stage Complete!"
   - `Cutscene Description`: "All stages have been completed. Returning to the beginning..."

### Step 2: Create Cutscene UI Canvas
1. **Create Canvas** named "CutsceneCanvas"
2. **Set Canvas settings:**
   - Render Mode: Screen Space - Overlay
   - Sort Order: 100 (above other UI)
   - Scale With Screen Size: ✅

### Step 3: Setup Cutscene UI Elements
1. **Create background Image** (child of CutsceneCanvas)
   - Name: "Background"
   - Color: Black with 80% alpha
   - Stretch to fill entire canvas

2. **Create title Text** (child of CutsceneCanvas)
   - Name: "TitleText"
   - Use TextMeshPro component
   - Font Size: 48
   - Color: White
   - Position: Top center

3. **Create description Text** (child of CutsceneCanvas)
   - Name: "DescriptionText"
   - Use TextMeshPro component
   - Font Size: 24
   - Color: Light Gray
   - Position: Center

4. **Create skip Button** (child of CutsceneCanvas)
   - Name: "SkipButton"
   - Text: "Skip (ESC)"
   - Position: Bottom right
   - Size: 150x50

### Step 4: Wire Up References
1. **In CutsceneManager inspector:**
   - Drag `CutsceneCanvas` to `Cutscene Canvas` field
   - Drag `Background` image to `Background Image` field
   - Drag `TitleText` to `Title Text` field
   - Drag `DescriptionText` to `Description Text` field
   - Drag `SkipButton` to `Skip Button` field

### Step 5: Optional - Add Audio
1. **Add AudioClip** to `Cutscene Music` field for background music
2. **Add AudioClip** to `Cutscene Sound Effect` field for sound effects
3. **AudioSource** will be automatically created

### Step 6: Optional - Custom Content
1. **Add background sprite** to `Cutscene Background` field
2. **Modify text content** in inspector or via script:
   ```csharp
   CutsceneManager.Instance.SetCutsceneContent(
       "Custom Title", 
       "Custom description...", 
       backgroundSprite
   );
   ```

## How It Works

### Automatic Triggering
1. **Stage progression**: 0 → 1 → 2 → 3 → 4 → 0
2. **Cycle detection**: When stage 4 completes and goes to stage 0
3. **Cutscene trigger**: `OnStageCycleCompleted` event fires
4. **Stage pausing**: Auto stage changes stop during cutscene
5. **Cutscene plays**: Shows UI, plays audio, waits for duration
6. **Return to gameplay**: Cutscene ends, auto stage changes resume at stage 0

### Manual Control
```csharp
// Start cutscene manually
CutsceneManager.Instance.StartCutscene();

// Skip current cutscene
CutsceneManager.Instance.SkipCutscene();

// Check if cutscene is active
bool isActive = CutsceneManager.Instance.IsCutsceneActive();

// Enable/disable cutscenes
CutsceneManager.Instance.SetCutscenesEnabled(false);

// Manual stage control (pauses auto changes during cutscenes)
StageManager.Instance.PauseAutoStageChange();
StageManager.Instance.ResumeAutoStageChange();
```

### Events
```csharp
// Subscribe to cutscene events
CutsceneManager.Instance.OnCutsceneStarted += () => Debug.Log("Cutscene started!");
CutsceneManager.Instance.OnCutsceneCompleted += () => Debug.Log("Cutscene completed!");
```

## Testing

### Debug Methods
- **Right-click CutsceneManager** → "Test Cutscene"
- **Right-click CutsceneManager** → "Skip Current Cutscene"
- **Right-click StageManager** → "Change to Next Stage" (to test cycle completion)

### Manual Testing
1. Set StageManager to stage 4
2. Use "Change to Next Stage" context menu
3. Cutscene should trigger automatically
4. Wait for duration or click Skip button

## Customization

### Timing
- **Cutscene Duration**: Adjust in inspector (default: 5 seconds)
- **Stage Change Interval**: Adjust in StageManager (default: 60 seconds)

### Content
- **Dynamic content**: Use `SetCutsceneContent()` method
- **Multiple cutscenes**: Create different CutsceneManager instances
- **Conditional cutscenes**: Check conditions in `OnStageCycleCompleted()`

### UI Styling
- **Background**: Change color, add images, animations
- **Text**: Custom fonts, colors, animations
- **Skip button**: Custom styling, different input methods

## Integration with Existing Systems

The cutscene system integrates seamlessly with:
- ✅ **StageManager**: Automatic triggering after cycle completion
- ✅ **CharacterManager**: Characters continue normal behavior
- ✅ **Clue System**: No interference with clue collection
- ✅ **Room System**: No impact on room switching
- ✅ **Item Interaction**: Paused during cutscene (can be modified)

## Troubleshooting

### Cutscene Not Triggering
- Check `Enable Cutscenes` is ✅
- Verify CutsceneManager is in scene
- Check StageManager is cycling properly
- Test with manual "Test Cutscene" method

### UI Not Showing
- Check Canvas Sort Order (should be 100+)
- Verify all UI references are assigned
- Check Canvas is active and enabled

### Audio Not Playing
- Check AudioClip assignments
- Verify AudioSource component exists
- Check audio volume settings
