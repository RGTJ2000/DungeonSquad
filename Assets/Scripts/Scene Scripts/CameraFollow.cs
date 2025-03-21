using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject core;

    [SerializeField] private float x_offset = 0.0f;
    [SerializeField] private float y_offset = 15.0f;
    [SerializeField] private float z_setback = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3 (core.transform.position.x + x_offset, core.transform.position.y + y_offset, core.transform.position.z - z_setback);
        transform.LookAt(core.transform.position);   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(core.transform.position.x + x_offset, core.transform.position.y + y_offset, core.transform.position.z - z_setback);
        transform.LookAt(core.transform.position);
    }
}
