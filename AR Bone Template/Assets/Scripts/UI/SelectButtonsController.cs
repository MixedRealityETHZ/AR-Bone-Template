using UnityEngine;

public class SelectButtonsController : MonoBehaviour
{
    public void OnToggleOnOff(bool active)
    {
        Object[] arObjects = FindObjectsOfType(typeof(ARObject));
        for (int i = 0; i < arObjects.Length; i++) {
            if (arObjects[i] is ARObject arObject) {
                arObject.EnableDisableSelectButton(active);
            }
        }
    }
}
