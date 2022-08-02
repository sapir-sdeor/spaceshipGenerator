using UnityEngine;
using Avrahamy;

namespace SpaceshipGenerator {
    public class Rotator : OptimizedBehaviour {
        [SerializeField] Vector3 axis;
        [SerializeField] float angle;

        protected void Update() {
            transform.Rotate(axis, angle * Time.deltaTime);
        }
    }
}
