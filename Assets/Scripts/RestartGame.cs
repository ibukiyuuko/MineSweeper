using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestartGame : MonoBehaviour
{
    // Start is called before the first frame update
    private Game game;
    public Button cbutton;

    private void Start()
    {
        cbutton.onClick.AddListener(onBClick);
        Debug.Log("click");
    }
    public void onBClick()
    {
        Debug.Log("restart");
        game.NewGame();

        
    }
}
