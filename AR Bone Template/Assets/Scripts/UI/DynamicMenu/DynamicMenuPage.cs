using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicMenuPage : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ButtonPrefab, m_RotationSliderPrefab, m_OffsetSliderPrefab;

    // Private
    private List<IElementDefinition> m_ElementDefinitions;
    private Dictionary<IElementDefinition, GameObject> m_ElementMapping = new();
    private const float m_ElementHolderYOffset = -0.04f;

    public void AddElement(IElementDefinition elementDefinition, Transform elementHolder)
    {
        int numElements = m_ElementMapping.Count;
        GameObject elementObj = null;

        if (elementDefinition is ButtonDefinition buttonDefinition) {
            elementObj = Instantiate(m_ButtonPrefab, elementHolder);
            elementObj.transform.localPosition = new(0, numElements * m_ElementHolderYOffset, 0);
            PressableButtonWrapper pbWrapper = elementObj.GetComponent<PressableButtonWrapper>();
            pbWrapper.Init(buttonDefinition);
        } else if (elementDefinition is RotationSliderDefinition rotationSliderDefinition) {
            elementObj = Instantiate(m_RotationSliderPrefab, elementHolder);
            elementObj.transform.localPosition = new(0, numElements * m_ElementHolderYOffset, 0);
            RotationSlider rotationSlider = elementObj.GetComponent<RotationSlider>();
            rotationSlider.Init(rotationSliderDefinition);
        } else if (elementDefinition is OffsetSliderDefinition offsetSliderDefinition) {
            elementObj = Instantiate(m_OffsetSliderPrefab, elementHolder);
            elementObj.transform.localPosition = new(0, numElements * m_ElementHolderYOffset, 0);
            OffsetSlider offsetSlider = elementObj.GetComponent<OffsetSlider>();
            offsetSlider.Init(offsetSliderDefinition);
        }

        if(elementObj == null) {
            return;
        }

        m_ElementMapping.Add(elementDefinition, elementObj);
    }

    public void RemoveElement(IElementDefinition elementDefinition)
    {
        if (!m_ElementMapping.TryGetValue(elementDefinition, out GameObject elementObj)) {
            // If element to remove not found, then return function.
            return;
        }

        // Remove element
        m_ElementMapping.Remove(elementDefinition);
        Destroy(elementObj);

        // Adjust position of remaining elements
        int numElements = 0;
        foreach (KeyValuePair<IElementDefinition, GameObject> entry in m_ElementMapping) {
            entry.Value.transform.localPosition = new(0, numElements * m_ElementHolderYOffset, 0);
            numElements++;
        }
    }

    public void RemoveAllElements()
    {
        List<IElementDefinition> elementDefinitions = m_ElementMapping.Keys.ToList();
        foreach (IElementDefinition elementDefinition in elementDefinitions) {
            RemoveElement(elementDefinition);
        }
    }
}
