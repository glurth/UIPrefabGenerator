
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

// Using prefabs for Canvas-based UI allows for reusable, modular UI elements.
// Changes made to base prefabs propagate across all instances, ensuring consistency and reducing manual updates.
// Ideal for maintaining UI structure across scenes and improving performance by reusing elements.
// This class will automatically generate the most commonly used UI components as prefabs, and store them in the Asset folder.
// There are some variants of the baseText Prefab, for example, that have different properties.
// Changes to the base prefab will affect these variants (unless the variant has the changed property already overridden itself).
public class UIPrefabGenerator 
{
    private static string currentPreFabPath;
    private const string PrefabPath = "Assets/Prefabs/UI";
    private const string PrefabPathTMP = "Assets/Prefabs/UI_TextMeshPro";

    [MenuItem("Tools/Generate UI Prefabs")]
    public static void GenerateUIPrefabs()
    {
        // Ensure Prefab Directory Exists
        EnsureDirectoryExists(PrefabPath);
        currentPreFabPath = PrefabPath;

        #region TextBaseAndVariants
        // Create BaseTextPrefab
       // GameObject baseTextPrefab = CreateOrGetPreFabFromMenuNoChanges("GameObject/UI/Text", "BaseTextPrefab");
        string baseTextName = "BaseTextPrefab";
        GameObject baseTextPrefab;
        bool forceRecreateTextVariants = false;
        if (!TryGetPreFabAsset(baseTextName, out baseTextPrefab))
        {
            baseTextPrefab = CreateMenuObject("GameObject/UI/Text", baseTextName);
            baseTextPrefab = SaveAsPrefab(baseTextPrefab);
            forceRecreateTextVariants = true;
        }

        // Create PlaceholderTextPrefab

        string placeholderTextName = "PlaceholderTextPrefab";
        GameObject placeholderTextPrefab;
        if (forceRecreateTextVariants || !TryGetPreFabAsset(placeholderTextName, out placeholderTextPrefab))
        {
            placeholderTextPrefab = CreatePrefabInstance(baseTextPrefab, placeholderTextName);
            placeholderTextPrefab.GetComponent<Text>().color = Color.gray;
            placeholderTextPrefab = SaveAsPrefab(placeholderTextPrefab);
        }
        // Create TitleTextPrefab
        string titleTextName = "TitleTextPrefab";
        GameObject titleTextPrefab;
        if (forceRecreateTextVariants || !TryGetPreFabAsset(titleTextName, out titleTextPrefab))
        {
            titleTextPrefab = CreatePrefabInstance(baseTextPrefab, titleTextName);
            Text textComponent = titleTextPrefab.GetComponent<Text>();
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.fontSize += 2;
            titleTextPrefab = SaveAsPrefab(titleTextPrefab);
        }

        // Create LabelTextPrefab
        string labelTextName = "LabelTextPrefab";
        GameObject labelTextPrefab;
        if (forceRecreateTextVariants || !TryGetPreFabAsset(labelTextName, out labelTextPrefab))
        {
            labelTextPrefab = CreatePrefabInstance(baseTextPrefab, labelTextName);
            Text labelTextComponent = labelTextPrefab.GetComponent<Text>();
            labelTextComponent.alignment = TextAnchor.MiddleLeft;
            labelTextPrefab = SaveAsPrefab(labelTextPrefab);
        }
        #endregion

        GameObject inputFieldObj = CreateOrGetPreFabFromMenuWithChanges<InputField>("GameObject/UI/Input Field", "InputFieldPrefab",
                                    (inputField) =>
                                    {
                                        inputField.textComponent = ReplaceComponentsGO(inputField.textComponent, baseTextPrefab);
                                        inputField.placeholder = ReplaceComponentsGO(inputField.placeholder, placeholderTextPrefab);
                                    });
        GameObject buttonObj = CreateOrGetPreFabFromMenuWithChanges<Button>("GameObject/UI/Button", "BaseButtonPrefab",
                                (button) =>
                                {
                                    Text buttonText = button.gameObject.GetComponentInChildren<Text>();
                                    ReplaceComponentsGO(buttonText, titleTextPrefab);
                                });
        GameObject dropdownObj = CreateOrGetPreFabFromMenuWithChanges<Dropdown>("GameObject/UI/Dropdown", "DropdownPreFab",
                        (dropdown) =>
                        {
                            dropdown.captionText = ReplaceComponentsGO(dropdown.captionText, labelTextPrefab);
                            dropdown.itemText = ReplaceComponentsGO(dropdown.itemText, labelTextPrefab);
                        });

        GenerateNonTextUsingPreFabs();
        Debug.Log("UI Prefabs Generated and Customized!");
    }

