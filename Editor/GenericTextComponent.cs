using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TwoComponentTypeAdapter<L, M> where L : Component where M : Component
{
    public L legacyComponent;
    public M modernComponent;

    // Property to check if we're using the modern component
    public bool IsUsingModern => modernComponent != null;

    // Constructor for legacy component
    public TwoComponentTypeAdapter(L legacyComponent)
    {
        this.legacyComponent = legacyComponent;
    }

    // Constructor for modern component
    public TwoComponentTypeAdapter(M modernComponent)
    {
        this.modernComponent = modernComponent;
    }

    // Constructor accepting any component, trying to cast to L or M
    public TwoComponentTypeAdapter(Component objAsComponent)
    {
        if (objAsComponent == null)
        {
            Debug.LogError($"Creation of {typeof(TwoComponentTypeAdapter<L, M>)} via Component failed. Must be passed a not-null value.");
            return;
        }
        if (objAsComponent is L)
        {
            legacyComponent = objAsComponent as L;
        }
        else if (objAsComponent is M)
        {
            modernComponent = objAsComponent as M;
        }
        else
        {
            Debug.LogWarning($"Creation of {typeof(TwoComponentTypeAdapter<L, M>)} via Component failed. Must be passed a {typeof(L)} or {typeof(M)} type object. Object passed type: {objAsComponent.GetType()}");
        }
    }

    //alows decentant classes to create an instance from generic 
    protected TwoComponentTypeAdapter(TwoComponentTypeAdapter<L, M> fieldAsBaseClass) 
    {
        legacyComponent = fieldAsBaseClass.legacyComponent;
        modernComponent = fieldAsBaseClass.modernComponent;
    }
    // Access the transform, using whichever component is set
    public Transform transform => IsUsingModern ? modernComponent.transform : legacyComponent.transform;
    // Access the gameObject, using whichever component is set
    public GameObject gameObject => IsUsingModern ? modernComponent.gameObject : legacyComponent.gameObject;

    // Add the appropriate component to the GameObject
    public static TwoComponentTypeAdapter<L, M> AddComponent(GameObject objectToAddTo, bool useModern)
    {
        if (useModern)
            return new TwoComponentTypeAdapter<L, M>(objectToAddTo.AddComponent<M>());
        else
            return new TwoComponentTypeAdapter<L, M>(objectToAddTo.AddComponent<L>());
    }

    // Try to get the appropriate component from the GameObject
    public static TwoComponentTypeAdapter<L, M> GetComponent(GameObject objectToGetFrom)
    {
        M foundModern = objectToGetFrom.GetComponent<M>();
        if (foundModern != null) return new TwoComponentTypeAdapter<L, M>(foundModern);

        L foundLegacy = objectToGetFrom.GetComponent<L>();
        if (foundLegacy != null) return new TwoComponentTypeAdapter<L, M>(foundLegacy);

        return null;
    }
    public static TwoComponentTypeAdapter<L, M> GetComponentInChildren(GameObject objectToGetFrom)
    {
        M foundModern = objectToGetFrom.GetComponentInChildren<M>();
        if (foundModern != null) return new TwoComponentTypeAdapter<L, M>(foundModern);

        L foundLegacy = objectToGetFrom.GetComponentInChildren<L>();
        if (foundLegacy != null) return new TwoComponentTypeAdapter<L, M>(foundLegacy);

        return null;
    }
}


public class GenericTextComponent : TwoComponentTypeAdapter<Text, TextMeshProUGUI>
{
    public GenericTextComponent(Text legacyComponent) : base(legacyComponent) { }
    public GenericTextComponent(TextMeshProUGUI modernComponent) : base(modernComponent) { }
    public GenericTextComponent(Component textAsComponent) : base(textAsComponent) { }
    protected GenericTextComponent(TwoComponentTypeAdapter<Text, TextMeshProUGUI> fieldAsBaseClass) : base(fieldAsBaseClass) { }
    public new static GenericTextComponent AddComponent(GameObject objectToAddTo, bool useTMP)
    {
        return new GenericTextComponent(TwoComponentTypeAdapter<Text, TextMeshProUGUI>.AddComponent(objectToAddTo, useTMP));
    }
    public new static GenericTextComponent GetComponent(GameObject objectToGetFrom)
    {
        return new GenericTextComponent(TwoComponentTypeAdapter<Text, TextMeshProUGUI>.GetComponent(objectToGetFrom));
    }
    public new static GenericTextComponent GetComponentInChildren(GameObject objectToGetFrom)
    {
        return new GenericTextComponent(TwoComponentTypeAdapter<Text, TextMeshProUGUI>.GetComponentInChildren(objectToGetFrom));
    }
    public string text
    {
        get => IsUsingModern ? modernComponent.text : legacyComponent.text;
        set { if (IsUsingModern) modernComponent.text = value; else legacyComponent.text = value; }
    }

