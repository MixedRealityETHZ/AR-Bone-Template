using TMPro;
using UnityEngine;

public class TutorialSlide : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro m_SlideNumberText;
    [SerializeField]
    private TutorialHint[] m_TutorialHints;

    private int m_SlideNumber;

    public void Init(int slideNumber, int totalSlideCount)
    {
        m_SlideNumber = slideNumber + 1;
        m_SlideNumberText.text = $"{m_SlideNumber}/{totalSlideCount}";
        SetActiveSlide(false);
    }

    public void SetActiveSlide(bool isActive)
    {
        for (int i = 0; i < m_TutorialHints.Length; i++) {
            if(m_TutorialHints[i] != null) {
                m_TutorialHints[i].SetActive(isActive);
            }
        }

        gameObject.SetActive(isActive);
    }
}