    [MenuItem("Tools/Generate TextMeshPro UI Prefabs")]
    public static void GenerateTextMeshProPrefabs()
    {
        // Set a separate path for TextMeshPro prefabs
        EnsureDirectoryExists(PrefabPathTMP);
        currentPreFabPath = PrefabPathTMP;


        #region TestBaseAndVariants
        // Create BaseTextPrefab
        string baseTextName = "BaseTMPTextPrefab";
        GameObject baseTextPrefab;
        bool forceRecreateTextVariants = false;
        if (!TryGetPreFabAsset(baseTextName, out baseTextPrefab))
        {
            baseTextPrefab = CreateMenuObject("GameObject/UI/Text - TextMeshPro", baseTextName);
            TMPro.TextMeshProUGUI textComponent = baseTextPrefab.GetComponent<TMPro.TextMeshProUGUI>();
            textComponent.fontSize = 24;
            textComponent.color = Color.black;
            baseTextPrefab = SaveAsPrefab(baseTextPrefab);
            forceRecreateTextVariants = true;
        }

        // Create PlaceholderTextPrefab
        string placeholderTextName = "PlaceholderTMPTextPrefab";
        GameObject placeholderTextPrefab;
        if (forceRecreateTextVariants || !TryGetPreFabAsset(placeholderTextName, out placeholderTextPrefab))
        {
            placeholderTextPrefab = CreatePrefabInstance(baseTextPrefab, placeholderTextName);
            placeholderTextPrefab.GetComponent<TMPro.TextMeshProUGUI>().color = Color.gray;
            placeholderTextPrefab = SaveAsPrefab(placeholderTextPrefab);
        }

        // Create TitleTextPrefab
        string titleTextName = "TitleTMPTextPrefab";
        GameObject titleTextPrefab;
        if (forceRecreateTextVariants || !TryGetPreFabAsset(titleTextName, out titleTextPrefab))
        {
            titleTextPrefab = CreatePrefabInstance(baseTextPrefab, titleTextName);
            TMPro.TextMeshProUGUI textComponent = titleTextPrefab.GetComponent<TMPro.TextMeshProUGUI>();
            textComponent.alignment = TMPro.TextAlignmentOptions.Center;
            textComponent.fontSize += 2;
            titleTextPrefab = SaveAsPrefab(titleTextPrefab);
        }

        // Create LabelTextPrefab
        string labelTextName = "LabelTMPTextPrefab";
        GameObject labelTextPrefab;
        if (forceRecreateTextVariants || !TryGetPreFabAsset(labelTextName, out labelTextPrefab))
        {
            labelTextPrefab = CreatePrefabInstance(baseTextPrefab, labelTextName);
            TMPro.TextMeshProUGUI labelTextComponent = labelTextPrefab.GetComponent<TMPro.TextMeshProUGUI>();
            labelTextComponent.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
            labelTextPrefab = SaveAsPrefab(labelTextPrefab);
        }
        #endregion

        GameObject inputFieldObj = CreateOrGetPreFabFromMenuWithChanges<TMPro.TMP_InputField>("GameObject/UI/Input Field - TextMeshPro", "InputFieldTMPPrefab",
                                    (inputField) =>
                                    {
                                        inputField.textComponent = ReplaceComponentsGO(inputField.textComponent, baseTextPrefab);
                                        inputField.placeholder = ReplaceComponentsGO(inputField.placeholder, placeholderTextPrefab);
                                    });
        GameObject buttonObj = CreateOrGetPreFabFromMenuWithChanges<Button>("GameObject/UI/Button - TextMeshPro", "BaseButtonTMPPrefab",
                                    (button) =>
                                    {
                                        TMPro.TextMeshProUGUI buttonText = button.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                                        buttonText = ReplaceComponentsGO(buttonText, titleTextPrefab);
                                        buttonText.text = "Button";
                                    });
        GameObject dropdownObj = CreateOrGetPreFabFromMenuWithChanges<TMPro.TMP_Dropdown>("GameObject/UI/Dropdown - TextMeshPro", "DropdownTMPPreFab",
                                    (dropdown) =>
                                    {
                                        dropdown.captionText = ReplaceComponentsGO(dropdown.captionText, labelTextPrefab);
                                        dropdown.itemText = ReplaceComponentsGO(dropdown.itemText, labelTextPrefab);
                                    });

        GenerateNonTextUsingPreFabs();
        Debug.Log("UI Prefabs Generated and Customized!");
        Debug.Log("TextMeshPro UI Prefabs Generated and Customized!");
    }


