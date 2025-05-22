using System.Collections;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    private string damageType;

    public GameObject healthBar_prefab; 
    public GameObject floatingTextPrefab; // Instantiate in this script
    private GameObject healthBar_instance;

    public Vector3 textOffset = new Vector3(0, 2, 0); // Offset for the floating text position

    public Camera mainCamera;

    private FloatingHealthbar _healthbar;
    private EntityStats _entityStats;

    private void Awake()
    {
        currentHealth = maxHealth; // Initialize health to max on start
    }
    private void Start()
    {
        mainCamera = Camera.main;

        healthBar_instance = Instantiate(healthBar_prefab, transform.position, Quaternion.identity); //instantiate with no parent
        healthBar_instance.GetComponent<HealthBarCanvasPosition>().SetEntityToFollow(gameObject);

        _healthbar = healthBar_instance.GetComponentInChildren<FloatingHealthbar>();

        _entityStats = GetComponent<EntityStats>();

        maxHealth = _entityStats.health_max;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, string type)
    {

        
        currentHealth -= damage;
    
        currentHealth = Mathf.Max(currentHealth, 0); // Ensure health doesn't go below 0

       if (_healthbar != null)
        {
            _healthbar.UpdateHealthbar(currentHealth, maxHealth);
        }

        _entityStats.health_current = currentHealth;


        ShowFloatingText( Mathf.RoundToInt(damage).ToString(), type);

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    public void Miss()
    {
        ShowFloatingText("miss", "physical");
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure health doesn't exceed max
        if (_healthbar != null)
        {
            _healthbar.UpdateHealthbar(currentHealth, maxHealth);
        }
       // Debug.Log($"{gameObject.name} healed for {amount}! Current health: {currentHealth}");
       _entityStats.health_current = currentHealth;
    }

    private void Die()
    {
        //Debug.Log($"{gameObject.name} has died!");
        // Handle death logic (e.g., play animation, remove object, etc.)
        DropManager.Instance.DropLoot(gameObject);
        Destroy(healthBar_instance);
        Destroy(gameObject, 0.1f);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    void ShowFloatingText(string text, string type)
    {
        if (floatingTextPrefab != null)
        {
            // Instantiate the floating text at the entity's position with an offset

            Vector3 randomOffset = new Vector3(Random.Range(-0.8f, 0.8f), 0, 0);

            GameObject floatingText = Instantiate(floatingTextPrefab, transform.position + textOffset + randomOffset, Quaternion.identity);

            FloatingTextBehavior _floatBehavior = floatingText.GetComponent<FloatingTextBehavior>();
            _floatBehavior.parentTransform = transform;
            //mainCamera.transform.rotation

            TextMeshPro _textMeshProComponent = floatingText.GetComponent<TextMeshPro>();

            if (type == "crit")
            {
                _textMeshProComponent.color = Color.red;
            }
            else if (type == "fire")
            {
                _textMeshProComponent.color = Color.red;
            }
            else if (type == "frost")
            {
                _textMeshProComponent.color = Color.blue;
            }
            else if (type == "physical")

            {
                _textMeshProComponent.color = Color.white;
            }
            else
            {
                _textMeshProComponent.color= Color.white;
            }

            _textMeshProComponent.text = text;

            


         
        }
    }

   
}