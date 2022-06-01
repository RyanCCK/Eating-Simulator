using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DeathFloor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Reload scene on death
    ////////////TODO////////////TODO////////////TODO////////////
    //  TODO: Replace this with a proper event, and switch gameobject to be a trigger 
    ////////////TODO////////////TODO////////////TODO////////////
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