    private static void GenerateNonTextUsingPreFabs()
    {
        GameObject panelObj = CreateOrGetPreFabFromMenuNoChanges("GameObject/UI/Panel", "PanelPrefab");
        GameObject scrollbarObj = CreateOrGetPreFabFromMenuNoChanges("GameObject/UI/Scrollbar", "ScrollbarPrefab");
        GameObject scrollViewObj = CreateOrGetPreFabFromMenuWithChanges<ScrollRect>("GameObject/UI/Scroll View", "ScrollViewPrefab",
                (scrollRect) =>
                {
                    scrollRect.horizontalScrollbar = ReplaceComponentsGO(scrollRect.horizontalScrollbar, scrollbarObj);
                    scrollRect.verticalScrollbar = ReplaceComponentsGO(scrollRect.verticalScrollbar, scrollbarObj);
                });
        GameObject sliderObj = CreateOrGetPreFabFromMenuNoChanges("GameObject/UI/Slider", "SliderPrefab");
    }

    /// <summary>
    /// Combines functions required to check for existing, and if not found create a new prefab using the menu item.
    /// </summary>
    /// <param name="menuPath"></param>
    /// <param name="nameToAssign"></param>
    /// <returns></returns>
    static GameObject CreateOrGetPreFabFromMenuNoChanges(string menuPath, string nameToAssign)
    {
        GameObject preFab;
        if (!TryGetPreFabAsset(nameToAssign, out preFab))
        {
            preFab = CreateMenuObject(menuPath, nameToAssign);
            preFab = SaveAsPrefab(preFab);
        }
        return preFab;
    }
    /// <summary>
    /// Combines functions required to check for existing, and if not found create a new prefab using the menu item.
    /// The last parameter contains a function that will be executed before the new object is saved.  The parameter passed in to this function will be the result of a GetComponent<MT> called upon the new GameObject.</MT>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="menuPath"></param>
    /// <param name="nameToAssign"></param>
    /// <param name="changesFunction"></param>
    /// <returns></returns>
    static GameObject CreateOrGetPreFabFromMenuWithChanges<T>(string menuPath, string nameToAssign, System.Action<T> changesFunction) where T:MonoBehaviour
    {
        GameObject preFab;
        if (!TryGetPreFabAsset(nameToAssign, out preFab))
        {
            preFab = CreateMenuObject(menuPath, nameToAssign);
            if (changesFunction != null)
            {
                T component = preFab.GetComponent<T>();
                changesFunction(component);
            }
            preFab = SaveAsPrefab(preFab);
        }
        return preFab;
    }


    /// <summary>
    /// Checks if the specified prefab exists in the currentPreFabPath, and populates the foundPreFab parameter with it.
    /// Returns true if a prefab is found, and false if not.
    /// </summary>
    /// <param name="nameOfPreFab"></param>
    /// <param name="foundPrefab">the found prefab, or null if none was found</param>
    /// <returns></returns>
    private static bool TryGetPreFabAsset(string nameOfPreFab,out GameObject foundPrefab)
    {
        foundPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{currentPreFabPath}/{nameOfPreFab}.prefab");
        return foundPrefab != null;
    }

