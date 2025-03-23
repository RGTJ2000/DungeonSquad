using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemy_prefabs_array;
   
    [SerializeField] int number_of_enemies = 5;

    private int max_attempts = 100; //number of attempts to spawn each enemy


    //Level boundaries, to be read from the level creator object/script after level is created
    [SerializeField] private float boundmax_x = 20f;
    [SerializeField] private float boundmax_z = 20f;

    public void SpawnEnemies()
    {

        for (int i = 0; i < number_of_enemies; i++)
        {
            GameObject enemyToInstantiate = GetRandomEnemyPrefab();
            EntityStats _entityStats = enemyToInstantiate.GetComponent<EntityStats>();

            bool spawnSuccssful = false;

            for (int j = 0; j < max_attempts; j++)
            {

                Vector3 random_position = new Vector3(Random.Range(-boundmax_x, boundmax_x), 0, Random.Range(-boundmax_z, boundmax_z));

                if (!Physics.CheckSphere(random_position + Vector3.up, _entityStats.entity_radius))
                {
                    Debug.Log("Spawning Enemy#"+i+" :"+enemyToInstantiate.name + "location:"+random_position);
                    
                    Instantiate(enemyToInstantiate, random_position+Vector3.up, Quaternion.identity);
                    spawnSuccssful = true;
                    break;
                }
            }

            if (!spawnSuccssful) 
            {
                Debug.Log("Spawn #" + i+ " failed");
            }

        }


    }

    private GameObject GetRandomEnemyPrefab()
    {
        return enemy_prefabs_array[Random.Range(0, enemy_prefabs_array.Length)];
    }
   
}
