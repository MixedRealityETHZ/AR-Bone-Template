using System;
using UnityEngine;

[Serializable]
public readonly struct OffsetSliderDefinition : IElementDefinition
{
    public readonly Transform ReferenceTransform;
    public readonly Transform TargetTransform;

    public OffsetSliderDefinition(Transform referenceTransform, Transform targetTransform)
    {
        ReferenceTransform = referenceTransform;
        TargetTransform = targetTransform;
    }
}
