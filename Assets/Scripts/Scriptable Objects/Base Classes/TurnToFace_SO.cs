using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "TurnToFace_SO", menuName = "Enemy Behavior/TurnToFace_SO")]
public class TurnToFace_SO : AlertBehavior_SO
{
    public override void Perform(GameObject enemy_obj, ScanForCharacters _scanForCharacters, NavMeshAgent _navMeshAgent, EntityStats _entityStats, float range)
    {
        
        if (enemy_obj != null)
        {
            if (_scanForCharacters.targeted_character == null)
            {
                if (_scanForCharacters.SetAndReturnNearestCharacter(range) != null)
                {
                    Vector3 target_direction = (_scanForCharacters.targeted_character.transform.position - enemy_obj.transform.position).normalized;
                    target_direction.y = 0;

                    FaceTarget(enemy_obj, target_direction);
                }
            }
            else
            {
                //check if target is visible
                if (_scanForCharacters.CheckCharacterIsVisible(_scanForCharacters.targeted_character, range))
                {
                    Vector3 target_direction = (_scanForCharacters.targeted_character.transform.position - enemy_obj.transform.position).normalized;
                    target_direction.y = 0;


                    FaceTarget(enemy_obj, target_direction);
                }
            }
        }
        

        /*
        
        if (enemy_obj != null)
        {
            
            if (_scanForCharacters.targeted_character != null)
            {
                //check if visible
                if (_scanForCharacters.CheckCharacterIsVisible(_scanForCharacters.targeted_character, _entityStats.visible_distance))
                {
                    Vector3 target_direction = (_scanForCharacters.targeted_character.transform.position - enemy_obj.transform.position).normalized;
                    target_direction.y = 0;


                    FaceTarget(enemy_obj, target_direction);
                }
                else
                {
                    if(_scanForCharacters.SetAndReturnNearestCharacter(_entityStats.visible_distance) != null)
                    {
                        Vector3 target_direction = (_scanForCharacters.targeted_character.transform.position - enemy_obj.transform.position).normalized;
                        target_direction.y = 0;

                        FaceTarget(enemy_obj, target_direction);
                    }
                }

            }
            else
            {
                if (_scanForCharacters.SetAndReturnNearestCharacter(_entityStats.visible_distance) != null)
                {
                    Vector3 target_direction = (_scanForCharacters.targeted_character.transform.position - enemy_obj.transform.position).normalized;
                    target_direction.y = 0;


                    FaceTarget(enemy_obj, target_direction);
                }
            }

        }
        */
    }

    private void FaceTarget(GameObject origin_obj, Vector3 target_direction)
    {


        if (target_direction != Vector3.zero) // Avoid errors if direction is zero
        {
            Quaternion targetRotation = Quaternion.LookRotation(target_direction);
            origin_obj.transform.rotation = Quaternion.Slerp(origin_obj.transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }
}
