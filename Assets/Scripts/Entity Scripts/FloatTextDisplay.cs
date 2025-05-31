using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using static UnityEngine.LightTransport.IProbeIntegrator;

public class FloatTextDisplay : MonoBehaviour
{
    [SerializeField] private GameObject floatingTextPrefab;
    Camera mainCamera;
    private Vector3 textOffset;
    private Vector3 defaultTextOffset = new Vector3(0, 2, 0);
    private float offset_randomXrange;
    private Vector3 sideFloatVector = new Vector3(0.5f, -1f , 0);
    private Vector3 defaultFloatVector = Vector3.up;
    private float defaultFontSize = 10f;
    private float smallTextSize = 5f;

    void Start()
    {
        mainCamera = Camera.main;
        textOffset = defaultTextOffset; // Offset for the floating text position
        offset_randomXrange = 0.8f;

    }

    public void ShowFloatDamage(float damageAmount, CombatResultType hitType, DamageType damageType)
    {
        if (floatingTextPrefab != null)
        {
            
            GameObject newFloatingText = CreateFloatTextInstance();
            TextMeshPro _tmp = newFloatingText.GetComponent<TextMeshPro>();
            FloatingTextBehavior _floatTextBehavior = newFloatingText.GetComponent<FloatingTextBehavior>();

            //use switch of damageType to set float text behavior for status effect text (sideways vector?)
            switch (damageType)
            {
                case DamageType.physical:
                    if (transform.CompareTag("Character"))
                    {
                        _tmp.color = Color.red;
                        
                    }
                    else
                    {
                        _tmp.color = GameColors.AmberCRTSolid;
                    }

                    _floatTextBehavior.SetFloatVector(defaultFloatVector);
                    break;
                case DamageType.confusion:
                    _tmp.color = Color.gray;
                    _tmp.fontSize = smallTextSize;
                    _floatTextBehavior.SetFloatVector(sideFloatVector);
                    break;
                case DamageType.fear:
                    _tmp.color = Color.yellow;
                    _tmp.fontSize = smallTextSize;
                    _floatTextBehavior.SetFloatVector(sideFloatVector);
                    break;
                case DamageType.fire:
                    if (transform.CompareTag("Character"))
                    {
                        _tmp.color = Color.red;

                    }
                    else
                    {
                        _tmp.color = GameColors.AmberCRTSolid;
                    }
                    _tmp.fontSize = defaultFontSize;
                    _floatTextBehavior.SetFloatVector(defaultFloatVector);
                    break;
                case DamageType.frost:
                    _tmp.color = Color.blue;
                    _tmp.fontSize = smallTextSize;
                    _floatTextBehavior.SetFloatVector(sideFloatVector);
                    break;
                case DamageType.poison:
                    _tmp.color = Color.green;
                    _tmp.fontSize = smallTextSize;
                    _floatTextBehavior.SetFloatVector(sideFloatVector);
                    break;
                case DamageType.sleep:
                    _tmp.color = Color.white;
                    _tmp.fontSize = smallTextSize;
                    _floatTextBehavior.SetFloatVector(sideFloatVector);
                    break;
                default:
                    _tmp.color = Color.black;
                    Debug.Log("DamageType for float text not recognized.");
                    break;
            }

            if (hitType == CombatResultType.critical)
            {
                //if critical, set text material to outline with red
                _tmp.fontMaterial = Instantiate(_tmp.fontMaterial);
                _tmp.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.3f);

                _tmp.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);

                
            }

            _tmp.text = $"{(int)damageAmount}";


        }

    }

    private GameObject CreateFloatTextInstance()
    {
        // Instantiate the floating text at the entity's position with an offset

        Vector3 randomOffset = new Vector3(Random.Range(-offset_randomXrange, offset_randomXrange), 0, 0);

        GameObject newFloatingText = Instantiate(floatingTextPrefab, transform.position + textOffset + randomOffset, Quaternion.identity);

        FloatingTextBehavior _floatBehavior = newFloatingText.GetComponent<FloatingTextBehavior>();

        _floatBehavior.parentTransform = transform;

        return newFloatingText;


    }

    public void ShowFloatMiss(CombatResultType missType)
    {
        if (floatingTextPrefab != null)
        {
            GameObject newFloatingText = CreateFloatTextInstance();
            TextMeshPro _tmp = newFloatingText.GetComponent<TextMeshPro>();

            _tmp.color = Color.white;
            _tmp.fontSize = 5f;

            switch (missType)
            {
                case CombatResultType.miss:
                    _tmp.text = "miss";
                    
                    break;
                case CombatResultType.block:
                    _tmp.text = "block";
                    break;
                case CombatResultType.dodge:
                    _tmp.text = "dodge";
                    break;
                case CombatResultType.parry:
                    _tmp.text = "parry";
                    break;
                case CombatResultType.resist:
                    _tmp.text = "resist";
                    break;
                default:
                    _tmp.text = "???CombatResultType";
                    break;
            }


        }



    }
}
