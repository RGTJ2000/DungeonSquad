using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Progress;

public enum SelectDirection
{
    up,
    down,
    left,
    right
}

public class UICanvasManager : ManagerBase<UICanvasManager>
{

    private SquadManager _squadManager;
    private GameObject[] slots_array;
    private GameObject current_character;
    [SerializeField] private Transform grid;
    [SerializeField] private Transform ringPanel;
    [SerializeField] private Transform helmPanel;
    [SerializeField] private Transform amuletPanel;

    [SerializeField] private Transform meleePanel;
    [SerializeField] private Transform armorPanel;
    [SerializeField] private Transform rangedPanel;

    [SerializeField] private Transform shieldPanel;
    [SerializeField] private Transform bootsPanel;
    [SerializeField] private Transform missilePanel;

    [SerializeField] private Transform potionPanel;

    [SerializeField] private GameObject itemEntry_prefab;
    [SerializeField] private GameObject unequipItem_prefab;


    [SerializeField] private GameObject inventoryPanel;
    //[SerializeField] private GameObject inventoryDescriptPanel;

    private ScrollRect _scrollRect;
    private RectTransform _srContent;
    private RectTransform _srViewport;

    public bool inventoryActive = false;
    public bool equipPanelActive = false;
    public bool equipPanelFocus = false;

    public List<GameObject> inventoryEntries = new List<GameObject>();
    public List<RuntimeItem> inventoryItems = new List<RuntimeItem>();
    public int inventoryIndex = 0;

    private Coroutine _scrollCoroutine;

    [SerializeField] private GameObject equipPanel;
    [SerializeField] private TextMeshProUGUI equipHeaderTMP;
    [SerializeField] private TextMeshProUGUI inventoryHeaderTMP;
    [SerializeField] private GameObject bodyPanel;
    [SerializeField] private GameObject descriptionPanel_equip;
    [SerializeField] private GameObject descriptionPanel_inventory;

    [SerializeField] private Image equipPanelBackground;
    [SerializeField] private Image equipDescriptBackground;
    [SerializeField] private Image inventoryPanelBackground;
    [SerializeField] private Image inventoryDescriptBackground;

    [SerializeField] private GameObject profilePanel;
    private ProfilePanelController _profilePanelController;


    Image inventoryDescript_icon;
    TextMeshProUGUI inventoryDescript_txt;

    Image equipDescript_icon;
    TextMeshProUGUI equipDescript_txt;

    #region EquipPanel References
    [SerializeField] private GameObject ring_EPanel;
    [SerializeField] private GameObject helm_EPanel;
    [SerializeField] private GameObject amulet_EPanel;
    [SerializeField] private GameObject melee_EPanel;
    [SerializeField] private GameObject armor_EPanel;
    [SerializeField] private GameObject ranged_EPanel;
    [SerializeField] private GameObject shield_EPanel;
    [SerializeField] private GameObject boots_EPanel;
    [SerializeField] private GameObject missile_EPanel;

    private Transform ring_descript_default;
    private Transform ring_descript_equipped;
    private Transform ring_icon_default;
    private Transform ring_icon_equipped;
    private Transform ring_selectAmber;
    private Transform ring_selectGreen;

    private Transform helm_descript_default;
    private Transform helm_descript_equipped;
    private Transform helm_icon_default;
    private Transform helm_icon_equipped;
    private Transform helm_selectAmber;
    private Transform helm_selectGreen;

    private Transform amulet_descript_default;
    private Transform amulet_descript_equipped;
    private Transform amulet_icon_default;
    private Transform amulet_icon_equipped;
    private Transform amulet_selectAmber;
    private Transform amulet_selectGreen;

    private Transform melee_descript_default;
    private Transform melee_descript_equipped;
    private Transform melee_icon_default;
    private Transform melee_icon_equipped;
    private Transform melee_selectAmber;
    private Transform melee_selectGreen;

    private Transform armor_descript_default;
    private Transform armor_descript_equipped;
    private Transform armor_icon_default;
    private Transform armor_icon_equipped;
    private Transform armor_selectAmber;
    private Transform armor_selectGreen;

    private Transform ranged_descript_default;
    private Transform ranged_descript_equipped;
    private Transform ranged_icon_default;
    private Transform ranged_icon_equipped;
    private Transform ranged_selectAmber;
    private Transform ranged_selectGreen;

    private Transform shield_descript_default;
    private Transform shield_descript_equipped;
    private Transform shield_icon_default;
    private Transform shield_icon_equipped;
    private Transform shield_selectAmber;
    private Transform shield_selectGreen;

    private Transform boots_descript_default;
    private Transform boots_descript_equipped;
    private Transform boots_icon_default;
    private Transform boots_icon_equipped;
    private Transform boots_selectAmber;
    private Transform boots_selectGreen;

    private Transform missile_descript_default;
    private Transform missile_descript_equipped;
    private Transform missile_icon_default;
    private Transform missile_icon_equipped;
    private Transform missile_selectAmber;
    private Transform missile_selectGreen;

    private TextMeshProUGUI ring_text_equipped;
    private Image ring_iconImg_equipped;

    private TextMeshProUGUI helm_text_equipped;
    private Image helm_iconImg_equipped;

    private TextMeshProUGUI amulet_text_equipped;
    private Image amulet_iconImg_equipped;

    private TextMeshProUGUI melee_text_equipped;
    private Image melee_iconImg_equipped;

    private TextMeshProUGUI armor_text_equipped;
    private Image armor_iconImg_equipped;

    private TextMeshProUGUI ranged_text_equipped;
    private Image ranged_iconImg_equipped;

    private TextMeshProUGUI shield_text_equipped;
    private Image shield_iconImg_equipped;

    private TextMeshProUGUI boots_text_equipped;
    private Image boots_iconImg_equipped;

    private TextMeshProUGUI missile_text_equipped;
    private Image missile_iconImg_equipped;

    private TextMeshProUGUI ring_text_default;
    private Image ring_iconImg_default;

    private TextMeshProUGUI helm_text_default;
    private Image helm_iconImg_default;

    private TextMeshProUGUI amulet_text_default;
    private Image amulet_iconImg_default;

    private TextMeshProUGUI melee_text_default;
    private Image melee_iconImg_default;

    private TextMeshProUGUI armor_text_default;
    private Image armor_iconImg_default;

    private TextMeshProUGUI ranged_text_default;
    private Image ranged_iconImg_default;

    private TextMeshProUGUI shield_text_default;
    private Image shield_iconImg_default;

    private TextMeshProUGUI boots_text_default;
    private Image boots_iconImg_default;

    private TextMeshProUGUI missile_text_default;
    private Image missile_iconImg_default;

    // Ring
    private Image ring_selectAmberImg;
    private Image ring_selectGreenImg;

    // Helm
    private Image helm_selectAmberImg;
    private Image helm_selectGreenImg;

    // Amulet
    private Image amulet_selectAmberImg;
    private Image amulet_selectGreenImg;

    // Melee
    private Image melee_selectAmberImg;
    private Image melee_selectGreenImg;

    // Armor
    private Image armor_selectAmberImg;
    private Image armor_selectGreenImg;

    // Ranged
    private Image ranged_selectAmberImg;
    private Image ranged_selectGreenImg;

    // Shield
    private Image shield_selectAmberImg;
    private Image shield_selectGreenImg;

    // Boots
    private Image boots_selectAmberImg;
    private Image boots_selectGreenImg;

    // Missile
    private Image missile_selectAmberImg;
    private Image missile_selectGreenImg;


    #endregion

    public ItemCategory currentCatSelect = ItemCategory.none;



    private PlayerInputActions _playerInputActions;

