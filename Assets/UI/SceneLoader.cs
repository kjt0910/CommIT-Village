using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 加载目标场景
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