    public Color color
    {
        get => IsUsingModern ? modernComponent.color : legacyComponent.color;
        set { if (IsUsingModern) modernComponent.color = value; else legacyComponent.color = value; }
    }

    public float fontSize
    {
        get => IsUsingModern ? modernComponent.fontSize : legacyComponent.fontSize;
        set { if (IsUsingModern) modernComponent.fontSize = value; else legacyComponent.fontSize = (int)value; }
    }

    public Font font
    {
        get => IsUsingModern ? null : legacyComponent.font;
        set { if (!IsUsingModern) legacyComponent.font = value; }
    }

    public TMP_FontAsset fontTMP
    {
        get => IsUsingModern ? modernComponent.font : null;
        set { if (IsUsingModern) modernComponent.font = value; }
    }

    public FontStyle fontStyle
    {
        get => IsUsingModern ? (FontStyle)modernComponent.fontStyle : legacyComponent.fontStyle;
        set { if (IsUsingModern) modernComponent.fontStyle = (FontStyles)value; else legacyComponent.fontStyle = value; }
    }

    public TextAnchor alignment
    {
        get => IsUsingModern ? ConvertAlignment(modernComponent.alignment) : legacyComponent.alignment;
        set { if (IsUsingModern) modernComponent.alignment = ConvertAlignment(value); else legacyComponent.alignment = value; }
    }
    public TMPro.TextAlignmentOptions alignmentTMP
    {
        get => IsUsingModern ? modernComponent.alignment : ConvertAlignment(legacyComponent.alignment);
        set { if (IsUsingModern) modernComponent.alignment = value; else legacyComponent.alignment = ConvertAlignment(value); }
    }
    public FontWeight fontWeight
    {
        get => IsUsingModern ? modernComponent.fontWeight : FontWeight.Regular;
        set { if (IsUsingModern) modernComponent.fontWeight = value; }
    }

    private TextAnchor ConvertAlignment(TextAlignmentOptions tmpAlignment)
    {
        return tmpAlignment switch
        {
            TextAlignmentOptions.TopLeft => TextAnchor.UpperLeft,
            TextAlignmentOptions.Top => TextAnchor.UpperCenter,
            TextAlignmentOptions.TopRight => TextAnchor.UpperRight,
            TextAlignmentOptions.Left => TextAnchor.MiddleLeft,
            TextAlignmentOptions.Center => TextAnchor.MiddleCenter,
            TextAlignmentOptions.Right => TextAnchor.MiddleRight,
            TextAlignmentOptions.BottomLeft => TextAnchor.LowerLeft,
            TextAlignmentOptions.Bottom => TextAnchor.LowerCenter,
            TextAlignmentOptions.BottomRight => TextAnchor.LowerRight,
            _ => TextAnchor.MiddleCenter,
        };
    }

    private TextAlignmentOptions ConvertAlignment(TextAnchor legacyAlignment)
    {
        return legacyAlignment switch
        {
            TextAnchor.UpperLeft => TextAlignmentOptions.TopLeft,
            TextAnchor.UpperCenter => TextAlignmentOptions.Top,
            TextAnchor.UpperRight => TextAlignmentOptions.TopRight,
            TextAnchor.MiddleLeft => TextAlignmentOptions.Left,
            TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
            TextAnchor.MiddleRight => TextAlignmentOptions.Right,
            TextAnchor.LowerLeft => TextAlignmentOptions.BottomLeft,
            TextAnchor.LowerCenter => TextAlignmentOptions.Bottom,
            TextAnchor.LowerRight => TextAlignmentOptions.BottomRight,
            _ => TextAlignmentOptions.Center,
        };
    }
}

public class GenericInputFieldComponent:TwoComponentTypeAdapter<InputField, TMP_InputField>
{

    public GenericInputFieldComponent(Text legacyComponent) : base(legacyComponent) { }
    public GenericInputFieldComponent(TextMeshProUGUI modernComponent) : base(modernComponent) { }
    public GenericInputFieldComponent(Component inputFieldAsComponent) : base(inputFieldAsComponent) { }
    protected GenericInputFieldComponent(TwoComponentTypeAdapter<InputField, TMP_InputField> inputFieldAsBaseClass) : base(inputFieldAsBaseClass) { }

    public new static GenericInputFieldComponent AddComponent(GameObject objectToAddTo, bool useTMP)
    {
        return new GenericInputFieldComponent(TwoComponentTypeAdapter<InputField, TMP_InputField>.AddComponent(objectToAddTo, useTMP));
    }
    public new static GenericInputFieldComponent GetComponent(GameObject objectToGetFrom)
    {
        return new GenericInputFieldComponent(TwoComponentTypeAdapter<InputField, TMP_InputField>.GetComponent(objectToGetFrom));
    }
    public new static GenericInputFieldComponent GetComponentInChildren(GameObject objectToGetFrom)
    {
        return new GenericInputFieldComponent(TwoComponentTypeAdapter<InputField, TMP_InputField>.GetComponentInChildren(objectToGetFrom));
    }
    public string text
    {
        get => IsUsingModern ? modernComponent.text : legacyComponent.text;
        set { if (IsUsingModern) modernComponent.text = value; else legacyComponent.text = value; }
    }

