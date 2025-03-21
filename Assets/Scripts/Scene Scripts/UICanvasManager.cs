using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class UICanvasManager : MonoBehaviour
{

    private GameObject[] slots;
    private GameObject current_character;
    [SerializeField] private Transform grid;


    private void OnEnable()
    {
        SquadManager.OnCharacterSelected += HandleSkillsUI;

    }

    private void OnDisable()
    {
        SquadManager.OnCharacterSelected -= HandleSkillsUI;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        slots = new GameObject[grid.childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = grid.GetChild(i).gameObject;
            //Debug.Log("Slot " + i + " found: " + slots[i].name);

            slots[i].SetActive(false); //deactivate the slots by default

        }



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
                    for (int i = 0; i < slots.Length; i++)
                    {
                        if (i < _entityStats.skill_slot.Length && _entityStats.skill_slot[i] != null)
                        {
                            Skill_SO skill = _entityStats.skill_slot[i];
                            if (skill.cooldown != 0f)
                            {
                                float remainingCooldown = cooldownTracker.GetRemainingCooldown(skill);
                                UpdateOverlay(slots[i], remainingCooldown, skill.cooldown);
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
        if (index >= 0 && index < slots.Length)
        {
            return slots[index];
        }
        else
        {
            Debug.LogWarning("Slot index out of range: " + index);
            return null;
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

        for (int i = 0; i < slots.Length; i++)
        {
            
            Image _image = slots[i].GetComponent<Image>();


            Transform slot_text = slots[i].transform.Find("SkillText");
            TextMeshProUGUI _textComponent = slot_text.gameObject.GetComponent<TextMeshProUGUI>();

            Image overlayImage = slots[i].transform.Find("OverlayImage")?.GetComponent<Image>();


            if (i < ch_slottedSkillsLength && _entityStats.skill_slot[i] != null)
            {
                slots[i].SetActive(true);
                _image.sprite = _entityStats.skill_slot[i].skill_icon;

                if (_entityStats.selected_skill.skill_name == _entityStats.skill_slot[i].skill_name)

                {
                    //activate slot text
                    slot_text.gameObject.SetActive(true);

                    //set text to the skillname in the skill slot
                    _textComponent.text = _entityStats.skill_slot[i].skill_name;

                    slots[i].transform.localScale = new Vector3(1.2f, 1.2f, 0f);
                    //overlayImage.transform.localScale = new Vector3(1.2f, 1.2f, 0f);

                    Color color = _image.color;
                    color.a = 1.0f;
                    _image.color = color;
                }
                else
                {
                    slot_text.gameObject.SetActive(false);

                    slots[i].transform.localScale = new Vector3(1.0f, 1.0f, 0f);
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
                    Debug.Log($"Slot {i}: remaining cooldown={remainingCooldown} | cooldown total: {skill.cooldown}");
                    if (skill.cooldown == 0.0f)
                    {
                        Debug.Log($"{ch_obj.name}: Setting slot {i} to overlayImage.fillAmount =0f");
                        overlayImage.fillAmount = 0.0f;
                        Debug.Log($"Post-set: fillAmount = {overlayImage.fillAmount}");
                    } else
                    {
                        Debug.Log($"{ch_obj.name}: Setting slot {i} to fraction {remainingCooldown}/{skill.cooldown}");
                        overlayImage.fillAmount = Mathf.Clamp01(remainingCooldown / skill.cooldown);
                    }

                }
            }
            else
            {
                slots[i].SetActive(false); //deactivate the image slot
            }

                      
           




        }
    }

    private void DeactivateSkillsUI()
    {
        grid.gameObject.SetActive(false);
    }


}
