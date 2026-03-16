using TRIA.Enemies;
using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    [SerializeField] private MonoBehaviour boss;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            boss.SendMessage("ActivateFromRoom");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            boss.SendMessage("ResetFromRoom");
        }
    }
}