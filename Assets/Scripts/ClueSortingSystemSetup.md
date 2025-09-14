# Clue Sorting System Setup Guide

This guide explains how to set up and use the new clue sorting system with categories and colors.

## Overview

The enhanced clue system now includes:
- **Four Categories**: Name (Blue), Location (Green), Object (Yellow), Action (Magenta)
- **Customizable Background Colors**: Each category background color can be adjusted in the inspector
- **Color Coding**: Each category has distinct background colors for easy identification
- **Automatic Sorting**: Clues can be sorted by category, title, or time earned
- **Visual Indicators**: Colored backgrounds on clue tokens

## Quick Setup (5 minutes)

### 1. Update Your Clue Prefab
Your clue token prefab needs a **Background Image** for the colored background:
- **Background Image**: An Image component for the colored background (required)

#### Clue Token Prefab Structure:
```
ClueToken (GameObject)
├── Background (Image) - Set to "Background Image" in ClueToken component
├── Icon (Image) - Set to "Icon Image" in ClueToken component  
├── Title (TextMeshProUGUI) - Set to "Title Text" in ClueToken component
└── Description (TextMeshProUGUI) - Set to "Description Text" in ClueToken component
```

### 2. Configure ClueManager
1. **Enable Auto Sorting**: Check "Enable Auto Sorting" (default: true)
2. **Set Sorting Method**: Choose "By Category" (default) for grouped display
3. **Assign UI References**: Make sure clueScrollContent and clueTokenPrefab are assigned
4. **Customize Background Colors**: Adjust the "Color Settings" section to change background colors

### 3. Set Clue Categories
In your ClueManager's "All Clues" list, set the category for each clue:
- **Name**: Character names, person information (Blue)
- **Location**: Places, rooms, areas (Green)  
- **Object**: Physical items, evidence, objects (Yellow)
- **Action**: Actions, events, activities (Magenta)

### 4. Add Sorting UI (Optional)
Create a ClueSortingUI GameObject for manual sorting controls:
1. Create empty GameObject named "ClueSortingUI"
2. Add `ClueSortingUI` component
3. Assign button references for sort options
4. Assign TextMeshProUGUI for current sort display

## Background Colors

| Category | Default Background Color | Usage |
|----------|-------------------------|-------|
| **Name** | Light Blue (RGBA: 0.3, 0.3, 1, 0.8) | Character names, person information |
| **Location** | Light Green (RGBA: 0.3, 1, 0.3, 0.8) | Places, rooms, areas |
| **Object** | Light Yellow (RGBA: 1, 1, 0.3, 0.8) | Physical items, evidence, objects |
| **Action** | Light Magenta (RGBA: 1, 0.3, 1, 0.8) | Actions, events, activities |

### Customizing Background Colors
You can change any category background color in the ClueManager inspector:
1. **Select your ClueManager** in the scene
2. **Expand "Color Settings"** section
3. **Adjust the background color pickers** for each category
4. **Right-click ClueManager** → "Refresh All Clue Colors" to apply changes

## Sorting Methods

### By Category (Default)
- Groups clues by category: Name → Location → Object
- Within each category, sorts alphabetically by title
- Best for organized investigation

### By Title
- Sorts all clues alphabetically by title
- Ignores categories
- Best for finding specific clues

### By Earned Time
- Shows newest clues first
- Maintains the order clues were discovered
- Best for chronological investigation

## Code Examples

### Creating Clues with Categories
```csharp
// Create clues with specific categories
ClueData nameClue = new ClueData("character_alice", "Alice", "A mysterious character", null, ClueCategory.Name);
ClueData locationClue = new ClueData("room_library", "Library", "A quiet reading room", null, ClueCategory.Location);
ClueData objectClue = new ClueData("book_evidence", "Suspicious Book", "A book with strange markings", null, ClueCategory.Object);
```

### Manual Sorting Control
```csharp
// Get ClueManager reference
ClueManager clueManager = ClueManager.Instance;

// Change sorting method
clueManager.SetSortingMethod(ClueSortingMethod.ByCategory);

// Get clues grouped by category
Dictionary<ClueCategory, List<ClueData>> categorizedClues = clueManager.GetCluesByCategory();
```

### Custom Background Colors
```csharp
// Get background color
Color nameBackground = ClueCategoryColors.GetBackgroundColor(ClueCategory.Name);
```

## UI Setup Details

### Clue Token Prefab Requirements
1. **Background Image**: Should be the main background of the token
2. **Category Text**: Should be positioned to show the category name
3. **All Text Components**: Should use appropriate fonts and sizes

### ClueSortingUI Setup
1. **Sort Buttons**: Create buttons for each sorting method
2. **Current Sort Label**: Text component to show active sorting method
3. **Button Colors**: Set active/inactive colors for visual feedback

## Migration from Old System

If you have existing clues:
1. **Existing clues will default to "Object" category**
2. **Colors will be applied automatically**
3. **No changes needed to existing clue data**
4. **Update clue prefab to include new UI elements**

## Testing

### Test Categories
1. Create clues with different categories
2. Grant clues and observe color coding
3. Check that sorting works correctly

### Test Sorting
1. Use context menu on ClueManager: "Test Grant First Clue"
2. Use ClueSortingUI buttons to change sorting
3. Verify clues rearrange correctly

### Debug Features
- **ClueManager Context Menu**: "Test Grant First Clue"
- **ClueSortingUI Context Menu**: Test individual sorting methods
- **Console Logging**: Enable debug logging to see sorting events

## Common Issues

### Clues Not Showing Colors
- Check that ClueToken prefab has Background Image and Category Text assigned
- Verify ClueCategoryColors script is in the project

### Sorting Not Working
- Ensure "Enable Auto Sorting" is checked in ClueManager
- Check that clueScrollContent is properly assigned
- Verify ClueToken components are properly set up

### UI Layout Issues
- Make sure VerticalLayoutGroup is on clueScrollContent
- Check that ContentSizeFitter is configured correctly
- Verify all UI elements have proper RectTransform settings

This system provides organized, color-coded clue management that makes investigation more intuitive and visually appealing!