    protected override void Awake()
    {
        base.Awake();

        _playerInputActions = new PlayerInputActions();

        GetInventoryDescriptPanelReferences();
        GetEquipPanelReferences();
        _profilePanelController = profilePanel.GetComponent<ProfilePanelController>();
       
    }
    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.UI_navigate.performed += OnInventoryUINavigate;
        _playerInputActions.Player.UIPanelSelect.performed += OnPanelSelect;
        _playerInputActions.Player.UnequipItem.performed += OnUnequipItem;

        SquadManager.OnCharacterSelected += HandleSkillsUI;
        SquadManager.OnCharacterSelected += UpdateInventoryPanelsToCharacter;

        SquadManager.OnInventorySelected += ToggleInventoryAndEquipUI;

        ItemEvents.OnItemPickedUp += RefreshInventoryListsAndObjects;
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Disable();
        _playerInputActions.Player.UI_navigate.performed -= OnInventoryUINavigate;
        _playerInputActions.Player.UIPanelSelect.performed -= OnPanelSelect;
        _playerInputActions.Player.UnequipItem.performed -= OnUnequipItem;


        SquadManager.OnCharacterSelected -= HandleSkillsUI;
        SquadManager.OnCharacterSelected -= UpdateInventoryPanelsToCharacter;
        SquadManager.OnInventorySelected -= ToggleInventoryAndEquipUI;

        ItemEvents.OnItemPickedUp -= RefreshInventoryListsAndObjects;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _squadManager = FindFirstObjectByType<SquadManager>();
        //inventory panel
        _scrollRect = inventoryPanel.GetComponentInChildren<ScrollRect>();
        _srContent = _scrollRect.content;
        _srViewport = _scrollRect.viewport;

        //populate slot_array with slot gameobjects
        slots_array = new GameObject[grid.childCount];
        for (int i = 0; i < slots_array.Length; i++)
        {
            slots_array[i] = grid.GetChild(i).gameObject;
            slots_array[i].SetActive(false); //deactivate the slots by default

        }


        //set Panel states
        inventoryPanel.SetActive(false);
        bodyPanel.SetActive(true);
        descriptionPanel_equip.SetActive(false);
        descriptionPanel_inventory.SetActive(false);
        equipPanel.SetActive(false);
        profilePanel.SetActive(false);


