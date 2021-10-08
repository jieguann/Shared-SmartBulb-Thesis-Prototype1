using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Text.RegularExpressions;
using System;
using UnityGLTF.Extensions;
using System.Reflection;

public class GLTFUtils
{
	public static string buildBlendShapeName(int meshIndex, int targetIndex)
	{
		return "Target_" + meshIndex + "_" + targetIndex;
	}

	public static float[] Vector4ToArray(Vector4 vector)
	{
		float[] arr = new float[4];
		arr[0] = vector.x;
		arr[1] = vector.y;
		arr[2] = vector.z;
		arr[3] = vector.w;

		return arr;
	}

	public static float[] normalizeBoneWeights(Vector4 weights)
	{
		float sum = weights.x + weights.y + weights.z + weights.w;
		if (sum != 1.0f)
		{
			weights = weights / sum;
		}

		return Vector4ToArray(weights);
	}
}
