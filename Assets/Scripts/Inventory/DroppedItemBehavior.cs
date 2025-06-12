using Unity.VisualScripting;
using UnityEngine;

public class DroppedItemBehavior : MonoBehaviour
{
    public RuntimeItem _runtimeItem;

    public GameObject targetEntity;
    private Vector3 startPosition;
    private GameObject core_obj;
    private Inventory _coreInventory;
    private bool pickup_active;
    private EntityStats _entityStats;
    private float entityRadius;
    private float entityReach;

    private Rigidbody _rb;
    private Collider _collider;

    private float item_acc = 40f;

    private float cancelDistanceFactor = 2.0f;

    private void Awake()
    {
        core_obj = GameObject.FindGameObjectWithTag("Core");
        _coreInventory = core_obj.GetComponent<Inventory>();
        pickup_active = false;
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

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
                if (Vector3.Distance(startPosition, transform.position) > cancelDistanceFactor * entityReach)
                {
                    CancelPickup();
                }
                else if (Vector3.Distance(transform.position, targetEntity.transform.position) < entityRadius)
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
        _coreInventory.AddItem(_runtimeItem);
        ItemEvents.RaiseItemPickedUp();
        Destroy(gameObject);

    }

    private void AccelerateToTarget()
    {
        if (_collider.enabled)
        {
            _collider.enabled = false;
        }
        _rb.linearVelocity = (targetEntity.transform.position - transform.position).normalized * (_rb.linearVelocity.magnitude + item_acc * Time.deltaTime);

    }

    private void CancelPickup()
    {
        pickup_active = false;
        _collider.enabled = true;
    }

    public void ActivatePickup(GameObject target)
    {
        targetEntity = target;
        _entityStats = targetEntity.GetComponent<EntityStats>();
        entityRadius = _entityStats.entity_radius;
        entityReach = _entityStats.entity_reach;
        startPosition = transform.position;
        pickup_active = true;

    }

    public void SetRuntimeItem(RuntimeItem item)
    {
        _runtimeItem = item;
    }

    public RuntimeItem GetRuntimeItem()
    {
        return _runtimeItem;
    }
}
