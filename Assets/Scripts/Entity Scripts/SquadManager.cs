using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditorInternal.VersionControl.ListControl;

public class SquadManager : MonoBehaviour
{
    public static event Action<GameObject> OnCharacterSelected;

    //public static event Action<GameObject, int> OnUISelected;

    public struct ch_info
    {
        public string ch_type;
        public int slot_number;
        public GameObject obj_ref;
        // Constructor
        public ch_info(string type, int slot, GameObject obj)
        {
            ch_type = type;
            slot_number = slot;
            obj_ref = obj;
        }
    }

    private ch_info[] ch_info_array = new ch_info[4]
    {
        new ch_info("Red", 0, null),
        new ch_info("Yellow", 1, null),
        new ch_info("Blue", 2, null),
        new ch_info("Green", 3, null),

    };

    private string[] select_activeToStrings = new string[4] { "fighter", "cleric", "wizard", "ranger" };

    public GameObject RedPrefab;
    public GameObject GreenPrefab;
    public GameObject BluePrefab;
    public GameObject YellowPrefab;

    public int select_active = -1;




    private PlayerInputActions playerControls;
    private InputAction SelectRed;
    private InputAction SelectGreen;
    private InputAction SelectBlue;
    private InputAction SelectYellow;
    private InputAction Engage;
    private InputAction UISelect;

    private InputAction ReturnToFormation;
    private InputAction AllReturnToFormation;

    private InputAction Rotate;

    private GameObject selectring_obj;
    public GameObject selectring_prefab;
    public GameObject core_prefab;
    private GameObject core_obj_ref;
    private MoveInput moveinput;

    public GameObject main_camera_obj;

    private bool[] isReturning = { false, false, false, false };

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        core_obj_ref = Instantiate(core_prefab, Vector3.zero, Quaternion.identity);
        moveinput = core_obj_ref.GetComponent<MoveInput>();

        InstantiateCHPrefabs();
        //AssignWeaponsToCharacters();

