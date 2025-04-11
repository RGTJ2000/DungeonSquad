using UnityEngine;

public class CoinCollisionBehavior : MonoBehaviour
{
    [SerializeField] CoinType _coinType = CoinType.gold;
    public bool isGrounded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded && collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }

        if (isGrounded && collision.gameObject.CompareTag("Character"))
        {
            if (gameObject != null)
            {
                Inventory _inventory = collision.gameObject.GetComponent<Inventory>();
                if (_inventory != null) 
                {
                    _inventory.ReceiveCoin(_coinType, 1);
                
                }

                Destroy(gameObject);

            }
        }
    }
}
