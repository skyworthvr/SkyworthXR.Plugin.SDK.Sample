using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToSceen1() 
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    public void ToSceen0()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
