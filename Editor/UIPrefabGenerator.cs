
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
    private const string PrefabPathTMP = "Assets/Prefabs/UI/TextMeshPro";
    private const string PrefabPathLegacyText = "Assets/Prefabs/UI/Legacy";


    [MenuItem("Tools/Generate UI Prefabs")]
    public static void GenerateUIPrefabs()
    {
        GenericGeneratePrefabs(false);
        return;
    }

    [MenuItem("Tools/Generate TextMeshPro UI Prefabs")]
    public static void GenerateTextMeshProPrefabs()
    {
        GenericGeneratePrefabs(true);
        return;
    }

    //one of main class functions.  specifies, in code the creation of all prefabs via in-code accessed unity editor menu items where appropriate, and as variants of other prefabs
    private static void GenericGeneratePrefabs(bool useTMP)
    {
        string preFabPathToUse = useTMP ? PrefabPathTMP : PrefabPathLegacyText;
        // Set a separate path for TextMeshPro prefabs
        EnsureDirectoryExists(PrefabPath);
        EnsureDirectoryExists(preFabPathToUse);
        EnsureDirectoryExists(preFabPathToUse+"/TextVariants");
        currentPreFabPath = preFabPathToUse;
        string logStr = "\nGeneration start. Using TextMeshPro:"+useTMP+" prefabPath: " + currentPreFabPath;
        string unityLegacyUIMenuPathPrefix = "GameObject/UI";
        if (string.Compare(Application.unityVersion, "2022.1.0") >= 0)
        {
           // Debug.Log("Legacy");
            unityLegacyUIMenuPathPrefix = "GameObject/UI/Legacy";
        }
        #region TextBaseAndVariants

        // local Helper function to create text variants
        GameObject CreateTextVariant(string prefabName, GameObject basePrefab, TMPro.TextAlignmentOptions alignment, float fontSize, TMPro.FontWeight fontWeight = FontWeight.Regular, Color? color = null)
        {
            GameObject variant;
            if (!TryGetPreFabAsset(prefabName, out variant))
            {
                variant = CreatePrefabInstance(basePrefab, prefabName);
                GenericTextComponent textComponent = GenericTextComponent.GetComponent(variant);
                textComponent.alignmentTMP = alignment;
                textComponent.fontSize = fontSize;
                textComponent.fontWeight = fontWeight;
                if (color.HasValue) textComponent.color = color.Value;
                /*RectTransform rectTransform = variant.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    PrefabUtility.RevertObjectOverride(rectTransform, InteractionMode.UserAction);
                }*/
                variant = SaveAsPrefab(variant, "TextVariants");
                logStr += "\n Variant of " + basePrefab.name + " created: " + variant.name;
            }
            return variant;
        }



        ////////////////
        //create BaseTextPrefab base
        ////////////////
        string baseTextName = "BaseTextPrefab";
        GameObject baseTextPrefab;
        //bool forceRecreateTextVariants = false;
        if (!TryGetPreFabAsset(baseTextName, out baseTextPrefab))
        {
            if(useTMP)
                baseTextPrefab = CreateMenuObject("GameObject/UI/Text - TextMeshPro", baseTextName); // we call the menu item to create the base text prefab
            else
                baseTextPrefab = CreateMenuObject(unityLegacyUIMenuPathPrefix + "/Text", baseTextName);

            ((RectTransform)baseTextPrefab.transform).anchoredPosition = Vector3.zero;
            //TMPro.TextMeshProUGUI textComponent = baseTextPrefab.GetComponent<TMPro.TextMeshProUGUI>();
            GenericTextComponent textComponent = GenericTextComponent.GetComponent(baseTextPrefab);
            textComponent.fontSize = 24;
            textComponent.color = Color.black;
            baseTextPrefab = SaveAsPrefab(baseTextPrefab, "TextVariants");
            logStr += "\n BasePrefab created: " + baseTextPrefab.name;
            //forceRecreateTextVariants = true;
        }

        ////////////////
        //create TextPrefab variants
        ////////////////

        // Large & Prominent Styles
        GameObject titleTextPrefab = CreateTextVariant("TitleTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Center, 32, FontWeight.Bold);
        CreateTextVariant("SubtitleTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Center, 28, FontWeight.Bold);
        GameObject bodyTextPrefab = CreateTextVariant("BodyTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Left, 22);
        // Button Styles
        GameObject buttonTextPrefab = CreateTextVariant("ButtonTextPrefab", bodyTextPrefab, TMPro.TextAlignmentOptions.Center, 24, FontWeight.Bold);
        CreateTextVariant("TooltipTextPrefab", bodyTextPrefab, TMPro.TextAlignmentOptions.Left, 18, FontWeight.Regular, Color.yellow);
        CreateTextVariant("TinyTextPrefab", baseTextPrefab, TMPro.TextAlignmentOptions.Left, 12, FontWeight.Regular);

        // System & Field Styles
        CreateTextVariant("SystemMessageTitlePrefab", titleTextPrefab, TMPro.TextAlignmentOptions.Center, 32, FontWeight.Bold, Color.red);
        CreateTextVariant("SystemMessageTextPrefab", bodyTextPrefab, TMPro.TextAlignmentOptions.Left, 22, FontWeight.Regular, Color.gray);

        GameObject labelTextPrefab = CreateTextVariant("FieldLabelTextPrefab", bodyTextPrefab, TMPro.TextAlignmentOptions.Right, 20, FontWeight.Bold);
        SetRectTransformToFull(labelTextPrefab.transform);//set to fill parent
        ((RectTransform)labelTextPrefab.transform).anchorMax= new Vector2(0.5f,1);
        ((RectTransform)labelTextPrefab.transform).offsetMax = new Vector2(-5, 0);
        GameObject placeholderTextPrefab = CreateTextVariant("InputPlaceholderTextPrefab", bodyTextPrefab, TMPro.TextAlignmentOptions.Left, 20, FontWeight.Regular, Color.gray);

       // Debug.Log("Text variants creation complete.  Log: " + logStr);
        #endregion


        ////////////////
        //create inputfield base
        ////////////////
        string inputFieldMenuString = "GameObject/UI/Input Field - TextMeshPro";
        string inputPreFabName = "InputFieldTMPPrefab";
        System.Type inputFieldType = typeof(TMPro.TMP_InputField);
        if (!useTMP)
        {
            inputFieldMenuString = unityLegacyUIMenuPathPrefix + "/Input Field";
            inputPreFabName = "InputFieldPrefab";
            inputFieldType = typeof(InputField);
        }
        GameObject inputFieldObj = CreateOrGetPreFabFromMenuWithChanges(inputFieldMenuString, inputPreFabName, inputFieldType,
                                    (inputFieldComponent) =>
                                    {
                                        GenericInputFieldComponent inputField = new GenericInputFieldComponent(inputFieldComponent);
                                        GenericTextComponent textComponent = GenericTextComponent.GetComponent(ReplaceGO(inputField.textComponent.gameObject, bodyTextPrefab));
                                        inputField.textComponent = textComponent;
                                        //inputField.textComponent = ReplaceComponentsGO(inputField.textComponent, bodyTextPrefab);
                                        textComponent.text = "";
                                        Vector2 offsetMin = ((RectTransform)textComponent.transform).offsetMin;
                                        offsetMin.y = 2;
                                        ((RectTransform)textComponent.transform).offsetMin= offsetMin;

                                        Vector2 offsetMax = ((RectTransform)textComponent.transform).offsetMax;
                                        offsetMax.y = -2;
                                        ((RectTransform)textComponent.transform).offsetMax = offsetMax;

                                        GenericTextComponent placeholder= GenericTextComponent.GetComponent(ReplaceGO(inputField.placeholder.gameObject, placeholderTextPrefab));
                                        inputField.placeholder = placeholder;
                                        offsetMin = ((RectTransform)placeholder.transform).offsetMin;
                                        offsetMin.y = 2;
                                        ((RectTransform)placeholder.transform).offsetMin = offsetMin;

                                        offsetMax = ((RectTransform)placeholder.transform).offsetMax;
                                        offsetMax.y = -2;
                                        ((RectTransform)placeholder.transform).offsetMax = offsetMax;

                                        //inputField.placeholder = ReplaceComponentsGO(inputField.placeholder, placeholderTextPrefab);
                                        //move BG to it's own transform
                                        GameObject newBackgroundObject = new GameObject("BackgroundImage", new System.Type[] { typeof(RectTransform), typeof(Image) });
                                        newBackgroundObject.transform.SetParent(inputField.transform, false);
                                        newBackgroundObject.transform.SetSiblingIndex(0);
                                        SetRectTransformToFull(newBackgroundObject.transform);
                                        Image oldBackgroundComponent = inputField.gameObject.GetComponent<Image>();
                                        Image newImage = CopyComponent<Image>(oldBackgroundComponent, newBackgroundObject);
                                        Object.DestroyImmediate(oldBackgroundComponent);
                                        inputField.targetGraphic = newImage;
                                    });
        ////////////////
        //create button base
        ////////////////
        string buttonMenuString = "GameObject/UI/Button - TextMeshPro";
        string buttonPreFabName = "BaseButtonTMPPrefab";
        if (!useTMP)
        {
            inputFieldMenuString = unityLegacyUIMenuPathPrefix + "/Button";
            inputPreFabName = "BaseButtonPrefab";
        }
        Button buttonObj = CreateOrGetPreFabFromMenuWithChanges<Button>(buttonMenuString, buttonPreFabName,
                                    (button) =>
                                    {
                                        GenericTextComponent buttonText = GenericTextComponent.GetComponentInChildren(button.gameObject);
                                        buttonText = GenericTextComponent.GetComponent(ReplaceGO(buttonText.gameObject, buttonTextPrefab));
                                        buttonText.text = "Button";
                                    });

        ////////////////
        //create dropdown base
        ////////////////
        string dropDownMenuString = "GameObject/UI/Dropdown - TextMeshPro";
        string dropDownPreFabName = "DropdownTMPPreFab";
        System.Type dropdownType = typeof(TMPro.TMP_Dropdown);
        if (!useTMP)
        {
            dropDownMenuString = unityLegacyUIMenuPathPrefix + "/Dropdown";
            dropDownPreFabName = "DropdownPreFab";
            dropdownType = typeof(Dropdown);
        }

        GameObject dropdownObj = CreateOrGetPreFabFromMenuWithChanges(dropDownMenuString, dropDownPreFabName, dropdownType,
                            (dropdown) =>
                            {
                                GenericDropdownComponent dropdownField = new GenericDropdownComponent(dropdown);
                                dropdownField.captionText = GenericTextComponent.GetComponent(ReplaceGO(dropdownField.captionText.gameObject, bodyTextPrefab));
                                RectTransform captionRect = ((RectTransform)dropdownField.captionText.transform);
                                captionRect.offsetMin = new Vector2(captionRect.offsetMin.x, 2); 
                                captionRect.offsetMax = new Vector2(captionRect.offsetMax.x, -2);
                                dropdownField.itemText = GenericTextComponent.GetComponent(ReplaceGO(dropdownField.itemText.gameObject, bodyTextPrefab));
                                dropdownField.itemText.name = "SelectionText";
                                RectTransform itemTextTransform = (RectTransform)dropdownField.itemText.transform;
                                itemTextTransform.offsetMin = new Vector2(itemTextTransform.offsetMin.x, 2);
                                itemTextTransform.offsetMax = new Vector2(itemTextTransform.offsetMax.x, -2);
                                //move bg to it's own child object
                                GameObject backgroundGO= new GameObject("Background", typeof(Image));
                                backgroundGO.transform.SetParent(dropdown.transform, false);
                                SetRectTransformToFull(backgroundGO.transform);
                                Image oldBG = (Image)dropdownField.targetGraphic;
                                Image newBG= CopyComponent<Image>(oldBG, backgroundGO);
                                newBG.transform.SetSiblingIndex(0);
                                dropdownField.targetGraphic=newBG;
                                Component.DestroyImmediate(oldBG);
                            });

        currentPreFabPath = PrefabPath;
        
        GenerateNonTextUsingPreFabs();//same w/ or w/o textMeshPro
        GameObject toggleObjNoLabelGameObject;
        TryGetPreFabAsset("TogglePreFab", out toggleObjNoLabelGameObject);
        GameObject sliderObjNoLabelGameObject;
        TryGetPreFabAsset("SliderPrefab", out sliderObjNoLabelGameObject);

        currentPreFabPath = preFabPathToUse;

        


        ////////////////
        //create toggle WITH label variant
        ////////////////
        
        string toggleWithLabelPreFabName = "TogglePreFabTMPLabeled";
        if (!useTMP)
        {
            toggleWithLabelPreFabName = "TogglePreFabLabeled";
        }
        GameObject toggleObj = CreatePrefabInstance(toggleObjNoLabelGameObject, toggleWithLabelPreFabName);
        Graphic togBackground = toggleObj.GetComponent<Toggle>().targetGraphic;
        RectTransform bgRect = ((RectTransform)togBackground.transform);
        SetRectTransformToFull(bgRect);
        bgRect.anchorMin = new Vector2(1, .5f);
        bgRect.anchorMax = new Vector2(1, .5f);
        bgRect.pivot = new Vector2(1, 0.5f);
        bgRect.sizeDelta = new Vector2(20, 20);

        GameObject label = CreatePrefabInstance(labelTextPrefab, "Label");
        label.transform.SetParent(toggleObj.transform, true);
        SetRectTransformToFull(label.transform);//set to fill parent, minus space on right for toggle
        ((RectTransform)label.transform).offsetMax = new Vector2(-25, 0);
        toggleObj = SaveAsPrefab(toggleObj);


        ////////////////
        //create inputfield and slider with label variants: 50%/50% label/control width
        ////////////////
        AddLabelAndMakeNewPreFab(inputFieldObj, labelTextPrefab);
        AddLabelAndMakeNewPreFab(sliderObjNoLabelGameObject, labelTextPrefab);
        AddLabelAndMakeNewPreFab(dropdownObj, labelTextPrefab);
        

        Debug.Log("UI Prefabs Generated and Customized!");
    }

    private static void GenerateNonTextUsingPreFabs()
    {
        GameObject panelObj = CreateOrGetPreFabFromMenuNoChanges("GameObject/UI/Panel", "PanelPrefab");
        GameObject scrollbarObj = CreateOrGetPreFabFromMenuNoChanges("GameObject/UI/Scrollbar", "ScrollbarPrefab");
        ScrollRect scrollViewObj = CreateOrGetPreFabFromMenuWithChanges<ScrollRect>("GameObject/UI/Scroll View", "ScrollViewPrefab",
                (scrollRect) =>
                {
                    scrollRect.horizontalScrollbar = ReplaceComponentsGO(scrollRect.horizontalScrollbar, scrollbarObj);
                    scrollRect.verticalScrollbar = ReplaceComponentsGO(scrollRect.verticalScrollbar, scrollbarObj);
                });
        GameObject sliderObj = CreateOrGetPreFabFromMenuNoChanges("GameObject/UI/Slider", "SliderPrefab");
        Toggle toggleObjNoLabel = CreateOrGetPreFabFromMenuWithChanges<Toggle>("GameObject/UI/Toggle", "TogglePreFab",
                    (toggle) =>
                    {
                        Text label = toggle.GetComponentInChildren<Text>();
                        if (label != null)
                            GameObject.DestroyImmediate(label.gameObject);
                    });
    }

    private static GameObject AddLabelAndMakeNewPreFab(GameObject basePrefab,GameObject labelTextPrefab)
    {
        string newPreFabName = basePrefab.name + "Labeled";
        GameObject labeledPreFab = CreatePrefabInstance(basePrefab, newPreFabName);
        RectTransform root = (RectTransform)labeledPreFab.transform;
        foreach (RectTransform childTransform in root)
        {
            if(childTransform.anchorMin.x==0)
                childTransform.anchorMin = new Vector2(0.5f, childTransform.anchorMin.y);
        }
        CreatePrefabInstance(labelTextPrefab, "FieldLabel", root);


        labeledPreFab = SaveAsPrefab(labeledPreFab);//reverts transfrom

        root = (RectTransform)labeledPreFab.transform;
        root.sizeDelta *= new Vector2(2, 1);//double width
        //PrefabUtility.ApplyPrefabInstance(labeledPreFab, InteractionMode.AutomatedAction);
        labeledPreFab = SaveAsPrefabNoRevert(labeledPreFab);
        return labeledPreFab;
    }
    /// <summary>
    /// Combines functions required to check for existing, and if not found create a new prefab using the menu item.
    /// </summary>
    /// <param name="menuPath"></param>
    /// <param name="nameToAssign"></param>
    /// <returns></returns>
    static GameObject CreatePreFabFromMenuNoChanges(string menuPath, string nameToAssign)
    {
        GameObject preFab;
        preFab = CreateMenuObject(menuPath, nameToAssign);
        RectTransform rt = preFab.transform as RectTransform;
        if (rt != null)
            rt.anchoredPosition = Vector3.zero;
        preFab = SaveAsPrefab(preFab);
        return preFab;
    }

    static GameObject CreateOrGetPreFabFromMenuNoChanges(string menuPath, string nameToAssign)
    {
        if (TryGetPreFabAsset(nameToAssign, out GameObject foundPreFab)) return foundPreFab;
        return CreatePreFabFromMenuNoChanges(menuPath, nameToAssign);
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
    static T CreateOrGetPreFabFromMenuWithChanges<T>(string menuPath, string nameToAssign, System.Action<T> changesFunction) where T : Component
    {
        return CreateOrGetPreFabFromMenuWithChanges(menuPath, nameToAssign, typeof(T), (Component component) =>{ changesFunction((T)component); }).GetComponent<T>();
        GameObject preFab;
        if (!TryGetPreFabAsset(nameToAssign, out preFab))
        {
            preFab = CreateMenuObject(menuPath, nameToAssign);
            if (preFab == null)
            {
                Debug.LogError("Failed to create prefab from menu: " + menuPath);
                return null;
            }
            RectTransform rt = preFab.transform as RectTransform;
            if (rt != null)
                rt.anchoredPosition = Vector3.zero;
        }
        T component = preFab.GetComponent<T>();
        if(component==null)
            Debug.LogWarning("Unable to find component of type <" + typeof(T) + "> on prefab: " + preFab.name);

        if (changesFunction != null)
        {
            changesFunction(component);
        }
        GameObject savedPreFab = SaveAsPrefab(preFab.gameObject);
        return savedPreFab.GetComponent<T>();
    }


    static GameObject CreateOrGetPreFabFromMenuWithChanges(string menuPath, string nameToAssign,System.Type componentType, System.Action<Component> changesFunction)
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
            RectTransform rt = preFab.transform as RectTransform;
            if (rt != null)
                rt.anchoredPosition = Vector3.zero;
            Component component = preFab.GetComponent(componentType);
            if (component == null)
                Debug.LogWarning("Unable to find component of type <" + componentType + "> on prefab: " + preFab.name);

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
    /// <param name="replacementPrefab"></param>
    /// <returns></returns>
    private static T ReplaceComponentsGO<T>(T original, GameObject replacementPrefab) where T : MonoBehaviour
    {
        // Return the requested component from the new GameObject
        GameObject newObj = ReplaceGO(original.gameObject, replacementPrefab);
        if(newObj==null) return null;
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
    private static GameObject SaveAsPrefab(GameObject obj, string subFolder=null)
    {
        
        GameObject preFab = null;
        string path = $"{currentPreFabPath}/{obj.name}.prefab";
        if(subFolder!=null)
            path = $"{currentPreFabPath}/{subFolder}/{obj.name}.prefab";
        try
        {
            preFab = PrefabUtility.SaveAsPrefabAsset(obj, path);//, InteractionMode.UserAction);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Exception generated during save of object " + obj.name + ": " + e.Message);
        }
        Object.DestroyImmediate(obj);
        RevertRectTransformOnly((RectTransform)preFab.transform);
        return preFab;
    }
    private static GameObject SaveAsPrefabNoRevert(GameObject obj, string subFolder = null)
    {

        GameObject preFab = null;
        string path = $"{currentPreFabPath}/{obj.name}.prefab";
        if (subFolder != null)
            path = $"{currentPreFabPath}/{subFolder}/{obj.name}.prefab";
        try
        {
            preFab = PrefabUtility.SaveAsPrefabAsset(obj, path);//, InteractionMode.UserAction);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Exception generated during save of object " + obj.name + ": " + e.Message);
        }
        Object.DestroyImmediate(obj);
        
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
          //  Debug.Log("Reverting RectTransform of object: " + rt.name);

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

    private static void SetRectTransformToFull(Transform transform)
    {
        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform == null) return;
        // Stretch to fill parent
        rectTransform.anchorMin = Vector2.zero; // Bottom-left
        rectTransform.anchorMax = Vector2.one;  // Top-right
        rectTransform.offsetMin = Vector2.zero; // Remove any offsets
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }
    //creates or updates  component of type T on destination, with data of original.
    private static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        if (original == null || destination == null) return null;

        // Create a new component of the same type on the destination GameObject
        T copiedComponent = destination.GetComponent<T>();
        if(copiedComponent==null)
            copiedComponent = destination.AddComponent<T>();

        // Copy properties from original to new component
        UnityEditorInternal.ComponentUtility.CopyComponent(original);
        UnityEditorInternal.ComponentUtility.PasteComponentValues(copiedComponent);

        return copiedComponent;
    }
    private static Transform FindDescendantByName(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform found = FindDescendantByName(child, childName);
            if (found != null)
                return found;
        }
        return null;
    }
}
