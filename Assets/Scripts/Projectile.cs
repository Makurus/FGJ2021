using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public bool hurtsEnemies = true;
    public bool hurtsPlayer = true;
    public float lag;
    public float hitTime = -1;
    public float waitBeforeDestroy = 0;
    public Vector2 direction;
    public float speed;
    public int damage;
    Rigidbody2D body;
    public UnityEvent hitEvent;
    public UnityEvent activationEvent;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        activationEvent.Invoke();
        if (hitTime >= 0)
            StartCoroutine(kill());
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = speed * direction; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hurtsEnemies && !collision.isTrigger && collision.gameObject.GetComponent<Health>())
        {
            var h = collision.gameObject.GetComponent<Health>();

            h.hurt(damage);
            hitEvent.Invoke();
        }
        if(hurtsPlayer && !collision.isTrigger && (collision.name == "Player" || collision.name == "Hearth"))
        {
            GameObject.FindObjectOfType<Humanity>().updateHumanity(damage);
            hitEvent.Invoke();
        }
       
        
        print("MOI");

    }

    IEnumerator kill()
    {
        yield return new WaitForSeconds(hitTime);
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, waitBeforeDestroy);
    }



}
