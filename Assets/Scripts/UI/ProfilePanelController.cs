using System.Xml.Serialization;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanelController : MonoBehaviour
{
    [SerializeField] private Image characterPortrait;
    [SerializeField] private TextMeshProUGUI characterName_TMP;
    [SerializeField] private TextMeshProUGUI characterHPText_TMP;

    [SerializeField] private TextMeshProUGUI str_base_TMP;
    [SerializeField] private TextMeshProUGUI dex_base_TMP;
    [SerializeField] private TextMeshProUGUI int_base_TMP;
    [SerializeField] private TextMeshProUGUI will_base_TMP;
    [SerializeField] private TextMeshProUGUI soul_base_TMP;

    [SerializeField] private TextMeshProUGUI str_adj_TMP;
    [SerializeField] private TextMeshProUGUI dex_adj_TMP;
    [SerializeField] private TextMeshProUGUI int_adj_TMP;
    [SerializeField] private TextMeshProUGUI will_adj_TMP;
    [SerializeField] private TextMeshProUGUI soul_adj_TMP;

    [SerializeField] private TextMeshProUGUI str_readj_TMP;
    [SerializeField] private TextMeshProUGUI dex_readj_TMP;
    [SerializeField] private TextMeshProUGUI int_readj_TMP;
    [SerializeField] private TextMeshProUGUI will_readj_TMP;
    [SerializeField] private TextMeshProUGUI soul_readj_TMP;

    [SerializeField] private Slider str_adj_slider;
    [SerializeField] private Slider dex_adj_slider;
    [SerializeField] private Slider int_adj_slider;
    [SerializeField] private Slider will_adj_slider;
    [SerializeField] private Slider soul_adj_slider;

    [SerializeField] private Slider str_readj_slider;
    [SerializeField] private Slider dex_readj_slider;
    [SerializeField] private Slider int_readj_slider;
    [SerializeField] private Slider will_readj_slider;
    [SerializeField] private Slider soul_readj_slider;

    [SerializeField] private Slider str_marker_slider;
    [SerializeField] private Slider dex_marker_slider;
    [SerializeField] private Slider int_marker_slider;
    [SerializeField] private Slider will_marker_slider;
    [SerializeField] private Slider soul_marker_slider;

    [SerializeField] private RectTransform str_marker;
    [SerializeField] private RectTransform dex_marker;
    [SerializeField] private RectTransform int_marker;
    [SerializeField] private RectTransform will_marker;
    [SerializeField] private RectTransform soul_marker;

    private EntityStats _chStats;


    private void FixedUpdate()
    {
        UpdateHitPoints();
    }

    public void UpdatePanelToCharacter(GameObject ch_obj)
    {
        Debug.Log("Updating PanelToCharacter.");

        _chStats = ch_obj.GetComponent<EntityStats>();

        UpdatePortraitPanel();
        UpdateStatsPanel_1();
    }


    private void UpdatePortraitPanel()
    {
        if (_chStats != null)
        {
            characterPortrait.sprite = _chStats.characterPortrait;
           
            characterName_TMP.text = _chStats.characterName;
            UpdateHitPoints();
        }
    }

    private void UpdateHitPoints()
    {
        if (_chStats != null)
        {
            float current = _chStats.health_current;
            float max = _chStats.health_max;

            characterHPText_TMP.text =
                $"<color=#DD9327>{((int)current)}</color><color=#1EBE05>/{((int)max)}</color>";

        }
        

    }

    public void UpdateStatsPanel_1()
    {
        if (_chStats !=null)
        {
            str_base_TMP.text = $"{(int)_chStats.strength}";
            dex_base_TMP.text = $"{(int)_chStats.dexterity}";
            int_base_TMP.text = $"{(int)_chStats.intelligence}";
            will_base_TMP.text = $"{(int)_chStats.will}";
            soul_base_TMP.text = $"{(int)_chStats.soul}";

            str_adj_TMP.text = $"{(int)_chStats.str_adjusted}";
            dex_adj_TMP.text = $"{(int)_chStats.dex_adjusted}";
            int_adj_TMP.text = $"{(int)_chStats.int_adjusted}";
            will_adj_TMP.text = $"{(int)_chStats.will_adjusted}";
            soul_adj_TMP.text = $"{(int)_chStats.soul_adjusted}";

            UpdateAdjustedSlider(str_adj_slider, _chStats.str_adjusted);
            UpdateAdjustedSlider(dex_adj_slider, _chStats.dex_adjusted);
            UpdateAdjustedSlider(int_adj_slider, _chStats.int_adjusted);
            UpdateAdjustedSlider(will_adj_slider, _chStats.will_adjusted);
            UpdateAdjustedSlider(soul_adj_slider, _chStats.soul_adjusted);

            int inventoryIndex = UICanvasManager.Instance.inventoryIndex;
            UpdateAllReadjustSliders(UICanvasManager.Instance.inventoryItems[inventoryIndex]);

            UpdateMarker(str_marker_slider, str_marker, _chStats.strength);
            UpdateMarker(dex_marker_slider, dex_marker, _chStats.dexterity);
            UpdateMarker(int_marker_slider, int_marker, _chStats.intelligence);
            UpdateMarker(will_marker_slider, will_marker, _chStats.will);
            UpdateMarker(soul_marker_slider, soul_marker, _chStats.soul);

           





        }
    }

    public void UpdateAllReadjustSliders(RuntimeItem newItem)
    {
        if (_chStats != null && newItem != null && newItem.IsEquippable)
        {
            float newStr = ReturnAdjustStatToNewItem(_chStats.strength, StatCategory.strength, newItem);
            float newDex = ReturnAdjustStatToNewItem(_chStats.dexterity, StatCategory.dexterity, newItem);
            float newInt = ReturnAdjustStatToNewItem(_chStats.intelligence, StatCategory.intelligence, newItem);
            float newWill = ReturnAdjustStatToNewItem(_chStats.will, StatCategory.will, newItem);
            float newSoul = ReturnAdjustStatToNewItem(_chStats.soul, StatCategory.soul, newItem);


            UpdateReadjSlider(str_readj_slider, str_readj_TMP, _chStats.str_adjusted, newStr);
            UpdateReadjSlider(dex_readj_slider, dex_readj_TMP, _chStats.dex_adjusted, newDex);
            UpdateReadjSlider(int_readj_slider, int_readj_TMP, _chStats.int_adjusted, newInt);
            UpdateReadjSlider(will_readj_slider, will_readj_TMP, _chStats.will_adjusted, newWill);
            UpdateReadjSlider(soul_readj_slider, soul_readj_TMP, _chStats.soul, newSoul);

        }
        else 
        {
            str_readj_slider.gameObject.SetActive(false);
            str_readj_TMP.enabled = false;

            dex_readj_slider.gameObject.SetActive(false);
            dex_readj_TMP.enabled = false;

            int_readj_slider.gameObject.SetActive(false);
            int_readj_TMP.enabled = false;

            will_readj_slider.gameObject.SetActive(false);
            will_readj_TMP.enabled = false;

            soul_readj_slider.gameObject.SetActive(false);
            soul_readj_TMP.enabled = false;

        }
    }


    private void UpdateMarker(Slider slider, RectTransform marker, float value)
    {

        float normalized = Mathf.InverseLerp(slider.minValue, slider.maxValue, value);

        // Assume marker is positioned relative to the slider fill area's parent
        RectTransform fillArea = slider.fillRect.parent.GetComponent<RectTransform>();
        float width = fillArea.rect.width;

        Vector2 pos = marker.anchoredPosition;
        pos.x = normalized * width;
        marker.anchoredPosition = pos;
    }

    private void UpdateAdjustedSlider(Slider slider, float adjustedValue)
    {
       
            slider.value = adjustedValue;
        
    }

    private void UpdateReadjSlider(Slider slider, TextMeshProUGUI readoutText, float adjustedValue, float readjustedValue)
    {
        if (adjustedValue == readjustedValue)
        {
            slider.gameObject.SetActive(false);
            readoutText.enabled = false;
        }
        else
        {
            slider.value = readjustedValue;
            readoutText.text = $"{(int)readjustedValue}";
            readoutText.enabled = true;
            slider.gameObject.SetActive(true);
        }
    }

    private float ReturnAdjustStatToNewItem(float stat, StatCategory statCategory, RuntimeItem newItem)
    {
        float adjustedStat;
        float totalAdjustment = 0f;

        ItemCategory newItemCategory = newItem.category;

        if (newItemCategory == ItemCategory.ring)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(newItem, statCategory);
        }
        else if (_chStats.equipped_ring != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_ring, statCategory);
        }

        if (newItemCategory == ItemCategory.helm)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(newItem, statCategory);
        }
        else if (_chStats.equipped_helm != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_helm, statCategory);
        }

        // Amulet
        if (newItemCategory == ItemCategory.amulet)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(newItem, statCategory);
        }
        else if (_chStats.equipped_amulet != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_amulet, statCategory);
        }

        // Melee Weapon
        if (newItemCategory == ItemCategory.melee_weapon)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(newItem, statCategory);
        }
        else if (_chStats.equipped_meleeWeapon != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_meleeWeapon, statCategory);
        }

        // Armor
        if (newItemCategory == ItemCategory.armor)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(newItem, statCategory);
        }
        else if (_chStats.equipped_armor != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_armor, statCategory);
        }

        // Ranged Weapon
        if (newItemCategory == ItemCategory.ranged_weapon)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(newItem, statCategory);
        }
        else if (_chStats.equipped_rangedWeapon != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_rangedWeapon, statCategory);
        }

        // Shield
        if (newItemCategory == ItemCategory.shield)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(newItem, statCategory);
        }
        else if (_chStats.equipped_shield != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_shield, statCategory);
        }

        // Boots
        if (newItemCategory == ItemCategory.boots)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(newItem, statCategory);
        }
        else if (_chStats.equipped_boots != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_boots, statCategory);
        }


        adjustedStat = stat + totalAdjustment;

        return adjustedStat;

    }

    private float ReturnAdjustmentToStatCategory(RuntimeItem item, StatCategory category)
    {
        float adjustment = 0f;

        switch (category)
        {
            case StatCategory.strength:
                adjustment = CalculateAdjustment(_chStats.strength, item.strModifier);
                break;
            case StatCategory.dexterity:
                adjustment = CalculateAdjustment(_chStats.dexterity, item.dexModifier);
                break;
            case StatCategory.intelligence:
                adjustment = CalculateAdjustment(_chStats.intelligence, item.intModifier);
                break;
            case StatCategory.will:
                adjustment = CalculateAdjustment(_chStats.will, item.willModifier);
                break;
            case StatCategory.soul:
                adjustment = CalculateAdjustment(_chStats.soul, item.soulModifier);
                break;
            default:
                adjustment = 0f;
                break;

        }

        return adjustment;
    }

    private float CalculateAdjustment(float stat, StatAdjustment statadjustment)
    {
        float value;

        switch (statadjustment.operatorType)
        {
            case (OperatorType.percent):
                value = stat * statadjustment.amount;
                break;
            case (OperatorType.additive):
                value = statadjustment.amount;
                break;
            default:
                value = 0f;
                break;
        }

        return value;
    }

}
