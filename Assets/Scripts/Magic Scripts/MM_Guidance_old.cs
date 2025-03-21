using UnityEngine;
using System.Collections;

public class MM_Guidance : MonoBehaviour
{
    private GameObject missile_target;

    private float max_wander = 5f;
    private float min_wander = 5f;
    private float max_veerAngle = 90f;
    private int max_steps = 5;

    private int step_count = 0;

    private Vector3 original_heading;
    private Vector3 current_heading;
    private float current_stepDistance;
    private Vector3 start_point;

    private float mm_speed = 40f;

    private Rigidbody _rb;
    //private ParticleSystem _particleSystem;

    private bool paused = false;
    private float missile_pause = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        //_particleSystem = GetComponent<ParticleSystem>();

        original_heading = (missile_target.transform.position - transform.position).normalized;
        start_point = transform.position;
        current_heading = original_heading;
        current_stepDistance = GetRandomStep();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + current_heading, Vector3.up);
        //Debug.DrawLine(transform.position, transform.position + current_heading * 2, Color.blue);
        //Debug.DrawLine(transform.position, (transform.forward * 3) + transform.position, Color.red);

        if (!paused && missile_target != null)
        {
            if (step_count >= max_steps)
            {
                current_heading = (missile_target.transform.position - transform.position).normalized;
            }
            else if ((transform.position - start_point).magnitude >= current_stepDistance)
            {
                current_heading = RandomVeer((missile_target.transform.position - transform.position).normalized);
                current_stepDistance = GetRandomStep();
                start_point = transform.position;
                step_count++;
                paused = true;
                StartCoroutine(PauseForStep());
            }
        }
     
    }

    private void FixedUpdate()
    {
        if (!paused)
        {
            _rb.linearVelocity = current_heading * mm_speed;

        }
        else
        {
            _rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == missile_target)
        {
            //_particleSystem.Stop();

            StartCoroutine(DestroyAfterDelay());
        } else
        {
            current_heading = RandomVeer(current_heading);
        }
    }

    private Vector3 RandomVeer(Vector3 heading)
    {
        Vector3 veer_vector = Quaternion.AngleAxis(max_veerAngle * Random.Range(-1f,1f), Vector3.up) * heading;
        return veer_vector.normalized;
    }

    private float GetRandomStep()
    {
        float step = Random.Range(min_wander, max_wander);
        return step;
    }

    private float GetNewStep()
    {
        float step = (missile_target.transform.position - transform.position).magnitude / 2;

        return step;
    }
    public void SetTarget(GameObject target)
    {
        missile_target = target;
    }

    IEnumerator PauseForStep()
    {
        yield return new WaitForSeconds(missile_pause/ (step_count*step_count));
        paused = false;

    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
