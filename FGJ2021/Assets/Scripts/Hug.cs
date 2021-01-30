using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hug : MonoBehaviour
{
    public Health enemyToHug;
    // Start is called before the first frame update
    void Awake()
    {
        enemyToHug = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "enemy")
        { var h = collision.GetComponent<Health>();
            if (h.canHug)
                enemyToHug = h;
        }
    }
}
