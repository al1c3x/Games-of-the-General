using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class LoaderScript
{
    private static Action onLoadqueue;

    
    public static void loadScene(int scene, int loadingScene){
        
        onLoadqueue = () => { SceneManager.LoadScene(scene); };
        SceneManager.LoadScene(loadingScene);
    }

    public static void LoaderCallBack(){
        if(onLoadqueue != null){
            onLoadqueue(); 
            onLoadqueue = null;  
        }
    }
}
