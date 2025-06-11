# UIPrefabGenerator
This Unity script allows you to automatically generate reusable and customizable UI prefabs for your project. It supports both standard Unity UI and TextMeshPro UI components. The prefabs are saved in the specified Asset folder and are designed to ensure consistency across scenes, reducing manual updates.

## Instllation

In the unity editor, package manager, click to add a package from a git url:  https://github.com/glurth/UIPrefabGenerator.git

## Usage
Either option may be used, but you probably don't want to use both.

- **Generate Standard UI Prefabs**:
   - Open Unity Editor.
   - Go to `Tools > Generate UI Prefabs` to generate and customize the standard UI prefabs.
   - This will create the prefabs in the `Assets/Prefabs/UI` directory.

- **Generate TextMeshPro UI Prefabs**:
   - Go to `Tools > Generate TextMeshPro UI Prefabs` to generate and customize the TextMeshPro UI prefabs.
   - The prefabs will be saved in the `Assets/Prefabs/UI_TextMeshPro` directory.

## Purpose
The UIPrefabGenerator aligns with the **Prefab-Based UI Composition** workflow, a modular approach to UI design in Unity. This workflow emphasizes the creation of reusable prefab templates and their variants to achieve consistency, efficiency, and scalability in UI development.

### Prefab-Based UI Composition Workflow

**Overview:**
This workflow involves creating reusable prefabs for Unity UI components. These prefabs act as templates that are instantiated and customized as needed. Variants of these prefabs are created for specific use cases, ensuring consistency while allowing for tailored designs.
Variants in Unity are prefabs that inherit properties from a base prefab but allow for specific overrides. Changes to the base prefab propagate to its variants, except for properties explicitly modified in the variant.

**Advantages:**
- **Consistency**: Ensures a uniform look and feel across the UI, reducing design discrepancies.
- **Efficiency**: Saves time by avoiding repetitive recreation of UI elements.
- **Flexibility with Variants**: Allows for quick customization while maintaining a connection to the original prefab.
- **Ease of Maintenance**: Updating the base prefab automatically reflects changes across all instances and variants.
- **Collaboration-Friendly**: Teams can work with pre-defined prefabs, reducing the risk of unaligned designs or functionality.
- **Scalability**: Facilitates faster iteration and expansion of UI as the project grows.

## Features
- **Automatic Prefab Generation**: Generates prefabs for commonly used UI elements such as Text, Buttons, Input Fields, Dropdowns, and more.
- **Variants Support**: Creation of some prefab variants, such as TitleText, LabelText, etc., with different properties.
- **TextMeshPro Support**: Can create TextMeshPro-based UI prefabs, or legacy.
- **Reusable Components**: Changes made to the base prefab will propagate across all variants and instances, ensuring uniformity in the UI.

## How It Works
1. **Base Prefabs**: Base UI prefabs are created and saved in the appropriate directory. These base prefabs serve as templates for creating variant prefabs.  Those UI Controls that use other controls (like an ``InputField`` uses a ``Text``), will have those internal control replaced with a prefab (or variant). This will allow changes to the base text prefabs to work for say.. Buttons.

2. **Prefab Variants**: Prefab variants such as `PlaceholderTextPrefab`, `TitleTextPrefab`, `LabelTextPrefab` are automatically generated from the base prefab with specific changes (e.g., font size, alignment, color).  Custom prefab Variants can of course be createdat any time.

3. **UI Components**: The script automatically generates and customizes various UI elements like:
   - `InputField`
   - `Button`
   - `Dropdown`
   - And their respective TextMeshPro variants

5. **Customization**: For each UI component, specific changes are applied, such as setting text properties or replacing components with the appropriate prefab variants. The user can create additional prefab variants using any of the generated prefabs, normally in the Unity editor.

## Prefab Structure Overview

After running the generator, your project will contain a set of UI prefabs organized for easy use and extension. Here’s how the generated prefabs are structured:

### 1. **Prefab Directories**
- **Standard Unity UI Prefabs:**  
  - Location: `Assets/Prefabs/UI/Legacy/`
