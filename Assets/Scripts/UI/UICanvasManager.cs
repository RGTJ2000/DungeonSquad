using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UICanvasManager : MonoBehaviour
{

    private SquadManager _squadManager;
    private GameObject[] slots_array;
    private GameObject current_character;
    [SerializeField] private Transform grid;
    [SerializeField] private Transform meleePanel;
    [SerializeField] private Transform rangedPanel;
    [SerializeField] private Transform missilePanel;
    [SerializeField] private GameObject itemEntry_prefab;


    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject inventoryDescriptPanel;

    private ScrollRect _scrollRect;
    private RectTransform _srContent;
    private RectTransform _srViewport;

    private bool inventoryActive = false;

    List<GameObject> inventoryEntries = new List<GameObject>();
    List<RuntimeItem> inventoryItems = new List<RuntimeItem>();
    private int inventoryIndex = 0;

    private Coroutine _scrollCoroutine;

    [SerializeField] private GameObject equipPanel;
    [SerializeField] private GameObject bodyPanel;
    [SerializeField] private GameObject descriptionPanel_equip;
    [SerializeField] private GameObject descriptionPanel_inventory;

    Image inventoryDescript_icon;
    TextMeshProUGUI inventoryDescript_txt;

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

    [SerializeField] private ItemCategory currentCatSelect = ItemCategory.none;



    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();

        GetInventoryPanelReferences();
        GetEquipPanelReferences();
    }
    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.UI_navigate.performed += OnUINavigate;

        SquadManager.OnCharacterSelected += HandleSkillsUI;
        SquadManager.OnCharacterSelected += UpdateInventoryPanelToCharacter;

        SquadManager.OnInventorySelected += HandleInventoryUI;

        ItemEvents.OnItemPickedUp += RefreshInventoryUI;
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Disable();
        _playerInputActions.Player.UI_navigate.performed -= OnUINavigate;

        SquadManager.OnCharacterSelected -= HandleSkillsUI;
        SquadManager.OnCharacterSelected -= UpdateInventoryPanelToCharacter;
        SquadManager.OnInventorySelected -= HandleInventoryUI;

        ItemEvents.OnItemPickedUp -= RefreshInventoryUI;
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

    private void UpdateInventoryPanelToCharacter(GameObject ch_obj)
    {
       if (inventoryActive)
        {
            if (inventoryEntries.Count > 0)
            {
                SelectInventoryItem(inventoryEntries[inventoryIndex]);


            }

            if (ch_obj != null)
            {
                PopulateEquipPanel(ch_obj);
                equipPanel.gameObject.SetActive(true);
            }
            else
            {
                equipPanel.gameObject.SetActive(false);
            }

        }

    }


    private void HandleInventoryUI(GameObject ch_obj)
    {
        current_character = ch_obj;

        if (!inventoryActive)
        {
            ClearInventoryUI();
            PopulateInventoryPanel();

            if (ch_obj != null)
            {
                PopulateEquipPanel(ch_obj);
            }
            
            ActivateInventoryUI(ch_obj);
            //set scrollview to top
            _scrollRect.verticalNormalizedPosition = 1;
            inventoryActive = true;

        }
        else
        {
            DeactivateInventoryUI();
            inventoryActive = false;
        }

    }

    public void RefreshInventoryUI()
    {
        if (inventoryActive)
        {
            ClearInventoryUI();
            PopulateInventoryPanel();

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

    private void OnUINavigate(InputAction.CallbackContext context)
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
                    BumpInventorySelectIndex(-1);
                }
                else if (input.y < 0f)
                {
                    BumpInventorySelectIndex(1);
                }
                else if (input.x > 0f)
                {
                    DropItem(inventoryIndex);
                }
                else if(input.x <0f)
                {
                    if (current_character !=null && inventoryItems[inventoryIndex].IsEquippable)
                    {
                        EquipItem(inventoryIndex);
                    }
                }


            }

        }
       

    }

    private void EquipItem(int index)
    {
        /*
        RuntimeItem addedItem = InventoryManager.Instance.EquipItemToCharacter(inventoryItems[index], current_character);

        //destroy inventoryEntry object
        Destroy(inventoryEntries[index]);
        //remove inventoryEntry from list
        inventoryEntries.RemoveAt(index);
        //remove inventoryItem from list
        inventoryItems.RemoveAt(index);

        if (addedItem != null)
        {
            Transform catPanel;

            switch (addedItem.category)
            {
                case ItemCategory.melee_weapon:
                    catPanel = meleePanel;
                    break;
                case ItemCategory.ranged_weapon:
                    catPanel = rangedPanel;
                    break;
                case ItemCategory.missile:
                    catPanel = missilePanel;
                    break;
                default:
                    catPanel = null;
                    break;
            
            }

            GameObject newObj = Instantiate(itemEntry_prefab, catPanel);

            Transform itemName = newObj.transform.Find("ItemName");
            TextMeshProUGUI nameText = itemName.GetComponent<TextMeshProUGUI>();

            nameText.text = addedItem.item_name;

            inventoryEntries.Add(newObj);
            inventoryItems.Add(addedItem);

            //add item to lists
        }

        BumpInventorySelectIndex(-1);
        PopulateEquipPanel(current_character);
        */

        // this code works
        InventoryManager.Instance.EquipItemToCharacter(inventoryItems[index], current_character);
        RefreshInventoryUI();
        MoveToInventoryIndex(index);
        UpdateInventoryDescriptPanel();
        PopulateEquipPanel(current_character);
        

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

        

        //set inventory index to new location
        inventoryIndex = Mathf.Clamp(index-1, 0, inventoryEntries.Count-1);
        if (inventoryEntries.Count > 0)
        {
            SelectInventoryItem(inventoryEntries[inventoryIndex]);
            ScrollToItem(inventoryEntries[inventoryIndex]);

        }
        
        
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
                DeselectInventoryItem(inventoryEntries[inventoryIndex]);
                //select new index
                inventoryIndex = newIndex;
                SelectInventoryItem(inventoryEntries[inventoryIndex]);



                UpdateCategorySelection(inventoryItems[inventoryIndex].baseItem.category);
                //currentCatSelect = (inventoryItems[inventoryIndex].baseItem.category);
                ScrollToItem(inventoryEntries[inventoryIndex]);
            }

        }
        
    }

    private void MoveToInventoryIndex(int index)
    {
        if (inventoryEntries.Count == 0) { return; }
        else
        {
            index = Mathf.Clamp(index, 0, inventoryEntries.Count - 1);

            DeselectInventoryItem(inventoryEntries[inventoryIndex]);
            SelectInventoryItem(inventoryEntries[index]);
            inventoryIndex = index;
            Debug.Log("Moving. Index=" + index + " inventoryIndex=" + inventoryIndex);
            UpdateCategorySelection(inventoryItems[inventoryIndex].baseItem.category);
            JumpToItem(inventoryEntries[inventoryIndex]);
        }
        
    }


    private void UpdateCategorySelection(ItemCategory newCategory)
    {
        if (current_character != null && currentCatSelect != newCategory)
        {

            SetAmberSelectState(currentCatSelect, false);
            SetAmberSelectState(newCategory, true);

        }

        currentCatSelect = newCategory;

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



    private void PopulateInventoryPanel()
    {

        //ClearInventoryPanel();
        foreach (var meleeItem in InventoryManager.Instance.GetCoreMeleeWeaponsList())
        {
            AddItemToCategoryPanel(meleeItem, meleePanel); // prefab instantiation + text set
        }

        foreach (var rangedItem in InventoryManager.Instance.GetCoreRangedWeaponsList())
        {
            AddItemToCategoryPanel(rangedItem, rangedPanel);
        }


        foreach (var missiles in InventoryManager.Instance.GetCoreMissilesList())
        {
            AddItemToCategoryPanel(missiles, missilePanel);

        }

    }

    public void ClearInventoryUI()
    {
        ClearPanel(meleePanel);
        ClearPanel(rangedPanel);
        ClearPanel(missilePanel);
        //clear main lists
        inventoryEntries.Clear();
        inventoryItems.Clear();
        //reset index
        inventoryIndex = 0;
        //reset scroll view
        _scrollRect.verticalNormalizedPosition = 1;

        //reset currentCat
        currentCatSelect = ItemCategory.none;

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
        GameObject newObj = Instantiate(itemEntry_prefab, categoryPanel);

        Transform itemName = newObj.transform.Find("ItemName");
        TextMeshProUGUI nameText = itemName.GetComponent<TextMeshProUGUI>();

        nameText.text = item.item_name;

        inventoryEntries.Add(newObj);
        inventoryItems.Add(item);

        //activate/deactivate selection options
        if (inventoryEntries.Count == inventoryIndex+1) //select the current index
        {
            SelectInventoryItem(newObj);

            
            currentCatSelect = item.category; //initialize the currentCatSelect
            //ScrollToItem(newObj);
        }
        else if (inventoryEntries.Count > 1)
        {
            DeselectInventoryItem(newObj);

        }


    }

    private void DeselectInventoryItem(GameObject entry)
    {
        Transform itemName = entry.transform.Find("ItemName");
        TextMeshProUGUI nameText = itemName.GetComponent<TextMeshProUGUI>();

        Transform equipText = entry.transform.Find("Equip");
        Transform dropText = entry.transform.Find("Drop");
        Image background = entry.GetComponentInChildren<Image>();

        nameText.fontStyle = FontStyles.Normal;
        equipText.gameObject.SetActive(false);
        dropText.gameObject.SetActive(false);
        background.color = new Color32(152, 152, 152, 22);
    }

    private void SelectInventoryItem(GameObject entry)
    {
        Transform itemName = entry.transform.Find("ItemName");
        TextMeshProUGUI nameText = itemName.GetComponent<TextMeshProUGUI>();


        Transform equipText = entry.transform.Find("Equip");
        Transform dropText = entry.transform.Find("Drop");
        Image background = entry.GetComponentInChildren<Image>();

        nameText.fontStyle = FontStyles.Bold;
        if (!inventoryItems[inventoryIndex].IsEquippable ||  _squadManager.select_active == -1 || (_squadManager.select_active != -1 && _squadManager.ch_in_slot_array[_squadManager.select_active] == null))
        {
            equipText.gameObject.SetActive(false);
        }
        else
        {
            equipText.gameObject.SetActive(true);
        }
        dropText.gameObject.SetActive(true);
        background.color = new Color32(221, 147, 39, 20);

        UpdateInventoryDescriptPanel();

    }

    private void JumpToItem(GameObject entry)
    {
        RectTransform entryRect = entry.GetComponent<RectTransform>();

        // Calculate the world space bounds
        Bounds entryBounds = GetWorldSpaceBounds(entryRect);
        Bounds viewportBounds = GetWorldSpaceBounds(_srViewport);

        // Find the index of this entry in our list
        int entryIndex = inventoryEntries.IndexOf(entry);

        // Special handling for top items (first 2 items)
        if (entryIndex <= 2)
        {
            if (_scrollCoroutine != null)
                StopCoroutine(_scrollCoroutine);

            _scrollRect.verticalNormalizedPosition = 1.0f; // Top of the scroll view
            return;
        }

        // Calculate target position
        Vector2 entryPosition = entryRect.anchoredPosition;
        float itemHeight = entryRect.rect.height;
        float contentHeight = _srContent.rect.height;
        float viewportHeight = _srViewport.rect.height;

        float targetPosition;

        if (entryBounds.min.y > viewportBounds.max.y)
        {
            // Item is above viewport
            targetPosition = -entryPosition.y;
        }
        else if (entryBounds.max.y < viewportBounds.min.y)
        {
            // Item is below viewport
            targetPosition = -entryPosition.y - itemHeight + viewportHeight;
        }
        else if (entryBounds.min.y < viewportBounds.min.y)
        {
            // Bottom of item is outside viewport
            targetPosition = -entryPosition.y - itemHeight + viewportHeight;
        }
        else
        {
            // Top of item is outside viewport
            targetPosition = -entryPosition.y;
        }

        // Convert to normalized scroll position
        float targetScrollPosition = 1 - (targetPosition / (contentHeight - viewportHeight));
        targetScrollPosition = Mathf.Clamp01(targetScrollPosition);

        // Instantly jump to the position
        if (_scrollCoroutine != null)
            StopCoroutine(_scrollCoroutine);

        _scrollRect.verticalNormalizedPosition = targetScrollPosition;
    }

    
    
    private void ScrollToItem(GameObject entry)
    {
        RectTransform entryRect = entry.GetComponent<RectTransform>();

        // Calculate the world space bounds
        Bounds entryBounds = GetWorldSpaceBounds(entryRect);
        Bounds viewportBounds = GetWorldSpaceBounds(_srViewport);

        // Find the index of this entry in our list
        int entryIndex = inventoryEntries.IndexOf(entry);

        // Special handling for top items (first 2 items)
        if (entryIndex <= 2)
        {
            // For the first few items, just scroll all the way to the top
            if (_scrollCoroutine != null)
                StopCoroutine(_scrollCoroutine);

            _scrollCoroutine = StartCoroutine(SmoothScrollTo(1.0f)); // 1.0 = top position
            return;
        }

        // Standard visibility check
        if (viewportBounds.Contains(entryBounds.min) && viewportBounds.Contains(entryBounds.max))
        {
            // Item is fully visible, no need to scroll
            return;
        }

        /*
        // Calculate target position for other items
        Vector2 entryPosition = entryRect.anchoredPosition;
        float itemHeight = entryRect.rect.height;
        float contentHeight = _srContent.rect.height;
        float viewportHeight = _srViewport.rect.height;

        

        float targetPosition;

        // Determine target scroll position based on item visibility
        if (entryBounds.min.y > viewportBounds.max.y)
        {
            // Item is above viewport
            targetPosition = -entryPosition.y;
        }
        else if (entryBounds.max.y < viewportBounds.min.y)
        {
            // Item is below viewport
            targetPosition = -entryPosition.y - itemHeight + viewportHeight;
        }
        else if (entryBounds.min.y < viewportBounds.min.y)
        {
            // Bottom of item is outside viewport
            targetPosition = -entryPosition.y - itemHeight + viewportHeight;
        }
        else
        {
            // Top of item is outside viewport
            targetPosition = -entryPosition.y;
        }

        */
        // Get item's position in content's local space
        float localY = _srContent.InverseTransformPoint(entryRect.position).y;
        Debug.Log("localY=" + localY);
        float itemHeight = entryRect.rect.height;
        float contentHeight = _srContent.rect.height;
        float viewportHeight = _srViewport.rect.height;

        // Flip Y because UI scroll direction is inverted
        float flippedY = - localY;
        Debug.Log("flippedY=" + flippedY);

        float targetPosition;

        // Determine target scroll position
        if (entryBounds.min.y > viewportBounds.max.y) // above
        {
            targetPosition = flippedY - itemHeight;
        }
        else if (entryBounds.max.y < viewportBounds.min.y) // below
        {
            targetPosition = flippedY +itemHeight - viewportHeight;
        }
        else if (entryBounds.min.y < viewportBounds.min.y) // bottom clipped
        {
            targetPosition = flippedY +itemHeight - viewportHeight;
        }
        else // top clipped
        {
            targetPosition = flippedY - itemHeight;
        }

        // Convert to normalized position
        float targetScrollPosition = 1 - (targetPosition / (contentHeight - viewportHeight));
        targetScrollPosition = Mathf.Clamp01(targetScrollPosition);

        Debug.Log($"tpos={targetPosition}  tscrolpos={targetScrollPosition}  contentheight={contentHeight}  viewportheight={viewportHeight}");
        // Start smooth scrolling
        if (_scrollCoroutine != null)
            StopCoroutine(_scrollCoroutine);

        _scrollCoroutine = StartCoroutine(SmoothScrollTo(targetScrollPosition));
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

    private void DeactivateSkillsUI()
    {
        grid.gameObject.SetActive(false);
    }



    private void PopulateEquipPanel(GameObject ch_obj)
    {
        if (ch_obj != null)
        {
            EntityStats entityStats = ch_obj.GetComponent<EntityStats>();
           

            if (entityStats.equipped_ring != null)
            {
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


            if (entityStats.equipped_missile  != null)
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
    private void ActivateInventoryUI(GameObject ch_obj)
    {
        inventoryPanel.SetActive(true);

        if (inventoryEntries.Count > 0)
        {
            UpdateInventoryDescriptPanel();
            inventoryDescriptPanel.SetActive(true);
        }
        else 
        {
            inventoryDescriptPanel.SetActive(false);
        }

        if (ch_obj != null)
        {
            equipPanel.SetActive(true);
        }
    }

    private void UpdateInventoryDescriptPanel()
    {
        inventoryDescript_icon.sprite = inventoryItems[inventoryIndex].Icon;
        Debug.Log("baseitem sprite="+ inventoryDescript_icon.sprite.name);
        inventoryDescript_txt.text = inventoryItems[inventoryIndex].baseItem.description;

    }

    private void DeactivateInventoryUI()
    {
        inventoryPanel.SetActive(false);
        equipPanel.SetActive(false);
    }

    private void GetInventoryPanelReferences()
    {
        Transform icon = descriptionPanel_inventory.transform.Find("itemIcon");
        Transform text = descriptionPanel_inventory.transform.Find("descriptText");

        inventoryDescript_icon = icon.gameObject.GetComponent<Image>();
        inventoryDescript_txt = text.gameObject.GetComponent<TextMeshProUGUI>();


    }
    private void GetEquipPanelReferences()
    {
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

}


