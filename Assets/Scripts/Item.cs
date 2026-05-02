using UnityEngine;

public class Item : MonoBehaviour
{
    public bool isForSale=false;
    public int cost;
    public PlayerMoneySystem playerMoneySystem;
    public Type type;
    public PlayerHealthSystem playerHealthSystem;
    public PlayerController playerController;
    public TextMesh costText;
    public GameObject price;
    public GameObject nameText;
    public Billboard billboard;
    public enum Type
    {
        MaxHealth,
        Heal,
        NoDashCooldown,
        InvincibleDash,
        DamageCooldown
    }
    private void Awake()
    {
        GameObject[] playerObject = GameObject.FindGameObjectsWithTag("Player");
        if (isForSale && playerMoneySystem==null)
        {
            playerMoneySystem = playerObject[0].GetComponent<PlayerMoneySystem>();
        }
        if ((type == Type.MaxHealth || type==Type.Heal || type==Type.DamageCooldown) && playerHealthSystem==null)
        {
            playerHealthSystem = playerObject[0].GetComponent<PlayerHealthSystem>();
        }
        if ((type == Type.NoDashCooldown || type == Type.InvincibleDash) && playerController == null)
        {
            playerController = playerObject[0].GetComponent<PlayerController>();
        }
        if (billboard.cam == null)
        {
            GameObject[] mainCamera = GameObject.FindGameObjectsWithTag("MainCamera");
            SetBillboardCamera(mainCamera[0]);
        }
    }
    public void SetBillboardCamera(GameObject cam)
    {
        billboard.cam = cam.transform;
    }
    public void UseItem()
    {
        if (isForSale)
        {
            var bought = playerMoneySystem.SpendMoney(cost);
            if (!bought)
            {
                return;
            }
        }
        switch (type)
        {
            case Type.MaxHealth:
                MaxHealth();
                break;
            case Type.Heal:
                Heal();
                break;
            case Type.NoDashCooldown:
                NoDashCooldown(); 
                break;
            case Type.InvincibleDash:
                InvincibleDash();
                break;
            case Type.DamageCooldown:
                DamageCooldown();
                break;
            default:
                break;
        }
        Destroy(gameObject);
    }
    public void SetCost(int cost)
    {
        this.cost=cost;
        isForSale = true;
        costText.text = cost.ToString();
        price.SetActive(true);
        GameObject[] playerObject = GameObject.FindGameObjectsWithTag("Player");
        playerMoneySystem = playerObject[0].GetComponent<PlayerMoneySystem>();
    }
    public void SetTextVisible()
    {
        nameText.SetActive(true);
    }
    public void SetTextInvisible()
    {
        nameText.SetActive(false);
    }
    private void MaxHealth()
    {
        playerHealthSystem.MoreMaxHealth(50);
    }
    private void Heal()
    {
        playerHealthSystem.Heal(Mathf.CeilToInt(playerHealthSystem.health * 30 / 100));
    }
    private void DamageCooldown()
    {
        playerHealthSystem.AddDamageCooldown(1f);
    }

    private void NoDashCooldown()
    {
        playerController.SetDashCooldown(0f);
    }
    private void InvincibleDash()
    {
        playerController.InvincibleDash();
    }
}
