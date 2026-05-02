using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject playerObj;
    public PlayerController controller;
    public PlayerHealthSystem healthSystem;
    public PlayerMoneySystem moneySystem;
    public PlayerAttack attackSystem;
    public PlayerShoot shootSystem;
    public int level=0;
    public int speedUpgrades = 0;
    public int attackUpgrades = 0;
    public int healthUpgrades = 0;
    public Camera cam;
    public GameObject isometricCam;
}
