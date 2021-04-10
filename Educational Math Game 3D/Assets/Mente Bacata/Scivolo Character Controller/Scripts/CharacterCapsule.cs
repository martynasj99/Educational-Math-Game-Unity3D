using UnityEngine;
using MenteBacata.ScivoloCharacterController.Internal;

namespace MenteBacata.ScivoloCharacterController
{
    public class CharacterCapsule : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Vertical offset from the game object position to the bottom of the capsule.")]
        private float verticalOffset = 0f;

        [SerializeField]
        private float height = 2f;

        [SerializeField]
        private float radius = 0.5f;


        /// <summary>
        /// Vertical offset from the game object position to the bottom of the capsule.
        /// </summary>
        public float VerticalOffset
        {
            get => verticalOffset;
            set => verticalOffset = value;
        }

        /// <summary>
        /// Height of the capsule.
        /// </summary>
        public float Height
        {
            get => height;
            set => height = Mathf.Max(2f * radius, value);
        }

        /// <summary>
        /// Radius of the capsule.
        /// </summary>
        public float Radius
        {
            get => radius;
            set => radius = Mathf.Max(0f, value);
        }

        /// <summary>
        /// Capsule up direction.
        /// </summary>
        public Vector3 UpDirection
        {
            get => transform.up;
            set => transform.up = value;
        }

        /// <summary>
        /// Rotation of the capsule.
        /// </summary>
        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        /// <summary>
        /// World space center of the capsule.
        /// </summary>
        public Vector3 Center => transform.position + transform.TransformVector(LocalCenter);

        /// <summary>
        /// World space center of the capsule lower hemisphere.
        /// </summary>
        public Vector3 LowerHemisphereCenter => transform.position + ToLowerHemisphereCenter;

        /// <summary>
        /// World space center of the capsule upper hemisphere.
        /// </summary>
        public Vector3 UpperHemisphereCenter => transform.position + ToUpperHemisphereCenter;

        /// <summary>
        /// World space vector from game object position to the center of the capsule lower hemisphere.
        /// </summary>
        internal Vector3 ToLowerHemisphereCenter => transform.TransformVector(0f, verticalOffset + radius, 0f);

        /// <summary>
        /// World space vector from game object position to the center of the capsule upper hemisphere.
        /// </summary>
        internal Vector3 ToUpperHemisphereCenter => transform.TransformVector(0f, verticalOffset + height - radius, 0f);

        internal CapsuleCollider Collider { get; private set; }

        internal Rigidbody Rigidbody { get; private set; }

        private Vector3 LocalCenter => new Vector3(0f, verticalOffset + 0.5f * height, 0f);


        private void Awake()
        {
            DoPreliminaryCheck();
            InstantiateComponents();
        }
        
        private void DoPreliminaryCheck()
        {
            if (!Mathf.Approximately(transform.lossyScale.x, 1f) ||
                !Mathf.Approximately(transform.lossyScale.y, 1f) ||
                !Mathf.Approximately(transform.lossyScale.z, 1f))
            {
                Debug.LogWarning($"{nameof(CharacterCapsule)}: Object scale is not (1, 1, 1).");
            }
            
            foreach (var col in gameObject.GetComponentsInChildren<Collider>(true))
            {
                if (col != Collider && !col.isTrigger && !Physics.GetIgnoreLayerCollision(gameObject.layer, col.gameObject.layer))
                {
                    Debug.LogWarning($"{nameof(CharacterCapsule)}: Found other colliders on this gameobject or in its childrens.");
                    break;
                }
            }
        }

        private void InstantiateComponents()
        {
            Collider = gameObject.AddComponent<CapsuleCollider>();
            Collider.center = LocalCenter;
            Collider.height = height;
            Collider.radius = radius;
            Collider.direction = 1; // Asse Y

            Rigidbody = gameObject.AddComponent<Rigidbody>();
            Rigidbody.isKinematic = true;
        }
        
        private void OnValidate()
        {
            Height = height;
            Radius = radius;

            if (Collider != null)
            {
                Collider.center = LocalCenter;
                Collider.height = height;
                Collider.radius = radius;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = GizmosUtility.defaultColliderColor;
            GizmosUtility.DrawWireCapsule(LowerHemisphereCenter, UpperHemisphereCenter, radius);
        }
    }
}
