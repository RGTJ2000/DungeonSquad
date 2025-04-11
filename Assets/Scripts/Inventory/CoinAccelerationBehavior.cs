using UnityEngine;

public class CoinAccelerationBehavior : MonoBehaviour
{
    private float coin_acc = 30f;

    private Rigidbody _rb;
    private CoinCollisionBehavior _coinCollisionBehavior;


    private bool isAccelerating = false;
    private Transform targetTransform;


    private void Awake()
    {
        _rb = transform.parent.GetComponent<Rigidbody>();
        _coinCollisionBehavior = transform.parent.GetComponent<CoinCollisionBehavior>();
        
    }

    private void FixedUpdate()
    {
        if (isAccelerating)
        {
            _rb.linearVelocity = (  (targetTransform.position - transform.parent.transform.position).normalized * (_rb.linearVelocity.magnitude + coin_acc*Time.deltaTime) );

            //_rb.AddForce((targetTransform.position - transform.parent.transform.position).normalized * coin_acc, ForceMode.Force);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Character") && !isAccelerating && _coinCollisionBehavior.isGrounded)
        {
            //Debug.Log("Character triggered coin,");

           
           targetTransform = other.transform;

            isAccelerating = true;
            if(_rb != null)
            {
                _rb.useGravity = false;

            }
        }

       
    }


}
