using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [HideInInspector]public string gameMode; //humanvsai or aivsai
    [HideInInspector]public float time;
    private bool Lose = false;
    private GameManagerScript instance;
    public Sprite[] gamePiecesImagesB = new Sprite[21];
    public Sprite[] gamePiecesImagesW = new Sprite[21];
    public string[] gamePiecesNames;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(instance);
    }
    // Start is called before the first frame update
    void Start()
    {
        gamePiecesNames = new string[21]{ "Flag", "Spy", "Spy", "Private", "Private", "Private", "Private", "Private", "Private",
        "Sergeant", "2nd Lieutenant", "1st Lieutenant", "Captain", "Major", "Lieutenant Colonel", "Colonel", "One-Star General", 
        "Two-Star General", "Three-Star General", "Four-Star General", "Five-Star General"};
    }

    // Update is called once per frame
    void Update()
    {
        configureTime();
    }

    public void chosenGameMode(string choice)
    {
        this.gameMode = choice;
    }

    public void configureTime(bool stop = false)
    {
        if(stop==false)
        {
            time += Time.deltaTime;
        }
    }
}
