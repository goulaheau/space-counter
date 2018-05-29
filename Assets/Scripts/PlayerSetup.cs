using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] private Behaviour[] _componentsToDisable;
    
    [SerializeField] private string _remotePlayerLayerName = "RemotePlayer";
    [SerializeField] private string _dontDrawLayerName = "DontDraw";
    [SerializeField] private GameObject _playerGraphics;

    [SerializeField] private GameObject _playerUIPrefab;
    private GameObject _playerUIInstance;

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        GameManager.RegisterPlayer(netId.ToString(), GetComponent<Player>());
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemotePlayerLayer();
        }
        else
        {
            // Disable player graphics for local player
            SetLayerRecursively(_playerGraphics, LayerMask.NameToLayer(_dontDrawLayerName));
            
            // Create PlayerUI
            _playerUIInstance = Instantiate(_playerUIPrefab);
            _playerUIInstance.name = _playerUIPrefab.name;
            
            // Configure PlayerUI
            var playerUI = _playerUIInstance.GetComponent<PlayerUI>();
            if (playerUI == null)
            {
                Debug.LogError("PlayerSetup: No PlayerUI component on PlayerUI Prefab.");
            }
            else
            {
                playerUI.SetPlayer(GetComponent<Player>());
            }
            
            GetComponent<Player>().SetupPlayer();
        }
    }

    private static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    private void AssignRemotePlayerLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(_remotePlayerLayerName);
    }

    private void DisableComponents()
    {
        foreach (var component in _componentsToDisable)
        {
            component.enabled = false;
        }
    }

    private void OnDisable()
    {
        Destroy(_playerUIInstance);

        if (isLocalPlayer)
        {
            GameManager.Instance.SetSceneCameraActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }
}