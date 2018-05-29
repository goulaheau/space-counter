using UnityEngine;

public class HeadFollower : MonoBehaviour
{
    [SerializeField] private Transform _head;
    [SerializeField] private Player _player;

    private void Start()
    {
        if (!_player.isLocalPlayer)
        {
            enabled = false;
        }
    }

    // Follow the head when the player crouched
    private void Update()
    {
        if (_head == null)
        {
            Debug.LogError("HeadFollower : no Head serialized.");
            return;
        }
        
        var difference = _head.transform.position.y - transform.position.y;

        if (!(Mathf.Abs(difference) > 0.03f))
            return;

        var position = transform.position;
        position.y = transform.position.y + difference * 0.7f;
        transform.position = position;
    }
}