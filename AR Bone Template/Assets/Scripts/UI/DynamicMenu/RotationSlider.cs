using MixedReality.Toolkit;
using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;

public class RotationSlider : MonoBehaviour
{
    // Field assigned in inspector
    [SerializeField]
    private Slider m_Slider;
    [SerializeField]
    private TextMeshPro m_LabelText, m_ValueText;

    // Private
    private Axis m_Axis;
    private Transform m_TargetTransform;
    private float m_angle;
    private int MAX_VALUE = 90;

    #region MonoBehaviour functions

    private void OnEnable()
    {
        m_Slider.OnValueUpdated.AddListener(OnSliderValueUpdate);
    }

    private void OnDisable()
    {
        m_Slider.OnValueUpdated.RemoveListener(OnSliderValueUpdate);
    }

    #endregion

    public void Init(RotationSliderDefinition rotationSliderDefinition)
    {
        m_Axis = rotationSliderDefinition.Axis;
        m_TargetTransform = rotationSliderDefinition.TargetTransform;

        // Set the label text for clarity
        var labelText = m_Axis switch
        {
            Axis.X => "X: ",
            Axis.Y => "Y: ",
            Axis.Z => "Z: ",
            _ => throw new System.NotImplementedException()
        };
        m_LabelText.text = labelText;

        // Initialize the angle display value
        float initialAngle = GetCurrentAngle();
        float sliderAngle = (initialAngle + MAX_VALUE) / (2 * MAX_VALUE);
        m_Slider.Value = sliderAngle;

        // Apply the initial angle based on slider value
        UpdateAngle(initialAngle);
    }

    private void OnSliderValueUpdate(SliderEventData sliderEventData)
    {
        float sliderAngle = sliderEventData.NewValue;
        float actualAngle = -MAX_VALUE + (2 * MAX_VALUE * sliderAngle);
        UpdateAngle(actualAngle);
    }

    private float GetCurrentAngle()
    {
        Vector3 localEulerAngles = m_TargetTransform.localEulerAngles;
        float angle = m_Axis switch {
            Axis.X => localEulerAngles.x,
            Axis.Y => localEulerAngles.y,
            Axis.Z => localEulerAngles.z,
            _ => throw new System.NotImplementedException()
        };

        while(angle < -90) {
            angle += 360;
        }

        while(angle >= 270) {
            angle -= 360;
        }

        if(angle > 90) {
            angle -= 180;
        }
        return angle;
    }

    private void UpdateAngle(float angle)
    {
        m_angle = angle;
        m_ValueText.text = angle + "°";
        float xAngle = m_TargetTransform.localEulerAngles.x;
        float yAngle = m_TargetTransform.localEulerAngles.y;
        float zAngle = m_TargetTransform.localEulerAngles.z;

        switch (m_Axis)
        {
            case Axis.X:
                xAngle = angle;
                break;
            case Axis.Y:
                yAngle = angle;
                break;
            case Axis.Z:
                zAngle = angle;
                break;
        }

        m_TargetTransform.localEulerAngles = new Vector3(xAngle, yAngle, zAngle);
    }

    public void IncrementAngle()
    {
        m_angle = Mathf.Clamp(Mathf.Round(m_angle + 1), -MAX_VALUE, MAX_VALUE);
        float sliderAngle = (m_angle + MAX_VALUE) / (2 * MAX_VALUE);
        m_Slider.Value = sliderAngle;
        UpdateAngle(m_angle);
    }

    public void DecrementAngle()
    {
        m_angle = Mathf.Clamp(Mathf.Round(m_angle - 1), -MAX_VALUE, MAX_VALUE);
        float sliderAngle = (m_angle + MAX_VALUE) / (2 * MAX_VALUE);
        m_Slider.Value = sliderAngle;
        UpdateAngle(m_angle);
    }

    public void UpdateTargetTransforms(Transform targetTransform) => m_TargetTransform = targetTransform;
}
