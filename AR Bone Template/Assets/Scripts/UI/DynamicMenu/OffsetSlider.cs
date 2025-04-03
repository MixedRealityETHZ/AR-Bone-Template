using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;

public class OffsetSlider : MonoBehaviour
{
    // Field assigned in inspector
    [SerializeField]
    private Slider m_Slider;                 // Reference to the slider
    [SerializeField]
    private TextMeshPro m_LabelText;         // Label for the offset slider
    [SerializeField]
    private TextMeshPro m_ValueText;         // Value display for the offset slider

    // Private
    private Transform m_ReferenceTransform;  // Reference to the ParentPlane
    private Transform m_AnchorChildPlane;    // Reference to the AnchorChildPlane
    private Vector3 m_OffsetDirection;
    private float SCALING_FACTOR_METER_TO_CM = 0.01f;
    private int MAX_VALUE = 50;
    private float m_CurrentOffset;

    #region MonoBehaviour functions

    private void OnEnable()
    {
        m_Slider.OnValueUpdated.AddListener(OnSliderValueUpdate);
    }

    private void OnDisable()
    {
        m_Slider.OnValueUpdated.RemoveListener(OnSliderValueUpdate);
    }

    public void Init(OffsetSliderDefinition offsetSliderDefinition)
    {
        m_ReferenceTransform = offsetSliderDefinition.ReferenceTransform;
        m_AnchorChildPlane = offsetSliderDefinition.TargetTransform;
        m_OffsetDirection = (m_AnchorChildPlane.position - m_ReferenceTransform.position).normalized;


        // Set the label text for clarity
        m_LabelText.text = "Offset:";

        // Initialize the offset display value
        float initialOffset = GetCurrentOffset() / SCALING_FACTOR_METER_TO_CM;
        float sliderOffset = (initialOffset + MAX_VALUE) / (2 * MAX_VALUE);
        m_Slider.Value = sliderOffset;

        // Apply the initial offset based on slider value
        UpdateOffset(initialOffset);
    }

    #endregion

    private void OnSliderValueUpdate(SliderEventData sliderEventData)
    {
        // Calculate the new offset distance
        float sliderOffset = sliderEventData.NewValue;
        float acutalOffset = -MAX_VALUE + (2 * MAX_VALUE * sliderOffset);

        // Apply the offset to position AnchorChildPlane
        UpdateOffset(acutalOffset);
    }

    private float GetCurrentOffset() => Vector3.Distance(m_AnchorChildPlane.position, m_ReferenceTransform.position);

    private void UpdateOffset(float offset)
    {
        m_CurrentOffset = offset;
        // Update the value text
        m_ValueText.text = offset.ToString("F1") + " cm";

        // Position AnchorChildPlane along ParentPlane's normal at the specified offset
        Vector3 offsetPosition = m_ReferenceTransform.position + m_OffsetDirection * offset * SCALING_FACTOR_METER_TO_CM;
        m_AnchorChildPlane.position = offsetPosition;
    }


    public void IncrementOffset()
    {
        m_CurrentOffset = Mathf.Clamp(Mathf.Round(m_CurrentOffset + 1), -MAX_VALUE, MAX_VALUE);
        float sliderOffset = (m_CurrentOffset + MAX_VALUE) / (2 * MAX_VALUE);
        m_Slider.Value = sliderOffset;
        UpdateOffset(m_CurrentOffset);
    }

    public void DecrementOffset()
    {
        m_CurrentOffset = Mathf.Clamp(Mathf.Round(m_CurrentOffset - 1), -MAX_VALUE, MAX_VALUE);
        float sliderOffset = (m_CurrentOffset + MAX_VALUE) / (2 * MAX_VALUE);
        m_Slider.Value = sliderOffset;
        UpdateOffset(m_CurrentOffset);
    }

    public void UpdateTargetTransforms(Transform referencePlane, Transform childPlane)
    {
        m_ReferenceTransform = referencePlane;
        m_AnchorChildPlane = childPlane;
    }
}
