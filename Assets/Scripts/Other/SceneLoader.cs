using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string name;

    void Start()
    {
        SceneManager.LoadScene(name);
    }
}
