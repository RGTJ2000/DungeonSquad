using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SlotProjector : MonoBehaviour
{
    public int slot_count = 4;
    public Vector3[] slot_array;
    private int squad_rotation = 0;

    public float slot_radius = 0.5f;
    public float squad_distance = 3.0f;
    private CharacterController controller;

    private Vector3 tilt_plane_normal = new Vector3(0f,1f,0f);

    //only hit walls
    int layermask = 1 << 6;

    void Start()
    {
        slot_array = new Vector3[slot_count];
        controller = GetComponent<CharacterController>();
    }   

    
    void Update()
    {
        



        for (int i = 0; i < slot_count; i++)
        {
           
            //*** determine the direction outward from transform.position
            Vector3 direction_vector = (Quaternion.AngleAxis((360/slot_count)*i + 180*squad_rotation/slot_count, Vector3.up) * transform.forward);
     
            //*** set the target slot position
            Vector3 target_position = direction_vector * squad_distance;
            //project the target position onto the tilt plane, then rotate possibly shortened vector back to the level plane
            target_position = RotateVectorBetweenPlanes(Vector3.ProjectOnPlane(target_position, tilt_plane_normal), tilt_plane_normal, Vector3.up);

            //DrawCircle(transform.position + target_position, slot_radius, Color.green);


            RaycastHit hitinfo;
            RaycastHit hitinfo2;
            //*** Spherecast to the target_position, if hits occur, then project the drawpoint of the target_position along the wall. 
            if (Physics.SphereCast(transform.position - target_position.normalized*slot_radius, slot_radius, target_position, out hitinfo, target_position.magnitude + slot_radius, layermask))
            {
                Vector3 drawpoint;
                Vector3 slop_vector = transform.position + target_position - hitinfo.point;
                Vector3 tangent_vector = TangentofNormal(hitinfo.normal) * Mathf.Sign(Vector3.Cross(target_position, hitinfo.normal).y);
                

                if(Physics.SphereCast(hitinfo.point + hitinfo.normal*slot_radius, slot_radius, tangent_vector, out hitinfo2, Vector3.Project(slop_vector, tangent_vector).magnitude, layermask))
                {
                    //*** if second hit detected back off from second hit point and set drawpoint there
                    //Debug.DrawLine(hitinfo.point + hitinfo.normal * slot_radius, hitinfo2.point, Color.cyan);
                    drawpoint = hitinfo2.point + hitinfo2.normal * slot_radius;
                    slot_array[i] = drawpoint;

                }
                else
                {
                    //*** if no second wall in the way, then project slop vector down and set drawpoint along wall
                    drawpoint = hitinfo.point + Vector3.Project(slop_vector, tangent_vector) + (hitinfo.normal * slot_radius);
                    slot_array[i] = drawpoint;
                    //Debug.DrawLine(hitinfo.point + (hitinfo.normal * slot_radius), drawpoint, Color.green);

                }

                //Debug.DrawLine(transform.position, hitinfo.point, Color.green);
                //*** Draw slot position at the determined drawpoint
                DrawCircle(drawpoint, slot_radius, Color.blue);
                slot_array[i] = drawpoint;

                //DrawCircle(transform.position + (direction_vector * hitinfo.distance), slot_radius, Color.red);
                //Debug.DrawLine( hitinfo.point, hitinfo.point+TangentofNormal(hitinfo.normal), Color.blue);
            }
            else
            {
                //*** if no spherecast hit, then just draw the calculated target position
                //Debug.DrawLine(transform.position, transform.position + target_position, Color.green);
                DrawCircle(transform.position + (target_position), slot_radius, Color.blue);
                slot_array[i] = transform.position + (target_position);
            }

        }

    }

    void DrawCircle(Vector3 position, float radius, Color color)
    {
        for (int i = 0;i < 10;i++)
        {
            Debug.DrawLine(position + (Quaternion.AngleAxis((360 / 10) * i, Vector3.up) * transform.forward*radius), position + (Quaternion.AngleAxis((360 / 10) * (i + 1), Vector3.up) * transform.forward*radius), color);
        }
    }

    Vector3 TangentofNormal(Vector3 normal)
    {
        Vector3 tangent;
        tangent = Vector3.Cross(normal, Vector3.up).normalized;
        return tangent;
    }

    public void TiltProjectionPlane(Vector3 move_vector, float move_speed, float tilt_speed)
    {
        Vector3 tilttarget_vector;

        if (controller.velocity.magnitude == 0)
        {
            //if the velocity == 0 then align the compression with the movement vector
            tilttarget_vector = new Vector3(move_vector.x, Mathf.Clamp(Mathf.Sqrt  ( 1 - Mathf.Clamp(Mathf.Pow(move_vector.magnitude, 2), 0f, 1.0f)  ), 0.1f, 1.0f ), move_vector.z);
        }
        else
        {
            //if the velocity.magnitude > 0, then the tilttarget is the orthogonal to velocity vector
            tilttarget_vector = TangentofNormal(controller.velocity) * Mathf.Sign(  Vector3.Cross(move_vector, controller.velocity).y  );
            tilttarget_vector = new Vector3(tilttarget_vector.x, Mathf.Clamp(Mathf.Sqrt(1 - Mathf.Clamp(Mathf.Pow(move_vector.magnitude, 2), 0f, 1.0f)), 0.1f, 1.0f), tilttarget_vector.z);
        }


        //rotate the tilt_plane_normal towards the tilttarget_vector
        tilt_plane_normal = Vector3.RotateTowards(tilt_plane_normal, tilttarget_vector, 0.5f * Mathf.PI * tilt_speed/squad_distance * Time.deltaTime, 0.0f);

       
    }
    


    public void Untilt_Projection_Plane(float move_speed)
    {
        if (Vector3.Angle(tilt_plane_normal, Vector3.up) > 0.0f)
        {
            tilt_plane_normal = Vector3.RotateTowards(tilt_plane_normal, Vector3.up, 0.5f * Mathf.PI * move_speed*2 / squad_distance * Time.deltaTime, 0.0f);
        }

    }

    public Vector3 RotateVectorBetweenPlanes (Vector3 originalVector, Vector3 originalPlaneNormal, Vector3 targetPlaneNormal)
    {
        Vector3 rotationAxis = Vector3.Cross(originalPlaneNormal, targetPlaneNormal);
        
        float angle = Vector3.Angle(originalPlaneNormal, targetPlaneNormal);
        Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);
        return rotation * originalVector;
    }

   
}
