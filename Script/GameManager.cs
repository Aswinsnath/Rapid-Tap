using UnityEngine;
using UnityEngine.SceneManagement;  
using UnityEngine.UI;  
public class GameManager : MonoBehaviour
{
    
    public Button homeButton;
    public Button nextButton;
    public Button replayButton;


   
    private int currentSceneIndex;

    void Start()
    {
       
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

       
        homeButton.onClick.AddListener(LoadHomeMenu);
        nextButton.onClick.AddListener(LoadNextScene);
        replayButton.onClick.AddListener(ReplayCurrentScene);
    }

   
    public void LoadHomeMenu()
    {
        SceneManager.LoadScene("Main Menu");  
    }

   
    public void LoadNextScene()
    {
        int nextSceneIndex = currentSceneIndex + 1;  
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more scenes to load.");
        }
    }

    
    public void ReplayCurrentScene()
    {
        SceneManager.LoadScene(currentSceneIndex);  
    }
}
