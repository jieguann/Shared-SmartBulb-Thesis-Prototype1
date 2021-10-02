using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piglet
{
	/// <summary>
	/// Utility methods for working with Unity meshes.
	/// </summary>
	public class MeshUtil
	{
		/// <summary>
		/// Vertically flip all texture UV coords for the given
		/// Unity mesh.
		/// </summary>
		/// <param name="mesh">The target Unity mesh.</param>
		public static IEnumerable VerticallyFlipTextureCoords(Mesh mesh)
		{
			var uv = new List<Vector2>();

			// Note: We use 8 here because Unity supports up to 8
			// sets of texture coordinates per mesh, which are accessible as
			// mesh.uv, mesh.uv2, ..., mesh.uv8.

			for (var i = 0; i < 8; ++i)
			{
				uv.Clear();

				mesh.GetUVs(i, uv);

				if (uv.Count == 0)
					continue;

				for (var j = 0; j < uv.Count; ++j)
					uv[j] = new Vector2(uv[j].x, 1 - uv[j].y);

				mesh.SetUVs(i, uv);

				yield return null;
			}
		}
	}
}
