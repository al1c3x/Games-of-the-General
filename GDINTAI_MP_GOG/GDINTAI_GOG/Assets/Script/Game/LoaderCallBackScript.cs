using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallBackScript : MonoBehaviour
{
    private bool firstCall = true;
    
    void Update(){
        if(this.firstCall){
            this.firstCall = false;
            LoaderScript.LoaderCallBack();
        }
    }
}
