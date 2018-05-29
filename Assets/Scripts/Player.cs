using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Player : NetworkBehaviour
{
    [SyncVar] private bool _isDead;

    public bool IsDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField] private int _maxHealth = 100;

    [SyncVar] public int CurrentHealth;

    [SerializeField] private Behaviour[] _disableOnDeath;
    [SerializeField] private GameObject[] _disableGameObjectsOnDeath;

    [SerializeField] private Animator _animator;

    private bool[] _wasEnabled;

    private bool _firstSetup = true;

    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            // Switch cameras
            GameManager.Instance.SetSceneCameraActive(false);
        }

        CmdBroadcastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (_firstSetup)
        {
            _wasEnabled = new bool[_disableOnDeath.Length];

            for (var i = 0; i < _wasEnabled.Length; ++i)
            {
                _wasEnabled[i] = _disableOnDeath[i].enabled;
            }

            _firstSetup = false;
        }

        SetDefaults();
    }

    [ClientRpc]
    public void RpcTakeDamage(int amount)
    {
        if (_isDead)
        {
            return;
        }

        CurrentHealth -= amount;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;
        
        // Disable components
        foreach (var component in _disableOnDeath)
        {
            component.enabled = false;
        }

        // Disable game objects
        foreach (var component in _disableGameObjectsOnDeath)
        {
            component.SetActive(false);
        }

        // Disable collider
        var col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Start the death animation
        _animator.SetBool("Dying", IsDead);

        // Switch cameras
        if (isLocalPlayer)
        {
            GameManager.Instance.SetSceneCameraActive(true);
        }

        StartCoroutine(Respawn());
    }

    private IEnumerator<WaitForSeconds> Respawn()
    {
        yield return new WaitForSeconds(GameManager.Instance.MatchSettings.RespawnTime);

        var startPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();
    }

    private void SetDefaults()
    {
        IsDead = false;
        _animator.SetBool("Dying", IsDead);

        CurrentHealth = _maxHealth;

        // Enable the components
        for (var i = 0; i < _disableOnDeath.Length; ++i)
        {
            _disableOnDeath[i].enabled = _wasEnabled[i];
        }

        // Enable the game objects
        for (var i = 0; i < _disableGameObjectsOnDeath.Length; ++i)
        {
            _disableGameObjectsOnDeath[i].SetActive(true);
        }

        // Enable the collider
        var col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }
}