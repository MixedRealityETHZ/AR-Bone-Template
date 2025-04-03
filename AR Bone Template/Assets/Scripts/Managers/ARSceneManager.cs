using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneManager : MonoBehaviour
{
    public void LoadMainScene()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void LoadTutorialScene()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
