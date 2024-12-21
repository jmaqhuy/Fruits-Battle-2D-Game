using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBackGroundMusic : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        backgroundMusic existingMusic = FindObjectOfType<backgroundMusic>();
        if (existingMusic != null)
        {
            Debug.Log("Back Ground Music Found");
            backgroundMusic.DestroyInstance();
        }
        else
        {
            Debug.Log("Back Ground Music Not Found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
