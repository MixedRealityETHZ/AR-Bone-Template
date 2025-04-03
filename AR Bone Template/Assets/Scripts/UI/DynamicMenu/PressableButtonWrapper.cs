using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;

public class PressableButtonWrapper : MonoBehaviour
{
    // Field assigned in inspector
    [SerializeField]
    private PressableButton m_PressableButton;
    [SerializeField]
    private TextMeshPro m_TextMeshPro;

    public void Init(ButtonDefinition buttonDefinition)
    {
        m_PressableButton.OnClicked.AddListener(() => buttonDefinition.OnClickAction?.Invoke());
        m_PressableButton.OnClicked.AddListener(() => buttonDefinition.OnClickSingleAction?.Invoke());
        m_TextMeshPro.SetText(buttonDefinition.ButtonName);
    }
}
