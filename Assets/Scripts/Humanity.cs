using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanity : MonoBehaviour
{
    public Transform player;
    public Transform hearth;
    public float safeZone;
    public float dangerTimer;
    public float healthDrippingSpeed;
    public float healthSuckingSpeed;
    public float changeSpeed;
    public float maxHumanity;
    float monstrosity;
    float visualX;
    float startScaleX;
    public Transform white;
    [SerializeField] GameObject deathScreen;
    // Start is called before the first frame update
    void Start()
    {
        startScaleX = transform.localScale.x;
        transform.DOScaleX(0, 0);
        white.DOScaleX(startScaleX, 0);
    }

    float notSafe;
    public bool heal;
    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(player.position, hearth.position) > safeZone)
        {
            notSafe += Time.deltaTime;
        }
        else
            notSafe = 0;

        if(notSafe > dangerTimer && player.GetComponent<PlayerMove>().isMonster)
        {
            monstrosity += Time.deltaTime * healthDrippingSpeed;
        }
        if (heal)
        {
            monstrosity -= Time.deltaTime * healthSuckingSpeed;
            monstrosity = Mathf.Max(0, monstrosity);
        }

        visualX = visualX + ((monstrosity / maxHumanity * startScaleX) - visualX) * changeSpeed;
        transform.localScale = new Vector3(visualX, transform.localScale.y, transform.localScale.z);

        if (monstrosity == maxHumanity)
            deathScreen.SetActive(true);
    }

    public void updateHumanity(float change)
    {
        monstrosity += change;
        monstrosity = Mathf.Min(maxHumanity, monstrosity);
        //transform.DOScaleX(monstrosity / maxHumanity * startScaleX, 0.5f);
        //white.DOScaleX(startScaleX -( monstrosity / maxHumanity * startScaleX), 0.5f);
       
    }

    private void OnDrawGizmos()
    {
       
        //Gizmos.(transform.position, enemysSee);
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(hearth.position, Vector3.back, safeZone);
    }

}
