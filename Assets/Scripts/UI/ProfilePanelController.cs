using System.Xml.Serialization;
using TMPro;
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
            Debug.Log("Ch Portrait="+_chStats.characterPortrait.name);
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

            UpdateReadjSlider(str_readj_slider, 0, 0);
            UpdateReadjSlider(dex_readj_slider, 0, 0);
            UpdateReadjSlider(int_readj_slider, 0, 0);
            UpdateReadjSlider(will_readj_slider, 0, 0);
            UpdateReadjSlider(soul_readj_slider, 0, 0);

            UpdateMarker(str_marker_slider, str_marker, _chStats.strength);
            UpdateMarker(dex_marker_slider, dex_marker, _chStats.dexterity);
            UpdateMarker(int_marker_slider, int_marker, _chStats.intelligence);
            UpdateMarker(will_marker_slider, will_marker, _chStats.will);
            UpdateMarker(soul_marker_slider, soul_marker, _chStats.soul);

           





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

    private void UpdateReadjSlider(Slider slider, float adjustedValue, float readjustedValue)
    {
        if (adjustedValue == readjustedValue)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            slider.value = readjustedValue;
            slider.gameObject.SetActive(true);
        }
    }



}
