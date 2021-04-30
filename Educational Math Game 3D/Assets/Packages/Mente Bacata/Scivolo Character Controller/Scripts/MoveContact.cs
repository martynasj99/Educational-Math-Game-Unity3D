using UnityEngine;

namespace MenteBacata.ScivoloCharacterController
{
    public struct MoveContact
    {
        /// <summary>
        /// World space position of the contact point.
        /// </summary>
        public readonly Vector3 position;

        /// <summary>
        /// Normal of the plane tangent to the capsule at the contact point.
        /// </summary>
        public readonly Vector3 normal;

        /// <summary>
        /// Collider of the object on which the contact happened.
        /// </summary>
        public readonly Collider collider;

        public MoveContact(Vector3 position, Vector3 normal, Collider collider)
        {
            this.position = position;
            this.normal = normal;
            this.collider = collider;
        }
    }
}
