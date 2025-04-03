using MixedReality.Toolkit.SpatialManipulation;
using MixedReality.Toolkit.UX;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ARObject : MonoBehaviour, IARObject
{
    // Field assigned in inspector
    [SerializeField]
    private Material defaultMaterial, selectedMaterial;
    [SerializeField]
    protected Transform m_RootTransform;
    [SerializeField]
    private PressableButton m_SelectButton;
    [SerializeField]
    private TextMeshPro m_SelectButtonName;
    [SerializeField]
    private ObjectManipulator m_ObjectManipulator;
    [SerializeField]
    private Renderer m_ObjectRenderer;
    [SerializeField]
    private DrawOutline m_DrawOutline;

    // Public
    public bool IsSelected {  get; private set; }
    public string Name { get; private set; }
    public virtual ARObjectEnum GetARType() => ARObjectEnum.Point;

    // Private
    private List<IARObject> m_AssociatedARObjects = new();
    private bool m_IsDestroyed = false;
    private Material m_CurrentMaterial;
    private Vector3 m_SelectButtonStartScale;
    private ARToolManager m_ARToolManager;

    #region MonoBehaviour functions


    public void Init(string name)
    {
        Name = name;
        m_SelectButtonName.text = name;
        m_CurrentMaterial = defaultMaterial;
        IsSelected = false;
        m_DrawOutline.SetColor(Color.red);
        m_ARToolManager = (ARToolManager) FindObjectOfType(typeof(ARToolManager));
    }

    void Start()
    {
        m_ObjectRenderer.material = m_CurrentMaterial;
        m_SelectButtonStartScale = m_SelectButton.transform.lossyScale;
    }

    private void Update()
    {
        Bounds bounds = m_ObjectRenderer.bounds;
        Vector3 highestYLocalPos = bounds.center + new Vector3(0, bounds.extents.y + 0.01f, 0);
        Vector3 toCamVec = (Camera.main.transform.position - transform.position).normalized;

        m_SelectButton.transform.position = highestYLocalPos + toCamVec * 0.01f;
        m_SelectButton.transform.LookAt(Camera.main.transform);
        m_SelectButton.transform.Rotate(0, 180, 0);
        Vector3 currButtonWorldScale = m_SelectButton.transform.lossyScale;
        if (m_SelectButtonStartScale != currButtonWorldScale) {
            Vector3 resizeRatio = new(
                m_SelectButtonStartScale.x / currButtonWorldScale.x,
                m_SelectButtonStartScale.y / currButtonWorldScale.y,
                m_SelectButtonStartScale.z / currButtonWorldScale.z
            );
            Vector3 currButtonLocScale = m_SelectButton.transform.localScale;
            m_SelectButton.transform.localScale = Vector3.Scale(currButtonLocScale, resizeRatio);
        }

        m_ObjectRenderer.material = m_CurrentMaterial;
    }

    #endregion

    public void SelectOrDeselect()
    {
        if (IsSelected) {
            Deselect();
        } else {
            Select();
        }
    }

    public void Select()
    {
        m_ARToolManager.AddSelectedARObject(this);
        m_CurrentMaterial = selectedMaterial;
        m_DrawOutline.SetColor(Color.green);
        IsSelected = true;
    }

    public void Deselect()
    {
        m_ARToolManager.RemoveSelectedARObject(this);
        m_CurrentMaterial = defaultMaterial;
        m_DrawOutline.SetColor(Color.red);
        IsSelected = false;
    }

    public void DestroyARObject()
    {
        if (m_IsDestroyed)
            return;
        m_IsDestroyed = true;

        Destroy(m_RootTransform.gameObject);
    }

    public void AddAssociatedGameObject(IARObject arObject) => m_AssociatedARObjects.Add(arObject);
    public void RemoveAssociatedGameObject(IARObject arObject) => m_AssociatedARObjects.Remove(arObject);
    public bool IsAssociatedWithGameObject(IARObject other) => m_AssociatedARObjects.Contains(other);

    public void EnableDisableSelectButton(bool active) => m_SelectButton.gameObject.SetActive(active);
    public Transform GetSelectButtonTransform() => m_SelectButton.transform;
}
