using TRIA.Core.Constants;
using UnityEngine;

namespace TRIA.Enemies
{
    public class BossTrigger : MonoBehaviour
    {
        [SerializeField]
        private Boss1 boss; // You can change this to a base Boss class later

        [SerializeField]
        private bool resetOnExit = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Tags.Player))
            {
                boss.ActivateBoss();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (resetOnExit && other.CompareTag(Tags.Player))
            {
                boss.ResetBoss();
            }
        }
    }
}
