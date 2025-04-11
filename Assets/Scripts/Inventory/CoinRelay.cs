using System.Collections;
using TMPro;
using UnityEngine;

public class CoinRelay : MonoBehaviour
{
    private float[] timeLastCoinReceived = new float[4];
    private float collectionWait = 0.4f;
    private GameObject core_obj;

    private Inventory _inventoryForCharacter;
    private Inventory _inventoryForCore;


    public GameObject floatingTextPrefab; // Instantiate in this script
    public Vector3 textOffset = new Vector3(0, 2, 0); // Offset for the floating text position

    private Vector3 sideOffset = Vector3.right * 0.6f;

    private float displayCooldown = 0.3f;
    private bool inCooldown = false;

    void Start()
    {
        _inventoryForCharacter = GetComponent<Inventory>();
        core_obj = GameObject.FindWithTag("Core");
        _inventoryForCore = core_obj.GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        //check inventory coins

        if (_inventoryForCharacter.copper_count > 0 && Time.time - timeLastCoinReceived[(int)CoinType.copper] > collectionWait && !inCooldown)
        {
            //show collection total floating text
            ShowFloatingTotal(CoinType.copper, _inventoryForCharacter.copper_count);
            //send copper to core
            _inventoryForCore.ReceiveCoin(CoinType.copper, _inventoryForCharacter.copper_count);
            _inventoryForCharacter.copper_count = 0;
            StartCoroutine(TextCooldown());
        }

        if (_inventoryForCharacter.silver_count > 0 && Time.time - timeLastCoinReceived[(int)CoinType.silver] > collectionWait && !inCooldown)
        {
            //show collection total floating text
            ShowFloatingTotal(CoinType.silver, _inventoryForCharacter.silver_count);
            //send silver to core
            _inventoryForCore.ReceiveCoin(CoinType.silver, _inventoryForCharacter.silver_count);
            _inventoryForCharacter.silver_count = 0;
            StartCoroutine(TextCooldown());

        }


        if (_inventoryForCharacter.gold_count > 0 && Time.time - timeLastCoinReceived[(int)CoinType.gold] > collectionWait && !inCooldown)
        {
            //show collection total floating text
            ShowFloatingTotal(CoinType.gold, _inventoryForCharacter.gold_count);
            //send gold to core
            _inventoryForCore.ReceiveCoin(CoinType.gold, _inventoryForCharacter.gold_count);
            _inventoryForCharacter.gold_count = 0;
            StartCoroutine(TextCooldown());

        }

        if (_inventoryForCharacter.platinum_count > 0 && Time.time - timeLastCoinReceived[(int)CoinType.platinum] > collectionWait && !inCooldown)
        {
            //show collection total floating text
            ShowFloatingTotal(CoinType.platinum, _inventoryForCharacter.platinum_count);
            //send platinum to core
            _inventoryForCore.ReceiveCoin(CoinType.platinum, _inventoryForCharacter.platinum_count);
            _inventoryForCharacter.platinum_count = 0;
            StartCoroutine(TextCooldown());

        }






    }

    public void CoinReceived(CoinType coinType)
    {
        timeLastCoinReceived[(int)coinType] = Time.time;
    }

    void ShowFloatingTotal(CoinType coinType, int amount)
    {
        if (floatingTextPrefab != null)
        {
            // Instantiate the floating text at the entity's position with an offset

            //Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);

            sideOffset *= -1;

            GameObject floatingText = Instantiate(floatingTextPrefab, transform.position + textOffset + sideOffset, Quaternion.identity);
            
            FloatingTextBehavior _floatBehavior = floatingText.GetComponent<FloatingTextBehavior>();
            _floatBehavior.parentTransform = transform;
            _floatBehavior.duration = 1.5f;

            TextMeshPro _textMeshProComponent = floatingText.GetComponent<TextMeshPro>();

            switch (coinType)
            {
                case CoinType.copper:
                    _textMeshProComponent.color = new Color(0.7843f, 0.3725f, 0, 1);
                    _textMeshProComponent.text = $"+{amount}c";
                    break;

                case CoinType.silver:
                    _textMeshProComponent.color = new Color(0.9019f, 0.9019f, 0.9019f, 1);
                    _textMeshProComponent.text = $"+{amount}s";
                    break;
                case CoinType.gold:
                    _textMeshProComponent.color = new Color(1, 0.8537f, 0, 1);
                    _textMeshProComponent.text = $"+{amount}g"; 
                    break;
                case CoinType.platinum:
                    _textMeshProComponent.color = new Color(0.72549f, 1f, 0.9801f, 1);
                    _textMeshProComponent.text = $"+{amount}p";
                    break;
                default:
                    break;


            }

           





        }
    }

    IEnumerator TextCooldown()
    {
        inCooldown = true;
        yield return new WaitForSeconds(displayCooldown);
        inCooldown = false;
    }
}
