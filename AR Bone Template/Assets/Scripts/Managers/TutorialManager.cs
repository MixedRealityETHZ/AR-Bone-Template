using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // Field assigned in inspector
    [SerializeField]
    private Transform m_TutorialSlideHolder;
    [SerializeField]
    public GameObject m_BackButton, m_NextButton;
    [SerializeField]
    private Transform m_QRCodeSlide;
    [SerializeField]
    private QRAnchorManager m_QRAnchorManager;
    [SerializeField]
    private ARToolManager m_ARToolManager;
    [SerializeField]
    private GameObject m_EndTutorialSlide;

    // Private
    private List<TutorialSlide> m_TutorialSlides;
    private int m_SlideIndex = 0;
    private bool m_Initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        m_TutorialSlides = new List<TutorialSlide>();
        foreach (Transform slide in m_TutorialSlideHolder){
            TutorialSlide tutorialSlide = slide.GetComponent<TutorialSlide>();
            if (tutorialSlide != null) {
                m_TutorialSlides.Add(tutorialSlide);
            }
        }

        int slideCount = m_TutorialSlides.Count;
        for (int i = 0; i < slideCount; i++) {
            m_TutorialSlides[i].Init(i, slideCount);
        }

        InitTutorial();
    }

    private void Update()
    {
        if (!m_Initialized && m_QRAnchorManager.QRCodeScannedConstistenly) {
            OnQRcodeFound();
        }
    }

    public void InitTutorial()
    {
        m_BackButton.SetActive(false);
        m_NextButton.SetActive(false);

        for (int i = 0; i < m_TutorialSlides.Count; i++) {
            m_TutorialSlides[i].gameObject.SetActive(false);
        }
        m_QRCodeSlide.gameObject.SetActive(true);
        m_EndTutorialSlide.gameObject.SetActive(false);
    }

    public void OnQRcodeFound()
    {
        m_Initialized = true;
        m_QRCodeSlide.gameObject.SetActive(false);

        m_SlideIndex = 0;
        InitInstruction(m_TutorialSlides[0]);

    }

    public void PreviousInstructions()
    {
        m_SlideIndex = Mathf.Max(0, m_SlideIndex - 1);
        InitInstruction(m_TutorialSlides[m_SlideIndex]);
    }

    public void NextInstructions()
    {
        m_SlideIndex = m_SlideIndex + 1;
        if (m_SlideIndex < m_TutorialSlides.Count) {
            InitInstruction(m_TutorialSlides[m_SlideIndex]);
        } else {
            DisableTutorialSlides();
            m_QRCodeSlide.gameObject.SetActive(false);
            m_EndTutorialSlide.gameObject.SetActive(true);
        }
    }

    private void InitInstruction(TutorialSlide tutorialSlide)
    {
        DisableTutorialSlides();
        if (m_SlideIndex == 0) {
            m_BackButton.SetActive(false);
            m_NextButton.SetActive(true);
        } else if (m_SlideIndex == m_TutorialSlides.Count) {
            m_BackButton.SetActive(true);
            m_NextButton.SetActive(false);
        } else {
            m_BackButton.SetActive(true);
            m_NextButton.SetActive(true);
        }
        tutorialSlide.SetActiveSlide(true);
    }

    private void DisableTutorialSlides()
    {
        m_BackButton.SetActive(false);
        m_NextButton.SetActive(false);

        for (int i = 0; i < m_TutorialSlides.Count; i++) {
            m_TutorialSlides[i].SetActiveSlide(false);
        }
    }
}
