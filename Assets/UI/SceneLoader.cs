using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // ����Ŀ�곡��
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

