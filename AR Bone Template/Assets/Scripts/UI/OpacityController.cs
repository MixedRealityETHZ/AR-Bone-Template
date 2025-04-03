using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;

public class OpacityController : MonoBehaviour
{
    // Field assigned in inspector
    [SerializeField]
    private Slider m_Slider;
    [SerializeField]
    private TextMeshPro m_LabelText;
    [SerializeField]
    private TextMeshPro m_ValueText;

    [SerializeField]
    private Material[] m_TargetMaterials;

    #region MonoBehaviour functions

    private void OnEnable()
    {
        m_Slider.OnValueUpdated.AddListener(OnSliderValueUpdate);
    }

    private void OnDisable()
    {
        m_Slider.OnValueUpdated.RemoveListener(OnSliderValueUpdate);
    }

    private void Start()
    {
        // Set the label text for clarity
        m_LabelText.text = "Opacity";

        // Initialize the initial opacity
        float initialOpacity = GetCurrentOpacity();
        m_Slider.Value = initialOpacity; // Set slider position to match the current opacity
        ApplyOpacity(initialOpacity);
    }

    #endregion

    private void OnSliderValueUpdate(SliderEventData sliderEventData)
    {
        // Calculate the new opacity
        float opacity = sliderEventData.NewValue;

        // Apply the new opacity to all target materials
        ApplyOpacity(opacity);
    }

    private float GetCurrentOpacity()
    {
        // Return the average opacity of all materials (assumes they start with the same alpha value)
        if (m_TargetMaterials.Length > 0 && m_TargetMaterials[0].HasProperty("_Color"))
        {
            return m_TargetMaterials[0].color.a; // Assume the first material represents the group
        }

        return 1f; // Default to fully opaque if no materials are available
    }

    private void ApplyOpacity(float opacity)
    {
        // Update the value text
        m_ValueText.text = (opacity * 100f).ToString("F0") + "%";

        foreach (Material material in m_TargetMaterials)
        {
            if (material.HasProperty("_Color"))
            {
                Color color = material.color;
                color.a = opacity; // Set the alpha value
                material.color = color;
            }
        }
    }

    public void UpdateTargetMaterials(Material[] targetMaterials)
    {
        // Update the list of target materials dynamically
        m_TargetMaterials = targetMaterials;
    }
}
