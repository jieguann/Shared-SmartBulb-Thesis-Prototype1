#if DRACO_UNITY_1_4_0_OR_NEWER
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

// Note: In DracoUnity 2.0.0, all classes were moved into the
// "Draco" namespace, whereas previously they lived in the
// default (global) namespace.
#if DRACO_UNITY_2_0_0_OR_NEWER
using Draco;
#endif

namespace Piglet
{
	/// <summary>
	/// Utility methods for the DracoUnity package:
	/// https://github.com/atteneder/DracoUnity
	/// </summary>
	public static class DracoUnityUtil
	{
		/// <summary>
		/// Load a Unity mesh from Draco-compressed mesh data,
		/// using the DracoUnity package. This method
		/// is a simple wrapper around DracoUnity that hides API
		/// differences between DracoUnity 1.4.0 and DracoUnity 2.0.0+.
		/// </summary>
		/// <param name="dracoData">
		/// Byte array containing Draco-compressed mesh data.
		/// </param>
		/// <param name="weightsId">
		/// The Draco ID of the WEIGHTS_0 mesh attribute. This ID mapping
		/// is provided by the "attributes" object of the
		/// "KHR_draco_mesh_compression" glTF extension.
		/// </param>
		/// <param name="jointsId">
		/// The Draco ID for the JOINTS_0 mesh attribute. This ID mapping
		/// is provided by the "attributes" object of the
		/// "KHR_draco_mesh_compression" glTF extension.
		/// </param>
		/// <returns>
		/// IEnumerable over Unity Mesh.
		/// </returns>
		public static IEnumerable<Mesh> LoadDracoMesh(
			byte[] dracoData, int weightsId = -1, int jointsId = -1)
		{
			using (var dracoDataNative = new NativeArray<byte>(dracoData, Allocator.Persistent))
			{
				if (YieldTimer.Instance.Expired)
				{
					yield return null;
					YieldTimer.Instance.Restart();
				}

				foreach (var result in LoadDracoMesh(dracoDataNative, weightsId, jointsId))
					yield return result;
			}
		}

		/// <summary>
		/// Load a Unity mesh from Draco-compressed mesh data,
		/// using the DracoUnity package. This method
		/// is a simple wrapper around DracoUnity that hides API
		/// differences between DracoUnity 1.4.0 and DracoUnity 2.0.0+.
		/// </summary>
		/// <param name="dracoData">
		/// NativeArray<byte> containing the Draco-compressed mesh data.
		/// </param>
		/// <param name="weightsId">
		/// The Draco ID of the WEIGHTS_0 mesh attribute. This ID mapping
		/// is provided by the "attributes" object of the
		/// "KHR_draco_mesh_compression" glTF extension.
		/// </param>
		/// <param name="jointsId">
		/// The Draco ID for the JOINTS_0 mesh attribute. This ID mapping
		/// is provided by the "attributes" object of the
		/// "KHR_draco_mesh_compression" glTF extension.
		/// </param>
		/// <returns>
		/// IEnumerable over Unity Mesh.
		/// </returns>
		public static IEnumerable<Mesh> LoadDracoMesh(
			NativeArray<byte> dracoData, int weightsId = -1, int jointsId = -1)
		{
			Mesh mesh = null;

#if DRACO_UNITY_2_0_0_OR_NEWER

			// Note: DracoUnity 2.x/3.x provides a convenience
			// method for decoding a mesh directly from a C# byte[]
			// (rather than a NativeArray<byte>), but according to
			// my testing, that method does not work correctly.
			// See: https://github.com/atteneder/DracoUnity/issues/21

			var dracoLoader = new DracoMeshLoader();
			var task = dracoLoader.ConvertDracoMeshToUnity(
				dracoData, true, false, weightsId, jointsId);

			while (!task.IsCompleted)
				yield return null;

			if (task.IsFaulted)
				throw task.Exception;

			mesh = task.Result;

#else

			// Set up Draco decompression task and completion callback
			//
			// Note: `DecodeMeshSkinned` is my own modified version of
			// DracoUnity's main `DracoMeshLoader.DecodeMesh` method. The
			// only difference is that `DecodeMeshSkinned` accepts
			// additional `jointsId` and `weightsId` arguments, in
			// order to support skinned meshes.

			var dracoLoader = new DracoMeshLoader();
			dracoLoader.onMeshesLoaded += _mesh => mesh = _mesh;
			var dracoTask = dracoLoader.DecodeMeshSkinned(
				dracoData, jointsId, weightsId);

			// perform Draco decompression

			while (dracoTask.MoveNext())
				yield return null;

#endif

			yield return null;

			// Workaround: Fix DracoUnity bug where texture coords are
			// upside-down (vertically flipped).

			foreach (var _ in MeshUtil.VerticallyFlipTextureCoords(mesh))
				yield return null;

			// Correct for orientation changes introduced in DracoUnity 3.0.0.
			//
			// Starting in DracoUnity 3.0.0, DracoUnity changed how it
			// performs its conversion from right-handed coordinates (glTF) to
			// left-handed coordinates (Unity). Instead of negating the
			// Z-coordinate of each vertex, it now negates the X-coordinate instead.
            // This has the effect rotating the model by 180 degrees around the
            // Y-axis, so that the front face of the model now looks down the positive
            // Z-axis rather than the negative Z-axis. See [1] and [2] for further
            // explanation/visualization.
			//
			// Here we simply reverse the changes, so that Piglet continues to load
            // models with the same orientation as previously.
			//
			// [1]: https://github.com/atteneder/DracoUnity/releases/tag/v3.0.0
			// [2]: https://github.com/atteneder/glTFast/blob/main/Documentation%7E/glTFast.md#coordinate-system-conversion-change

#if DRACO_UNITY_3_0_0_OR_NEWER

			var vertices = mesh.vertices;
			var normals = mesh.normals;

			for (var i = 0; i < vertices.Length; ++i)
			{
				vertices[i] = new Vector3(-vertices[i].x, vertices[i].y, -vertices[i].z);
				normals[i] = new Vector3(-normals[i].x, normals[i].y, -normals[i].z);
			}

			mesh.vertices = vertices;
			mesh.normals = normals;

#endif

			yield return mesh;
		}
	}
}
#endif
