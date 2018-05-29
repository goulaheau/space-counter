using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _layerMask;
    
    private PlayerWeapon _currentWeapon;
    private WeaponManager _weaponManager;
    
    private void Start()
    {
        if (_camera == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced!");
            enabled = false;
        }

        _weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        _currentWeapon = _weaponManager.GetCurentWeapon();

        if (Pause.IsOn)
        {
            return;
        }

        if (_currentWeapon.Bullets < _currentWeapon.MaxBullets)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _weaponManager.Reload();
                return;
            }
        }
       
        if (_currentWeapon.FireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / _currentWeapon.FireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }   
    }

    // Is called on the server when a player shoot
    [Command]
    private void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    // Is called on all clients when we need to do a shoot effect
    [ClientRpc]
    private void RpcDoShootEffect()
    {
        _weaponManager.GetCurrentGraphics().MuzzleFlash.Play();
    }

    // Is called on the server when we hit something
    // Takes in the hit point and the normal of the surface
    [Command]
    private void CmdOnHit(Vector3 position, Vector3 normal)
    {
        RpcDoHitEffect(position, normal);
    }
    
    // Is called on all clients
    // Here we can spawn in cool effects
    [ClientRpc]
    private void RpcDoHitEffect(Vector3 position, Vector3 normal)
    {
        var hitEffect = Instantiate(_weaponManager.GetCurrentGraphics().HitEffectPrefab, position, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }
    
    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer && !_weaponManager.IsReloading)
            return;


        if (_currentWeapon.Bullets <= 0)
        {
            _weaponManager.Reload();
            return;
        }

        --_currentWeapon.Bullets;

        // We are shooting, call the OnShoot method on the server
        CmdOnShoot();
        
        RaycastHit hit;

        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, _currentWeapon.Range, _layerMask))
        {
            if (hit.collider.CompareTag(PlayerTag))
            {
                CmdPlayerShot(hit.collider.name, _currentWeapon.Damage);
            }
            
            // We hit something, call the OnHit method on the server
            CmdOnHit(hit.point, hit.normal);
        }
    }

    [Command]
    private void CmdPlayerShot(string playerId, int damage)
    {
        var player = GameManager.GetPlayer(playerId);

        player.RpcTakeDamage(damage);
    }
}