        main_camera_obj.GetComponent<CameraFollow>().core = core_obj_ref;
    }

    private void OnEnable()
    {
        SelectRed = playerControls.Player.SelectRed;
        SelectRed.Enable();
        SelectRed.started += OnSelectRed;
        SelectRed.canceled += OnSelectRed;

        SelectYellow = playerControls.Player.SelectYellow;
        SelectYellow.Enable();
        SelectYellow.started += OnSelectYellow;
        SelectYellow.canceled += OnSelectYellow;

        SelectBlue = playerControls.Player.SelectBlue;
        SelectBlue.Enable();
        SelectBlue.started += OnSelectBlue;
        SelectBlue.canceled += OnSelectBlue;

        SelectGreen = playerControls.Player.SelectGreen;
        SelectGreen.Enable();
        SelectGreen.started += OnSelectGreen;
        SelectGreen.canceled += OnSelectGreen;

        ReturnToFormation = playerControls.Player.ReturnToFormation;
        ReturnToFormation.Enable();
        ReturnToFormation.performed += OnReturnToFormation;

        AllReturnToFormation = playerControls.Player.AllReturnToFormation;
        AllReturnToFormation.Enable();
        AllReturnToFormation.performed += OnAllReturnToFormation;

        Engage = playerControls.Player.Engage;
        Engage.Enable();
        Engage.performed += OnEngage;

        Rotate = playerControls.Player.Rotate;
        Rotate.Enable();
        Rotate.performed += OnRotate;
        Rotate.canceled += OnRotate;

        UISelect = playerControls.Player.UISelect;
        UISelect.Enable();
        UISelect.performed += OnUISelect;


    }

    private void OnDisable()
    {
        SelectRed.canceled -= OnSelectRed;
        SelectRed.started -= OnSelectRed;
        SelectRed.Disable();

        SelectYellow.canceled -= OnSelectYellow;
        SelectYellow.started -= OnSelectYellow;
        SelectYellow.Disable();

        SelectBlue.canceled -= OnSelectBlue;
        SelectBlue.started -= OnSelectBlue;
        SelectBlue.Disable();

        SelectGreen.canceled -= OnSelectGreen;
        SelectGreen.started -= OnSelectGreen;
        SelectGreen.Disable();

        ReturnToFormation.performed -= OnReturnToFormation;
        ReturnToFormation.Disable();

        AllReturnToFormation.performed -= OnAllReturnToFormation;
        AllReturnToFormation.Disable();

        Engage.performed -= OnEngage;
        Engage.Disable();

        Rotate.performed -= OnRotate;
        Rotate.canceled -= OnRotate;
        Rotate.Disable();

        UISelect.performed -= OnUISelect;
        UISelect.Disable();


    }


    //************************ CLASS METHODS *************************************************************************

    void InstantiateCHPrefabs()
    {
        for (int i = 0; i < ch_info_array.Length; i++)
        {
            // Determine which prefab to instantiate based on ch_type
            GameObject prefabToInstantiate = null;
            switch (ch_info_array[i].ch_type)
            {
                case "Red":
                    prefabToInstantiate = RedPrefab;
                    break;
                case "Green":
                    prefabToInstantiate = GreenPrefab;
                    break;
                case "Blue":
                    prefabToInstantiate = BluePrefab;
                    break;
                case "Yellow":
                    prefabToInstantiate = YellowPrefab;
                    break;
            }

            // Instantiate the prefab if it's found
            if (prefabToInstantiate != null)
            {
                ch_info_array[i].obj_ref = Instantiate(prefabToInstantiate, new Vector3(i * 2.0f, 0, 0), Quaternion.identity);
                Ch_Behavior controller = ch_info_array[i].obj_ref.GetComponent<Ch_Behavior>();
                controller.slot_num = i;
                controller.core_obj = core_obj_ref;

            }
            else
            {
                Debug.LogWarning($"No prefab found for type: {ch_info_array[i].ch_type}");
            }

        }
    }

    private void AssignWeaponsToCharacters()
    {
        ch_info_array[0].obj_ref.GetComponent<EntityStats>().equipped_meleeWeapon = WeaponDatabase.Instance.GetWeaponByName("Crude Iron Sword");

        ch_info_array[3].obj_ref.GetComponent<EntityStats>().equipped_meleeWeapon = WeaponDatabase.Instance.GetWeaponByName("Simple Bow");

        ch_info_array[3].obj_ref.GetComponent<EntityStats>().equipped_missile = WeaponDatabase.Instance.GetWeaponByName("Wooden Arrow");



    }

    private void OnEngage(InputAction.CallbackContext context)
    {


        if (select_active >= 0)
        {
            EngageTarget();
        }
    }

    private void OnSelectRed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (select_active == 0)
            {
                EngageTarget();
            }
            else
            {
                SwitchCharacterSelect(0);

            }

        }

    }

    private void OnSelectYellow(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            if (select_active == 1)
            {
                EngageTarget();
            }
            else
            {
                SwitchCharacterSelect(1);

            }

        }


        /*
        if (context.started)
        {
            SwitchCharacterSelect(1);
        }
        */

    }

    private void OnSelectBlue(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (select_active == 2)
            {
                EngageTarget();
            }
            else
            {
                SwitchCharacterSelect(2);

            }

        }
        /*
        if (context.started)
        {
            SwitchCharacterSelect(2);
        }
        */
    }

    private void OnSelectGreen(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (select_active == 3)
            {
                EngageTarget();
            }
            else
            {
                SwitchCharacterSelect(3);

            }

        }

        /*
        if (context.started)
        {
            SwitchCharacterSelect(3);
        }
        */

    }

    private void SwitchCharacterSelect(int pressed_select_index)
    {

        if (select_active != -1)
        {
            //Debug.Log("SM: Deactivating character Select lines.");
            DeactivateCharacterSelectLines(select_active);
        }

        select_active = pressed_select_index;
        SoundManager.Instance.PlayCharacterSelectAffirm(select_active);
        if (ch_info_array[pressed_select_index].obj_ref != null)
        {
            ActivateCharacterSelectLines(select_active);

        }

        //event trigger UI update
        OnCharacterSelected?.Invoke(ch_info_array[select_active].obj_ref);


    }

    private void OnUISelect(InputAction.CallbackContext context)
    {
        if (context.performed && select_active != -1)
        {
            Vector2 inputVector = context.ReadValue<Vector2>();

            EntityStats _entityStats = ch_info_array[select_active].obj_ref.GetComponent<EntityStats>();

            int new_activeSlot = _entityStats.active_skillSlot;

            if (Mathf.Sign(inputVector.x) == 1)
            {
                new_activeSlot++;
                if (new_activeSlot > _entityStats.skill_slot.Length - 1)
                {
                    new_activeSlot = 0;
                }
                else if (_entityStats.skill_slot[new_activeSlot].skill_type == "none")
                {
                    new_activeSlot = 0;
                }

            }
            else if (Mathf.Sign(inputVector.x) == -1)
            {
                new_activeSlot--;

                if (new_activeSlot < 0)
                {
                    new_activeSlot = _entityStats.skill_slot.Length - 1;

                    while (_entityStats.skill_slot[new_activeSlot].skill_type == "none" && new_activeSlot > 0)
                    {
                        new_activeSlot--;
                    }


                }
            }

            if (new_activeSlot != _entityStats.active_skillSlot)
            {
                _entityStats.active_skillSlot = new_activeSlot; //change the slot that's active
                _entityStats.selected_skill = _entityStats.skill_slot[_entityStats.active_skillSlot];
                SoundManager.Instance.PlayClick();

                DeactivateCharacterSelectLines(select_active);
                ActivateCharacterSelectLines(select_active);

                OnCharacterSelected?.Invoke(ch_info_array[select_active].obj_ref); //trigger update to UI



                //Debug.Log("SquadManager:" + gameObject.name + "changed active skill to" + _entityStats.active_skill.skill_name);


            }




        }
    }

    private void EngageTarget()
    {
        SkillCooldownTracker _skillCooldownTracker = ch_info_array[select_active].obj_ref.GetComponent<SkillCooldownTracker>();
        Skill_SO active_skill = ch_info_array[select_active].obj_ref.GetComponent<EntityStats>().selected_skill;
       

        if (_skillCooldownTracker == null || !_skillCooldownTracker.IsSkillOnCooldown(active_skill))
        {
            ch_info_array[select_active].obj_ref.GetComponent<TargetingScan>().SetTargetedEntity(); //set highlighted entity to target entity

            //change performed skill to the currently active skill in stats
            ch_info_array[select_active].obj_ref.GetComponent<Ch_Behavior>().SetSkillPerforming(ch_info_array[select_active].obj_ref.GetComponent<EntityStats>().selected_skill);

            //turn on isEngaging
            ch_info_array[select_active].obj_ref.GetComponent<Ch_Behavior>().isEngaging = true;

            DeactivateCharacterSelectLines(select_active);
            select_active = -1;

        }

     

    }

    private void ActivateCharacterSelectLines(int select_index)
    {
        selectring_obj = Instantiate(selectring_prefab, ch_info_array[select_index].obj_ref.transform); //create selectingring on character

        ch_info_array[select_index].obj_ref.GetComponent<TargetingScan>().scanningOn = true;  //turn on enemy scanning

        moveinput.rotation_lock = true; //lock squad rotation when selecting active
    }

    private void DeactivateCharacterSelectLines(int select_index)
    {

        moveinput.rotation_lock = false; //turn off squard rotation lock
        if (ch_info_array[select_index].obj_ref != null)
        {
            ch_info_array[select_index].obj_ref.GetComponent<TargetingScan>().scanningOn = false; //turn off scanning
        }

        Destroy(selectring_obj); //remove the selection ring
        DisableActiveEntityReturnLines(); //turn off all enemy selection lines
        //select_active = -1; //the active select index is turned off

        //UI deactivate because no character selected
        OnCharacterSelected?.Invoke(null);


    }

    private void OnReturnToFormation(InputAction.CallbackContext context)
    {
        if (select_active >= 0)
        {
            ch_info_array[select_active].obj_ref.GetComponent<Ch_Behavior>().CancelEngage();
            DeactivateCharacterSelectLines(select_active);
            select_active = -1;
        }
    }

    private void OnAllReturnToFormation(InputAction.CallbackContext context)
    {
        for (int i = 0; i < 4; i++)
        {
            if (ch_info_array[i].obj_ref != null)
            {
                ch_info_array[i].obj_ref.GetComponent<Ch_Behavior>().CancelEngage();

            }

        }
        if (select_active >= 0)
        {
            DeactivateCharacterSelectLines(select_active);
            select_active = -1;

        }
    }

    private void OnRotate(InputAction.CallbackContext context)
    {
        if (select_active >= 0)
        {
            if (context.performed)
            {
                ch_info_array[select_active].obj_ref.GetComponent<TargetingScan>().SelectNewEntity(Rotate.ReadValue<Vector2>());
            }
            else if (context.canceled)
            {
                ch_info_array[select_active].obj_ref.GetComponent<TargetingScan>().ResetNewSelectionMade();
            }
        }
    }

    private void DisableActiveEntityReturnLines()
    {
        //Debug.Log("Squad Manager: DisablingActiveEntityReturnLines");
        List<GameObject> entities = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));

        entities.AddRange(new List<GameObject>(GameObject.FindGameObjectsWithTag("Character")));

        /*
        GameObject[] entities = GameObject.FindGameObjectsWithTag("Enemy");
        entities.AddRange(new List<GameObject>(FindGameObjectsWithTag("tag2")));
        */

        foreach (GameObject entity in entities)
        {
            //Debug.Log("SM Disabling:"+entity.name);
            ReturnLinePlot entityReturnLinePlot = entity.GetComponent<ReturnLinePlot>();

            entityReturnLinePlot.active_line = false;
            entityReturnLinePlot.target_obj = null;

        }
    }




}

