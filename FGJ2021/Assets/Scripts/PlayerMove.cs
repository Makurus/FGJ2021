using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D body;
    public float maxSpeed;
    float slowDownTime;
    float speedUpTime;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = Vector2.zero;
        //INPUTS
        if (Input.GetKey(KeyCode.A))
        {
            body.velocity += Vector2.left * maxSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            body.velocity += Vector2.right * maxSpeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            body.velocity += Vector2.up * maxSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            body.velocity += Vector2.down * maxSpeed;
        }
    }
}
