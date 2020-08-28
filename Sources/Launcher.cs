using UnityEngine;

namespace PlutoECL
{
    public abstract class Launcher : MonoBehaviour
    {
        void Start() => StartAction();

        protected abstract void StartAction();
        
        void Update() => ExtendedBehaviour.RunAll();
    }
}