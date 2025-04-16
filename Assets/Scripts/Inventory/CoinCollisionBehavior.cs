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
                SoundManager.Instance.PlaySoundByKeyAtPosition("coinCollect_v2", transform.position, SoundCategory.sfx);
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
