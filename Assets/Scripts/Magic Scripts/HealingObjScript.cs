using UnityEngine;

public class HealingObjScript : MonoBehaviour
{
    private GameObject target_obj;
    private float healing_rate;
    private Health _targetHealth;
    private GameObject source_obj;
    private Ch_Behavior _chMoveOfIncanter;

    private bool waitingToDestroy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waitingToDestroy = false;
    }

    // Update is called once per frame
    void Update()
    {
        

        if ((target_obj == null || source_obj == null || _targetHealth.currentHealth >= _targetHealth.maxHealth) && !waitingToDestroy )
        {
            Debug.Log("Destroy from null or health max. target current H"+ _targetHealth.currentHealth + " target Max H"+_targetHealth.maxHealth);
            Destroy(gameObject, 1.0f);
           waitingToDestroy=true;

        }
        if (target_obj != null)
        {
            transform.position = target_obj.transform.position;
            _targetHealth.Heal(healing_rate * Time.deltaTime);

        }
      

    }



    public void SetHealingObjParameters(GameObject target, float rate, GameObject source_entity)
    {
        target_obj = target;
        healing_rate = rate;
        source_obj = source_entity;

        _chMoveOfIncanter = source_obj?.GetComponent<Ch_Behavior>();

        _targetHealth = target_obj.GetComponent<Health>();

        _chMoveOfIncanter.OnIncantFocusChanged += HandleIncantFocusChange;
    }

    private void HandleIncantFocusChange()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        if (_chMoveOfIncanter != null)
        {
            _chMoveOfIncanter.CancelEngage();
            _chMoveOfIncanter.OnIncantFocusChanged -= HandleIncantFocusChange;
        }
    }
}
