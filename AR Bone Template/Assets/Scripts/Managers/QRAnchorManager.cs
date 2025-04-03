using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.OpenXR;

// This manages QR anchors, i.e. it creates an anchor object once a QR code is detected and makes things track the anchor
public class QRAnchorManager : MonoBehaviour
{
    // Field assigned in inspector
    [SerializeField]
    private ARMarkerManager m_ARMarkerManager;
    [SerializeField]
    private ARToolManager m_ARToolManager;

    // Public
    public bool FoundQRCode { get; private set; }
    public bool QRCodeScannedConstistenly { get; private set; }

    // Private
    private ARMarker m_TrackedQRcode; // We only track one QR code (I think) so we set this one variable to the first tracked code
    private GameObject m_TrackedQRAnchor;
    private List<GameObject> m_TrackedObjects = new();
    private const float TRACKING_LOST_TIMEOUT = 1f;
    private const float CONSISTENT_QR_SCANNED_WAIT_TIME = TRACKING_LOST_TIMEOUT + 1f;
    private float m_TimeOffset = 0;
    private float m_StableQRCodeTimer;

    private void Awake()
    {
        m_ARToolManager.DebugText("Try find QR Code");
        m_ARMarkerManager.markersChanged += OnQRCodesChanged;
        m_TrackedQRcode = null;
        m_TrackedQRAnchor = new GameObject("QRCodeAnchor");
        StopTracking();
        m_StableQRCodeTimer = CONSISTENT_QR_SCANNED_WAIT_TIME;

        if (!m_ARMarkerManager.enabled || m_ARMarkerManager.subsystem == null)
        {
            m_ARToolManager.DebugText($"ARMarkerManager not enabled or available; sample marker functionality will not be enabled.");
            return;
        }
    }

    private void Update()
    {
        float realtimeSinceStartupWithOffset = Time.realtimeSinceStartup + m_TimeOffset;
        if (m_TrackedQRcode != null) {
            if (FoundQRCode && Mathf.Abs(m_TrackedQRcode.lastSeenTime - realtimeSinceStartupWithOffset) > TRACKING_LOST_TIMEOUT) {
                StopTracking();
            }else if(!FoundQRCode) {
                if (Mathf.Abs(m_TrackedQRcode.lastSeenTime - realtimeSinceStartupWithOffset) < TRACKING_LOST_TIMEOUT) {
                    StartTracking();
                }
            }
        }

        if (FoundQRCode && !QRCodeScannedConstistenly) {
            if (!QRCodeScannedConstistenly) {
                m_StableQRCodeTimer -= Time.deltaTime;
                if (m_StableQRCodeTimer < 0) {
                    QRCodeScannedConstistenly = true;
                    foreach (GameObject entity in m_TrackedObjects) { AttachToQRAnchor(entity); }
                }
            }
        } else {
            m_StableQRCodeTimer = CONSISTENT_QR_SCANNED_WAIT_TIME;
            QRCodeScannedConstistenly = false;
        }
    }

    void OnQRCodesChanged(ARMarkersChangedEventArgs args)
    {
        foreach (ARMarker qrCode in args.added)
        {
            if (m_TrackedQRcode == null)
            {
                m_TrackedQRcode = qrCode;
                StartTracking();
            }
        }

        foreach (ARMarker qrCode in args.updated)
        {
            if (m_TrackedQRcode == null) {
                m_TrackedQRcode = qrCode;
                StartTracking();
            }
            else if (qrCode == m_TrackedQRcode)
            {
                m_TimeOffset = m_TrackedQRcode.lastSeenTime - Time.realtimeSinceStartup;
                UpdateQRAnchor(qrCode);
            }
        }

        foreach (ARMarker qrCode in args.removed)
        {
            m_ARToolManager.DebugText("Found removed QR Code");
            if (qrCode == m_TrackedQRcode)
            {
                m_TrackedQRcode = null;
                StopTracking();
            }
        }
    }

    public void UpdateQRAnchor(ARMarker qrCode)
    {
        if(m_TrackedQRAnchor == null) {
            return;
        }
        float translateSpeed = 5f;
        float rotateSpeed = 10f;
        m_TrackedQRAnchor.transform.position = Vector3.MoveTowards(m_TrackedQRAnchor.transform.position, qrCode.transform.position, translateSpeed * Time.deltaTime);
        m_TrackedQRAnchor.transform.rotation = Quaternion.Slerp(m_TrackedQRAnchor.transform.rotation, qrCode.transform.rotation, rotateSpeed * Time.deltaTime);
    }

    public void AttachToQRAnchor(GameObject entity)
    {
        entity.transform.SetParent(m_TrackedQRAnchor.transform, worldPositionStays: true);
    }

    // Adds a reference to this object and attaches it to the anchor (in case there is one)
    // We need the reference in case tehre is no anchor so that we can track it later if one appears
    public void TrackObject(GameObject entity)
    {
        m_TrackedObjects.Add(entity);
        if (QRCodeScannedConstistenly && m_TrackedQRAnchor != null) {
            AttachToQRAnchor(entity);
        }
    }

    private void StartTracking()
    {
        m_TimeOffset = m_TrackedQRcode.lastSeenTime - Time.realtimeSinceStartup;
        FoundQRCode = true;
        m_ARToolManager.DebugText("Found QR Code");
        UpdateQRAnchor(m_TrackedQRcode);
    }
    
    private void StopTracking()
    {
        m_ARToolManager.DebugText("Stopped Tracking");
        FoundQRCode = false;
    }
}