    /// <summary>
    /// Create an object in the scene by invoking the unity GameObject Menu option. Once created, this object will be selected by unity, and that is how we get the reference returned. 
    /// WARNING: NO checks are performed to confirm the passed menuPath is valid, but an exception will be throw if no object is created.
    /// </summary>
    /// <param name="menuPath">pass in the full menu path e.g. "GameObject/UI/Text"</param>
    /// <param name="nameToAssign">name that will be assigned to the newly created game object</param>
    /// <returns></returns>
    private static GameObject CreateMenuObject(string menuPath, string nameToAssign)
    {
        GameObject previouslySelectedObject = Selection.activeGameObject;
        EditorApplication.ExecuteMenuItem(menuPath);
        GameObject menuCreatedObj = Selection.activeGameObject;
        if (menuCreatedObj == previouslySelectedObject)
            throw new System.Exception("Unexpected result: The Unity menu command ["+ menuPath + "] should have generated a new object, that unity would automatically select. But, the selected object has not changed implying a new object has not been created: Check the menu path for errors.");
        menuCreatedObj.name = nameToAssign;
        return (menuCreatedObj);
    }

    /// <summary>
    /// Copies the transform properties of the original into a new instance of a prefab, with the same parent.  When this replacement object is in place, the original object, the one being replaced, is destroyed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="original"></param>
    /// <param name="prefab"></param>
    /// <returns></returns>
    private static T ReplaceComponentsGO<T>(T original, GameObject prefab) where T : MonoBehaviour
    {
        if (original == null) return null;

        // Instantiate the new prefab as a replacement
        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        newObj.transform.SetParent(original.transform.parent, false);
        newObj.name = original.gameObject.name;

        // Copy RectTransform properties
        RectTransform originalRect = original.GetComponent<RectTransform>();
        RectTransform newRect = newObj.GetComponent<RectTransform>();
        if (originalRect != null && newRect != null)
        {
            newRect.anchoredPosition = originalRect.anchoredPosition;
            newRect.sizeDelta = originalRect.sizeDelta;
            newRect.anchorMin = originalRect.anchorMin;
            newRect.anchorMax = originalRect.anchorMax;
            newRect.pivot = originalRect.pivot;
        }

        // Destroy the original GameObject
        GameObject.DestroyImmediate(original.gameObject);

        // Return the requested component from the new GameObject
        return newObj.GetComponent<T>();
    }

    /// <summary>
    /// Saves the provided object as a prefab.  The provided object is then Destroyed, and a reference to the new prefab is returned.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static GameObject SaveAsPrefab(GameObject obj)
    {
        GameObject preFab = PrefabUtility.SaveAsPrefabAssetAndConnect(obj, $"{currentPreFabPath}/{obj.name}.prefab", InteractionMode.UserAction);
        Object.DestroyImmediate(obj);
        return preFab;
    }


    /// <summary>
    /// creates a scene instance of the prefab- it will still need to be saved as a new prefab using SaveAsPrefab, to create a variant prefab.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="variantName"></param>
    /// <returns></returns>
    private static GameObject CreatePrefabInstance(GameObject original, string variantName)
    {
        GameObject originalInstance = (GameObject)PrefabUtility.InstantiatePrefab(original);
        originalInstance.name = variantName;
        return originalInstance;
    }

    private static void EnsureDirectoryExists(string fullPath)
    {
        string[] folders = fullPath.Split('/');
        string currentPath = "";

        for (int i = 0; i < folders.Length; i++)
        {
            string folder = folders[i];
            if (string.IsNullOrEmpty(currentPath))
            {
                currentPath = folder;
            }
            else
            {
                currentPath = $"{currentPath}/{folder}";
            }

            if (!AssetDatabase.IsValidFolder(currentPath))
            {
                string parentPath = i > 0 ? string.Join("/", folders, 0, i) : "Assets";
                AssetDatabase.CreateFolder(parentPath, folder);
            }
        }
    }
}
