using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public enum EquipState
{
    equip,
    unequip
}
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

    [SerializeField] private TextMeshProUGUI meleeAR_adj_TMP;
    [SerializeField] private TextMeshProUGUI meleeDR_adj_TMP;
    [SerializeField] private TextMeshProUGUI rangedAR_adj_TMP;
    [SerializeField] private TextMeshProUGUI rangedDR_adj_TMP;
    [SerializeField] private TextMeshProUGUI magicAR_adj_TMP;

    [SerializeField] private TextMeshProUGUI meleeAR_readj_TMP;
    [SerializeField] private TextMeshProUGUI meleeDR_readj_TMP;
    [SerializeField] private TextMeshProUGUI rangedAR_readj_TMP;
    [SerializeField] private TextMeshProUGUI rangedDR_readj_TMP;
    [SerializeField] private TextMeshProUGUI magicAR_readj_TMP;

    [SerializeField] private TextMeshProUGUI confusion_DR_TMP;
    [SerializeField] private TextMeshProUGUI fear_DR_TMP;
    [SerializeField] private TextMeshProUGUI fire_DR_TMP;
    [SerializeField] private TextMeshProUGUI frost_DR_TMP;
    [SerializeField] private TextMeshProUGUI poison_DR_TMP;
    [SerializeField] private TextMeshProUGUI sleep_DR_TMP;

    [SerializeField] private TextMeshProUGUI confusion_DR_readj_TMP;
    [SerializeField] private TextMeshProUGUI fear_DR_readj_TMP;
    [SerializeField] private TextMeshProUGUI fire_DR_readj_TMP;
    [SerializeField] private TextMeshProUGUI frost_DR_readj_TMP;
    [SerializeField] private TextMeshProUGUI poison_DR_readj_TMP;
    [SerializeField] private TextMeshProUGUI sleep_DR_readj_TMP;

    Color32 greenCRT_color = new Color32(30, 190, 5, 255);
    Color32 amberCRT_color_transparent = new Color32(221, 147, 39, 150);
    Color32 amberCRT_color_solid = new Color32(221, 147, 39, 255);
    Color32 blueCRT_color_transparent = new Color32(85, 85, 255, 150);
    Color32 blueCRT_color_solid = new Color32(85, 85, 255, 255);
    Color32 lightBlueCRT_color_transparent = new Color32(85, 255, 255, 150);
    Color32 lightBlueCRT_color_solid = new Color32(85, 255, 255, 255);
    Color32 redCRT_color_solid = new Color32(196, 0, 0, 255);
    Color32 redCRT_color_transparent = new Color32(196, 0, 0, 150);



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
        UpdateStatsPanels_1_2();
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

    public void UpdateStatsPanels_1_2()
    {
        if (_chStats != null)
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

            if (!UICanvasManager.Instance.equipPanelFocus)
            {
                Debug.Log("Not EquipPanel Focus.");
                int inventoryIndex = UICanvasManager.Instance.inventoryIndex;
                UpdateReadjustSlidersAndStats(UICanvasManager.Instance.inventoryItems[inventoryIndex], EquipState.equip);
            }
            else
            {
                Debug.Log("EquipPanel in Focus.");

                UpdateReadjustSlidersAndStats(_chStats.GetEquippedByCategory(UICanvasManager.Instance.currentCatSelect), EquipState.unequip);

            }


            UpdateMarker(str_marker_slider, str_marker, _chStats.strength);
            UpdateMarker(dex_marker_slider, dex_marker, _chStats.dexterity);
            UpdateMarker(int_marker_slider, int_marker, _chStats.intelligence);
            UpdateMarker(will_marker_slider, will_marker, _chStats.will);
            UpdateMarker(soul_marker_slider, soul_marker, _chStats.soul);

            UpdateReadout(meleeAR_adj_TMP, _chStats.melee_attackRating);
            UpdateReadout(meleeDR_adj_TMP, _chStats.melee_defenseRating);
            UpdateReadout(rangedAR_adj_TMP, _chStats.ranged_attackRating);
            UpdateReadout(rangedDR_adj_TMP, _chStats.ranged_attackRating);
            UpdateReadout(magicAR_adj_TMP, _chStats.magic_attackRating);

            UpdateReadout(confusion_DR_TMP, _chStats.confusion_defenseRating);
            UpdateReadout(fear_DR_TMP, _chStats.fear_defenseRating);
            UpdateReadout(fire_DR_TMP, _chStats.fire_defenseRating);
            UpdateReadout(frost_DR_TMP, _chStats.frost_defenseRating);
            UpdateReadout(poison_DR_TMP, _chStats.poison_defenseRating);
            UpdateReadout(sleep_DR_TMP, _chStats.sleep_defenseRating);




        }
    }

    

    private void UpdateReadout(TextMeshProUGUI readoutText, float value)
    {
        readoutText.text = $"{(int)value}";
    }
    public void UpdateReadjustSlidersAndStats(RuntimeItem item, EquipState equipState)
    {
        if (_chStats != null && item != null && item.IsEquippable)
        {
            //find new stats with selected equipment, if equipped or unequipped
            float newStr = ReturnAdjustStatToItemEquipUneqip(_chStats.strength, StatCategory.strength, item, equipState);
            float newDex = ReturnAdjustStatToItemEquipUneqip(_chStats.dexterity, StatCategory.dexterity, item, equipState);
            float newInt = ReturnAdjustStatToItemEquipUneqip(_chStats.intelligence, StatCategory.intelligence, item, equipState);
            float newWill = ReturnAdjustStatToItemEquipUneqip(_chStats.will, StatCategory.will, item, equipState);
            float newSoul = ReturnAdjustStatToItemEquipUneqip(_chStats.soul, StatCategory.soul, item, equipState);


            UpdateReadjSlider(str_readj_slider, str_readj_TMP, _chStats.str_adjusted, newStr);
            UpdateReadjSlider(dex_readj_slider, dex_readj_TMP, _chStats.dex_adjusted, newDex);
            UpdateReadjSlider(int_readj_slider, int_readj_TMP, _chStats.int_adjusted, newInt);
            UpdateReadjSlider(will_readj_slider, will_readj_TMP, _chStats.will_adjusted, newWill);
            UpdateReadjSlider(soul_readj_slider, soul_readj_TMP, _chStats.soul, newSoul);

            //calculate readjustedARDRs
            float newMeleeAR = ReturnAdjustedMeleeAR(item, newStr, newDex, equipState);
            float newMeleeDR = ReturnAdjustedMeleeDR(item, newStr, newDex, equipState);
            float newRangedAR = ReturnAdjustedRangedAR(item, newDex, newInt, equipState);
            float newRangedDR = ReturnAdjustedRangedDR(item, newDex, equipState);
            float newMagicAR = ReturnAdjustedMagicAR(item, newInt, newWill, equipState);


            UpdateReadjustReadout(meleeAR_readj_TMP, _chStats.melee_attackRating, newMeleeAR);
            UpdateReadjustReadout(meleeDR_readj_TMP, _chStats.melee_defenseRating, newMeleeDR);
            UpdateReadjustReadout(rangedAR_readj_TMP, _chStats.ranged_attackRating, newRangedAR);
            UpdateReadjustReadout(rangedDR_readj_TMP, _chStats.ranged_defenseRating, newRangedDR);
            UpdateReadjustReadout(magicAR_readj_TMP, _chStats.magic_attackRating, newMagicAR);

            //calculate resistDRs
            float newConfusionDR = ReturnAdjustedResistDR(newInt, newWill);
            float newFearDR = ReturnAdjustedResistDR(newInt, newSoul);
            float newFireDR = ReturnAdjustedResistDR(newStr, newWill);
            float newFrostDR = ReturnAdjustedResistDR(newDex, newSoul);
            float newPoisonDR = ReturnAdjustedResistDR(newStr, newSoul);
            float newSleepDR = ReturnAdjustedResistDR(newStr, newInt);

            UpdateReadjustReadout(confusion_DR_readj_TMP, _chStats.confusion_defenseRating, newConfusionDR);
            UpdateReadjustReadout(fear_DR_readj_TMP, _chStats.fear_defenseRating, newFearDR);
            UpdateReadjustReadout(fire_DR_readj_TMP, _chStats.fire_defenseRating, newFireDR);
            UpdateReadjustReadout(frost_DR_readj_TMP, _chStats.frost_defenseRating, newFrostDR);
            UpdateReadjustReadout(poison_DR_readj_TMP, _chStats.poison_defenseRating, newPoisonDR);
            UpdateReadjustReadout(sleep_DR_readj_TMP, _chStats.sleep_defenseRating, newSleepDR);


        }
        else
        {
            str_readj_slider.gameObject.SetActive(false);
            str_readj_TMP.transform.parent.gameObject.SetActive(false);
            str_readj_TMP.enabled = false;

            dex_readj_slider.gameObject.SetActive(false);
            dex_readj_TMP.transform.parent.gameObject.SetActive(false);
            dex_readj_TMP.enabled = false;

            int_readj_slider.gameObject.SetActive(false);
            int_readj_TMP.transform.parent.gameObject.SetActive(false);
            int_readj_TMP.enabled = false;

            will_readj_slider.gameObject.SetActive(false);
            will_readj_TMP.transform.parent.gameObject.SetActive(false);
            will_readj_TMP.enabled = false;

            soul_readj_slider.gameObject.SetActive(false);
            soul_readj_TMP.transform.parent.gameObject.SetActive(false);
            soul_readj_TMP.enabled = false;

            meleeAR_readj_TMP.enabled = false;
            meleeDR_readj_TMP.enabled = false;
            rangedAR_readj_TMP.enabled= false;
            rangedDR_readj_TMP.enabled = false;
            magicAR_readj_TMP.enabled = false;


            confusion_DR_readj_TMP.enabled = false;
            fear_DR_readj_TMP.enabled = false;
            fire_DR_readj_TMP.enabled = false;
            frost_DR_readj_TMP.enabled = false;
            poison_DR_readj_TMP.enabled = false;
            sleep_DR_readj_TMP.enabled = false;

        }
    }

    private float ReturnAdjustedResistDR(float stat1, float stat2)
    {
        return (stat1 + stat2) / 2;

        //add code for amulet resists

    }
    private void UpdateReadjustReadout(TextMeshProUGUI readoutText, float originalValue,  float newValue)
    {
        if (newValue != originalValue)
        {

            if (newValue < originalValue)
            {
                //decrease
                readoutText.color = amberCRT_color_solid;
            }
            else
            {
                //increase
                readoutText.color = lightBlueCRT_color_solid;
            }


            readoutText.enabled = true;
            readoutText.text = $"{(int)newValue}";
        }
        else
        {
            readoutText.enabled = false;
        }

    }

    private float ReturnAdjustedMeleeAR(RuntimeItem item, float newStr, float newDex, EquipState equipState)
    {
        float AR_str;
        float AR_dex;

        if (equipState == EquipState.equip)
        {
            if (item.category == ItemCategory.melee_weapon)
            {
                AR_str = newStr + (newStr * item.MeleeWeapon.attack_strModifier);
                AR_dex = newDex + (newDex * item.MeleeWeapon.attack_dexModifier);
            }
            else if (_chStats.equipped_meleeWeapon != null)
            {
                AR_str = newStr + (newStr * _chStats.equipped_meleeWeapon.MeleeWeapon.attack_strModifier);
                AR_dex = newDex + (newDex * _chStats.equipped_meleeWeapon.MeleeWeapon.attack_dexModifier);
            }
            else
            {
                AR_str = newStr;
                AR_dex = newDex;
            }

            return ((AR_str + AR_dex) / 2);
        }
        else if (equipState == EquipState.unequip)
        {
            if (item.category == ItemCategory.melee_weapon || _chStats.equipped_meleeWeapon == null)
            {
                AR_str = newStr;
                AR_dex = newDex;
            }
            else
            {
                AR_str = newStr + (newStr * _chStats.equipped_meleeWeapon.MeleeWeapon.attack_strModifier);
                AR_dex = newDex + (newDex * _chStats.equipped_meleeWeapon.MeleeWeapon.attack_dexModifier);
            }


            return ((newStr + newDex) / 2);

        }
        else
        {
            Debug.Log("Undefined EquipState specified.");
            return 0f;
        }
    }
    private float ReturnAdjustedMeleeDR(RuntimeItem item, float newStr, float newDex, EquipState equipState)
    {
        
        float DR_str;
        float DR_dex;

        if (equipState == EquipState.equip)
        {
            //find DR_str for DR calculation
            if (item.category == ItemCategory.shield)
            {
                DR_str = newStr + (newStr * item.Shield.defense_strModifier);
            }
            else if (_chStats.equipped_shield != null)
            {
                DR_str = newStr + (newStr * _chStats.equipped_shield.Shield.defense_strModifier);
            }
            else
            {
                DR_str = newStr;
            }

            //find DR_dex for DR calculation
            if (item.category == ItemCategory.melee_weapon)
            {
                DR_dex = newDex + (newDex * item.MeleeWeapon.defense_dexModifier);
            }
            else if (_chStats.equipped_meleeWeapon != null)
            {
                DR_dex = newDex + (newDex * _chStats.equipped_meleeWeapon.MeleeWeapon.defense_dexModifier);
            }
            else
            {
                DR_dex = newDex;
            }

            
;            return ((DR_str + DR_dex) / 2);
        }
        else if (equipState == EquipState.unequip)
        {
            if (item.category == ItemCategory.shield || _chStats.equipped_shield == null)
            {
                DR_str = newStr;
            }
            else
            {
                DR_str = newStr + (newStr * _chStats.equipped_shield.Shield.defense_strModifier);
            }

            if (item.category == ItemCategory.melee_weapon || _chStats.equipped_meleeWeapon == null)
            {
                DR_dex = newDex;
            }
            else
            {
                DR_dex = newDex + (newDex * _chStats.equipped_meleeWeapon.MeleeWeapon.defense_dexModifier);
            }

            return ((DR_str + DR_dex) / 2);


        }
        else
        {
            Debug.Log("Undefined EquipState specified.");
            return 0f;
        }

    }

    private float ReturnAdjustedRangedAR(RuntimeItem item, float newDex, float newInt, EquipState equipState)
    {


        if (equipState == EquipState.equip)
        {
            float rangedBonus = 0f;
            
            if (item.category == ItemCategory.ranged_weapon)
            {
                rangedBonus += newDex * item.RangedWeapon.attack_dexModifier;
            }
            else if (_chStats.equipped_rangedWeapon != null)
            {
                rangedBonus += newDex * _chStats.equipped_rangedWeapon.RangedWeapon.attack_dexModifier;
            }
            else
            {
                //leave rangedBonus at 0
            }

            if (item.category == ItemCategory.missile)
            {
                rangedBonus += newDex * item.Missile.attack_dexModifier;
            }
            else if (_chStats.equipped_missile != null)
            {
                rangedBonus += newDex * _chStats.equipped_missile.Missile.attack_dexModifier;
            }
            else
            { 
                //leave ranged unmodified
            }

            return ( (newDex + rangedBonus + newInt) / 2);

        }
        else if (equipState == EquipState.unequip)
        {
            float rangedBonus = 0f;

            if (item.category != ItemCategory.ranged_weapon && _chStats.equipped_rangedWeapon != null)
            {
                rangedBonus += newDex * _chStats.equipped_rangedWeapon.RangedWeapon.attack_dexModifier;
            }
            else
            {
                //leave rangedBonus at 0
            }

            if (item.category != ItemCategory.missile && _chStats.equipped_missile != null)
            {
                rangedBonus += newDex * _chStats.equipped_missile.Missile.attack_dexModifier;
            }
            else
            {
                //leave rangedBonus unmodified
            }

            return ((newDex + rangedBonus + newInt) / 2);
        }
        else
        {
            Debug.Log("Undefined EquipState specified.");
            return 0f;
        }

    }

    private float ReturnAdjustedRangedDR(RuntimeItem item, float newDex, EquipState equipState)
    {
        float rangedDR_dex;

        if (equipState == EquipState.equip)
        {
            if (item.category == ItemCategory.shield)
            {
                rangedDR_dex = newDex + (newDex * item.Shield.defense_dexModifier);
            }
            else if (_chStats.equipped_shield != null)
            {
                rangedDR_dex = newDex + (newDex * _chStats.equipped_shield.Shield.defense_dexModifier);
            }
            else
            {
                rangedDR_dex = newDex;
            }

            return rangedDR_dex;

        }
        else if (equipState == EquipState.unequip)
        {

            if (item.category != ItemCategory.shield && _chStats.equipped_shield != null)
            {
                rangedDR_dex = newDex + (newDex * _chStats.equipped_shield.Shield.defense_dexModifier);
            }
            else
            {
                rangedDR_dex= newDex;
            }

            return rangedDR_dex;


        }
        else
        {
            Debug.Log("Undefined EquipState specified.");
            return 0f;
        }

    }

    private float ReturnAdjustedMagicAR(RuntimeItem item, float newInt, float newWill, EquipState equipState)
    {
        if (equipState == EquipState.equip)
        {
            float AR_int;
            float AR_will;

            if (item.category == ItemCategory.melee_weapon)
            {
                AR_int = newInt + (newInt * item.MeleeWeapon.attack_intModifier);
                AR_will = newWill + (newWill * item.MeleeWeapon.attack_willModifer);
            }
            else if (_chStats.equipped_meleeWeapon != null)
            {
                AR_int = newInt + (_chStats.equipped_meleeWeapon.MeleeWeapon.attack_intModifier);
                AR_will = newWill + (_chStats.equipped_meleeWeapon.MeleeWeapon.attack_willModifer);
            }
            else
            {
                AR_int = newInt;
                AR_will = newWill;
            }

            return ( (AR_int + AR_will) / 2 );
        }
        else if (equipState == EquipState.unequip )
        {
            float AR_int;
            float AR_will;

            if (item.category != ItemCategory.melee_weapon && _chStats.equipped_meleeWeapon != null)
            {
                AR_int = newInt + (_chStats.equipped_meleeWeapon.MeleeWeapon.attack_intModifier);
                AR_will = newWill + (_chStats.equipped_meleeWeapon.MeleeWeapon.attack_willModifer);
            }
            else
            {
                AR_int = newInt;
                AR_will = newWill;
            }

            return ((AR_int + AR_will) / 2);


        }
        else
        {
            Debug.Log("Undefined EquipState specified.");
            return 0f;
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
            readoutText.transform.parent.gameObject.SetActive(false);

        }
        else
        {
            Image fillImage = slider.gameObject.GetComponentInChildren<Image>();

            if (readjustedValue < adjustedValue)
            {
                //decrease
                fillImage.color = amberCRT_color_transparent;
                readoutText.color = amberCRT_color_solid;
            }
            else
            {
                //increase
                fillImage.color = lightBlueCRT_color_transparent;
                readoutText.color = lightBlueCRT_color_solid;
            }

            slider.value = readjustedValue;
            readoutText.text = $"{(int)readjustedValue}";
            readoutText.transform.parent.gameObject.SetActive(true);
            readoutText.enabled = true;
            slider.gameObject.SetActive(true);
        }
    }



    // *** equip/unequip calculations
    private float ReturnAdjustStatToItemEquipUneqip(float stat, StatCategory statCategory, RuntimeItem item, EquipState equipState)
    {
        float adjustedStat;
        float totalAdjustment = 0f;

        ItemCategory itemCategory = item.category;

        if (itemCategory == ItemCategory.ring)
        {
            if (equipState == EquipState.equip)
            {
                totalAdjustment += ReturnAdjustmentToStatCategory(item, statCategory);
            }
            else
            {
                //add nothing if this category is to be unequipped
            }
        }
        else if (_chStats.equipped_ring != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_ring, statCategory);
        }


        if (itemCategory == ItemCategory.helm)
        {
            if (equipState == EquipState.equip)
            {
                totalAdjustment += ReturnAdjustmentToStatCategory(item, statCategory);
            }
        }
        else if (_chStats.equipped_helm != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_helm, statCategory);
        }

        // Amulet
        if (itemCategory == ItemCategory.amulet)
        {
            if (equipState == EquipState.equip)
            {
                totalAdjustment += ReturnAdjustmentToStatCategory(item, statCategory);
            }
        }
        else if (_chStats.equipped_amulet != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_amulet, statCategory);
        }

        // Melee Weapon
        if (itemCategory == ItemCategory.melee_weapon)
        {
            if (equipState == EquipState.equip)
            {
                totalAdjustment += ReturnAdjustmentToStatCategory(item, statCategory);
            }
        }
        else if (_chStats.equipped_meleeWeapon != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_meleeWeapon, statCategory);
        }

        // Armor
        if (itemCategory == ItemCategory.armor)
        {
            if (equipState == EquipState.equip)
            {
                totalAdjustment += ReturnAdjustmentToStatCategory(item, statCategory);
            }
        }
        else if (_chStats.equipped_armor != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_armor, statCategory);
        }

        // Ranged Weapon
        if (itemCategory == ItemCategory.ranged_weapon)
        {
            if (equipState == EquipState.equip)
            {
                totalAdjustment += ReturnAdjustmentToStatCategory(item, statCategory);
            }
        }
        else if (_chStats.equipped_rangedWeapon != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_rangedWeapon, statCategory);
        }

        // Shield
        if (itemCategory == ItemCategory.shield)
        {
            if (equipState == EquipState.equip)
            {
                totalAdjustment += ReturnAdjustmentToStatCategory(item, statCategory);
            }
        }
        else if (_chStats.equipped_shield != null)
        {
            totalAdjustment += ReturnAdjustmentToStatCategory(_chStats.equipped_shield, statCategory);
        }

        // Boots
        if (itemCategory == ItemCategory.boots)
        {
            if (equipState == EquipState.equip)
            {
                totalAdjustment += ReturnAdjustmentToStatCategory(item, statCategory);
            }
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
