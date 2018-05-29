using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public string Name = "Space AK";

    public int Damage = 10;
    public float Range = 1000f;

    public float FireRate = 0f;

    public int MaxBullets = 30;

    [HideInInspector] public int Bullets;

    public float ReloadTime = 2f;

    public GameObject Graphics;

    public PlayerWeapon()
    {
        Bullets = MaxBullets;
    }
}  