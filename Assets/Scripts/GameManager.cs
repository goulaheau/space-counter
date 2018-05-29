using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public MatchSettings MatchSettings;

    [SerializeField] private GameObject _sceneCamera;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one GameManager in scene.");
        }
        else
        {
            Instance = this;            
        }
    }

    public void SetSceneCameraActive(bool isActive)
    {
        if (_sceneCamera == null)
            return;
        
        _sceneCamera.SetActive(isActive);
    }

    #region Player tracking

    public static Dictionary<string, Player> Players
    {
        get { return _players; }
        set { _players = value; }
    }

    private const string PlayerIdPrefix = "Player ";
    
    private static Dictionary<string, Player> _players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string netId, Player player)
    {
        var playerId = PlayerIdPrefix + netId;
        
        _players.Add(playerId, player);

        player.transform.name = playerId;
    }

    public static void UnRegisterPlayer(string playerId)
    {
        _players.Remove(playerId);
    }

    public static Player GetPlayer(string playerId)
    {
        return _players[playerId];
    }

    public static Player[] GetPlayers()
    {
        return _players.Values.ToArray();
    }
    
    #endregion
}