using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour {

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GotoInfoScreen()
    {
        SceneManager.LoadScene("Stats");
    }

}
