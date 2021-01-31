using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float hugEfectiveness;
    public float hp_dark;
    public float hp_light;
    float maxHPdark;
    float maxHPlight;

    public float maxHealing;

    public float timeBetweenHits;
    [HideInInspector]
    public float timer;
    public UnityEvent deathEvent;
    public UnityEvent hitEvent;
    public UnityEvent stunEvent;
    public bool canUseForHealing;

    SpriteRenderer renderer;

    public Transform healthBar;
    public bool canHug;
    // Start is called before the first frame update
    void Start()
    {
        maxHPdark = hp_dark;
        maxHPlight = hp_light;
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
    }

    IEnumerator flash_(float times)
    {
        for (var n = 0; n < times; n++)
        {
            renderer.enabled = true;
            yield return new WaitForSeconds(.1f);
        renderer.enabled = false;
        yield return new WaitForSeconds(.1f);
    }
        renderer.enabled = true;
    }
    public void flash(float times)
    {
        StartCoroutine(flash_(times));
    }

    public void hurt(int damage)
    {
        if (timer < 0)
        {
            hp_dark = Mathf.Max(0, hp_dark - damage);
            timer = timeBetweenHits;

            float x = 1 * (hp_dark / maxHPdark);
            Vector3 scale = healthBar.GetChild(0).localScale;
            healthBar.GetChild(0).DOScaleX(x,0.2f);// localScale = new Vector3(x, scale.y, 1);
        }

        if (hp_dark == 0)
        {
            canHug = true;
            GetComponent<Enemy>().stunned = true;
            stunEvent.Invoke();

        }
        else
            hitEvent.Invoke();
    }

    public bool isBeeingHugged;
    public void hug()
    {
        isBeeingHugged = true;
        Vector3 scale = healthBar.GetChild(1).localScale;
        float x = Mathf.Max(0, scale.x - Time.deltaTime * hugEfectiveness);
        healthBar.GetChild(1).localScale = new Vector3(x, scale.y, 1);

        if(x == 0)
        {
            dying = true;
            StartCoroutine(waitBeforeHealing());
            deathEvent.Invoke();
        }
    }

    IEnumerator waitBeforeHealing()
    {
        yield return new WaitForSeconds(4);
        canUseForHealing = true;
    }

    public bool dying;
    void rest()
    {
        hp_dark = maxHPdark;
        hp_light = maxHPlight;
        healthBar.GetChild(0).DOScaleX(1, 1f);
        healthBar.GetChild(1).DOScaleX(1, 0f);
        GetComponent<Enemy>().animator.SetBool("stunned", false);
        GetComponent<Enemy>().stunned = false;
    }

    public void waitAndRest(float time)
    {
       
        StartCoroutine(waitAndRest_(time));
    }
    IEnumerator waitAndRest_(float time)
    {
        yield return new WaitForSeconds(time);
        while(isBeeingHugged)
            yield return new WaitForSeconds(2);
        if (!dying)
            rest();
    }


   
}
