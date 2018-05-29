using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour
{
    [SerializeField] private uint _roomSize = 6;

    private string _roomName;

    private NetworkManager _networkManager;

    public void SetRoomName(string roomName)
    {       
        _roomName = roomName;
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(_roomName))
        {
            return;
        }
        
        _networkManager.matchMaker.CreateMatch(_roomName, _roomSize, true, "", "", "", 0, 0, _networkManager.OnMatchCreate);
    }
    
    private void Start()
    {
        _networkManager = NetworkManager.singleton;
        
        if (_networkManager.matchMaker == null)
        {
            _networkManager.StartMatchMaker();
        }
    }
}