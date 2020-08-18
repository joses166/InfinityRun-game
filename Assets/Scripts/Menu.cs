using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void abrirJogo() {
        SceneManager.LoadScene("Jogo", LoadSceneMode.Single);
    }
    public void abrirMenu() {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
    public void abrirGameOver() {
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }

}
