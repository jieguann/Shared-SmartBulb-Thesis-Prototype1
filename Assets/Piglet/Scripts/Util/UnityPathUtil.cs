using System;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Piglet
{
    public class UnityPathUtil
    {
        /// <summary>
        /// Characters that are illegal to use in filenames.
        /// I generated this list by printing the output of
        /// Path.GetInvalidFileNameChars() on Windows 10.
        /// I used to query the list of illegal chars directly
        /// from Path.GetInvalidFileNameChars(), but
        /// I discovered (via a user's bug report) that
        /// Path.GetInvalidFileNameChars() does not work
        /// correctly on MacOS.
        /// </summary>
        public static readonly char[] INVALID_FILENAME_CHARS = {
            '\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005',
            '\u0006', '\u0007', '\u0008', '\u0009', '\u000a', '\u000b',
            '\u000c', '\u000d', '\u000e', '\u000f', '\u0010', '\u0011',
            '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017',
            '\u0018', '\u0019', '\u001a', '\u001b', '\u001c', '\u001d',
            '\u001e', '\u001f', '"', '<', '>', '|', ':', '*', '?', '\\', '/' };

        /// <summary>
        /// Translate the given name to a name that is safe
        /// to use as the basename of a Unity asset file,
        /// by masking illegal characters with '_'.
        /// </summary>
        public static string GetLegalAssetName(string name)
        {
            // replace illegal filename chars with '_'
            var result = new string(name
                .Select(c => INVALID_FILENAME_CHARS.Contains(c) ? '_' : c)
                .ToArray());

            // replace '.' because we use asset names as AnimatorController state names
            result = result.Replace(".", "_");

            return result;
        }

        public static string NormalizePathSeparators(string path)
        {
            string result = path.Replace("\\\\", "/").Replace("\\", "/");

            // remove trailing slash if present, because this can affect
            // the results of some .NET methods (e.g. `Path.GetDirectoryName`)
            if (result.EndsWith("/"))
                result = result.Substring(0, result.Length - 1);

            return result;
        }

        public static string GetProjectPath(string absolutePath)
        {
            return NormalizePathSeparators(absolutePath.Replace(Application.dataPath, "Assets"));
        }

        public static string GetAbsolutePath(string projectPath)
        {
            return NormalizePathSeparators(projectPath.Replace("Assets", Application.dataPath));
        }

        public static string GetParentDir(string path)
        {
            return NormalizePathSeparators(Path.GetDirectoryName(path));
        }

        public static string GetFileURI(string absolutePath)
        {
            return "file://" + NormalizePathSeparators(absolutePath);
        }

#if UNITY_EDITOR
        public static void RemoveProjectDir(string path)
        {
            if (!Directory.Exists(path))
                return;

            Directory.Delete(path, true);
            AssetDatabase.Refresh();
        }
#endif

    }
}
