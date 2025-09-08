# Clue System Setup Guide

## Overview
A robust clue system that allows players to earn clues from items and drag them onto notebook slots. Optimized for performance and ease of use.

## Components

### 1. ClueData.cs
- Lightweight data structure for clue information
- Contains: id, title, description, icon, isEarned

### 2. ClueManager.cs
- Central manager for all clues
- Handles granting clues and creating tokens
- **Setup**: Add to a GameObject in your scene

### 3. ClueToken.cs
- Optimized drag & drop functionality
- **Setup**: Add to your ClueTokenPrefab

### 4. ClueSlot.cs
- Defines slots where clues can be dropped
- **Setup**: Add to each slot GameObject on your notebook page

### 5. ClueGiver.cs
- Grants clues when items are clicked (one-time only)
- **Setup**: Add to clickable items that should give clues

## Setup Instructions

### Step 1: Create Clue Database
1. **Select ClueManager** in the scene
2. **In Inspector**, expand "Clue Database"
3. **Add clues** by setting the list size and filling in:
   - **Id**: Unique identifier (e.g., "clue_001")
   - **Title**: Display name (e.g., "Mysterious Key")
   - **Description**: Clue text (e.g., "A rusty key found under the table")
   - **Icon**: Optional sprite for the clue
   - **Is Earned**: Leave unchecked (managed automatically)

### Step 2: Setup ClueManager
1. **Create empty GameObject** named "ClueManager"
2. **Add ClueManager component**
3. **Assign references**:
   - **Clue Scroll Content**: The Content Transform of your scroll view
   - **Clue Token Prefab**: Your ClueTokenPrefab

### Step 3: Create ClueTokenPrefab
1. **Create UI Image** as base
2. **Add ClueToken component**
3. **Add child TextMeshProUGUI** for title
4. **Add child Image** for icon (optional)
5. **Add CanvasGroup** for drag feedback
6. **Add GraphicRaycaster** if not on Canvas
7. **Save as prefab**

### Step 4: Setup Clue Slots
1. **For each slot** on your notebook page:
2. **Add ClueSlot component**
3. **Set Expected Clue Id** to match a clue from your database
4. **Assign Snap Point** (optional, uses transform if null)
5. **Assign Slot Image** for visual feedback

### Step 5: Setup Clue Givers
1. **For each item** that should give clues:
2. **Add ClueGiver component**
3. **Set Clue Id** to match a clue from your database
4. **Set Open Notebook On Grant** (optional)
5. **Assign Visual Feedback** (optional)

## Usage Examples

### Granting Clues from Code
```csharp
// Get clue manager
ClueManager clueManager = FindObjectOfType<ClueManager>();

// Grant a clue
clueManager.GrantClue("clue_001", true); // true = open notebook

// Check if clue is earned
bool hasClue = clueManager.HasClue("clue_001");
```

### Setting Up Items
1. **Add ItemBehaviour** to clickable item
2. **Add ClueGiver** to same item
3. **Set Clue Id** in ClueGiver
4. **Item will grant clue** when clicked (one time only)

### Custom Clue Givers
```csharp
// In your custom script
ClueGiver clueGiver = GetComponent<ClueGiver>();
if (clueGiver != null)
{
    clueGiver.GrantClue();
}
```

## Features

### ✅ Clue Management
- **One-time earning**: Clues can only be earned once per item
- **No duplicates**: Same clue won't appear twice in scroll view
- **Persistent**: Clues stay earned throughout the game

### ✅ Drag & Drop
- **Smooth dragging**: Works with Canvas scaling
- **Visual feedback**: Token becomes semi-transparent while dragging
- **Snap to slots**: Tokens snap into correct slots
- **Return on miss**: Tokens return to scroll view if dropped incorrectly

### ✅ Slot System
- **Specific matching**: Each slot expects a specific clue ID
- **Visual feedback**: Slots change color when filled
- **One-time use**: Filled slots can't be used again

### ✅ Integration
- **Item interaction**: Works with existing ItemBehaviour system
- **Notebook integration**: Can open notebook when clue is granted
- **Event system**: Fires events when clues are earned/placed

## Troubleshooting

### Clue not appearing
- Check if ClueManager has the clue in its database
- Verify ClueTokenPrefab is assigned
- Check if ClueScrollContent is assigned

### Drag & drop not working
- Ensure ClueToken has ClueToken component
- Check if Canvas has GraphicRaycaster
- Verify ClueSlot has ClueSlot component

### Clue not snapping to slot
- Check if Expected Clue Id matches the clue's ID
- Verify slot is not already filled
- Ensure ClueSlot is on the correct GameObject

### Item not granting clues
- Check if ClueGiver component is added
- Verify Clue Id is set correctly
- Ensure ClueManager exists in scene
