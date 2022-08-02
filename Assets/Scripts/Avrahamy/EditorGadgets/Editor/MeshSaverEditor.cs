using UnityEngine;
using UnityEditor;

namespace Avrahamy.EditorGadgets {
    public static class MeshSaverEditor {
        private const string SAVE_MESH_PATH_KEY = "SAVE_MESH_PATH";

	    [MenuItem("CONTEXT/MeshFilter/Save Mesh...")]
	    public static void SaveMeshInPlace(MenuCommand menuCommand) {
		    var mf = menuCommand.context as MeshFilter;
		    var mesh = mf.sharedMesh;
		    SaveMesh(mesh, mesh.name, false, true);
	    }

	    [MenuItem("CONTEXT/MeshFilter/Save Mesh As New Instance...")]
	    public static void SaveMeshNewInstanceItem(MenuCommand menuCommand) {
		    var mf = menuCommand.context as MeshFilter;
		    var mesh = mf.sharedMesh;
		    SaveMesh(mesh, mesh.name, true, true);
	    }

	    public static Mesh SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh) {
		    var path = EditorUtility.SaveFilePanel("Save Mesh Asset", EditorPrefs.GetString(SAVE_MESH_PATH_KEY, "Assets/"), name, "asset");
		    if (string.IsNullOrEmpty(path)) return null;

		    path = FileUtil.GetProjectRelativePath(path);

		    var meshToSave = makeNewInstance ? Object.Instantiate(mesh) : mesh;

		    if (optimizeMesh) {
		         MeshUtility.Optimize(meshToSave);
            }

		    AssetDatabase.CreateAsset(meshToSave, path);
		    AssetDatabase.SaveAssets();

            var filenameStartIndex = path.LastIndexOf('/');
            if (filenameStartIndex > 0) {
                path = path.Substring(0, filenameStartIndex);
                EditorPrefs.SetString(SAVE_MESH_PATH_KEY, path);
            }

            return meshToSave;
	    }

    }
}
