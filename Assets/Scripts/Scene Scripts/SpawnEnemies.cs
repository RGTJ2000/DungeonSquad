using Unity.VisualScripting;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{

    public GameObject enemy_prefab;

    [SerializeField] int number_of_enemies = 5;
    private float boundmax_x = 20f;
    private float boundmax_z = 20f;
    private int max_attempts = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        for (int i = 0; i <number_of_enemies; i++)
        {
           

            for(int j = 0; j < max_attempts; j++)
            {
                Vector3 random_position = new Vector3(Random.Range(-boundmax_x, boundmax_x), 0, Random.Range(-boundmax_z, boundmax_z));
                if (!Physics.CheckSphere(random_position+Vector3.up, 0.5f))
                {

                    GameObject thisEnemy = Instantiate(enemy_prefab, random_position, Quaternion.identity);
                    break;
                }
            }

        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
