using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogWarning("no se encontro ningun objeto con el tag 'player'");
    }

    void Update()
    {
        if (player == null) return;

        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPos);
    } 
}