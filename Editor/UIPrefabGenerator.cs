
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

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

        string unityLegacyUIMenuPathPrefix = "GameObject/UI";
        if (string.Compare(Application.unityVersion, "2022.1.0") >= 0)
        {
            Debug.Log("Legacy");
            unityLegacyUIMenuPathPrefix = "GameObject/UI/Legacy";
        }

        #region TextBaseAndVariants
        // Create BaseTextPrefab
        // GameObject baseTextPrefab = CreateOrGetPreFabFromMenuNoChanges("GameObject/UI/Text", "BaseTextPrefab");
        string baseTextName = "BaseTextPrefab";
        GameObject baseTextPrefab;
        bool forceRecreateTextVariants = false;
        if (!TryGetPreFabAsset(baseTextName, out baseTextPrefab))
        {
            baseTextPrefab = CreateMenuObject(unityLegacyUIMenuPathPrefix + "/Text", baseTextName);
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

        GameObject inputFieldObj = CreateOrGetPreFabFromMenuWithChanges<InputField>(unityLegacyUIMenuPathPrefix + "/Input Field", "InputFieldPrefab",
                                    (inputField) =>
                                    {
                                        inputField.textComponent = ReplaceComponentsGO(inputField.textComponent, baseTextPrefab);
                                        inputField.placeholder = ReplaceComponentsGO(inputField.placeholder, placeholderTextPrefab);
                                    });
        GameObject buttonObj = CreateOrGetPreFabFromMenuWithChanges<Button>(unityLegacyUIMenuPathPrefix + "/Button", "BaseButtonPrefab",
                                (button) =>
                                {
                                    Text buttonText = button.gameObject.GetComponentInChildren<Text>();
                                    ReplaceComponentsGO(buttonText, titleTextPrefab);
                                });
        GameObject dropdownObj = CreateOrGetPreFabFromMenuWithChanges<Dropdown>(unityLegacyUIMenuPathPrefix + "/Dropdown", "DropdownPreFab",
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
        string logStr="\nGeneration start.  prefabPath: " + currentPreFabPath;

        #region TextBaseAndVariants

        // local Helper function to create text variants
        GameObject CreateTextVariant(string prefabName, GameObject basePrefab, TMPro.TextAlignmentOptions alignment, float fontSize, TMPro.FontWeight fontWeight = FontWeight.Regular, Color? color = null)
        {
            GameObject variant;
            if (!TryGetPreFabAsset(prefabName, out variant))
            {
                variant = CreatePrefabInstance(basePrefab, prefabName);
                TMPro.TextMeshProUGUI textComponent = variant.GetComponent<TMPro.TextMeshProUGUI>();
                textComponent.alignment = alignment;
                textComponent.fontSize = fontSize;
                textComponent.fontWeight = fontWeight;
                if (color.HasValue) textComponent.color = color.Value;
                /*RectTransform rectTransform = variant.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    PrefabUtility.RevertObjectOverride(rectTransform, InteractionMode.UserAction);
                }*/
                variant = SaveAsPrefab(variant);
                logStr += "\n Variant of " + basePrefab.name + " created: " + variant.name; 
            }
            return variant;
        }
        // Create BaseTextPrefab
        string baseTextName = "BaseTMPTextPrefab";
        GameObject baseTextPrefab;
        //bool forceRecreateTextVariants = false;
        if (!TryGetPreFabAsset(baseTextName, out baseTextPrefab))
        {
            baseTextPrefab = CreateMenuObject("GameObject/UI/Text - TextMeshPro", baseTextName); // we call the menu item to create the base text prefab
            TMPro.TextMeshProUGUI textComponent = baseTextPrefab.GetComponent<TMPro.TextMeshProUGUI>();
            textComponent.fontSize = 24;
            textComponent.color = Color.black;
            baseTextPrefab = SaveAsPrefab(baseTextPrefab);
            logStr += "\n BasePrefab created: " + baseTextPrefab.name; 
            //forceRecreateTextVariants = true;
        }

        // Large & Prominent Styles
        CreateTextVariant("TitleTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Center, 32, FontWeight.Bold);
        CreateTextVariant("SubtitleTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Center, 28, FontWeight.Bold);
        CreateTextVariant("NotificationTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Center, 26, FontWeight.Bold);

        // Button Styles
        GameObject buttonTextPrefab = CreateTextVariant("ButtonTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Center, 24, FontWeight.Bold);
        CreateTextVariant("PrimaryButtonTMPTextPrefab", buttonTextPrefab, TMPro.TextAlignmentOptions.Center, 24, FontWeight.Bold, Color.white);
        CreateTextVariant("SecondaryButtonTMPTextPrefab", buttonTextPrefab, TMPro.TextAlignmentOptions.Center, 24, FontWeight.Bold, Color.gray);
        CreateTextVariant("DangerButtonTMPTextPrefab", buttonTextPrefab, TMPro.TextAlignmentOptions.Center, 24, FontWeight.Bold, Color.red);

        // Body & Dialog Styles
        GameObject bodyTextPrefab= CreateTextVariant("BodyTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Left, 22);
        CreateTextVariant("NarrativeTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Left, 22, FontWeight.Regular);
        CreateTextVariant("QuestTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Left, 22, FontWeight.Bold);
        CreateTextVariant("FlavorTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Left, 20, FontWeight.Regular, Color.gray);

        // Tooltip & Status Styles
        CreateTextVariant("TooltipTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Left, 18, FontWeight.Regular, Color.yellow);
        CreateTextVariant("HealthStatusTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Right, 18, FontWeight.Bold, Color.green);
        CreateTextVariant("BuffStatusTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Right, 18, FontWeight.Bold, Color.blue);

        // System & Field Styles
        CreateTextVariant("SystemMessageTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Right, 18, FontWeight.Bold, Color.gray);
        GameObject labelTextPrefab = CreateTextVariant("FieldLabelTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Left, 20, FontWeight.Bold);
        GameObject placeholderTextPrefab = CreateTextVariant("InputPlaceholderTMPTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Left, 20, FontWeight.Regular, Color.gray);

        Debug.Log("Text variants creation complete.  Log: " + logStr);
        #endregion

        GameObject inputFieldObj = CreateOrGetPreFabFromMenuWithChanges<TMPro.TMP_InputField>("GameObject/UI/Input Field - TextMeshPro", "InputFieldTMPPrefab",
                                    (inputField) =>
                                    {
                                        inputField.textComponent = ReplaceComponentsGO(inputField.textComponent, bodyTextPrefab);
                                        inputField.textComponent.text = "";
                                        inputField.placeholder = ReplaceComponentsGO(inputField.placeholder, placeholderTextPrefab);
                                    });
        GameObject buttonObj = CreateOrGetPreFabFromMenuWithChanges<Button>("GameObject/UI/Button - TextMeshPro", "BaseButtonTMPPrefab",
                                    (button) =>
                                    {
                                        TMPro.TextMeshProUGUI buttonText = button.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                                        buttonText = ReplaceComponentsGO(buttonText, buttonTextPrefab);
                                        buttonText.text = "Button";
                                    });
        GameObject dropdownObj = CreateOrGetPreFabFromMenuWithChanges<TMPro.TMP_Dropdown>("GameObject/UI/Dropdown - TextMeshPro", "DropdownTMPPreFab",
                                    (dropdown) =>
                                    {
                                        dropdown.captionText = ReplaceComponentsGO(dropdown.captionText, labelTextPrefab);
                                        dropdown.itemText = ReplaceComponentsGO(dropdown.itemText, bodyTextPrefab);
                                    });
        GameObject toggleObjNoLabel = CreateOrGetPreFabFromMenuWithChanges<Toggle>("GameObject/UI/Toggle", "ToggleNoLabelPreFab",
                            (toggle) =>
                            {
                                Text label = toggle.GetComponentInChildren<Text>();
                                if(label!=null)
                                    GameObject.DestroyImmediate(label.gameObject);
                            });
        GameObject toggleObjTMP = CreatePrefabInstance(toggleObjNoLabel, "ToggleTMPPreFab");
        
        GameObject label = CreatePrefabInstance(labelTextPrefab, "Label");
        label.transform.SetParent(toggleObjTMP.transform,false);
        toggleObjTMP = SaveAsPrefab(toggleObjTMP);
        label = toggleObjTMP.transform.Find("Label").gameObject;//.GetComponentInChildren<
        RevertRectTransformOnly((RectTransform)label.transform, true);
        PrefabUtility.SavePrefabAsset(toggleObjTMP);
        
        //RevertRectTransformOnly((RectTransform)label.transform, toggleObjTMP);
        GenerateNonTextUsingPreFabs();//same w/ or w/o textMeshPro

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
        //if (!TryGetPreFabAsset(nameToAssign, out preFab))
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
            if (preFab == null)
            {
                Debug.LogError("Failed to create prefab from menu: " + menuPath);
                return null;
            }

            T component = preFab.GetComponent<T>();
            if(component==null)
                Debug.LogWarning("Unable to find component of type <" + typeof(T) + "> on prefab: " + preFab.name);

            if (changesFunction != null)
            {
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
        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, original.transform.parent);
//        newObj.transform.SetParent(original.transform.parent, false);
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
    private static GameObject ReplaceGO(GameObject original, GameObject prefab)
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
        GameObject.DestroyImmediate(original);

        // Return the requested component from the new GameObject
        return newObj;
    }
    /// <summary>
    /// Saves the provided object as a prefab.  The provided object is then Destroyed, and a reference to the new prefab is returned.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static GameObject SaveAsPrefab(GameObject obj)
    {
        
        GameObject preFab = null;
        try
        {
            preFab = PrefabUtility.SaveAsPrefabAsset(obj, $"{currentPreFabPath}/{obj.name}.prefab");//, InteractionMode.UserAction);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Exception generated during save of object " + obj.name + ": " + e.Message);
        }
        Object.DestroyImmediate(obj);
        RevertRectTransformOnly((RectTransform)preFab.transform);
        return preFab;
    }


    /// <summary>
    /// creates a scene instance of the prefab- it will still need to be saved as a new prefab using SaveAsPrefab, to create a variant prefab.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="variantName"></param>
    /// <returns></returns>
    private static GameObject CreatePrefabInstance(GameObject original, string variantName,Transform parent=null)
    {
        GameObject originalInstance = (GameObject)PrefabUtility.InstantiatePrefab(original,parent);
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

    // Static method to revert RectTransform properties of a given object to the base prefab state
    public static void RevertRectTransformOnly(RectTransform rt, bool skipCheckAndSave=false)
    {
        // Ensure this object is part of a prefab variant
        if (skipCheckAndSave || PrefabUtility.IsPartOfVariantPrefab(rt))//IsPartOfAnyPrefab(rt))
        {
            Debug.Log("Reverting RectTransform of object: " + rt.name);

            // Create a serialized object for this GameObject
            SerializedObject serializedObject = new SerializedObject(rt);

            // Revert the RectTransform properties specifically
            RevertProperty(serializedObject, "m_LocalPosition");
            RevertProperty(serializedObject, "m_LocalRotation");
            RevertProperty(serializedObject, "m_LocalScale");
            RevertProperty(serializedObject, "m_AnchorMin");
            RevertProperty(serializedObject, "m_AnchorMax");
            RevertProperty(serializedObject, "m_AnchoredPosition");
            RevertProperty(serializedObject, "m_Pivot");
            RevertProperty(serializedObject, "m_SizeDelta");
            if(!skipCheckAndSave)
                PrefabUtility.SavePrefabAsset(rt.gameObject);
        }
        else
        {
          //  Debug.Log("NOT Reverting RectTransform of object, because it is NOT a prefab: " + rt.name);
            //  Debug.LogError("This object is not part of a prefab variant.");
        }
    }

    // Helper function to revert the property
    private static void RevertProperty(SerializedObject serializedObject, string propertyName)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            PrefabUtility.RevertPropertyOverride(property, InteractionMode.AutomatedAction);
        }
        else
        {
            Debug.LogWarning($"Property {propertyName} not found on RectTransform.");
        }
    }

}
