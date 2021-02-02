using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearthMove : MonoBehaviour
{
    public float enemysSee;
    public bool callHearth;
    public float maxSpeed = 3;
    public float rayCastRadius;

    public float speedUpSpeed = 1;
    Rigidbody2D body;
    Vector2 direction;
    public bool closeToPlayer;
    public bool stop;
    public float origScale;
    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        direction = Random.insideUnitCircle;
        origScale = transform.localScale.x;
        player = GameObject.Find("Player").transform;
    }


    // Update is called once per frame
    void Update()
    {
        if (!stop)
        {
            body.velocity += direction * Time.deltaTime * speedUpSpeed;
            body.velocity = body.velocity.normalized * Mathf.Min(maxSpeed, body.velocity.magnitude);

            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, rayCastRadius, direction.normalized, 0);
            if (hits.Length > 1)
            {
                foreach(RaycastHit2D hit in hits)
                {
                    if (hit.transform.name != "Player" && hit.transform.name != "Hearth" && hit.transform.name != "HearthRadar")
                    {
                        direction = hit.normal;
                     
                        break;
                    }
                 
                }
             
            }

            if (callHearth)
            {
                var v = (player.position - transform.position).normalized * 2;
                direction = (direction + new Vector2(v.x, v.y)).normalized;

            }
        }
       


        //Physics2D.CircleCast(transform.position,rayCastRadius)
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.name == "Player" && !collision.isTrigger)
        {
            closeToPlayer = true;
            transform.GetChild(0).gameObject.SetActive(true);

            GameObject go = transform.GetChild(0).gameObject;

           go.GetComponent<Animator>().SetBool("transform",true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.name == "Player" && !collision.isTrigger)
        {
            closeToPlayer = false;
            transform.GetChild(0).gameObject.SetActive(false);

        }
    }

    //private void OnDrawGizmos()
    //{
    //    //Gizmos.(transform.position, enemysSee);
    //    UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, enemysSee);
    //}
}
