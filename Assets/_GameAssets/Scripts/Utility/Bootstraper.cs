using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstraper : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Scene _currentScene = SceneManager.GetActiveScene();
    
        if (_currentScene.name != "InitScene")
        {
            foreach (GameObject obj in FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                obj.SetActive(false);
                
            SceneManager.LoadScene("InitScene");
        }
        
        SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
    }
}