using UnityEngine;

namespace MenteBacata.ScivoloCharacterController.Internal
{
    public static class GameObjectExtensions
    {
        public static LayerMask CollisionMask(this GameObject gameObject)
        {
            int mask = 0;

            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(gameObject.layer, i))
                {
                    mask |= (1 << i);
                }
            }

            return mask;
        }
    }
}
