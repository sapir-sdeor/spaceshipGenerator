using UnityEngine;

namespace Avrahamy {
    public class OptimizedBehaviour : MonoBehaviour {
        public new Transform transform {
            get {
                if (_transform == null) {
                    _transform = GetComponent<Transform>();
                }
                return _transform;
            }
        }

        private Transform _transform;
    }
}
