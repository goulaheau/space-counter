using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject _pause;
    [SerializeField] private Text _ammoText;
    [SerializeField] private Text _healthText;

    private Player _player;
    private WeaponManager _weaponManager;

    public void SetPlayer(Player player)
    {
        _player = player;
        _weaponManager = _player.GetComponent<WeaponManager>();
    }
    
    public void TogglePause()
    {
        _pause.SetActive(!_pause.activeSelf);
        Pause.IsOn = _pause.activeSelf;
    }
    
    private void Update()
    {
        HandleCursor();
        
        SetAmmoAmount(
            _weaponManager.GetCurentWeapon().Bullets, 
            _weaponManager.GetCurentWeapon().MaxBullets
        );
        
        SetHealthAmmount(_player.CurrentHealth);
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void Start()
    {
        Pause.IsOn = false;
    }

    private static void HandleCursor()
    {
        if (Pause.IsOn)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (!Cursor.visible)
            {
                Cursor.visible = true;
            }
        }
        else
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (Cursor.visible)
            {
                Cursor.visible = false;
            }
        }
    }

    private void SetAmmoAmount(int amount, int total)
    {
        _ammoText.text = amount + "/" + total;     
    }
    
    private void SetHealthAmmount(int amount)
    {
        _healthText.text = amount.ToString();     
    }
}