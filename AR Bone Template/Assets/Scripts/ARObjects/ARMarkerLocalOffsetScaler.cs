using Microsoft.MixedReality.OpenXR;
using System;
using UnityEngine;

[RequireComponent(typeof(ARMarker))]
public class ARMarkerLocalOffsetScaler : MonoBehaviour
{
    private ARMarker m_arMarker;
    [SerializeField]
    private Vector3 initialOffset;

    /// <summary>
    /// Transform containing marker contents that needs to be scaled.
    /// </summary>
    [Tooltip("Transform containing marker contents that needs to be localy offset depending on the scale.")]
    public Transform markerLocalOffsetTransform;

    private void OnEnable()
    {
        m_arMarker = GetComponent<ARMarker>();
        if (markerLocalOffsetTransform == null) {
            markerLocalOffsetTransform = gameObject.transform;
        }
    }

    private void Update()
    {
        // Offset the marker contents based on the computed scale factor.
        float scaleFactor = (float) Math.Sqrt(m_arMarker.size.x * m_arMarker.size.y);
        markerLocalOffsetTransform.transform.localPosition = initialOffset * scaleFactor;
    }
}
