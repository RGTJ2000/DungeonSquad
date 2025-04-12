using TMPro;
using UnityEngine;

public class CoinCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _copperTMP;
    [SerializeField] private TextMeshProUGUI _silverTMP;
    [SerializeField] private TextMeshProUGUI _goldTMP;
    [SerializeField] private TextMeshProUGUI _platinumTMP;

    private GameObject core_obj;
    private Inventory _coreInventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        core_obj = GameObject.FindGameObjectWithTag("Core");
        if (core_obj != null)
        {
            _coreInventory = core_obj.GetComponent<Inventory>();
        }

        _copperTMP.text = _coreInventory.copper_count.ToString();
        _silverTMP.text = _coreInventory.silver_count.ToString();
        _goldTMP.text = _coreInventory.gold_count.ToString();
        _platinumTMP.text = _coreInventory.platinum_count.ToString();


    }

    // Update is called once per frame
    void Update()
    {
        _copperTMP.text = _coreInventory.copper_count.ToString();
        _silverTMP.text = _coreInventory.silver_count.ToString();
        _goldTMP.text = _coreInventory.gold_count.ToString();
        _platinumTMP.text = _coreInventory.platinum_count.ToString();


    }
}
