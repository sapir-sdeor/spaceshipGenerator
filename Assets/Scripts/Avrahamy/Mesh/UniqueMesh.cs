using UnityEngine;

namespace Avrahamy.Meshes {
    /// <summary>
    /// Based on this slide: https://docs.google.com/presentation/d/10XjxscVrm5LprOmG-VB2DltVyQ_QygD26N6XC2iap2A/edit#slide=id.gb871cf6ef_0_0
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class UniqueMesh : OptimizedBehaviour {
        // To ensure we have a unique mesh
        [HideInInspector, SerializeField] int ownerID;

        // Tries to find a mesh filter, adds one if it doesn't exist yet
        protected MeshFilter MeshFilter {
            get{
                _mf = _mf == null ? GetComponent<MeshFilter>() : _mf;
                _mf = _mf == null ? gameObject.AddComponent<MeshFilter>() : _mf;
                if (_mesh != null && _mf.sharedMesh == null) {
                    _mf.sharedMesh = _mesh;
                }
                return _mf;
            }
        }
        private MeshFilter _mf;

        public Mesh Mesh {
            get{
                bool isOwner = ownerID == name.GetHashCode();
                if (_mesh == null || !isOwner) {
                    MeshFilter.sharedMesh = _mesh = new Mesh();
                    _mesh.hideFlags = HideFlags.DontSave;
                    ownerID = name.GetHashCode();
                    _mesh.name = "Mesh [" + ownerID + "]";
                }
                return _mesh;
            }
        }
        private Mesh _mesh;
    }

}
