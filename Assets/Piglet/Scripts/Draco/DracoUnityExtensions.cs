#if DRACO_UNITY_1_4_0_OR_NEWER && !DRACO_UNITY_2_0_0_OR_NEWER
using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;

namespace Piglet
{
	/// <summary>
	/// Methods used for integrating Piglet with the DracoUnity package,
	/// in order to support Draco-compressed meshes.
	/// </summary>
    public static class DracoUnityExtensions
    {
	    /// <summary>
	    /// <para>
	    /// Coroutine that creates a Unity mesh from Draco-compressed mesh data.
	    /// Note that in order to a get access to the output Unity mesh, you
	    /// need to assign a handler method to DracoMeshLoader.onMeshesLoaded.
	    /// </para>
	    /// <para>
	    /// This method is nearly identical to the DracoMeshLoader.DecodeMesh provided
	    /// by the DracoUnity 1.4.0 package. The only change I have made is to add
	    /// the `jointsId` and `weightsId` arguments, in order to support
	    /// loading skinned meshes.
	    /// </para>
	    /// </summary>
	    /// <param name="loader">
	    /// Instance of DracoMeshLoader class from DracoUnity package.
	    /// </param>
	    /// <param name="data">
	    /// Native byte array containing the Draco-compressed mesh data.
	    /// </param>
	    /// <param name="jointsId">
	    /// The Draco ID for the JOINTS_0 mesh attribute. This ID mapping
	    /// is provided by the "attributes" object of the
	    /// "KHR_draco_mesh_compression" glTF extension.
	    /// </param>
	    /// <param name="weightsId">
	    /// The Draco ID of the WEIGHTS_0 mesh attribute. This ID mapping
	    /// is provided by the "attributes" object of the
	    /// "KHR_draco_mesh_compression" glTF extension.
	    /// </param>
		public static IEnumerator DecodeMeshSkinned(this DracoMeshLoader loader,
			NativeArray<byte> data, int jointsId = -1, int weightsId = -1)
		{
			Profiler.BeginSample("JobPrepare");
			var job = new DracoMeshLoader.DracoJob();

			job.data = data;
			job.result = new NativeArray<int>(1, Allocator.Persistent);
			job.outMesh = new NativeArray<IntPtr>(1, Allocator.Persistent);
			job.weightsId = weightsId;
			job.jointsId = jointsId;

			var jobHandle = job.Schedule();
			Profiler.EndSample();

			while(!jobHandle.IsCompleted) {
				yield return null;
			}
			jobHandle.Complete();

			int result = job.result[0];
			IntPtr dracoMesh = job.outMesh[0];

			job.result.Dispose();
			job.outMesh.Dispose();

			if (result <= 0) {
				Debug.LogError ("Failed: Decoding error.");
				yield break;
			}

			bool hasTexcoords;
			bool hasNormals;
			var mesh = DracoMeshLoader.CreateMesh(dracoMesh,out hasNormals, out hasTexcoords);

			if(!hasNormals) {
				mesh.RecalculateNormals();
			}
			if(hasTexcoords) {
				mesh.RecalculateTangents();
			}

			if(loader.onMeshesLoaded!=null) {
				loader.onMeshesLoaded(mesh);
			}
		}
    }
}
#endif