# UIPrefabGenerator

This Unity script allows you to automatically generate reusable and customizable UI prefabs for your project. It supports both standard Unity UI and TextMeshPro UI components. The prefabs are saved in the specified Asset folder and are designed to ensure consistency across scenes, reducing manual updates.

## Features
- **Automatic Prefab Generation**: Generates prefabs for commonly used UI elements such as Text, Buttons, Input Fields, Dropdowns, and more.
- **Variants Support**: Creation of prefab variants, such as TitleText, LabelText, etc., with different properties.
- **TextMeshPro Support**: Can create TextMeshPro-based UI prefabs, or legacy.
- **Reusable Components**: Changes made to the base prefab will propagate across all variants and instances, ensuring uniformity in the UI.

## How It Works
1. **Prefab Paths**: The prefabs are stored in two main directories:
   - Standard UI prefabs: `Assets/Prefabs/UI`
   - TextMeshPro UI prefabs: `Assets/Prefabs/UI_TextMeshPro`

2. **Base Prefabs**: Base UI prefabs are created and saved in the appropriate directory. These base prefabs serve as templates for creating variant prefabs.

3. **Prefab Variants**: Prefab variants such as `PlaceholderTextPrefab`, `TitleTextPrefab`, `LabelTextPrefab` are generated from the base prefab with specific changes (e.g., font size, alignment, color).

4. **UI Components**: The script automatically generates and customizes various UI elements like:
   - `InputField`
   - `Button`
   - `Dropdown`
   - And their respective TextMeshPro variants

5. **Customization**: For each UI component, specific changes are applied, such as setting text properties or replacing components with the appropriate prefab variants.  The user can create additional prefab variants using any of the generated prefabs, normally in the unity editor.

## Usage

Either option may be used, but you probably dont want to use both.

1. **Generate Standard UI Prefabs**:
   - Open Unity Editor.
   - Go to `Tools > Generate UI Prefabs` to generate and customize the standard UI prefabs.
   - This will create the prefabs in the `Assets/Prefabs/UI` directory.

2. **Generate TextMeshPro UI Prefabs**:
   - Go to `Tools > Generate TextMeshPro UI Prefabs` to generate and customize the TextMeshPro UI prefabs.
   - The prefabs will be saved in the `Assets/Prefabs/UI_TextMeshPro` directory.
