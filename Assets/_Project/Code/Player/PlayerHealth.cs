using UnityEngine;

namespace TRIA.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField]
        public int healthPoints;

        [SerializeField]
        public int maxHEalthPoints;

        public void TakeDamage(int _damage)
        {
            healthPoints -= _damage;
        }
    }
}
