using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private string _weaponLayerName = "Weapon";

    [SerializeField] private Transform _weaponHolderLocal;
    [SerializeField] private Transform _weaponHolderRemote;

    [SerializeField] private PlayerWeapon[] _playerWeapons;

    private PlayerWeapon _currentWeapon;
    private WeaponGraphics _currentGraphics;

    public bool IsReloading;
    
    public int SelectedWeapon;

    public PlayerWeapon GetCurentWeapon()
    {
        return _currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return _currentGraphics;
    }
    
    // Select the first weapon.
    private void Start()
    {
        SelectWeapon();
    }

    // Handle the switch of weapons.
    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        
        var previousSelectedWeapon = SelectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (SelectedWeapon >= _playerWeapons.Length - 1)
            {
                SelectedWeapon = 0;
            }
            else
            {
                ++SelectedWeapon;   
            }            
        }
        
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (SelectedWeapon <= 0)
            {
                SelectedWeapon = _playerWeapons.Length - 1;
            }
            else
            {
                --SelectedWeapon;   
            }            
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectedWeapon = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2) && _playerWeapons.Length >= 2)
        {
            SelectedWeapon = 1;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3) && _playerWeapons.Length >= 3)
        {
            SelectedWeapon = 2;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4) && _playerWeapons.Length >= 4)
        {
            SelectedWeapon = 3;
        }

        // Check if there was a change.
        if (previousSelectedWeapon != SelectedWeapon)
        {
            SelectWeapon();
        }
    }

    // Select and Equip the weapon.
    private void SelectWeapon()
    {
        for (var i = 0; i < _playerWeapons.Length; ++i)
        {
            if (i == SelectedWeapon)
            {
                EquipWeapon(i);
                break;
            }
        }
    }

    // Equip weapon by instancing it and deleting the previous one.
    private void EquipWeapon(int weaponIndex)
    {
        var weapon = _playerWeapons[weaponIndex];
        
        _currentWeapon = weapon;

        GameObject weaponInstance;

        if (isLocalPlayer)
        {
            foreach (Transform child in _weaponHolderLocal.transform)
            {
                Destroy(child.gameObject);
            }
            
            weaponInstance = Instantiate(weapon.Graphics, _weaponHolderLocal.position, Quaternion.Euler(0, 0, 0));
            weaponInstance.transform.SetParent(_weaponHolderLocal);
            Util.SetLayerRecursively(weaponInstance, LayerMask.NameToLayer(_weaponLayerName));
        }
        else
        {
            foreach (Transform child in _weaponHolderRemote.transform)
            {
                Destroy(child.gameObject);
            }
            
            weaponInstance = Instantiate(weapon.Graphics, _weaponHolderRemote.position, Quaternion.Euler(0, 0, 0));
            weaponInstance.transform.SetParent(_weaponHolderRemote);
        }

        _currentGraphics = weaponInstance.GetComponent<WeaponGraphics>();

        if (_currentGraphics == null)
        {
            Debug.LogError("No WeaponGraphics component on the weapon object: " + weaponInstance.name);
        }
    }

    public void Reload()
    {
        if (IsReloading)
        {
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }
    private IEnumerator ReloadCoroutine()
    {
        IsReloading = true;
        
        CmdOnReload();
        
        yield return new WaitForSeconds(_currentWeapon.ReloadTime);

        _currentWeapon.Bullets = _currentWeapon.MaxBullets;

        IsReloading = false;
    }

    [Command]
    private void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    private void RpcOnReload()
    {
        var animator = _currentGraphics.GetComponent<Animator>();

        if (animator == null)
        {
            return;
        }
        
        animator.SetTrigger("Reload");
    }
}