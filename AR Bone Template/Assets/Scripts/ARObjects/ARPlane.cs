
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;

public class ARPlane : ARObject
{
    // Field assigned in inspector
    [SerializeField]
    private Transform m_PlaneTransform;

    // Public
    public bool IsAnchored { get; set; } = false;
    public override ARObjectEnum GetARType() => ARObjectEnum.Plane;
    public string GetNextChildName()
    {
        m_CurrentChildIndex++;
        return $"{Name}_{m_CurrentChildIndex}";
    }

    // Private
    private Transform m_ReferencePlane;
    private int m_CurrentChildIndex;

    public Transform GetReferencePlane() => m_ReferencePlane;
    public Transform GetRootTransform() => m_RootTransform;
    public Vector3 GetNormal() => GetRootTransform().forward;

    public void Init(ARPlane referencePlane)
    {
        m_ReferencePlane = referencePlane.GetRootTransform();
        ObjectManipulator objectManipulator = GetRootTransform().GetComponent<ObjectManipulator>();
        objectManipulator.AllowedManipulations = MixedReality.Toolkit.TransformFlags.Scale;

        GetRootTransform().transform.position = referencePlane.GetRootTransform().position + referencePlane.GetNormal() * 0.05f;
        m_CurrentChildIndex = 0;
    }

}
