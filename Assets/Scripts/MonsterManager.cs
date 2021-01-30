using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public int enemies;
    public GameObject door;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    bool doorDestroyed;
    // Update is called once per frame
    void Update()
    {
        if(enemies == 0 && !doorDestroyed)
        {
            doorDestroyed = true;
            Destroy(door);
        }
    }
}
