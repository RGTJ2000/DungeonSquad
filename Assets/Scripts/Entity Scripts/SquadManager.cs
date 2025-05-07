using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
using static UnityEditorInternal.VersionControl.ListControl;

public class SquadManager : MonoBehaviour
{
    public static event Action<GameObject> OnCharacterSelected;

    public static event Action<GameObject> OnInventorySelected;




    public GameObject[] ch_in_slot_array = null;

    /*
    private string[] select_activeToStrings = new string[4] { "fighter", "cleric", "wizard", "ranger" };

    public GameObject RedPrefab;
    public GameObject GreenPrefab;
    public GameObject BluePrefab;
    public GameObject YellowPrefab;
    */

    public int select_active = -1;

    private PlayerInputActions playerControls;
    private InputAction SelectRed;
    private InputAction SelectGreen;
    private InputAction SelectBlue;
    private InputAction SelectYellow;
    private InputAction Engage;
    private InputAction SkillSelect;
    private InputAction InventorySelect;

    private InputAction ReturnToFormation;
    private InputAction AllReturnToFormation;

    private InputAction Rotate;

    private GameObject selectring_obj;
    public GameObject selectring_prefab;
    private GameObject core_obj_ref;
    private MoveInput _core_moveInput;

    public GameObject main_camera_obj;

    private bool[] isReturning = { false, false, false, false };

    private void Awake()
    {
        playerControls = new PlayerInputActions();

        main_camera_obj.GetComponent<CameraFollow>().core = core_obj_ref;
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();

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

      

        Rotate = playerControls.Player.Rotate;
        Rotate.Enable();
        Rotate.performed += OnRotate;
        Rotate.canceled += OnRotate;

        SkillSelect = playerControls.Player.SkillSelect;
        SkillSelect.Enable();
        SkillSelect.performed += OnUISelect;

        InventorySelect = playerControls.Player.InventorySelect;
        InventorySelect.Enable();
        InventorySelect.performed += OnInventorySelect;

        playerControls.Player.ToggleScanMode.started += OnToggleScanMode;

    }

    private void OnDisable()
    {
        playerControls.Player.Disable();

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

      

        Rotate.performed -= OnRotate;
        Rotate.canceled -= OnRotate;
        Rotate.Disable();

        SkillSelect.performed -= OnUISelect;
        SkillSelect.Disable();

        InventorySelect.performed -= OnInventorySelect;
        InventorySelect.Disable();

        playerControls.Player.ToggleScanMode.started -= OnToggleScanMode;

    }


    /*
     * SQUAD CONTROL METHODS
     * 
     */

   


 

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



        if (ch_in_slot_array[pressed_select_index] != null)
        {
            ActivateCharacterSelectLines(select_active);

            string variationID;

            switch (pressed_select_index)
            {

                case 0:
                    variationID = "fighterConfirm";
                    break;
                case 1:
                    variationID = "clericConfirm";
                    break;
                case 2:
                    variationID = "wizardConfirm";
                    break;
                case 3:
                    variationID = "rangerConfirm";
                    break;
                default:
                    variationID = "none";
                    break;
            }

            if (variationID == "none")
            {
                Debug.Log("No audio variation found for ch_ID");
            }

            SoundManager.Instance.PlayVariationAtObject(variationID, ch_in_slot_array[pressed_select_index], SoundCategory.sfx);





        }

