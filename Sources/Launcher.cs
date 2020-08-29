using UnityEngine;

namespace PlutoECL
{
    public class Launcher : MonoBehaviour
    {
        void Start() => StartAction();

        protected virtual void StartAction() { }
        
        void Update() => ExtendedBehaviour.RunAll();
    }
}