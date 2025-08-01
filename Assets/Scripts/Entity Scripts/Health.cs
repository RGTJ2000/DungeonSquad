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

    private FloatingHealthbarBehavior _healthbarBehavior;
    private EntityStats _entityStats;

    private void Awake()
    {
        currentHealth = maxHealth; // Initialize health to max on start
    }
    private void Start()
    {
        mainCamera = Camera.main;
        _entityStats = GetComponent<EntityStats>();
        maxHealth = _entityStats.health_max;
        currentHealth = maxHealth;

        healthBar_instance = Instantiate(healthBar_prefab, transform.position, Quaternion.identity); //instantiate with no parent
        _healthbarBehavior = healthBar_instance.GetComponent<FloatingHealthbarBehavior>();

        _healthbarBehavior.InitializeHealthBar(gameObject);
    }

    public void Update()
    {
        maxHealth = _entityStats.health_max;
    }

    public void TakeDamage(float damage)
    {

        
        currentHealth -= damage;
    
        currentHealth = Mathf.Max(currentHealth, 0); // Ensure health doesn't go below 0

      

        _entityStats.health_current = currentHealth;

        if (_healthbarBehavior != null)
        {
            _healthbarBehavior.TakeDamage();
        }


        //ShowFloatingText( Mathf.RoundToInt(damage).ToString(), resultType);

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    public void Miss()
    {
        ShowFloatingText("miss", CombatResultType.miss);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure health doesn't exceed max
        
       _entityStats.health_current = currentHealth;
        if (_healthbarBehavior != null)
        {
            _healthbarBehavior.Heal();
        }

        ShowFloatingText("+"+amount, CombatResultType.heal);
    }

    private void Die()
    {
        //Debug.Log($"{gameObject.name} has died!");
        // Handle death logic (e.g., play animation, remove object, etc.)
        DropManager.Instance.DropAllLoot(gameObject);
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

    void ShowFloatingText(string text, CombatResultType resultType)
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

            if (resultType == CombatResultType.critical)
            {
                _textMeshProComponent.color = Color.red;
                _textMeshProComponent.fontMaterial.SetColor(TMPro.ShaderUtilities.ID_OutlineColor, new Color(0, 0, 0, 0));

            }
            else if (resultType == CombatResultType.hit)
            {
                _textMeshProComponent.color = Color.red;
                _textMeshProComponent.fontMaterial.SetColor(TMPro.ShaderUtilities.ID_OutlineColor, new Color(0, 0, 0, 0));

            }
            else if (resultType == CombatResultType.miss)
            {
                _textMeshProComponent.color = Color.white;
                _textMeshProComponent.fontMaterial.SetColor(TMPro.ShaderUtilities.ID_OutlineColor, new Color(0, 0, 0, 0));

            }
            else if (resultType == CombatResultType.heal)
            {

                _textMeshProComponent.color = Color.white;
                _textMeshProComponent.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.red);
                _textMeshProComponent.fontMaterial.SetFloat(TMPro.ShaderUtilities.ID_OutlineWidth, 0.2f);

            }

            /*
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
            */

            _textMeshProComponent.text = text;

            


         
        }
    }

   
}