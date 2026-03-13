using System.Collections.Generic;
using UnityEngine;

namespace TRIA.Player
{
    public enum AbilityType
    {
        DoubleJump,
        Dash,
        WallClimb,
        HeavySlam,
    }

    public class PlayerAbilityHandler : MonoBehaviour
    {
        // Ideally, this list is loaded from a Save Data manager when the game boots.
        [SerializeField]
        private List<AbilityType> unlockedAbilities = new List<AbilityType>();

        public void UnlockAbility(AbilityType ability)
        {
            if (!unlockedAbilities.Contains(ability))
            {
                unlockedAbilities.Add(ability);
                Debug.Log($"TRIA Unlocked: {ability}");
            }
        }

        public bool HasAbility(AbilityType ability)
        {
            return unlockedAbilities.Contains(ability);
        }
    }
}
