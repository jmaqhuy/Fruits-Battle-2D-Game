using UnityEngine;

namespace Resources
{
    public static class ResourceLoader
    {
        public static Sprite LoadRankSprite(string spriteName)
        {
            return UnityEngine.Resources.Load<Sprite>($"Ranks/{spriteName}");
        }
    }
}