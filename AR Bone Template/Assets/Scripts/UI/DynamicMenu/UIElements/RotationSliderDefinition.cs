using MixedReality.Toolkit;
using System;
using UnityEngine;

[Serializable]
public readonly struct RotationSliderDefinition : IElementDefinition
{
    public readonly Axis Axis;
    public readonly Transform TargetTransform;

    public RotationSliderDefinition(Axis axis, Transform targetTransform)
    {
        Axis = axis;
        TargetTransform = targetTransform;
    }
}
