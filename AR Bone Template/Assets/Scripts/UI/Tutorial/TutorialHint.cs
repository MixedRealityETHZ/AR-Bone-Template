using UnityEngine;

public class TutorialHint : MonoBehaviour
{

    private void Awake()
    {
        SetActive(false);
    }

    public virtual void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
