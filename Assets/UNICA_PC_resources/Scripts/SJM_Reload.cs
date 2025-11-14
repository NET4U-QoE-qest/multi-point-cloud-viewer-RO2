using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SJM_Reload : MonoBehaviour
{
    public int sceneToLoad = 0;
    public bool autoLoad = false;
    private void Start()
    {
        if(autoLoad)
        Invoke("Restart", 1f);
    }

    // Update is called once per frame
    public void Restart()
    {
        
        SceneManager.LoadScene(sceneToLoad,LoadSceneMode.Single);  
    } 
    

}
