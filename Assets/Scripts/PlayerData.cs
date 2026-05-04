using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public float walkingSpeed;
    public float hitDamage;
    public float shootDamage;
    public float maxHealth;
    public int speedUpgrades = 0;
    public int attackUpgrades = 0;
    public int healthUpgrades = 0;

    public PlayerData(Player player)
    {
        level = player.level;
        walkingSpeed = player.controller.walkingSpeed;
        hitDamage = player.attackSystem.hitDamage;
        shootDamage = player.shootSystem.bulletDamage;
        maxHealth = player.healthSystem.maxHealth- player.healthSystem.runMaxHealth;
        speedUpgrades = player.speedUpgrades;
        attackUpgrades = player.attackUpgrades;
        healthUpgrades = player.healthUpgrades;
    }
    public void SetUpPlayer(Player player)
    {
        player.controller.AwakePlayer(walkingSpeed,speedUpgrades);
        player.level = level;
        player.speedUpgrades = speedUpgrades;
        player.attackUpgrades = attackUpgrades;
        player.healthUpgrades = healthUpgrades;
        player.attackSystem.SetUpAttack(hitDamage, attackUpgrades);
        player.healthSystem.AwakeHealth(maxHealth,healthUpgrades);
        player.shootSystem.SetUpShoot(shootDamage, attackUpgrades);
        player.moneySystem.ResetMoney();
        player.playerObj.SetActive(true);
    }
}
