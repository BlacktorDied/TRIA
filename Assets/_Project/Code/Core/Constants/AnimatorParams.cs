using UnityEngine;

namespace TRIA.Core.Constants
{
    public static class AnimatorParams
    {
        // Using StringToHash caches the parameter IDs for maximum performance
        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        public static readonly int IsDashing = Animator.StringToHash("IsDashing");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Hurt = Animator.StringToHash("Hurt");
        public static readonly int Dead = Animator.StringToHash("Dead");
    }
}
