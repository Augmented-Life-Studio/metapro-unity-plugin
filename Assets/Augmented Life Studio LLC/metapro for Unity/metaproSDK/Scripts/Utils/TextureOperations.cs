using UnityEngine;

namespace metaproSDK.Scripts.Utils
{
    public class TextureOperations
    {
        public static Sprite TextureToSprite(Texture2D texture) => 
            Sprite.Create(
                texture, new Rect(0f, 0f, texture.width, texture.height), 
                new Vector2(0.5f, 0.5f), 50f, 0, SpriteMeshType.FullRect);
    }
}