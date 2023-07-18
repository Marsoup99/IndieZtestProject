using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateMovement : MonoBehaviour
{
    [SerializeField] GateSpawner gateSpawner;
    [SerializeField] float speed = 5;
    void Start()
    {
        gateSpawner = GetComponentInParent<GateSpawner>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!gateSpawner.isStop)
            transform.Translate(Vector2.left * speed * Time.fixedDeltaTime);
        
        if(transform.position.x < -10)
        {
            gateSpawner.DeSpawn(this.gameObject);
        }
    }

}
