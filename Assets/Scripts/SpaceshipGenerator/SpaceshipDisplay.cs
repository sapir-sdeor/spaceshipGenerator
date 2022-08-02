using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using Avrahamy.EditorGadgets;
using Avrahamy.Meshes;
using BitStrap;

namespace SpaceshipGenerator {
    public class SpaceshipDisplay : MonoBehaviour {
        private const string HELP_TEXT =
            "P - pause editor, R - toggle rotation, G - toggle auto generation, Space - generate";

        [SerializeField] EditableMesh mesh;
        [SerializeField] MeshCollider meshCollider;
        [SerializeField] Rotator rotator;
        [SerializeField] bool autoGenerate;
        [ConditionalHide("autoGenerate")]
        [SerializeField] float secondsToAutoGenerate;
        [SerializeField] int currentSeed;
        [Info("Copy Current Seed here to get the same result. Useful for debugging")]
        [SerializeField] int nextSeed;
        [Inline]
        [SerializeField] SpaceshipGenerator generator;

        private Keyboard keyboard;
        private float nextGenerateTime;

        protected void Awake() {
            keyboard = Keyboard.current;
            Generate();
        }

        protected void Update() {
            if (keyboard.pKey.wasPressedThisFrame) {
                Debug.Break();
                return;
            }
            if (keyboard.rKey.wasPressedThisFrame) {
                rotator.enabled = !rotator.enabled;
                return;
            }
            if (keyboard.gKey.wasPressedThisFrame) {
                autoGenerate = !autoGenerate;
                return;
            }
            if (keyboard.spaceKey.wasPressedThisFrame) {
                Generate();
                return;
            }
            if (keyboard.anyKey.wasPressedThisFrame) {
                Debug.Log(HELP_TEXT);
                return;
            }
            if (!autoGenerate || Time.time < nextGenerateTime) {
                return;
            }
            Generate();
        }

        [Button]
        private void Generate() {
            nextGenerateTime = Time.time + secondsToAutoGenerate;

            Random.InitState(nextSeed);
            currentSeed = nextSeed;
            generator.Generate(mesh);
            meshCollider.sharedMesh = mesh.Mesh;

            nextSeed = Random.Range(0, int.MaxValue);
        }
    }
}