        //event trigger for UI update (send the character gameobject)
        OnCharacterSelected?.Invoke(ch_in_slot_array[select_active]);


    }

    private void OnUISelect(InputAction.CallbackContext context)
    {
        if (context.performed && select_active != -1)
        {
            float inputValue = context.ReadValue<float>();

            EntityStats _entityStats = ch_in_slot_array[select_active].GetComponent<EntityStats>();

            int new_activeSlot = _entityStats.active_skillSlot;

            if (inputValue > 0)
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
            else if (inputValue < 0)
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
                SoundManager.Instance.PlaySoundByKey("single_click", SoundCategory.UI);

                //DeactivateCharacterSelectLines(select_active);
                ActivateCharacterSelectLines(select_active);


                OnCharacterSelected?.Invoke(ch_in_slot_array[select_active]); //trigger update to UI



                //Debug.Log("SquadManager:" + gameObject.name + "changed active skill to" + _entityStats.active_skill.skill_name);


            }




        }
    }

    private void OnInventorySelect(InputAction.CallbackContext context)
    {
        if (select_active >= 0)
        {
            OnInventorySelected?.Invoke(ch_in_slot_array[select_active]);

        }
        else
        {
            OnInventorySelected?.Invoke(null);

        }
    }

    private void OnToggleScanMode(InputAction.CallbackContext context)
    {
        Debug.Log("Toggle ScanMode pressed.");

        if (select_active >= 0 && ch_in_slot_array[select_active] != null)
        {
            Ch_Behavior _chBehave = ch_in_slot_array[select_active].GetComponent<Ch_Behavior>();

           _chBehave.ToggleEngageMode();
           DisableActiveReturnLines();

        }
    }

    private void EngageTarget()
    {
        if (ch_in_slot_array[select_active] != null)
        {
            Ch_Behavior _chB = ch_in_slot_array[select_active].GetComponent<Ch_Behavior>();

            if (_chB.ActivateAction())
            {
                if (_chB.actionMode == ActionMode.combat)
                {
                    DeactivateCharacterSelectLines(select_active);
                    select_active = -1;
                    OnCharacterSelected?.Invoke(null);

                }
                else if (_chB.actionMode == ActionMode.item)
                {
                    //leave CharacterSelectLines active
                    //leave select active
                }
               
                
            }

        }

    }

    private void ActivateCharacterSelectLines(int select_index)
    {
        DisableActiveReturnLines();

        if (selectring_obj == null)
        {
            selectring_obj = Instantiate(selectring_prefab, ch_in_slot_array[select_index].transform); //create selectingring on character

        }
       

        

        ch_in_slot_array[select_index].GetComponent<TargetingScan>().ActivateTargetingScan();

        //ch_in_slot_array[select_index].GetComponent<TargetingScan>().scanningOn = true;  //turn on enemy scanning

        //don't need to lock rotation with current movement scheme
        // _core_moveInput.rotation_lock = true; //lock squad rotation when selecting active
    }

    private void DeactivateCharacterSelectLines(int select_index)
    {

        //_core_moveInput.rotation_lock = false; //turn off squard rotation lock
        if (ch_in_slot_array[select_index] != null)
        {
            ch_in_slot_array[select_index].GetComponent<TargetingScan>().scanningOn = false; //turn off scanning
        }

        Destroy(selectring_obj); //remove the selection ring
        selectring_obj = null;
        DisableActiveReturnLines(); //turn off all enemy selection lines
        //select_active = -1; //the active select index is turned off

        //UI deactivate because no character selected
        OnCharacterSelected?.Invoke(null);


    }

    private void OnReturnToFormation(InputAction.CallbackContext context)
    {
        if (select_active >= 0)
        {
            if (ch_in_slot_array[select_active] != null)
            {
                ch_in_slot_array[select_active].GetComponent<Ch_Behavior>().CancelActions();
                DeactivateCharacterSelectLines(select_active);


            }
            select_active = -1;

            OnCharacterSelected?.Invoke(null);

        }
    }

    private void OnAllReturnToFormation(InputAction.CallbackContext context)
    {
        for (int i = 0; i < 4; i++)
        {
            if (ch_in_slot_array[i] != null)
            {
                ch_in_slot_array[i].GetComponent<Ch_Behavior>().CancelEngage();

            }

        }
        if (select_active >= 0)
        {
            DeactivateCharacterSelectLines(select_active);
            select_active = -1;

        }
        OnCharacterSelected?.Invoke(null);

    }

    private void OnRotate(InputAction.CallbackContext context)
    {
        if (select_active >= 0 && ch_in_slot_array[select_active] != null)
        {
            if (context.performed)
            {
                ch_in_slot_array[select_active].GetComponent<TargetingScan>().SelectNewEntity(Rotate.ReadValue<Vector2>());
            }
            else if (context.canceled)
            {
                ch_in_slot_array[select_active].GetComponent<TargetingScan>().ResetNewSelectionMade();
            }
        }
    }

    private void DisableActiveReturnLines()
    {
        
        List<GameObject> entities = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));

        entities.AddRange(new List<GameObject>(GameObject.FindGameObjectsWithTag("Character")));

        entities.AddRange(new List<GameObject>(GameObject.FindGameObjectsWithTag("Item")));

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

    public void SetCoreObj(GameObject obj)
    {
        core_obj_ref = obj;
    }

    public void SetCharactersInSlots(GameObject[] slots)
    {
        ch_in_slot_array = slots;

        _core_moveInput = core_obj_ref.GetComponent<MoveInput>();

    }

}

