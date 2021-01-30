using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class door : MonoBehaviour
{
    public CanvasGroup cg;
    public int nextLevel;
    // Start is called before the first frame update
    void Start()
    {
        cg.DOFade(0, 1);
    }

    public void nextFloor()
    {
        cg.DOFade(1, 1);
        StartCoroutine(changeScene());
    }

    IEnumerator changeScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(nextLevel);
    }
    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.name == "Player")
            nextFloor();
    }
}
