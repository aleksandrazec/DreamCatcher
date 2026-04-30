using UnityEngine;

public class Coin : MonoBehaviour
{
    private int amount=0;
    private PlayerMoneySystem playerMoneySystem;
    public void SetAmount(int amount, PlayerMoneySystem playerMoneySystem)
    {
        this.amount = amount;
        this.playerMoneySystem = playerMoneySystem;
    }
    private void OnTriggerEnter(Collider other)
    {
        playerMoneySystem.GainMoney(amount);
        Destroy(gameObject);
    }
}
