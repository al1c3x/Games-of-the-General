using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCondition : MonoBehaviour
{
    private GameManagerScript manager;
    private AudioManagerScript audiomanager;
    [SerializeField] private GameObject winButton;
    [SerializeField] private GameObject loseButton;
    [SerializeField] private GameObject whitewinButton;

    private void Awake()
    {
        manager = FindObjectOfType<GameManagerScript>();
        audiomanager = FindObjectOfType<AudioManagerScript>();
        checkWinCondition();
    }

    private void Start()
    {
        if (manager.winner == "human")
        {
            audiomanager.playSound("WinSoundEffect");
        }
           
        else
        {
            audiomanager.playSound("LoseSoundEffect");
        }
           
    }
    public void winbutton()
    {
        audiomanager.stopSound("Win");
        LoaderScript.loadScene(0, 3);
    }
    public void whitewinbutton()
    {
        LoaderScript.loadScene(0, 3);
    }


    public void losebutton()
    {
        audiomanager.stopSound("Lose");
        SceneManager.LoadScene(0);
    }

    public void checkWinCondition()
    {
        if (manager.winner == "human")
        {
            winButton.SetActive(true);
            manager.scores[0]++;
        }
       else
        {
            loseButton.SetActive(true);
            manager.scores[1]++;
        }
       
       
        
    }
}
 