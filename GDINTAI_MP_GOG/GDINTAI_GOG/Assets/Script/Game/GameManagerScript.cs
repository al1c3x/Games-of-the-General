using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
   
    [HideInInspector]public float time;
    [HideInInspector]public string turn = "player";    
    [HideInInspector]public string gameState = "pre-game";  
    [HideInInspector]public string winner = "none";  
    [HideInInspector]public int []scores = new int[2];  
    private static GameManagerScript instance;
    public Sprite[] gamePiecesImagesB = new Sprite[22];
    public Sprite[] gamePiecesImagesW = new Sprite[22];
    public string[] gamePiecesNames;


    private void Awake(){
        if (instance == null) {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
            
        DontDestroyOnLoad(instance);
    }
   
    void Start(){
        gamePiecesNames = new string[15]{ "Flag", "Spy", "Private", "Sergeant", "2nd Lieutenant", "1st Lieutenant", 
        "Captain", "Major", "Lieutenant Colonel", "Colonel", "One-Star General", 
        "Two-Star General", "Three-Star General", "Four-Star General", "Five-Star General"};
    }

   
    void Update(){
        configureTime();
        
    }


    public void configureTime(bool stop = false){
        if(stop==false)
        {
            time += Time.deltaTime;
        }
    }

    public void setTurn(string side)
    {
        turn = side;
        if(turn == "human")
        {
            GameObject.FindGameObjectWithTag("EnemyPieceSetTag").transform.SetSiblingIndex(6);
        }
        
        else if (turn == "ai2")
        {
            GameObject.FindGameObjectWithTag("PlayerPieceSetTag").transform.SetSiblingIndex(6);
        }
    }
}
