using UnityEngine;
using UnityEngine.Networking;

public class Pause : MonoBehaviour
{
    public static bool IsOn = false;

    private NetworkManager _networkManager;

    private void Start()
    {
        _networkManager = NetworkManager.singleton;
    }

    public void LeaveRoom()
    {
        var matchInfo = _networkManager.matchInfo;

        _networkManager.matchMaker.DropConnection(
            matchInfo.networkId, 
            matchInfo.nodeId, 
            0, 
            _networkManager.OnDropConnection
        );
        
        _networkManager.StopHost();
    }
}