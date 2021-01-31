using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    bool pressed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !pressed)
        {
            GetComponent<Animator>().SetTrigger("exit");
            pressed = true;
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}
