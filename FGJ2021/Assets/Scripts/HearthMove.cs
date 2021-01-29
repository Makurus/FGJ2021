using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearthMove : MonoBehaviour
{

    public float maxSpeed = 3;
    public float rayCastRadius;
    //public float rayCastDistance;

    float speed;
    public float speedUpSpeed = 1;
    Rigidbody2D body;
    Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        direction = Random.insideUnitCircle;
    }

    // Update is called once per frame
    void Update()
    {

        body.velocity += direction * Time.deltaTime * speedUpSpeed;
        body.velocity = body.velocity.normalized * Mathf.Min(maxSpeed, body.velocity.magnitude);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, rayCastRadius, direction.normalized, 0);
        if(hits.Length > 1)
        {
            direction = hits[1].normal;
            if (hits[1].transform.name == "Player" && hits.Length > 2)
                direction = hits[2].normal;
            print("HOI");
        }


        //Physics2D.CircleCast(transform.position,rayCastRadius)
    }
}
