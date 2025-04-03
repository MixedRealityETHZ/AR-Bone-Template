using System;
using UnityEngine.Events;

[Serializable]
public readonly struct ButtonDefinition : IElementDefinition
{
    public readonly string ButtonName;
    public readonly UnityEvent OnClickAction;
    public readonly UnityAction OnClickSingleAction;

    public ButtonDefinition(string buttonName, UnityEvent onClickAction, UnityAction onClickSingleAction)
    {
        ButtonName = buttonName;
        OnClickAction = onClickAction;
        OnClickSingleAction = onClickSingleAction;
    }

    public ButtonDefinition(string buttonName, UnityEvent onClickAction) : this(buttonName, onClickAction, null) { }

    public ButtonDefinition(string buttonName, UnityAction onClickSingleAction) : this(buttonName, null, onClickSingleAction) { }
}
