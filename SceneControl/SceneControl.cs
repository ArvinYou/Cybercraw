
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    //load a scene from string
    public void LoadScene(string sceneName)
    {
        //load the scene
        SceneManager.LoadScene(sceneName);
    }

    //quit the game
    public void Quit()
    {
        Debug.Log("Quitting!");

        //actually quit
        Application.Quit();
    }
}