- **TextMeshPro UI Prefabs:**  
  - Location: `Assets/Prefabs/UI/TextMeshPro/`
- **Common Subfolders:**  
  - `TextVariants/` — contains specialized text prefab variants (e.g., title, label, tooltip).

### 2. **Base Prefabs**
- **Definition:** The foundational UI elements, intended to be used directly or as the parent for creating new prefab variants.
- **Examples:**
  - `BaseTextPrefab`
  - `InputFieldPrefab` or `InputFieldTMPPrefab`
  - `BaseButtonPrefab` or `BaseButtonTMPPrefab`
  - `DropdownPrefab` or `DropdownTMPPrefab`
  - `PanelPrefab`, `SliderPrefab`, `TogglePrefab`, etc.

### 3. **Prefab Variants**
- **Definition:** Prefabs derived from a base prefab, with specific property changes (e.g., font size, alignment, color).
- **Location:**  
  - `TextVariants/` subfolder within each UI type directory.
- **Examples:**
  - `TitleTextPrefab` — larger, bold, centered text for titles.
  - `SubtitleTextPrefab` — slightly smaller than title, bold, centered.
  - `BodyTextPrefab` — default body text style, left-aligned.
  - `ButtonTextPrefab` — text styling suitable for buttons.
  - `TooltipTextPrefab`, `TinyTextPrefab`, `FieldLabelTextPrefab`, etc.

### 4. **Relationships and Inheritance**
- Prefab Variants are children of their respective base prefabs.
- Changing a property on the base prefab (not overridden in a variant) will update all variants.
- If you override a property in a variant, future base changes to that property won’t propagate to that variant.
- Prefabs in Prefabs: some prefabs like an InputField, will use other prefabs like Text displays as part of its composition.  Note that these instances of the used prefab may be assigned custom property modifications.

### 5. **How to Use the Prefabs**
- **Direct Use:**  
  - Drag and drop any base or variant prefab into your Canvas.
- **Creating Your Own Variants:**  
  - Right-click any base or variant prefab in the Project window and select “Create Prefab Variant”.
  - Adjust only the properties you want to override (e.g., color, font size).

### 6. **Customizing Styles**
- **To update the look of all similar UI elements:**  
  - Modify the relevant base prefab (e.g., change font or color on `BaseTextPrefab`).
  - All variants and UI controls using that base will inherit the new style unless they have an override.
- **To create new prefab types:**  
  - Use Unity’s prefab system to create new variants from an existing base, and organize them within the same folder structure.

### 7. **Naming Conventions**
- **Base Prefabs:**  
  - `Base*Prefab` or `*Prefab` (e.g., `BaseTextPrefab`, `InputFieldPrefab`)
- **Variants:**  
  - Descriptive of their intended use (e.g., `TitleTextPrefab`, `ButtonTextPrefab`, `FieldLabelTextPrefab`)

---

**Tip:**  
If you’re unsure which prefab to use, start with a base prefab. If you need a specialized style (like a title or label), use the closest variant or create your own by duplicating or creating a prefab variant in Unity.

---

## Example Directory Structure
<pre>Assets/
+-- Prefabs/
    +-- UI/
        +-- Legacy/
        ¦   +-- BaseTextPrefab.prefab
        ¦   +-- InputFieldPrefab.prefab
        ¦   +-- TextVariants/
        ¦       +-- TitleTextPrefab.prefab
        ¦       +-- FieldLabelTextPrefab.prefab
        +-- TextMeshPro/
            +-- BaseTextPrefab.prefab
            +-- InputFieldTMPPrefab.prefab
            +-- TextVariants/
                +-- TitleTextPrefab.prefab
                +-- FieldLabelTextPrefab.prefab
</pre>

## License

All rights reserved.

No license is granted for use, modification, distribution, or any other purpose without prior written permission.

If you're an independent developer and would like to use this software, email glurth at gmail.com to request a license. I usually approve such requests for free.  Businesses may contact me for pricing.

## Contributions

While contributions are welcome, they cannot be used without your explicit written permission, as this project will remain proprietary software.