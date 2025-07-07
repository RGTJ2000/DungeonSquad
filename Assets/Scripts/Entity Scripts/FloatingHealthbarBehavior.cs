using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthbarBehavior : MonoBehaviour
{
    private Camera _mainCamera;
    private Vector3 offset_vector;
    private GameObject entity_to_follow;

    [SerializeField] private Slider _frontSlider;
    [SerializeField] private Image _frontImage;
    [SerializeField] private Slider _backSlider;
    [SerializeField] private Image _backImage;

    private Color safetyYellow = new Color(1f, 0.784f, 0f, 1f);

    private float chipSpeed = 2f;
    private float lerpTimer = 0f;
    private float barHealth = 0f;
    private float currentHealth = 0f;

    private EntityStats _entityStats;

    void Start()
    {
        _mainCamera = Camera.main;
        offset_vector = new Vector3(0f, 1.25f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        transform.rotation = _mainCamera.transform.rotation;
        //transform.rotation = Quaternion.Euler(_mainCamera.transform.rotation.x, _mainCamera.transform.rotation.y -transform.parent.rotation.y, _mainCamera.transform.rotation.z);
        // transform.rotation = _mainCamera.transform.rotation;
        if (entity_to_follow != null)
        {
            transform.position = entity_to_follow.transform.position + offset_vector;

        }
        UpdateHealthbar();


    }

    public void UpdateHealthbar()
    {

       

      if (_backSlider != null && _frontSlider != null)
        {
            currentHealth = Mathf.Clamp01(_entityStats.health_current / _entityStats.health_max);

            if (barHealth != currentHealth)
            {
                if (currentHealth < barHealth)
                {
                    //damage occured
                    _frontSlider.value = currentHealth;
                    _backSlider.value = barHealth;
                    _backImage.color = safetyYellow;
                }
                else if (currentHealth > barHealth)
                {
                    //healing occured
                    _frontSlider.value = barHealth;
                    _backSlider.value = currentHealth;
                    _backImage.color = Color.green;
                }

                barHealth = currentHealth;
                lerpTimer = 0f;

            }


            if (_backSlider.value > barHealth)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;
                _backSlider.value = Mathf.Lerp(_backSlider.value, barHealth, percentComplete);

                if (Mathf.Abs(_backSlider.value - barHealth) < 0.01f)
                {
                    _backSlider.value = barHealth; // Snap!
                }
            }

            if (_frontSlider.value < barHealth)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;
                _frontSlider.value = Mathf.Lerp(_frontSlider.value, barHealth, percentComplete);

                if (Mathf.Abs(_frontSlider.value - barHealth) < 0.01f)
                {
                    _frontSlider.value = barHealth; // Snap!
                }
            }

            /*
            float currentValue = Mathf.Clamp01(_entityStats.health_current / _entityStats.health_max);
            if (_backSlider.value > currentValue)
            {
                _backImage.color = safetyYellow;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;
                _backSlider.value = Mathf.Lerp(_backSlider.value, _frontSlider.value, percentComplete);

            }
            */
            
        }

        
        
    }


    public void TakeDamage()
    {
        _frontSlider.value = Mathf.Clamp01(_entityStats.health_current / _entityStats.health_max);
        lerpTimer = 0f;
    }

    public void Heal()
    {
        _frontSlider.value = Mathf.Clamp01(_entityStats.health_current / _entityStats.health_max);
        lerpTimer = 0f;
    }
  
  

    public void InitializeHealthBar(GameObject entity)
    {
        entity_to_follow = entity;
        _entityStats = entity.GetComponent<EntityStats>();

        currentHealth = Mathf.Clamp01(_entityStats.health_current / _entityStats.health_max);
        barHealth = currentHealth;

        _frontSlider.value = barHealth;
        _backSlider.value = barHealth;

       

       // Debug.Log($"Object = {entity.name} || front slider value = {_frontSlider.value} back slider value = {_backSlider.value}");

    }
}
