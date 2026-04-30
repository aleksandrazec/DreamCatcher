using TMPro;
using UnityEngine;

public class PlayerMoneySystem : MonoBehaviour
{
    public int money;
    public TMP_Text text;
    private void Awake()
    {
        ResetMoney();
    }
    public void ResetMoney()
    {
        money = 0;
        text.text = money.ToString();
    }
    public void GainMoney(int amount)
    {
        money += amount;
        text.text = money.ToString();
    }
    public bool SpendMoney(int amount)
    {
        if (money - amount >= 0)
        {
            money -= amount;
            text.text = money.ToString();
            return true;
        }
        return false;
    }
}
