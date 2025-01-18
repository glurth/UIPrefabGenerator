# UIPrefabGenerator

This Unity script allows you to automatically generate reusable and customizable UI prefabs for your project. It supports both standard Unity UI and TextMeshPro UI components. The prefabs are saved in the specified Asset folder and are designed to ensure consistency across scenes, reducing manual updates.

## Purpose
The UIPrefabGenerator aligns with the **Prefab-Based UI Composition** workflow, a modular approach to UI design in Unity. This workflow emphasizes the creation of reusable prefab templates and their variants to achieve consistency, efficiency, and scalability in UI development.

### Prefab-Based UI Composition Workflow

>**Overview:**
>This workflow involves creating reusable prefabs for Unity UI components. These prefabs act as templates that are instantiated and customized as needed. Variants of these prefabs are created for specific use cases, ensuring consistency while allowing for tailored designs.
>Variants in Unity are prefabs that inherit properties from a base prefab but allow for specific overrides. Changes to the base prefab propagate to its variants, except for properties explicitly modified in the variant.

>**Workflow Steps:**
>1. **Design Base Prefabs**: Build foundational UI prefabs for common elements (e.g., buttons, panels, text fields). [This is what this package is good for.]
>2. **Create Variants**: Use prefab variants to customize specific properties or styles while retaining a link to the base prefab for easier updates.
>3. **Organize Prefabs**: Optionally, store prefabs in a structured hierarchy (e.g., categorized folders) for easy access and management.
>4. **Integrate into Canvas**: Populate your Unity Canvas by instantiating prefabs instead of building UI elements directly in the scene.
>5. **Update with Consistency**: Modify the base prefab to propagate changes across all instances and variants, simplifying large-scale updates.

>**Advantages:**
>- **Consistency**: Ensures a uniform look and feel across the UI, reducing design discrepancies.
>- **Efficiency**: Saves time by avoiding repetitive recreation of UI elements.
>- **Flexibility with Variants**: Allows for quick customization while maintaining a connection to the original prefab.
>- **Ease of Maintenance**: Updating the base prefab automatically reflects changes across all instances and variants.
>- **Collaboration-Friendly**: Teams can work with pre-defined prefabs, reducing the risk of unaligned designs or functionality.
>- **Scalability**: Facilitates faster iteration and expansion of UI as the project grows.

## Features
- **Automatic Prefab Generation**: Generates prefabs for commonly used UI elements such as Text, Buttons, Input Fields, Dropdowns, and more.
- **Variants Support**: Creation of some prefab variants, such as TitleText, LabelText, etc., with different properties.
- **TextMeshPro Support**: Can create TextMeshPro-based UI prefabs, or legacy.
- **Reusable Components**: Changes made to the base prefab will propagate across all variants and instances, ensuring uniformity in the UI.

## How It Works
1. **Prefab Paths**: The prefabs are stored in two main directories:
   - Standard UI prefabs: `Assets/Prefabs/UI`
   - TextMeshPro UI prefabs: `Assets/Prefabs/UI_TextMeshPro`

2. **Base Prefabs**: Base UI prefabs are created and saved in the appropriate directory. These base prefabs serve as templates for creating variant prefabs.  Those UI Controls that use other controls (like an ``InputField`` uses a ``Text``), will have those internal control replaced with a prefab (or variant). This will allow changes to the base text prefabs to work for say.. Buttons.

3. **Prefab Variants**: Prefab variants such as `PlaceholderTextPrefab`, `TitleTextPrefab`, `LabelTextPrefab` are automatically generated from the base prefab with specific changes (e.g., font size, alignment, color).  Custom prefab Variants can of course be createdat any time.

4. **UI Components**: The script automatically generates and customizes various UI elements like:
   - `InputField`
   - `Button`
   - `Dropdown`
   - And their respective TextMeshPro variants

5. **Customization**: For each UI component, specific changes are applied, such as setting text properties or replacing components with the appropriate prefab variants. The user can create additional prefab variants using any of the generated prefabs, normally in the Unity editor.

## Usage

Either option may be used, but you probably don't want to use both.

1. **Generate Standard UI Prefabs**:
   - Open Unity Editor.
   - Go to `Tools > Generate UI Prefabs` to generate and customize the standard UI prefabs.
   - This will create the prefabs in the `Assets/Prefabs/UI` directory.

2. **Generate TextMeshPro UI Prefabs**:
   - Go to `Tools > Generate TextMeshPro UI Prefabs` to generate and customize the TextMeshPro UI prefabs.
   - The prefabs will be saved in the `Assets/Prefabs/UI_TextMeshPro` directory.
