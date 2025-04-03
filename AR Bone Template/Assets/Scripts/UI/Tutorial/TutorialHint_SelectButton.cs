using UnityEngine;

public class TutorialHint_SelectButton : TutorialHint
{
    [SerializeField]
    private ARObjectEnum TargetARObjectType;
    [SerializeField]
    private Vector3 OffsetToTarget;
    [SerializeField]
    private Vector3 HintOrientation;

    private Transform m_TargetTransform;

    public override void SetActive(bool isActive)
    {
        base.SetActive(isActive);
        if (!isActive) {
            return;
        }

        ARObject targetObject = null;
        Object[] arObjects = FindObjectsOfType(typeof(ARObject));
        for (int i = 0; i < arObjects.Length; i++) {
            if (arObjects[i] is ARObject arObject && arObject.GetARType() == TargetARObjectType) {
                targetObject = arObject;
                break;
            }
        }

        if (targetObject == null) {
            base.SetActive(false);
            return;
        }

        m_TargetTransform = targetObject.GetSelectButtonTransform();
        transform.eulerAngles = HintOrientation;
    }

    public void Update()
    {
        if (m_TargetTransform == null) {
            SetActive(false);
            return;
        }

        transform.position = m_TargetTransform.position + OffsetToTarget;
    }
}
