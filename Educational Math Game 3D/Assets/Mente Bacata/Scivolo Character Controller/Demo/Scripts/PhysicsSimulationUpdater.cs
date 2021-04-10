using UnityEngine;

namespace MenteBacata.ScivoloCharacterControllerDemo
{
    [DefaultExecutionOrder(3000)]
    public class PhysicsSimulationUpdater : MonoBehaviour
    {
        // It can happen that deltaTime is 0 even if timeScale is not (for instance when in the same frame timeScale turns from 0 
        // to 1). Since Physics.Simulate throws an error when called with a step of 0, it guarantees that a minimum value for 
        // deltaTime is used.
        public float DeltaTime
#if UNITY_EDITOR
            => UnityEditor.EditorApplication.isPaused ? Time.fixedDeltaTime : Mathf.Max(Time.deltaTime, 1E-05f);
#else
            => Mathf.Max(Time.deltaTime, 1E-05f);
#endif

        void Update()
        {
            if (Physics.autoSimulation)
                return;

            if (Time.timeScale == 0f)
                return;

            Physics.Simulate(DeltaTime);
        }
    }
}
