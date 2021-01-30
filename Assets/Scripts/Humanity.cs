using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanity : MonoBehaviour
{
    public float maxHumanity;
    float monstrosity;
    float startScaleX;
    public Transform white;
    // Start is called before the first frame update
    void Start()
    {
        startScaleX = transform.localScale.x;
        transform.DOScaleX(0, 0);
        white.DOScaleX(startScaleX, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateHumanity(float change)
    {
        monstrosity += change;
        monstrosity = Mathf.Min(maxHumanity, monstrosity);
        transform.DOScaleX(monstrosity / maxHumanity * startScaleX, 0.5f);
        white.DOScaleX(startScaleX -( monstrosity / maxHumanity * startScaleX), 0.5f);
        if (monstrosity == maxHumanity)
            print("GAME OVER");
    }
}
