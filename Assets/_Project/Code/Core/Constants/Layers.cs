using UnityEngine;

namespace TRIA.Core.Constants
{
    public static class Layers
    {
        public const string Player = "Player";
        public const string Enemy = "Enemy";
        public const string Ground = "Ground";
        public const string OneWayPlatform = "OneWayPlatform";
        public const string Projectile = "Projectile";

        // A handy helper method to get the actual integer index for raycasts and physics checks
        public static int GetLayerIndex(string layerName)
        {
            return LayerMask.NameToLayer(layerName);
        }
    }
}
