using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using UnityEngine;

public class ItemPickupBehavior : MonoBehaviour
{

    public GameObject targetEntity;
    private Vector3 startPosition;
    private GameObject core_obj;
    private Inventory _coreInventory;
    private bool pickup_active;
    private EntityStats _entityStats;
    private float entityRadius;

    private Rigidbody _rb;

    private float item_acc = 30f;

    private void Awake()
    {
        core_obj = GameObject.FindGameObjectWithTag("Core");
        _coreInventory = core_obj.GetComponent<Inventory>();
        pickup_active = false;
        _rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {
        if (pickup_active)
        {
            if (targetEntity != null)
            {
                if (Vector3.Distance(transform.position, targetEntity.transform.position) < entityRadius)
                {
                    TransferToInventory();
                }
                else
                {
                    AccelerateToTarget();
                }


            }
            else
            {
                CancelPickup();
            }
            

        }
        
    }
    private void TransferToInventory()
    {
        

    }

    private void AccelerateToTarget()
    {
        _rb.linearVelocity = (targetEntity.transform.position - transform.position).normalized * (_rb.linearVelocity.magnitude + item_acc * Time.deltaTime);

    }
    private void CancelPickup()
    {
        pickup_active = false;
    }

    public void ActivatePickup(GameObject target)
    {
        targetEntity = target;
        _entityStats = targetEntity.GetComponent<EntityStats>();
        entityRadius = _entityStats.entity_radius;
        startPosition = transform.position;
        pickup_active = true;

    }


}
