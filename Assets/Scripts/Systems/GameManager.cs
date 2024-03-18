using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public List<string> sceneNames; // List of scene names to load

    void Start()
    {
        LoadScenes();
        SoundManager.PlaySound("AmbientSound", true, 0.025f);
    }

    void LoadScenes()
    {
        foreach (string sceneName in sceneNames)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
        }
    }
}
