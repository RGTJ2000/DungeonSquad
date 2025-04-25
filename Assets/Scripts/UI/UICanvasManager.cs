using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class UICanvasManager : MonoBehaviour
{


    private GameObject[] slots_array;
    private GameObject current_character;
    [SerializeField] private Transform grid;
    [SerializeField] private Transform meleePanel;
    [SerializeField] private Transform rangedPanel;
    [SerializeField] private Transform missilePanel;
    [SerializeField] private GameObject itemEntry_prefab;

    private Inventory _coreInventory;

    [SerializeField] private GameObject inventoryPanel;
    private ScrollRect _scrollRect;
    private RectTransform _srContent;
    private RectTransform _srViewport;

    private bool inventoryActive = false;

    List<GameObject> inventoryEntries = new List<GameObject>();
    private int inventoryIndex = 0;

    private Coroutine _scrollCoroutine;


    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();

    }
    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.UI_navigate.performed += OnUINavigate;

        SquadManager.OnCharacterSelected += HandleSkillsUI;
        SquadManager.OnInventorySelected += HandleInventoryUI;

    }

    private void OnDisable()
    {
        _playerInputActions.Player.Disable();
        _playerInputActions.Player.UI_navigate.performed -= OnUINavigate;

        SquadManager.OnCharacterSelected -= HandleSkillsUI;
        SquadManager.OnInventorySelected -= HandleInventoryUI;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //inventory panel
        _scrollRect = inventoryPanel.GetComponentInChildren<ScrollRect>();
        if (_scrollRect != null)
        {
            Debug.Log("Scroll Rect FOUND!");
        }
        else
        { Debug.Log("SCROLL RECT = NULL"); }

        _srContent = _scrollRect.content;
        _srViewport = _scrollRect.viewport;

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
        if (inventoryEntries.Count > 0)
        {
            Vector2 input = context.ReadValue<Vector2>();

            if (input.y > 0f)
            {
                MoveInventorySelectIndex(-1);
            }
            else if (input.y < 0f)
            {
                MoveInventorySelectIndex(1);
            }


        }

    }

    private void MoveInventorySelectIndex(int indexChange)
    {
        int newIndex = Mathf.Clamp(inventoryIndex + indexChange, 0, inventoryEntries.Count - 1);

        if (newIndex != inventoryIndex)
        {
            //deselect inventory index
            DeselectInventoryItem(inventoryEntries[inventoryIndex]);
            //select new index
            SelectInventoryItem(inventoryEntries[newIndex]);

            inventoryIndex = newIndex;

            ScrollToItem(inventoryEntries[inventoryIndex]);
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
        //clear main list
        inventoryEntries.Clear();
        //reset index
        inventoryIndex = 0;
        //reset scroll view
        _scrollRect.verticalNormalizedPosition = 1;

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

        //activate/deactivate selection options
        if (inventoryEntries.Count == 1)
        {
            SelectInventoryItem(newObj);
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
        equipText.gameObject.SetActive(true);
        dropText.gameObject.SetActive(true);
        background.color = new Color32(221, 147, 39, 20);
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

        // Convert to normalized position
        float targetScrollPosition = 1 - (targetPosition / (contentHeight - viewportHeight));
        targetScrollPosition = Mathf.Clamp01(targetScrollPosition);

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

    private void ActivateInventoryUI(GameObject ch_obj)
    {
        inventoryPanel.SetActive(true);
    }

    private void DeactivateInventoryUI()
    {
        inventoryPanel.SetActive(false);
    }

}
