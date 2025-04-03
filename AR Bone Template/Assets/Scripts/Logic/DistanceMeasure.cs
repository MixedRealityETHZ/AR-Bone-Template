using UnityEngine;
using TMPro;

public class DistanceMeasure : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro m_DistanceText;
    [SerializeField]
    private LineRenderer m_LineRenderer;
    [SerializeField]
    private ARObject m_ARObject1, m_ARObject2;

    void Update()
    {
        if(m_ARObject1 == null || m_ARObject2 == null) {
            DestroyMeasurement();
            return;
        }

        Transform arObjectTranform1 = m_ARObject1.transform;
        Transform arObjectTranform2 = m_ARObject2.transform;

        m_LineRenderer.SetPosition(0, arObjectTranform1.position);
        m_LineRenderer.SetPosition(1, arObjectTranform2.position);

        float dinstanceMeter = Vector3.Distance(arObjectTranform1.position, arObjectTranform2.position); ;
        int distanceCm = Mathf.RoundToInt(100 * dinstanceMeter);
        m_DistanceText.text = $"{distanceCm} cm";
        // Position text in the middle of the line
        m_DistanceText.transform.position = (arObjectTranform1.position + arObjectTranform2.position) / 2;
        m_DistanceText.transform.LookAt(Camera.main.transform);
        m_DistanceText.transform.Rotate(0, 180, 0);
    }

    public void Init(ARObject aRObject1, ARObject aRObject2)
    {
        m_ARObject1 = aRObject1;
        m_ARObject2 = aRObject2;
    }

    public void DestroyMeasurement()
    {
        m_ARObject1?.RemoveAssociatedGameObject(m_ARObject2);
        m_ARObject2?.RemoveAssociatedGameObject(m_ARObject1);
        Destroy(gameObject);
    }

}
