using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GLTF.Schema
{
    /// <summary>
    /// C# class mirroring the JSON content of the KHR_draco_mesh_compression
    /// glTF extension. For details/examples about this glTF extension, see:
    /// https://github.com/KhronosGroup/glTF/blob/master/extensions/2.0/Khronos/KHR_draco_mesh_compression/README.md
    /// </summary>
    public class KHR_draco_mesh_compressionExtension : Extension
    {
        /// <summary>
        /// Index of glTF buffer view that contains the
        /// Draco-compressed mesh data.
        /// </summary>
        public int BufferViewId;

        /// <summary>
        /// Dictionary that maps mesh attributes (e.g. POSITON, NORMAL)
        /// to corresponding IDs in the Draco-compressed mesh data.
        /// Note: DracoUnity does not require us to provide ID mappings
        /// for any attributes except JOINT and WEIGHT when decompressing
        /// the Draco data, so in practice we only load those two mappings
        /// (if present). I'm not sure how DracoUnity determines the
        /// ID mappings for the other attributes (e.g. POSITION) --
        /// perhaps those attributes have standard ID mappings or
        /// can be looked up by name.
        /// </summary>
        public Dictionary<string, AccessorId> Attributes;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bufferViewId">
        /// Index of the glTF buffer view that contains the
        /// Draco-compressed mesh data.
        /// </param>
        /// <param name="attributes">
        /// Maps mesh attributes (e.g. POSITION, NORMAL) to
        /// corresponding IDs in the Draco-compressed data.
        /// </param>
        public KHR_draco_mesh_compressionExtension(int bufferViewId,
            Dictionary<string, AccessorId> attributes)
        {
            BufferViewId = bufferViewId;
            Attributes = attributes;
        }

        public JProperty Serialize()
        {
            throw new System.NotImplementedException();
        }
    }
}