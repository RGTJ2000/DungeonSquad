using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{

    [SerializeField] GameObject follow_obj;
    [SerializeField] float follow_speed;
   // [SerializeField] float movement_slop;

    void Start()
    {
    
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ( ((Input.GetAxis("Horizontal")+Input.GetAxis("Vertical"))  > 0 ) || (follow_obj.transform.position-transform.position).magnitude > .1 ) 
       {

            transform.Translate((follow_obj.transform.position - transform.position).normalized * follow_speed * Time.deltaTime);
       }
    }
}
