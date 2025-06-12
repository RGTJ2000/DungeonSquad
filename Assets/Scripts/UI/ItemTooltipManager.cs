using TMPro;
using UnityEngine;

public class ItemTooltipManager : ManagerBase<ItemTooltipManager> 
{
    [SerializeField] private GameObject itemTooltipPrefab;
    private GameObject currentTooltipInstance;

    private TextMeshProUGUI tooltipText;
   

    [SerializeField] private Vector3 displayOffset = new Vector3(0, 1f, 0);

    private GameObject activeTooltipObject = null;

    private void Start()
    {
        // Instantiate if not already created
        if (currentTooltipInstance == null)
        {
            Debug.Log("Instantiating tooltip.");
            currentTooltipInstance = Instantiate(itemTooltipPrefab);
            currentTooltipInstance.transform.SetParent(transform); // Optional: parent under manager
            tooltipText = currentTooltipInstance.GetComponentInChildren<TextMeshProUGUI>();
            currentTooltipInstance.SetActive(false);
        }


    }
    public void ShowTooltip(GameObject item_obj)
    {
       

        if (item_obj != activeTooltipObject)
        {
            activeTooltipObject = item_obj;
            Debug.Log("ShowTooltip called. Active Tooltip obj="+activeTooltipObject.name);

           

            

            // Update position and text
            currentTooltipInstance.transform.position = activeTooltipObject.transform.position + displayOffset;

            //Get Item info
            DroppedItemBehavior droppedItemBehavior = activeTooltipObject.GetComponent<DroppedItemBehavior>();
            RuntimeItem runtimeItem = droppedItemBehavior.GetRuntimeItem();

            tooltipText.text = runtimeItem.item_name;
            currentTooltipInstance.SetActive(true);


        }


       
    }

    public void HideTooltip()
    {
        if (currentTooltipInstance != null)
        {
            currentTooltipInstance.SetActive(false);
            activeTooltipObject = null;
        }
    }

}
