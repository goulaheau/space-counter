using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class JoinGame : MonoBehaviour
{
    private List<GameObject> _roomList = new List<GameObject>();

    [SerializeField] private Text _status;

    [SerializeField] private GameObject _roomListItemPrefab;

    [SerializeField] private Transform _roomListParent;

    private NetworkManager _networkManager;

    private void OnEnable()
    {
        if (Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Cursor.visible != true)
        {
            Cursor.visible = true;
        }
    }

    public void RefreshRoomList()
    {
        ClearRoomList();

        if (_networkManager.matchMaker == null)
        {
            _networkManager.StartMatchMaker();
        }

        _networkManager.matchMaker.ListMatches(
            0, 
            20, 
            "", 
            true, 
            0, 
            0, 
            OnMatchList
        );

        _status.text = "Loading...";
    }

    public void JoinRoom(MatchInfoSnapshot match)
    {
        _networkManager.matchMaker.JoinMatch(
            match.networkId,
            "",
            "",
            "",
            0,
            0,
            _networkManager.OnMatchJoined
        );
        
        StartCoroutine(WaitForJoin());
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        _status.text = "";

        if (!success || matches == null)
        {
            _status.text = "Couldn't get room list.";
            return;
        }

        ClearRoomList();

        foreach (var match in matches)
        {
            var roomListItemGO = Instantiate(_roomListItemPrefab);
            roomListItemGO.transform.SetParent(_roomListParent);

            var roomListItem = roomListItemGO.GetComponent<RoomListItem>();

            if (roomListItem != null)
            {
                roomListItem.Setup(match, JoinRoom);
            }

            // as well as setting up a callback function that will join the game.
            
            _roomList.Add(roomListItemGO);
        }

        if (_roomList.Count == 0)
        {
            _status.text = "No rooms at the moment.";
        }
    }

    private void ClearRoomList()
    {
        foreach (var room in _roomList)
        {
            Destroy(room);
        }

        _roomList.Clear();
    }

    private void Start()
    {
        _networkManager = NetworkManager.singleton;
        
        if (_networkManager.matchMaker == null)
        {
            _networkManager.StartMatchMaker();
        }
        
        RefreshRoomList();
    }
    
    private IEnumerator WaitForJoin()
    {
        ClearRoomList();

        var countdown = 10;

        while (countdown > 0)
        {
            _status.text = "Joining... (" + countdown + ")";

            yield return new WaitForSeconds(1f);

            --countdown;
        }

        // Failed to connect
        _status.text = "Failed to connect";

        yield return new WaitForSeconds(1f);

        var matchInfo = _networkManager.matchInfo;

        if (matchInfo != null)
        {
            _networkManager.matchMaker.DropConnection(
                matchInfo.networkId,
                matchInfo.nodeId,
                0,
                _networkManager.OnDropConnection
            );

            _networkManager.StopHost();
        }

        RefreshRoomList();
    }
}