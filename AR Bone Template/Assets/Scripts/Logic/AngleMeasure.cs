using UnityEngine;
using TMPro;

public class AngleMeasure : MonoBehaviour
{
    // Field assigned in inspector
    [SerializeField]
    private TextMeshPro m_AngleText1, m_AngleText2;

    // Private
    private ARPlane m_ARPlane1, m_ARPlane2;

    void Update()
    {
        if (m_ARPlane2 == null || m_ARPlane2 == null)
        {
            DestroyMeasurement();
            return;
        }

        Vector3 normal1 = m_ARPlane1.GetNormal();
        Vector3 normal2 = m_ARPlane2.GetNormal();
        int angleInner = CalculateAngle(normal1, normal2);
        int angleOuter = 180 - angleInner;

        m_AngleText1.text = $"{angleOuter}°";
        m_AngleText2.text = $"{angleInner}°";

        // Find the center position between the two planes
        Vector3 centerPosition = (m_ARPlane1.transform.position + m_ARPlane2.transform.position) / 2;

        // Calculate the arc positions using spherical interpolation (Slerp)
        Vector3 arcPosition1 = centerPosition + Vector3.Slerp(normal1, normal2, 0.5f).normalized * 0.05f; // Midpoint for angle1
        Vector3 arcPosition2 = centerPosition + Vector3.Slerp(normal1, -normal2, 0.5f).normalized * 0.05f; // Midpoint for angle2

        m_AngleText1.transform.position = arcPosition1;
        m_AngleText1.transform.LookAt(Camera.main.transform);
        m_AngleText1.transform.Rotate(0, 180, 0);

        m_AngleText2.transform.position = arcPosition2;
        m_AngleText2.transform.LookAt(Camera.main.transform);
        m_AngleText2.transform.Rotate(0, 180, 0);
    }

    private int CalculateAngle(Vector3 normal1, Vector3 normal2)
    {
        float dotProduct = Vector3.Dot(normal1.normalized, normal2.normalized);
        float angleRadians = Mathf.Acos(dotProduct);

        if (float.IsNaN(angleRadians)) {
            angleRadians = 0;
        }
        return Mathf.RoundToInt(angleRadians * Mathf.Rad2Deg); // Convert to degrees
    }

    public void Init(ARPlane aRPlane1, ARPlane aRPlane2)
    {
        m_ARPlane1 = aRPlane1;
        m_ARPlane2 = aRPlane2;
    }

    public void DestroyMeasurement()
    {
        Destroy(gameObject);
    }

}