        //clear inventory lists to ensure clean slate
        inventoryEntries.Clear();
        inventoryItems.Clear();

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
                                UpdateSkillCooldownOverlay(slots_array[i], remainingCooldown, skill.cooldown);
                            }

                        }
                    }
                }
            }
        }
    }


    #region ***** Inventory and Equip Panel Methods
    private void ToggleInventoryAndEquipUI(GameObject ch_obj)
    {
        current_character = ch_obj;

        if (!inventoryActive) //turn on the inventory and equip panels
        {
            ClearInventoryListsAndObjects();
            PopulateInventoryListsAndObjects();
            ResetInventoryScrollRect();
            UpdateInventoryDescriptPanel();

            if (ch_obj != null)
            {
                PopulateEquipPanel(ch_obj);
                UpdateEquipDescriptPanel(ch_obj);
                _profilePanelController.UpdatePanelToCharacter(ch_obj);
                
            }

            ActivateInventoryAndEquipUI(ch_obj);


        }
        else
        {
            DeactivateInventoryUI();
        }

    }

    public void ClearInventoryListsAndObjects()
    {
        //Clear all the lists for each category
        ClearInventoryListPanel(ringPanel);
        ClearInventoryListPanel(helmPanel);
        ClearInventoryListPanel(amuletPanel);

        ClearInventoryListPanel(meleePanel);
        ClearInventoryListPanel(armorPanel);
        ClearInventoryListPanel(rangedPanel);

        ClearInventoryListPanel(shieldPanel);
        ClearInventoryListPanel(bootsPanel);
        ClearInventoryListPanel(missilePanel);
        ClearInventoryListPanel(potionPanel);
        //clear main lists
        inventoryEntries.Clear();
        inventoryItems.Clear();
        //reset index
        inventoryIndex = 0;
        //reset currentCat
        currentCatSelect = ItemCategory.none;

    }

    void ClearInventoryListPanel(Transform panel)
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

    private void ResetInventoryScrollRect()
    {
        //reset scroll view to top
        _scrollRect.verticalNormalizedPosition = 1f;
    }

    private void PopulateInventoryListsAndObjects()
    {


        foreach (var ringItem in InventoryManager.Instance.GetCoreRingsList())
        {
            AddItemToCategoryPanel(ringItem, ringPanel); // prefab instantiation + text set
        }


        foreach (var helmItem in InventoryManager.Instance.GetCoreHelmsList())
        {
            AddItemToCategoryPanel(helmItem, helmPanel); // prefab instantiation + text set
        }
        foreach (var amuletItem in InventoryManager.Instance.GetCoreAmuletsList())
        {
            AddItemToCategoryPanel(amuletItem, amuletPanel); // prefab instantiation + text set
        }

        foreach (var meleeItem in InventoryManager.Instance.GetCoreMeleeWeaponsList())
        {
            AddItemToCategoryPanel(meleeItem, meleePanel); // prefab instantiation + text set
        }

        /*
        if (current_character != null && _currentChStats.equipped_meleeWeapon != null)
        {
            AddUnequipToCategoryPanel(_currentChStats.equipped_meleeWeapon.item_name, meleePanel, ItemCategory.melee_weapon);
        }
        */

        foreach (var armorItem in InventoryManager.Instance.GetCoreArmorsList())
        {
            AddItemToCategoryPanel(armorItem, armorPanel); // prefab instantiation + text set
        }

        foreach (var rangedItem in InventoryManager.Instance.GetCoreRangedWeaponsList())
        {
            AddItemToCategoryPanel(rangedItem, rangedPanel);
        }


        foreach (var shieldItem in InventoryManager.Instance.GetCoreShieldsList())
        {
            AddItemToCategoryPanel(shieldItem, shieldPanel); // prefab instantiation + text set
        }
        foreach (var bootItem in InventoryManager.Instance.GetCoreBootsList())
        {
            AddItemToCategoryPanel(bootItem, bootsPanel); // prefab instantiation + text set
        }

        foreach (var missileItem in InventoryManager.Instance.GetCoreMissilesList())
        {
            AddItemToCategoryPanel(missileItem, missilePanel);
        }

        foreach (var potionItem in InventoryManager.Instance.GetCorePotionsList())
        {
            AddItemToCategoryPanel(potionItem, potionPanel);
        }

    }

    private void AddItemToCategoryPanel(RuntimeItem item, Transform categoryPanel)
    {
        GameObject newObj = Instantiate(itemEntry_prefab, categoryPanel);

        Transform itemName = newObj.transform.Find("ItemName");
        TextMeshProUGUI nameText = itemName.GetComponent<TextMeshProUGUI>();

        nameText.text = item.item_name;

        inventoryEntries.Add(newObj);
        inventoryItems.Add(item);

        //activate/deactivate selection options
        if (inventoryEntries.Count == inventoryIndex + 1) //select the current index
        {
            if (!equipPanelFocus)
            {
                SelectInventoryEntry(newObj);

            }
            else
            {
                DeselectInventoryEntry(newObj);
            }


            currentCatSelect = item.category; //initialize the currentCatSelect
            //ScrollToItem(newObj);
        }
        else if (inventoryEntries.Count > 1)
        {
            DeselectInventoryEntry(newObj);

        }


    }

    private void AddUnequipToCategoryPanel(string itemName, Transform categoryPanel, ItemCategory category)
    {
        GameObject newObj = Instantiate(unequipItem_prefab, categoryPanel);
        Transform unequipText = newObj.transform.Find("UnequipText");
        TextMeshProUGUI tmpText = unequipText.GetComponent<TextMeshProUGUI>();

        tmpText.text = $"<Unequip: {itemName}"; ;

        inventoryEntries.Add(newObj);
        inventoryItems.Add(null);

        //activate/deactivate selection options
        if (inventoryEntries.Count == inventoryIndex + 1) //select the current index
        {
            SelectInventoryEntry(newObj);
            currentCatSelect = category; //initialize the currentCatSelect
            //ScrollToItem(newObj);
        }
        else if (inventoryEntries.Count > 1)
        {
            DeselectInventoryEntry(newObj);

        }
    }

    private void UpdateInventoryDescriptPanel()
    {
        if (inventoryEntries.Count > 0 && inventoryItems[inventoryIndex] != null)
        {
            descriptionPanel_inventory.SetActive(true);
            inventoryDescript_icon.sprite = inventoryItems[inventoryIndex].Icon;
            inventoryDescript_txt.text = inventoryItems[inventoryIndex].baseItem.description;

        }
        else
        {
            descriptionPanel_inventory.SetActive(false);
        }


    }

    private void PopulateEquipPanel(GameObject ch_obj)
    {
        if (ch_obj != null)
        {
            EntityStats entityStats = ch_obj.GetComponent<EntityStats>();


            if (entityStats.equipped_ring != null)
            {
                //Debug.Log("equipped ring=" + entityStats.equipped_ring);
                ring_text_equipped.text = entityStats.equipped_ring.item_name;
                ring_iconImg_equipped.sprite = entityStats.equipped_ring.Icon;

                ring_text_default.enabled = false;
                ring_text_equipped.enabled = true;
                ring_iconImg_default.enabled = false;
                ring_iconImg_equipped.enabled = true;
            }
            else
            {
                ring_text_default.enabled = true;
                ring_text_equipped.enabled = false;
                ring_iconImg_default.enabled = true;
                ring_iconImg_equipped.enabled = false;
            }

            if (currentCatSelect == ItemCategory.ring)
            {
                ring_selectAmberImg.enabled = true;
            }
            else
            {
                ring_selectAmberImg.enabled = false;
            }

            if (entityStats.equipped_helm != null)
            {
                helm_text_equipped.text = entityStats.equipped_helm.item_name;
                helm_iconImg_equipped.sprite = entityStats.equipped_helm.Icon;

                helm_text_default.enabled = false;
                helm_text_equipped.enabled = true;
                helm_iconImg_default.enabled = false;
                helm_iconImg_equipped.enabled = true;
            }
            else
            {
                helm_text_default.enabled = true;
                helm_text_equipped.enabled = false;
                helm_iconImg_default.enabled = true;
                helm_iconImg_equipped.enabled = false;
            }

            if (currentCatSelect == ItemCategory.helm)
            {
                helm_selectAmberImg.enabled = true;
            }
            else
            {
                helm_selectAmberImg.enabled = false;
            }

            if (entityStats.equipped_amulet != null)
            {
                amulet_text_equipped.text = entityStats.equipped_amulet.item_name;
                amulet_iconImg_equipped.sprite = entityStats.equipped_amulet.Icon;

                amulet_text_default.enabled = false;
                amulet_text_equipped.enabled = true;
                amulet_iconImg_default.enabled = false;
                amulet_iconImg_equipped.enabled = true;
            }
            else
            {
                amulet_text_default.enabled = true;
                amulet_text_equipped.enabled = false;
                amulet_iconImg_default.enabled = true;
                amulet_iconImg_equipped.enabled = false;
            }

            if (currentCatSelect == ItemCategory.amulet)
            {
                amulet_selectAmberImg.enabled = true;
            }
            else
            {
                amulet_selectAmberImg.enabled = false;
            }


            if (entityStats.equipped_meleeWeapon != null)
            {
                melee_text_equipped.text = entityStats.equipped_meleeWeapon.item_name;
                melee_iconImg_equipped.sprite = entityStats.equipped_meleeWeapon.Icon;

                melee_text_default.enabled = false;
                melee_text_equipped.enabled = true;
                melee_iconImg_default.enabled = false;
                melee_iconImg_equipped.enabled = true;
            }
            else
            {
                melee_text_default.enabled = true;
                melee_text_equipped.enabled = false;
                melee_iconImg_default.enabled = true;
                melee_iconImg_equipped.enabled = false;
            }

            if (currentCatSelect == ItemCategory.melee_weapon)
            {
                melee_selectAmberImg.enabled = true;
            }
            else
            {
                melee_selectAmberImg.enabled = false;
            }


            if (entityStats.equipped_armor != null)
            {
                armor_text_equipped.text = entityStats.equipped_armor.item_name;
                armor_iconImg_equipped.sprite = entityStats.equipped_armor.Icon;

                armor_text_default.enabled = false;
                armor_text_equipped.enabled = true;
                armor_iconImg_default.enabled = false;
                armor_iconImg_equipped.enabled = true;
            }
            else
            {
                armor_text_default.enabled = true;
                armor_text_equipped.enabled = false;
                armor_iconImg_default.enabled = true;
                armor_iconImg_equipped.enabled = false;
            }

            if (currentCatSelect == ItemCategory.armor)
            {
                armor_selectAmberImg.enabled = true;
            }
            else
            {
                armor_selectAmberImg.enabled = false;
            }


            if (entityStats.equipped_rangedWeapon != null)
            {
                ranged_text_equipped.text = entityStats.equipped_rangedWeapon.item_name;
                ranged_iconImg_equipped.sprite = entityStats.equipped_rangedWeapon.Icon;

                ranged_text_default.enabled = false;
                ranged_text_equipped.enabled = true;
                ranged_iconImg_default.enabled = false;
                ranged_iconImg_equipped.enabled = true;
            }
            else
            {
                ranged_text_default.enabled = true;
                ranged_text_equipped.enabled = false;
                ranged_iconImg_default.enabled = true;
                ranged_iconImg_equipped.enabled = false;
            }

            if (currentCatSelect == ItemCategory.ranged_weapon)
            {
                ranged_selectAmberImg.enabled = true;
            }
            else
            {
                ranged_selectAmberImg.enabled = false;
            }


            if (entityStats.equipped_shield != null)
            {
                shield_text_equipped.text = entityStats.equipped_shield.item_name;
                shield_iconImg_equipped.sprite = entityStats.equipped_shield.Icon;

                shield_text_default.enabled = false;
                shield_text_equipped.enabled = true;
                shield_iconImg_default.enabled = false;
                shield_iconImg_equipped.enabled = true;
            }
            else
            {
                shield_text_default.enabled = true;
                shield_text_equipped.enabled = false;
                shield_iconImg_default.enabled = true;
                shield_iconImg_equipped.enabled = false;
            }

            if (currentCatSelect == ItemCategory.shield)
            {
                shield_selectAmberImg.enabled = true;
            }
            else
            {
                shield_selectAmberImg.enabled = false;
            }


            if (entityStats.equipped_boots != null)
            {
                boots_text_equipped.text = entityStats.equipped_boots.item_name;
                boots_iconImg_equipped.sprite = entityStats.equipped_boots.Icon;

                boots_text_default.enabled = false;
                boots_text_equipped.enabled = true;
                boots_iconImg_default.enabled = false;
                boots_iconImg_equipped.enabled = true;
            }
            else
            {
                boots_text_default.enabled = true;
                boots_text_equipped.enabled = false;
                boots_iconImg_default.enabled = true;
                boots_iconImg_equipped.enabled = false;
            }

            if (currentCatSelect == ItemCategory.boots)
            {
                boots_selectAmberImg.enabled = true;
            }
            else
            {
                boots_selectAmberImg.enabled = false;
            }


            if (entityStats.equipped_missile != null)
            {
                missile_text_equipped.text = entityStats.equipped_missile.item_name;
                missile_iconImg_equipped.sprite = entityStats.equipped_missile.Icon;

                missile_text_default.enabled = false;
                missile_text_equipped.enabled = true;
                missile_iconImg_default.enabled = false;
                missile_iconImg_equipped.enabled = true;
            }
            else
            {
                missile_text_default.enabled = true;
                missile_text_equipped.enabled = false;
                missile_iconImg_default.enabled = true;
                missile_iconImg_equipped.enabled = false;
            }

            if (currentCatSelect == ItemCategory.missile)
            {
                missile_selectAmberImg.enabled = true;
            }
            else
            {
                missile_selectAmberImg.enabled = false;
            }




        }


    }

    private void UpdateEquipDescriptPanel(GameObject ch_obj)
    {
        EntityStats entityStats = ch_obj.GetComponent<EntityStats>();
        RuntimeItem equippedItem;

        switch (currentCatSelect)
        {
            case ItemCategory.ring:
                equippedItem = entityStats.equipped_ring;
                break;
            case ItemCategory.helm:
                equippedItem = entityStats.equipped_helm;
                break;
            case ItemCategory.amulet:
                equippedItem = entityStats.equipped_amulet;
                break;
            case ItemCategory.melee_weapon:
                equippedItem = entityStats.equipped_meleeWeapon;
                break;
            case ItemCategory.armor:
                equippedItem = entityStats.equipped_armor;
                break;
            case ItemCategory.ranged_weapon:
                equippedItem = entityStats.equipped_rangedWeapon;
                break;
            case ItemCategory.shield:
                equippedItem = entityStats.equipped_shield;
                break;
            case ItemCategory.boots:
                equippedItem = entityStats.equipped_boots;
                break;
            case ItemCategory.missile:
                equippedItem = entityStats.equipped_missile;
                break;
            default:
                equippedItem = null;
                break;

        }

        if (equippedItem != null)
        {
            descriptionPanel_equip.SetActive(true);
            equipDescript_icon.sprite = equippedItem.Icon;
            equipDescript_txt.text = equippedItem.description;
        }
        else
        {
            descriptionPanel_equip.SetActive(false);
        }


    }

    private void ActivateInventoryAndEquipUI(GameObject ch_obj)
    {
        SetEquipPanelFocus(false);
        inventoryPanel.SetActive(true);

        if (ch_obj != null)
        {
            equipPanel.SetActive(true);
            profilePanel.SetActive(true);
        }
        else
        {
            equipPanel.SetActive(false);
            profilePanel.SetActive(false);
        }

        inventoryActive = true;
    }

    private void DeactivateInventoryUI()
    {
        inventoryPanel.SetActive(false);
        equipPanel.SetActive(false);
        profilePanel.SetActive(false);

        SetEquipPanelFocus(false);

        inventoryActive = false;
    }

    //this is called when squadmanager selects or deselects a character
    private void UpdateInventoryPanelsToCharacter(GameObject ch_obj)
    {
        if (!inventoryActive)
        {
            return;
        }
        else
        {
            current_character = ch_obj;

           

            if (ch_obj != null)
            {
                equipPanel.gameObject.SetActive(true);
                PopulateEquipPanel(ch_obj);
                UpdateEquipDescriptPanel(ch_obj);

                equipPanelActive = true;
                SelectInventoryEntry(inventoryEntries[inventoryIndex]);

                //Debug.Log("Update Panel to Character FOUND. Changed to " + ch_obj.name + " Updating ProfilePanel to character. EquipPanelFocus=" +equipPanelFocus);

                _profilePanelController.UpdatePanelToCharacter(ch_obj);
                profilePanel.SetActive(true);

            }
            else
            {
                //Debug.Log("Update Panel to Character. Null characterObj found.");
                equipPanel.gameObject.SetActive(false);
                equipPanelActive = false;
                equipPanelFocus = false;
                SelectInventoryEntry(inventoryEntries[inventoryIndex]);

                profilePanel.SetActive(false);
            }

        }

    }

    //this updates the inventory list to show new items picked up or unequipped
    public void RefreshInventoryListsAndObjects()
    {
        if (!inventoryActive)
        {
            return;
        }
        else
        {
            ClearInventoryListsAndObjects();
            PopulateInventoryListsAndObjects();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_srContent);

        }


    }

    private void OnInventoryUINavigate(InputAction.CallbackContext context)
    {
        if (!inventoryActive)
        {
            return;
        }
        else
        {
            if (inventoryEntries.Count > 0)
            {
                Vector2 input = context.ReadValue<Vector2>();

                if (input.y > 0f)
                {
                    if (!equipPanelFocus)
                    {
                        BumpInventorySelectIndex(-1);
                    }
                    else if (equipPanelFocus)
                    {
                        //move equip category select
                        MoveEquipCategorySelect(SelectDirection.up);
                    }

                }
                else if (input.y < 0f)
                {
                    if (!equipPanelFocus)
                    {
                        BumpInventorySelectIndex(1);

                    }
                    else if (equipPanelFocus)
                    {
                        //move equip category select
                        MoveEquipCategorySelect(SelectDirection.down);

                    }
                }
                else if (input.x > 0f)
                {
                    if (!equipPanelFocus)
                    {
                        
                        DropItem(inventoryIndex);

                    }
                    else if (equipPanelFocus)
                    {
                        //move equip category select
                        MoveEquipCategorySelect(SelectDirection.right);

                    }
                }
                else if (input.x < 0f)
                {
                    if (!equipPanelFocus)
                    {
                        if (current_character != null && inventoryItems[inventoryIndex].IsEquippable)
                        {
                            EquipItem(inventoryIndex);
                        }
                        else if (current_character != null && inventoryItems[inventoryIndex].IsUseable)
                        {
                            UseItem(inventoryIndex);
                        }

                    }
                    else if (equipPanelFocus)
                    {
                        //move equip category select
                        MoveEquipCategorySelect(SelectDirection.left);


                    }

                }
            }
        }
    }

    private void MoveEquipCategorySelect(SelectDirection select)
    {
        switch (currentCatSelect)
        {
            case ItemCategory.ring:
                switch (select)
                {
                    case SelectDirection.up:
                        break;
                    case SelectDirection.down:
                        UpdateCategorySelection(ItemCategory.melee_weapon);
                        break;
                    case SelectDirection.left:
                        break;
                    case SelectDirection.right:
                        UpdateCategorySelection(ItemCategory.helm);
                        break;
                    default:
                        break;
                }
                break;

            case ItemCategory.helm:
                switch (select)
                {
                    case SelectDirection.up:
                        break;
                    case SelectDirection.down:
                        UpdateCategorySelection(ItemCategory.armor);
                        break;
                    case SelectDirection.left:
                        UpdateCategorySelection(ItemCategory.ring);
                        break;
                    case SelectDirection.right:
                        UpdateCategorySelection(ItemCategory.amulet);
                        break;
                    default:
                        break;
                }
                break;

            case ItemCategory.amulet:
                switch (select)
                {
                    case SelectDirection.up:
                        break;
                    case SelectDirection.down:
                        UpdateCategorySelection(ItemCategory.ranged_weapon);
                        break;
                    case SelectDirection.left:
                        UpdateCategorySelection(ItemCategory.ring);
                        break;
                    case SelectDirection.right:
                        break;
                    default:
                        break;
                }
                break;

            case ItemCategory.melee_weapon:
                switch (select)
                {
                    case SelectDirection.up:
                        UpdateCategorySelection(ItemCategory.ring);
                        break;
                    case SelectDirection.down:
                        UpdateCategorySelection(ItemCategory.shield);
                        break;
                    case SelectDirection.left:
                        break;
                    case SelectDirection.right:
                        UpdateCategorySelection(ItemCategory.armor);
                        break;
                    default:
                        break;
                }
                break;

            case ItemCategory.armor:
                switch (select)
                {
                    case SelectDirection.up:
                        UpdateCategorySelection(ItemCategory.helm);
                        break;
                    case SelectDirection.down:
                        UpdateCategorySelection(ItemCategory.boots);
                        break;
                    case SelectDirection.left:
                        UpdateCategorySelection(ItemCategory.melee_weapon);
                        break;
                    case SelectDirection.right:
                        UpdateCategorySelection(ItemCategory.ranged_weapon);
                        break;
                    default:
                        break;
                }
                break;

            case ItemCategory.ranged_weapon:
                switch (select)
                {
                    case SelectDirection.up:
                        UpdateCategorySelection(ItemCategory.amulet);
                        break;
                    case SelectDirection.down:
                        UpdateCategorySelection(ItemCategory.missile);
                        break;
                    case SelectDirection.left:
                        UpdateCategorySelection(ItemCategory.armor);
                        break;
                    case SelectDirection.right:
                        break;
                    default:
                        break;
                }
                break;

            case ItemCategory.shield:
                switch (select)
                {
                    case SelectDirection.up:
                        UpdateCategorySelection(ItemCategory.melee_weapon);
                        break;
                    case SelectDirection.down:
                        break;
                    case SelectDirection.left:
                        break;
                    case SelectDirection.right:
                        UpdateCategorySelection(ItemCategory.boots);
                        break;
                    default:
                        break;
                }
                break;

            case ItemCategory.boots:
                switch (select)
                {
                    case SelectDirection.up:
                        UpdateCategorySelection(ItemCategory.armor);
                        break;
                    case SelectDirection.down:
                        break;
                    case SelectDirection.left:
                        UpdateCategorySelection(ItemCategory.shield);
                        break;
                    case SelectDirection.right:
                        UpdateCategorySelection(ItemCategory.missile);
                        break;
                    default:
                        break;
                }
                break;

            case ItemCategory.missile:
                switch (select)
                {
                    case SelectDirection.up:
                        UpdateCategorySelection(ItemCategory.ranged_weapon);
                        break;
                    case SelectDirection.down:
                        break;
                    case SelectDirection.left:
                        UpdateCategorySelection(ItemCategory.boots);
                        break;
                    case SelectDirection.right:
                        break;
                    default:
                        break;
                }
                break;

            default:
                Debug.LogWarning("Unhandled item category: " + currentCatSelect);
                break;
        }



    }

    private void OnPanelSelect(InputAction.CallbackContext context)
    {
        if (!inventoryActive || !equipPanelActive)
        {
            return;
        }
        else
        {
            float inputValue = context.ReadValue<float>();

            if (inputValue > 0 && equipPanelFocus)
            {
                SetEquipPanelFocus(false);

            }
            else if (inputValue < 0 && !equipPanelFocus)
            {
                SetEquipPanelFocus(true);

            }

        }


    }

    private void SetEquipPanelFocus(bool state)
    {
        if (state)
        {
            equipHeaderTMP.fontStyle = FontStyles.Bold;
            inventoryHeaderTMP.fontStyle = FontStyles.Normal;

            Color32 color;
            color = equipPanelBackground.color;
            color.a = 240;
            equipPanelBackground.color = color;

            color = equipDescriptBackground.color;
            color.a = 240;
            equipDescriptBackground.color = color;

            color = inventoryPanelBackground.color;
            color.a = 200;
            inventoryPanelBackground.color = color;
            color = inventoryDescriptBackground.color;
            color.a = 200;
            inventoryDescriptBackground.color = color;

            DeselectInventoryEntry(inventoryEntries[inventoryIndex]);

            //update profilepanel to show dequip stats
            _profilePanelController.UpdateReadjustSlidersAndStats( current_character.GetComponent<EntityStats>().GetEquippedByCategory(currentCatSelect), EquipState.unequip);

            equipPanelFocus = true;
        }
        else if (!state)
        {
            equipHeaderTMP.fontStyle = FontStyles.Normal;
            inventoryHeaderTMP.fontStyle = FontStyles.Bold;

            Color32 color;
            color = equipPanelBackground.color;
            color.a = 200;
            equipPanelBackground.color = color;

            color = equipDescriptBackground.color;
            color.a = 200;
            equipDescriptBackground.color = color;

            color = inventoryPanelBackground.color;
            color.a = 240;
            inventoryPanelBackground.color = color;
            color = inventoryDescriptBackground.color;
            color.a = 240;
            inventoryDescriptBackground.color = color;

            SelectInventoryEntry(inventoryEntries[inventoryIndex]);

            UpdateCategorySelection(inventoryItems[inventoryIndex].category);

            //update profile panel to show new equipstats
            _profilePanelController.UpdateReadjustSlidersAndStats(inventoryItems[inventoryIndex], EquipState.equip);

            equipPanelFocus = false;

        }
    }
    private void EquipItem(int index)
    {
        InventoryManager.Instance.EquipItemToCharacter(inventoryItems[index], current_character);
        //Debug.Log("Initial scrolrect position=" + _scrollRect.verticalNormalizedPosition);
        RefreshInventoryListsAndObjects();
        MoveToInventoryIndex(index);
        //Debug.Log("New scrollrect="+_scrollRect.verticalNormalizedPosition);

        UpdateInventoryDescriptPanel();
        PopulateEquipPanel(current_character);
        UpdateEquipDescriptPanel(current_character);

        _profilePanelController.UpdateStatsPanels_1_2();
    }

    private void UseItem(int index)
    {
        if (inventoryItems[index].Potion != null)
        {
            inventoryItems[index].Potion.Use(current_character);
            
            InventoryManager.Instance.DeleteItemFromCore(inventoryItems[index]);
            RefreshInventoryListsAndObjects();
            MoveToInventoryIndex(index);

            UpdateInventoryDescriptPanel();

            _profilePanelController.UpdateHitPoints();
            _profilePanelController.UpdateStatsPanels_1_2();


        }


    }

    private void OnUnequipItem(InputAction.CallbackContext context)
    {
        if (!equipPanelFocus)
        {
            return;
        }
        else
        {
            if (current_character != null)
            {
                EntityStats _chStats = current_character.GetComponent<EntityStats>();

                RuntimeItem itemToUnequip = null;

                switch (currentCatSelect)
                {
                    case ItemCategory.ring:
                        itemToUnequip = _chStats.equipped_ring != null ? _chStats.equipped_ring : null;
                        break;

                    case ItemCategory.helm:
                        itemToUnequip = _chStats.equipped_helm != null ? _chStats.equipped_helm : null;
                        break;

                    case ItemCategory.amulet:
                        itemToUnequip = _chStats.equipped_amulet != null ? _chStats.equipped_amulet : null;
                        break;

                    case ItemCategory.melee_weapon:
                        itemToUnequip = _chStats.equipped_meleeWeapon != null ? _chStats.equipped_meleeWeapon : null;
                        break;

                    case ItemCategory.armor:
                        itemToUnequip = _chStats.equipped_armor != null ? _chStats.equipped_armor : null;
                        break;

                    case ItemCategory.ranged_weapon:
                        itemToUnequip = _chStats.equipped_rangedWeapon != null ? _chStats.equipped_rangedWeapon : null;
                        break;

                    case ItemCategory.shield:
                        itemToUnequip = _chStats.equipped_shield != null ? _chStats.equipped_shield : null;
                        break;

                    case ItemCategory.boots:
                        itemToUnequip = _chStats.equipped_boots != null ? _chStats.equipped_boots : null;
                        break;

                    case ItemCategory.missile:
                        itemToUnequip = _chStats.equipped_missile != null ? _chStats.equipped_missile : null;
                        break;

                    default:
                        itemToUnequip = null;
                        break;
                }

               if (itemToUnequip != null)
                {

                    InventoryManager.Instance.UnequipItemfromCharacter(itemToUnequip, current_character);

                    RefreshInventoryListsAndObjects();
                    int unequippedItemIndex = inventoryItems.IndexOf(itemToUnequip);
                    MoveToInventoryIndex(unequippedItemIndex);
                    //Debug.Log("New scrollrect="+_scrollRect.verticalNormalizedPosition);

                    UpdateInventoryDescriptPanel();
                    PopulateEquipPanel(current_character);
                    UpdateEquipDescriptPanel(current_character);


                }

                //update profile panel


                _profilePanelController.UpdateStatsPanels_1_2();

            }
        }
    }

    private void DropItem(int index)
    {
        Debug.Log("Dropping item " + inventoryItems[index].item_name);
        //destroy entry prefab
        Destroy(inventoryEntries[index]);

        //remove item from core inventory and instantiate on ground
        InventoryManager.Instance.DropItemFromCore(inventoryItems[index]);

        //remove from lists
        inventoryEntries.RemoveAt(index);
        inventoryItems.RemoveAt(index);

        //move index to next item down

        //keep inventory index the same (will highlight next item after the dropped one)
        inventoryIndex = Mathf.Clamp(index, 0, inventoryEntries.Count - 1);
        if (inventoryEntries.Count > 0)
        {
            SelectInventoryEntry(inventoryEntries[inventoryIndex]);
            ScrollToItem(inventoryEntries[inventoryIndex]);

        }


    }

    private void MoveToInventoryIndex(int index)
    {
        if (inventoryEntries.Count == 0)
        {
            return;
        }
        else
        {
            index = Mathf.Clamp(index, 0, inventoryEntries.Count - 1);


            DeselectInventoryEntry(inventoryEntries[inventoryIndex]);
            SelectInventoryEntry(inventoryEntries[index]);



            inventoryIndex = index;
            UpdateCategorySelection(inventoryItems[inventoryIndex].baseItem.category);
            //Debug.Log("Scrolling to index: " + inventoryIndex);
            ScrollToItem(inventoryEntries[inventoryIndex]);
        }

    }
    private void ScrollToItem(GameObject entry)
    {
        RectTransform entryRect = entry.GetComponent<RectTransform>();

        // Calculate the world space bounds
        Bounds entryBounds = GetWorldSpaceBounds(entryRect);
        Bounds viewportBounds = GetWorldSpaceBounds(_srViewport);

       

        // Find the index of this entry in our list
        int entryIndex = inventoryEntries.IndexOf(entry);

        //Debug.Log($"******ENTRY: {entryIndex} Entry: Min = {entryBounds.min.y}, Max = {entryBounds.max.y}");
        //Debug.Log($"Viewport Bounds: Min = {viewportBounds.min.y}, Max = {viewportBounds.max.y}");
        // Special handling for top items (first 2 items)
        if (entryIndex <= 2)
        {
            // For the first few items, just scroll all the way to the top
            if (_scrollCoroutine != null)
                StopCoroutine(_scrollCoroutine);

            _scrollCoroutine = StartCoroutine(SmoothScrollTo(1.0f)); // 1.0 = top position
            return;
        }


        // Special handling for bottom items
        if (entryIndex >= inventoryEntries.Count - 3)
        {
            // For the last few items, scroll all the way to the bottom
            if (_scrollCoroutine != null)
                StopCoroutine(_scrollCoroutine);
            _scrollCoroutine = StartCoroutine(SmoothScrollTo(0.0f)); // 0.0 = bottom position
            return;
        }


        // Standard visibility check
        if (viewportBounds.Contains(entryBounds.min) && viewportBounds.Contains(entryBounds.max))
        {
            // Item is fully visible, no need to scroll
            return;
        }


        // Get item's position in content's local space
        float localY = _srContent.InverseTransformPoint(entryRect.position).y;
        float itemHeight = entryRect.rect.height;
        float contentHeight = _srContent.rect.height;
        float viewportHeight = _srViewport.rect.height;

        // Flip Y because UI scroll direction is inverted
        float flippedY = -localY;

        float targetPosition;

        // Determine target scroll position
        if (entryBounds.min.y > viewportBounds.max.y) // above
        {
            //Debug.Log($"Entry is above viewport");

            targetPosition = flippedY - itemHeight;
        }
        else if (entryBounds.max.y < viewportBounds.min.y) // below
        {
            // Item is below viewport - ensure we get enough scroll
            //Debug.Log($"Entry is below viewport");
            targetPosition = flippedY + itemHeight - viewportHeight;

            /*
            // Add more scroll room for lower items in the list
            float listProgress = (float)entryIndex / inventoryEntries.Count;
            if (listProgress > 0.5f) // If item is in the bottom half of the list
            {
                // Apply a progressive adjustment based on position in list
                float adjustment = Mathf.Lerp(0, itemHeight * 0.5f, listProgress - 0.5f) * 2;
                targetPosition += adjustment;
                Debug.Log($"Applied additional scroll adjustment of {adjustment} for lower item");
            }
            */
            targetPosition = flippedY + itemHeight - viewportHeight;
        }
        else if (entryBounds.min.y < viewportBounds.min.y) // bottom clipped
        {
            //Debug.Log($"Entry is bottom is clipped.");

            targetPosition = flippedY + itemHeight - viewportHeight;
        }
        else // top clipped
        {
            //Debug.Log($"Entry is top is clipped ");

            targetPosition = flippedY - itemHeight;
        }

        // Convert to normalized position
        float targetScrollPosition = 1 - (targetPosition / (contentHeight - viewportHeight));
        targetScrollPosition = Mathf.Clamp01(targetScrollPosition);
       


        // Start smooth scrolling
        if (_scrollCoroutine != null)
            StopCoroutine(_scrollCoroutine);

        _scrollCoroutine = StartCoroutine(SmoothScrollTo(targetScrollPosition));
    }

    private void BumpInventorySelectIndex(int indexChange)
    {
        if (inventoryEntries.Count == 0)
        {
            return;
        }
        else
        {
            int newIndex = Mathf.Clamp(inventoryIndex + indexChange, 0, inventoryEntries.Count - 1);

            if (newIndex != inventoryIndex)
            {
                //SoundManager.Instance.PlaySoundByKey("single_click", SoundCategory.UI);

                //deselect inventory index
                DeselectInventoryEntry(inventoryEntries[inventoryIndex]);
                //select new index
                inventoryIndex = newIndex;
                SelectInventoryEntry(inventoryEntries[inventoryIndex]);



                UpdateCategorySelection(inventoryItems[inventoryIndex].baseItem.category);
                //currentCatSelect = (inventoryItems[inventoryIndex].baseItem.category);
                ScrollToItem(inventoryEntries[inventoryIndex]);

                
            }

        }

    }

    private void UpdateCategorySelection(ItemCategory newCategory)
    {

        //if (current_character != null && newCategory != currentCatSelect)
        if (current_character != null)

        {
            //switch highlighted equip slot
            SetAmberSelectState(currentCatSelect, false);
            SetAmberSelectState(newCategory, true);

            //switch the current category
            currentCatSelect = newCategory;

            //update the EquipDescript
            UpdateEquipDescriptPanel(current_character);

            if (equipPanelFocus)
            {

                _profilePanelController.UpdateReadjustSlidersAndStats(current_character.GetComponent<EntityStats>().GetEquippedByCategory(currentCatSelect), EquipState.unequip);
            }


        }
        else
        {

            currentCatSelect = newCategory; //even if the new cat is the same as currentCat still set it to the newCat


        }


    }

    private void SetAmberSelectState(ItemCategory category, bool state)
    {
        switch (category)
        {
            case ItemCategory.ring:
                ring_selectAmberImg.enabled = state;
                break;
            case ItemCategory.helm:
                helm_selectAmberImg.enabled = state;
                break;
            case ItemCategory.amulet:
                amulet_selectAmberImg.enabled = state;
                break;
            case ItemCategory.melee_weapon:
                melee_selectAmberImg.enabled = state;
                break;
            case ItemCategory.armor:
                armor_selectAmberImg.enabled = state;
                break;
            case ItemCategory.ranged_weapon:
                ranged_selectAmberImg.enabled = state;
                break;
            case ItemCategory.shield:
                shield_selectAmberImg.enabled = state;
                break;
            case ItemCategory.boots:
                boots_selectAmberImg.enabled = state;
                break;
            case ItemCategory.missile:
                missile_selectAmberImg.enabled = state;
                break;
            default:
                break;
        }


    }

    private void SelectInventoryEntry(GameObject entry)
    {
        if (entry.tag == "EquipItem")
        {
            Transform itemName = entry.transform.Find("ItemName");
            TextMeshProUGUI nameText = itemName.GetComponent<TextMeshProUGUI>();

            Transform equipText = entry.transform.Find("Equip");
            Transform dropText = entry.transform.Find("Drop");
            Transform useText = entry.transform.Find("Use");
            Image background = entry.GetComponentInChildren<Image>();

            nameText.fontStyle = FontStyles.Bold;
            if ((!inventoryItems[inventoryIndex].IsEquippable && !inventoryItems[inventoryIndex].IsUseable) || current_character == null || (_squadManager.select_active != -1 && _squadManager.ch_in_slot_array[_squadManager.select_active] == null))
            {
                equipText.gameObject.SetActive(false);
                useText.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log(inventoryItems[inventoryIndex].item_name+":  isEquippable=" + inventoryItems[inventoryIndex].IsEquippable+ "  isUseable="+ inventoryItems[inventoryIndex].IsUseable );

                if (inventoryItems[inventoryIndex].IsEquippable)
                {
                    equipText.gameObject.SetActive(true);
                    useText.gameObject.SetActive(false);

                    _profilePanelController.UpdateReadjustSlidersAndStats(inventoryItems[inventoryIndex], EquipState.equip);

                }
                else if (inventoryItems[inventoryIndex].IsUseable)
                {
                    Debug.Log("Setting USE TEXT to true.");
                    equipText.gameObject.SetActive(false);
                    useText.gameObject.SetActive(true);

                    _profilePanelController.UpdateReadjustSlidersAndStats(inventoryItems[inventoryIndex], EquipState.equip);
                }
                else
                {
                    equipText.gameObject.SetActive(false);
                    useText.gameObject.SetActive(false);

                }


            }
            dropText.gameObject.SetActive(true);
            background.color = new Color32(221, 147, 39, 20);

        }

        /*
        else if (entry.tag == "UnequipItem")
        {
            Transform unequipText = entry.transform.Find("UnequipText");
            TextMeshProUGUI tmpText = unequipText.GetComponent<TextMeshProUGUI>();

            Transform unequip = entry.transform.Find("Unequip");
            Image background = entry.GetComponentInChildren<Image>();

            tmpText.fontStyle = FontStyles.Bold;
            unequip.gameObject.SetActive(true);

            background.color = new Color32(221, 147, 39, 20);


        }
        */


        UpdateInventoryDescriptPanel();

    }

    private void DeselectInventoryEntry(GameObject entry)
    {
        if (entry.tag == "EquipItem")
        {
            Transform itemName = entry.transform.Find("ItemName");
            TextMeshProUGUI nameText = itemName.GetComponent<TextMeshProUGUI>();

            Transform equipText = entry.transform.Find("Equip");
            Transform dropText = entry.transform.Find("Drop");
            Transform useText = entry.transform.Find("Use");

            Image background = entry.GetComponentInChildren<Image>();

            nameText.fontStyle = FontStyles.Normal;
            equipText.gameObject.SetActive(false);
            dropText.gameObject.SetActive(false);
            useText .gameObject.SetActive(false);
            background.color = new Color32(152, 152, 152, 22);

        }
        /*
        else if (entry.tag == "UnequipItem")
        {
            Transform unequipText = entry.transform.Find("UnequipText");
            TextMeshProUGUI tmpText = unequipText.GetComponent<TextMeshProUGUI>();

            Transform unequip = entry.transform.Find("Unequip");
            Image background = entry.GetComponentInChildren<Image>();

            tmpText.fontStyle = FontStyles.Normal;
            unequip.gameObject.SetActive(false);

            background.color = new Color32(221, 147, 39, 20);

        }
        */

    }

    // Helper method to get world space bounds of a RectTransform
    private Bounds GetWorldSpaceBounds(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Bounds bounds = new Bounds(corners[0], Vector3.zero);
        for (int i = 1; i < 4; i++)
        {
            bounds.Encapsulate(corners[i]);
        }

        return bounds;
    }

    private IEnumerator SmoothScrollTo(float targetScrollPosition)
    {
        float startPosition = _scrollRect.verticalNormalizedPosition;
        float time = 0;
        float duration = 0.2f; // Adjust time as needed

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            // Ease in-out for smoother feel
            t = t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;

            _scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPosition, targetScrollPosition, t);
            yield return null;
        }

        _scrollRect.verticalNormalizedPosition = targetScrollPosition;
        _scrollCoroutine = null;
    }

    #endregion


    #region ******** Skills Selection UI
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

    private void UpdateSkillCooldownOverlay(GameObject slot, float remainingCooldown, float cooldownDuration)
    {
        Image overlayImage = slot.transform.Find("OverlayImage")?.GetComponent<Image>();
        if (overlayImage != null)
        {
            overlayImage.fillAmount = Mathf.Clamp01(remainingCooldown / cooldownDuration);
        }
    }

    public GameObject GetSkillSlot(int index)
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

    private void DeactivateSkillsUI()
    {
        grid.gameObject.SetActive(false);
    }

    #endregion



    #region ****** PanelReferences Getters
    private void GetInventoryDescriptPanelReferences()
    {
        Transform icon = descriptionPanel_inventory.transform.Find("itemIcon");
        Transform text = descriptionPanel_inventory.transform.Find("descriptText");

        inventoryDescript_icon = icon.gameObject.GetComponent<Image>();
        inventoryDescript_txt = text.gameObject.GetComponent<TextMeshProUGUI>();


    }
    private void GetEquipPanelReferences()
    {
        Transform icon = descriptionPanel_equip.transform.Find("itemIcon");
        Transform text = descriptionPanel_equip.transform.Find("descriptText");

        equipDescript_icon = icon.gameObject.GetComponent<Image>();
        equipDescript_txt = text.gameObject.GetComponent<TextMeshProUGUI>();

        // Ring
        ring_descript_default = ring_EPanel.transform.Find("descript_default");
        ring_descript_equipped = ring_EPanel.transform.Find("descript_equipped");
        ring_icon_default = ring_EPanel.transform.Find("icon_default");
        ring_icon_equipped = ring_EPanel.transform.Find("icon_equipped");
        ring_selectAmber = ring_EPanel.transform.Find("select_amber");
        ring_selectGreen = ring_EPanel.transform.Find("select_green");

        // Helm
        helm_descript_default = helm_EPanel.transform.Find("descript_default");
        helm_descript_equipped = helm_EPanel.transform.Find("descript_equipped");
        helm_icon_default = helm_EPanel.transform.Find("icon_default");
        helm_icon_equipped = helm_EPanel.transform.Find("icon_equipped");
        helm_selectAmber = helm_EPanel.transform.Find("select_amber");
        helm_selectGreen = helm_EPanel.transform.Find("select_green");

        // Amulet
        amulet_descript_default = amulet_EPanel.transform.Find("descript_default");
        amulet_descript_equipped = amulet_EPanel.transform.Find("descript_equipped");
        amulet_icon_default = amulet_EPanel.transform.Find("icon_default");
        amulet_icon_equipped = amulet_EPanel.transform.Find("icon_equipped");
        amulet_selectAmber = amulet_EPanel.transform.Find("select_amber");
        amulet_selectGreen = amulet_EPanel.transform.Find("select_green");

        // Melee
        melee_descript_default = melee_EPanel.transform.Find("descript_default");
        melee_descript_equipped = melee_EPanel.transform.Find("descript_equipped");
        melee_icon_default = melee_EPanel.transform.Find("icon_default");
        melee_icon_equipped = melee_EPanel.transform.Find("icon_equipped");
        melee_selectAmber = melee_EPanel.transform.Find("select_amber");
        melee_selectGreen = melee_EPanel.transform.Find("select_green");

        // Armor
        armor_descript_default = armor_EPanel.transform.Find("descript_default");
        armor_descript_equipped = armor_EPanel.transform.Find("descript_equipped");
        armor_icon_default = armor_EPanel.transform.Find("icon_default");
        armor_icon_equipped = armor_EPanel.transform.Find("icon_equipped");
        armor_selectAmber = armor_EPanel.transform.Find("select_amber");
        armor_selectGreen = armor_EPanel.transform.Find("select_green");

        // Ranged
        ranged_descript_default = ranged_EPanel.transform.Find("descript_default");
        ranged_descript_equipped = ranged_EPanel.transform.Find("descript_equipped");
        ranged_icon_default = ranged_EPanel.transform.Find("icon_default");
        ranged_icon_equipped = ranged_EPanel.transform.Find("icon_equipped");
        ranged_selectAmber = ranged_EPanel.transform.Find("select_amber");
        ranged_selectGreen = ranged_EPanel.transform.Find("select_green");

        // Shield
        shield_descript_default = shield_EPanel.transform.Find("descript_default");
        shield_descript_equipped = shield_EPanel.transform.Find("descript_equipped");
        shield_icon_default = shield_EPanel.transform.Find("icon_default");
        shield_icon_equipped = shield_EPanel.transform.Find("icon_equipped");
        shield_selectAmber = shield_EPanel.transform.Find("select_amber");
        shield_selectGreen = shield_EPanel.transform.Find("select_green");

        // Boots
        boots_descript_default = boots_EPanel.transform.Find("descript_default");
        boots_descript_equipped = boots_EPanel.transform.Find("descript_equipped");
        boots_icon_default = boots_EPanel.transform.Find("icon_default");
        boots_icon_equipped = boots_EPanel.transform.Find("icon_equipped");
        boots_selectAmber = boots_EPanel.transform.Find("select_amber");
        boots_selectGreen = boots_EPanel.transform.Find("select_green");

        // Missile
        missile_descript_default = missile_EPanel.transform.Find("descript_default");
        missile_descript_equipped = missile_EPanel.transform.Find("descript_equipped");
        missile_icon_default = missile_EPanel.transform.Find("icon_default");
        missile_icon_equipped = missile_EPanel.transform.Find("icon_equipped");
        missile_selectAmber = missile_EPanel.transform.Find("select_amber");
        missile_selectGreen = missile_EPanel.transform.Find("select_green");


        // Ring
        ring_text_default = ring_descript_default.GetComponent<TextMeshProUGUI>();
        ring_text_equipped = ring_descript_equipped.GetComponent<TextMeshProUGUI>();
        ring_iconImg_default = ring_icon_default.GetComponent<Image>();
        ring_iconImg_equipped = ring_icon_equipped.GetComponent<Image>();
        ring_selectAmberImg = ring_selectAmber.GetComponent<Image>();
        ring_selectGreenImg = ring_selectGreen.GetComponent<Image>();

        // Helm
        helm_text_default = helm_descript_default.GetComponent<TextMeshProUGUI>();
        helm_text_equipped = helm_descript_equipped.GetComponent<TextMeshProUGUI>();
        helm_iconImg_default = helm_icon_default.GetComponent<Image>();
        helm_iconImg_equipped = helm_icon_equipped.GetComponent<Image>();
        helm_selectAmberImg = helm_EPanel.transform.Find("select_amber").GetComponent<Image>();
        helm_selectGreenImg = helm_EPanel.transform.Find("select_green").GetComponent<Image>();

        // Amulet
        amulet_text_default = amulet_descript_default.GetComponent<TextMeshProUGUI>();
        amulet_text_equipped = amulet_descript_equipped.GetComponent<TextMeshProUGUI>();
        amulet_iconImg_default = amulet_icon_default.GetComponent<Image>();
        amulet_iconImg_equipped = amulet_icon_equipped.GetComponent<Image>();
        amulet_selectAmberImg = amulet_EPanel.transform.Find("select_amber").GetComponent<Image>();
        amulet_selectGreenImg = amulet_EPanel.transform.Find("select_green").GetComponent<Image>();

        // Melee
        melee_text_default = melee_descript_default.GetComponent<TextMeshProUGUI>();
        melee_text_equipped = melee_descript_equipped.GetComponent<TextMeshProUGUI>();
        melee_iconImg_default = melee_icon_default.GetComponent<Image>();
        melee_iconImg_equipped = melee_icon_equipped.GetComponent<Image>();
        melee_selectAmberImg = melee_EPanel.transform.Find("select_amber").GetComponent<Image>();
        melee_selectGreenImg = melee_EPanel.transform.Find("select_green").GetComponent<Image>();

        // Armor
        armor_text_default = armor_descript_default.GetComponent<TextMeshProUGUI>();
        armor_text_equipped = armor_descript_equipped.GetComponent<TextMeshProUGUI>();
        armor_iconImg_default = armor_icon_default.GetComponent<Image>();
        armor_iconImg_equipped = armor_icon_equipped.GetComponent<Image>();
        armor_selectAmberImg = armor_EPanel.transform.Find("select_amber").GetComponent<Image>();
        armor_selectGreenImg = armor_EPanel.transform.Find("select_green").GetComponent<Image>();

        // Ranged
        ranged_text_default = ranged_descript_default.GetComponent<TextMeshProUGUI>();
        ranged_text_equipped = ranged_descript_equipped.GetComponent<TextMeshProUGUI>();
        ranged_iconImg_default = ranged_icon_default.GetComponent<Image>();
        ranged_iconImg_equipped = ranged_icon_equipped.GetComponent<Image>();
        ranged_selectAmberImg = ranged_EPanel.transform.Find("select_amber").GetComponent<Image>();
        ranged_selectGreenImg = ranged_EPanel.transform.Find("select_green").GetComponent<Image>();

        // Shield
        shield_text_default = shield_descript_default.GetComponent<TextMeshProUGUI>();
        shield_text_equipped = shield_descript_equipped.GetComponent<TextMeshProUGUI>();
        shield_iconImg_default = shield_icon_default.GetComponent<Image>();
        shield_iconImg_equipped = shield_icon_equipped.GetComponent<Image>();
        shield_selectAmberImg = shield_EPanel.transform.Find("select_amber").GetComponent<Image>();
        shield_selectGreenImg = shield_EPanel.transform.Find("select_green").GetComponent<Image>();

        // Boots
        boots_text_default = boots_descript_default.GetComponent<TextMeshProUGUI>();
        boots_text_equipped = boots_descript_equipped.GetComponent<TextMeshProUGUI>();
        boots_iconImg_default = boots_icon_default.GetComponent<Image>();
        boots_iconImg_equipped = boots_icon_equipped.GetComponent<Image>();
        boots_selectAmberImg = boots_EPanel.transform.Find("select_amber").GetComponent<Image>();
        boots_selectGreenImg = boots_EPanel.transform.Find("select_green").GetComponent<Image>();

        // Missile
        missile_text_default = missile_descript_default.GetComponent<TextMeshProUGUI>();
        missile_text_equipped = missile_descript_equipped.GetComponent<TextMeshProUGUI>();
        missile_iconImg_default = missile_icon_default.GetComponent<Image>();
        missile_iconImg_equipped = missile_icon_equipped.GetComponent<Image>();
        missile_selectAmberImg = missile_EPanel.transform.Find("select_amber").GetComponent<Image>();
        missile_selectGreenImg = missile_EPanel.transform.Find("select_green").GetComponent<Image>();



    }
    #endregion
}


