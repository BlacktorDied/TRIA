using UnityEngine;

namespace TRIA.Player.Data
{
    [CreateAssetMenu(fileName = "NewPlayerData", menuName = "TRIA/Player/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [Header("Movement")]
        public float moveSpeed = 8f;
        public float acceleration = 50f;
        public float decceleration = 50f;

        [Header("Jump")]
        public float jumpForce = 12f;
        public float gravityScale = 3f;
        public float fallGravityMultiplier = 1.5f;
    }
}
