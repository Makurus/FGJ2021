using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    Transform player;
    Transform hearth;

    Rigidbody2D body;
    public float movSpeed;
    public float MaxCoolDown;
    public float MinCoolDown;
    float newCoolDown;
    float coolDownTimer;
    bool stop;
    public bool stunned;
   
    [Space]
    public Transform enemyWeapon;
    int attacking;
    
    public GameObject projectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        hearth = GameObject.Find("Hearth").transform;
        body = GetComponent<Rigidbody2D>();
        newCoolDown = Random.Range(MinCoolDown, MaxCoolDown);
    }

    // Update is called once per frame
    void Update()
    {
      

        var target =  Vector3.Distance(transform.position, player.position) < Vector3.Distance(transform.position, hearth.position) ? player : hearth;
        if (!stunned)
        {
            coolDownTimer += Time.deltaTime;
            if (attacking > 0)
            {
                Vector2 facingDir = (target.position - transform.position).normalized;

                enemyWeapon.rotation = Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, -facingDir)));

                if (coolDownTimer > newCoolDown)
                {
                    coolDownTimer = 0;
                    newCoolDown = Random.Range(MinCoolDown, MaxCoolDown);
                    var g = Instantiate(projectilePrefab, enemyWeapon.GetChild(0).position, enemyWeapon.rotation, null);
                    g.GetComponent<Projectile>().direction = (target.position - transform.position).normalized;
                }
            }

            if (!stop)
                body.velocity = (target.position - transform.position).normalized * movSpeed;
        }
       

       
    }

    public void Stop(float time)
    {
        if (stop)
            return;
        StartCoroutine(Stop_(time));
    }

    IEnumerator Stop_(float time)
    {
        stop = true;
        yield return new WaitForSeconds(time);
        if(!GetComponent<Health>().dying)
            stop = false;
    }

    public void pushBack(float force)
    {
        body.velocity = (transform.position - player.position).normalized * force;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
     
        if (!collision.isTrigger && (collision.name == "Player" || collision.name == "Hearth"))
        {
            attacking++;
               
        }
        

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (!collision.isTrigger && (collision.name == "Player" || collision.name == "Hearth"))
        {
            attacking--;

        }


    }
}
