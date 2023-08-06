using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public CustomSound[] sounds;
    public static AudioManagerScript instance;

    void Awake()
    {
        
        if (instance == null){
            instance = this;
        }
            
        else{
            Destroy(gameObject);
        }
            
        DontDestroyOnLoad(gameObject);

        foreach (var x in sounds){
            x.source = gameObject.AddComponent<AudioSource>();
            x.source.clip = x.clip;
            x.source.volume = x.volume;
            x.source.pitch = x.pitch;
            x.source.loop = x.loop;
        }
    }

    void Start() {
        playSound("BgMusic");
    }

 

    public void playSound(string name) {
        CustomSound music = Array.Find(sounds, sound => sound.name == name);
        music.source.Play();
    }

    public void stopSound(string name) {
        CustomSound music = Array.Find(sounds, sound => sound.name == name);
        music.source.Stop();
    }
}
