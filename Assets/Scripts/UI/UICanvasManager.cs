using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class UICanvasManager : MonoBehaviour
{


    private GameObject[] slots_array;
    private GameObject current_character;
    [SerializeField] private Transform grid;
    [SerializeField] private Transform meleePanel;
    [SerializeField] private Transform rangedPanel;
    [SerializeField] private Transform missilePanel;
    [SerializeField] private GameObject itemEntry_button;

    private Inventory _coreInventory;

    //[SerializeField] private GameObject skillPanel;      // parent of your grid
    [SerializeField] private GameObject inventoryPanel;  // your ScrollView parent

    private bool inventoryActive = false;

    private void OnEnable()
    {
        SquadManager.OnCharacterSelected += HandleSkillsUI;
        SquadManager.OnInventorySelected += HandleInventoryUI;

    }

    private void OnDisable()
    {
        SquadManager.OnCharacterSelected -= HandleSkillsUI;
        SquadManager.OnInventorySelected -= HandleInventoryUI;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //populate slot_array with slot gameobjects
        slots_array = new GameObject[grid.childCount];
        for (int i = 0; i < slots_array.Length; i++)
        {
            slots_array[i] = grid.GetChild(i).gameObject;
            slots_array[i].SetActive(false); //deactivate the slots by default

        }

        //assign coreInventory
        GameObject core = GameObject.FindWithTag("Core");
        _coreInventory = core.GetComponent<Inventory>();

        //set Inventory Panel state
        inventoryPanel.SetActive(false);

    }
    private void Update()
    {
        if (current_character != null)
        {
            // Update overlays based on SkillCooldownTracker
            EntityStats _entityStats = current_character.GetComponent<EntityStats>();
            if (_entityStats != null)
            {
                SkillCooldownTracker cooldownTracker = _entityStats.GetComponent<SkillCooldownTracker>();
                if (cooldownTracker != null)
                {
                    for (int i = 0; i < slots_array.Length; i++)
                    {
                        if (i < _entityStats.skill_slot.Length && _entityStats.skill_slot[i] != null)
                        {
                            Skill_SO skill = _entityStats.skill_slot[i];
                            if (skill.cooldown != 0f)
                            {
                                float remainingCooldown = cooldownTracker.GetRemainingCooldown(skill);
                                UpdateOverlay(slots_array[i], remainingCooldown, skill.cooldown);
                            }

                        }
                    }
                }
            }
        }
    }

    private void UpdateOverlay(GameObject slot, float remainingCooldown, float cooldownDuration)
    {
        Image overlayImage = slot.transform.Find("OverlayImage")?.GetComponent<Image>();
        if (overlayImage != null)
        {
            overlayImage.fillAmount = Mathf.Clamp01(remainingCooldown / cooldownDuration);
        }
    }

    public GameObject GetSlot(int index)
    {
        if (index >= 0 && index < slots_array.Length)
        {
            return slots_array[index];
        }
        else
        {
            Debug.LogWarning("Slot index out of range: " + index);
            return null;
        }
    }




    private void HandleInventoryUI(GameObject ch_obj)
    {
        current_character = ch_obj;

        if (!inventoryActive)
        {
            ClearInventoryUI();
            PopulateInventoryPanel();
            ActivateInventoryUI(ch_obj);
            inventoryActive = true;
        }
        else
        {
            DeactivateInventoryUI();
            inventoryActive = false;
        }

    }
    private void HandleSkillsUI(GameObject ch_obj)
    {
        current_character = ch_obj;

        if (ch_obj != null)
        {

            ActivateSkillsUI(ch_obj);
        }
        else
        {
            DeactivateSkillsUI();
        }
    }

    private void ActivateSkillsUI(GameObject ch_obj)
    {
        grid.gameObject.SetActive(true); //activate grid



        //Activate the slots according to ch_obj's entityStats skill_slot array
        EntityStats _entityStats = ch_obj.GetComponent<EntityStats>();

        SkillCooldownTracker _cooldownTracker = _entityStats.GetComponent<SkillCooldownTracker>();

        int ch_slottedSkillsLength = _entityStats.skill_slot.Length;

        for (int i = 0; i < slots_array.Length; i++)
        {

            Image _image = slots_array[i].GetComponent<Image>();


            Transform slot_text = slots_array[i].transform.Find("SkillText");
            TextMeshProUGUI _textComponent = slot_text.gameObject.GetComponent<TextMeshProUGUI>();

            Image overlayImage = slots_array[i].transform.Find("OverlayImage")?.GetComponent<Image>();


            if (i < ch_slottedSkillsLength && _entityStats.skill_slot[i] != null)
            {
                slots_array[i].SetActive(true);
                _image.sprite = _entityStats.skill_slot[i].skill_icon;

                if (_entityStats.selected_skill.skill_name == _entityStats.skill_slot[i].skill_name)

                {
                    //activate slot text
                    slot_text.gameObject.SetActive(true);

                    //set text to the skillname in the skill slot
                    _textComponent.text = _entityStats.skill_slot[i].skill_name;

                    slots_array[i].transform.localScale = new Vector3(1.2f, 1.2f, 0f);
                    //overlayImage.transform.localScale = new Vector3(1.2f, 1.2f, 0f);

                    Color color = _image.color;
                    color.a = 1.0f;
                    _image.color = color;
                }
                else
                {
                    slot_text.gameObject.SetActive(false);

                    slots_array[i].transform.localScale = new Vector3(1.0f, 1.0f, 0f);
                    //overlayImage.transform.localScale = new Vector3(1.0f, 1.0f, 0f);

                    Color color = _image.color;
                    color.a = 0.8f;
                    _image.color = color;
                }

                //set the cooldown overlay
                if (_cooldownTracker == null)
                {
                    overlayImage.fillAmount = 0f;

                }
                else
                {
                    Skill_SO skill = _entityStats.skill_slot[i];
                    float remainingCooldown = _cooldownTracker.GetRemainingCooldown(skill);
                    //Debug.Log($"Slot {i}: remaining cooldown={remainingCooldown} | cooldown total: {skill.cooldown}");
                    if (skill.cooldown == 0.0f)
                    {
                        //Debug.Log($"{ch_obj.name}: Setting slot {i} to overlayImage.fillAmount =0f");
                        overlayImage.fillAmount = 0.0f;
                        //Debug.Log($"Post-set: fillAmount = {overlayImage.fillAmount}");
                    }
                    else
                    {
                        //Debug.Log($"{ch_obj.name}: Setting slot {i} to fraction {remainingCooldown}/{skill.cooldown}");
                        overlayImage.fillAmount = Mathf.Clamp01(remainingCooldown / skill.cooldown);
                    }

                }
            }
            else
            {
                slots_array[i].SetActive(false); //deactivate the image slot
            }







        }
    }


    private void PopulateInventoryPanel()
    {
        //ClearInventoryPanel();
        foreach (var meleeItem in _coreInventory.meleeWeapons)
        {
            AddItemToCategoryPanel(meleeItem, meleePanel); // prefab instantiation + text set
        }

        foreach (var rangedItem in _coreInventory.rangedWeapons)
        {
            AddItemToCategoryPanel(rangedItem, rangedPanel);
        }

        foreach (var missiles in _coreInventory.missiles)
        {
            AddItemToCategoryPanel(missiles, missilePanel);
        }

    }

    public void ClearInventoryUI()
    {
        ClearPanel(meleePanel);
        ClearPanel(rangedPanel);
        ClearPanel(missilePanel);
    }

    void ClearPanel(Transform panel)
    {
        // Skip the first child (the heading)
        for (int i = panel.childCount - 1; i > 0; i--)
        {
            if (panel.GetChild(i).name != "Header")
            {
                Destroy(panel.GetChild(i).gameObject);
            }
        }
    }

    private void AddItemToCategoryPanel(RuntimeItem item, Transform categoryPanel)
    {
        GameObject newObj = Instantiate(itemEntry_button, categoryPanel);
        Button newButton = newObj.GetComponent<Button>();


        // Update the button's label text
        TextMeshProUGUI label = newButton.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = item.item_name;
        }

        /*
        // Optional: Select this button as the current selected UI object
        if (shouldSelectFirstItem)
        {
            EventSystem.current.SetSelectedGameObject(newButton.gameObject);
        }
        */
    }


    private void DeactivateSkillsUI()
    {
        grid.gameObject.SetActive(false);
    }

    private void ActivateInventoryUI(GameObject ch_obj)
    {
        inventoryPanel.SetActive(true);
    }

    private void DeactivateInventoryUI()
    {
        inventoryPanel.SetActive(false);
    }

}