    public int characterLimit
    {
        get => IsUsingModern ? modernComponent.characterLimit : legacyComponent.characterLimit;
        set { if (IsUsingModern) modernComponent.characterLimit = value; else legacyComponent.characterLimit = value; }
    }

    public bool isPassword
    {
        get => IsUsingModern ? modernComponent.contentType == TMP_InputField.ContentType.Password : legacyComponent.contentType == InputField.ContentType.Password;
        set
        {
            if (IsUsingModern) modernComponent.contentType = value ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            else legacyComponent.contentType = value ? InputField.ContentType.Password : InputField.ContentType.Standard;
        }
    }
    public Graphic targetGraphic
    {
        get => IsUsingModern ? modernComponent.targetGraphic : legacyComponent.targetGraphic;
        set { if (IsUsingModern) modernComponent.targetGraphic = value; else legacyComponent.targetGraphic = value; }
    }
    public GenericTextComponent textComponent
    {
        get => IsUsingModern ? new GenericTextComponent(modernComponent.textComponent) : new GenericTextComponent(legacyComponent.textComponent);
        set { if (IsUsingModern) modernComponent.textComponent = value.modernComponent; else legacyComponent.textComponent = value.legacyComponent; }
    }
    public GenericTextComponent placeholder
    {
        get => IsUsingModern ? new GenericTextComponent(modernComponent.placeholder) : new GenericTextComponent(legacyComponent.placeholder);
        set { if (IsUsingModern) modernComponent.placeholder = value.modernComponent; else legacyComponent.placeholder = value.legacyComponent; }
    }
}
public class GenericDropdownComponent: TwoComponentTypeAdapter<Dropdown, TMP_Dropdown>
{

    public GenericDropdownComponent(Text legacyComponent) : base(legacyComponent) { }
    public GenericDropdownComponent(TextMeshProUGUI modernComponent) : base(modernComponent) { }
    public GenericDropdownComponent(Component inputFieldAsComponent) : base(inputFieldAsComponent) { }
    protected GenericDropdownComponent(TwoComponentTypeAdapter<Dropdown, TMP_Dropdown> ddFieldAsBaseClass) : base(ddFieldAsBaseClass) { }

    public static new GenericDropdownComponent AddComponent(GameObject objectToAddTo, bool useTMP)
    {
        return new GenericDropdownComponent(TwoComponentTypeAdapter<Dropdown, TMP_Dropdown>.AddComponent(objectToAddTo, useTMP));
    }
    public static new GenericDropdownComponent GetComponent(GameObject objectToGetFrom)
    {
        return new GenericDropdownComponent( TwoComponentTypeAdapter<Dropdown, TMP_Dropdown>.GetComponent(objectToGetFrom));
    }
    public new static GenericDropdownComponent GetComponentInChildren(GameObject objectToGetFrom)
    {
        return new GenericDropdownComponent(TwoComponentTypeAdapter<Dropdown, TMP_Dropdown>.GetComponentInChildren(objectToGetFrom));
    }
    public int value
    {
        get => IsUsingModern ? modernComponent.value : legacyComponent.value;
        set { if (IsUsingModern) modernComponent.value = value; else legacyComponent.value = value; }
    }

    public List<string> options
    {
        get => IsUsingModern ? modernComponent.options.ConvertAll(option => option.text) : legacyComponent.options.ConvertAll(option => option.text);
        set
        {
            if (IsUsingModern) modernComponent.options = value.ConvertAll(option => new TMP_Dropdown.OptionData(option));
            else legacyComponent.options = value.ConvertAll(option => new Dropdown.OptionData(option));
        }
    }

    public void AddOptions(List<string> options)
    {
        if (IsUsingModern)
            modernComponent.AddOptions(options.ConvertAll(option => new TMP_Dropdown.OptionData(option)));
        else
            legacyComponent.AddOptions(options.ConvertAll(option => new Dropdown.OptionData(option)));
    }

    public GenericTextComponent captionText
    {
        get => IsUsingModern ? new GenericTextComponent(modernComponent.captionText) : new GenericTextComponent(legacyComponent.captionText);
        set { if (IsUsingModern) modernComponent.captionText = value.modernComponent; else legacyComponent.captionText = value.legacyComponent; }
    }
    public GenericTextComponent itemText
    {
        get => IsUsingModern ? new GenericTextComponent(modernComponent.itemText) : new GenericTextComponent(legacyComponent.itemText);
        set { if (IsUsingModern) modernComponent.itemText = value.modernComponent; else legacyComponent.itemText = value.legacyComponent; }
    }
}


