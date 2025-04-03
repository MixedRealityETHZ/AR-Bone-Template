using System.Collections.Generic;
using UnityEngine;

public class DynamicMenu : MonoBehaviour
{
    // Field assigned in inspector
    [SerializeField]
    private Transform m_ElementHolder;
    [SerializeField]
    private GameObject m_DynamicMenuPagePrefab;
    [SerializeField]
    private GameObject m_NextButton, m_BackButton;
    [SerializeField]
    private List<DynamicMenuPage> m_PageList;

    // Private
    private int m_CurrentMaxPageNumber = 0;
    private int m_CurrentPageIdx = 0;

    // Construction Dynamic Menu
    public void Init()
    {
        foreach (DynamicMenuPage page in m_PageList) {
            page.gameObject.SetActive(false);
        }

        m_CurrentPageIdx = 0;
        UpdateNavigationButtons();
        gameObject.SetActive(true);

        if(m_PageList.Count > 0) {
            m_PageList[m_CurrentPageIdx].gameObject.SetActive(true);
        }
    }

    public void AddElement(int pageIndex, IElementDefinition elementDefinition)
    {
        while(pageIndex >= m_PageList.Count) {
            GameObject menuPage = Instantiate(m_DynamicMenuPagePrefab, m_ElementHolder);
            DynamicMenuPage dynamicMenuPage = menuPage.GetComponent<DynamicMenuPage>();
            m_PageList.Add(dynamicMenuPage);
            menuPage.SetActive(false);
        }

        DynamicMenuPage targetMenuPage = m_PageList[pageIndex];
        targetMenuPage.AddElement(elementDefinition, targetMenuPage.transform);
        m_CurrentMaxPageNumber = Mathf.Max(m_CurrentMaxPageNumber, pageIndex);
    }

    public void RemoveElement(int pageIndex, IElementDefinition elementDefinition)
    {
        if(pageIndex >= m_PageList.Count) {
            return;
        }
        DynamicMenuPage targetMenuPage = m_PageList[pageIndex];
        targetMenuPage.RemoveElement(elementDefinition);
    }

    public void ResetMenu()
    {
        foreach (DynamicMenuPage page in m_PageList) {
            page.RemoveAllElements();
        }
        m_CurrentMaxPageNumber = 0;
    }

    // Navigating Dynamic Menu

    public void NextPage()
    {
        m_PageList[m_CurrentPageIdx].gameObject.SetActive(false);
        m_CurrentPageIdx++;
        m_PageList[m_CurrentPageIdx].gameObject.SetActive(true);

        UpdateNavigationButtons();
    }

    public void PreviousPage()
    {
        m_PageList[m_CurrentPageIdx].gameObject.SetActive(false);
        m_CurrentPageIdx--;
        m_PageList[m_CurrentPageIdx].gameObject.SetActive(true);

        UpdateNavigationButtons();
    }

    private void UpdateNavigationButtons()
    {
        m_NextButton.SetActive(m_CurrentPageIdx < m_CurrentMaxPageNumber);
        m_BackButton.SetActive(m_CurrentPageIdx > 0);
    }
}
