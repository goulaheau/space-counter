using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    public delegate void JoinRoomDelegate(MatchInfoSnapshot match);

    private JoinRoomDelegate _joinRoomCallback;
    
    [SerializeField] private Text _roomNameText;
    
    private MatchInfoSnapshot _match;

    public void Setup(MatchInfoSnapshot match, JoinRoomDelegate joinRoomCallback)
    {
        _match = match;
        _joinRoomCallback = joinRoomCallback;

        _roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
    }

    public void JoinRoom()
    {
        _joinRoomCallback.Invoke(_match);
    }
}