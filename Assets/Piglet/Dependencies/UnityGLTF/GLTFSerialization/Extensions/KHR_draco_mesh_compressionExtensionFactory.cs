using System.Collections.Generic;
using GLTF.Extensions;
using Newtonsoft.Json.Linq;

namespace GLTF.Schema
{
	/// <summary>
	/// Parses JSON data for the KHR_draco_mesh_compression glTF extension
	/// and loads it into an equivalent C# class (KHR_draco_mesh_compression).
    /// For details/examples of the KHR_draco_mesh_compression extension, see:
    /// https://github.com/KhronosGroup/glTF/blob/master/extensions/2.0/Khronos/KHR_draco_mesh_compression/README.md
	/// </summary>
    public class KHR_draco_mesh_compressionExtensionFactory : ExtensionFactory
    {
		public const string EXTENSION_NAME = "KHR_draco_mesh_compression";

        public KHR_draco_mesh_compressionExtensionFactory()
        {
            ExtensionName = EXTENSION_NAME;
        }

		/// <summary>
		/// Parse JSON for KHR_draco_mesh_compression glTF extension and load it
		/// into an equivalent C# class (`KHR_draco_mesh_compression`).
		/// </summary>
		/// <param name="root">
		/// C# object hierarchy mirroring entire JSON content of glTF file
		/// (everything except extensions).
		/// </param>
		/// <param name="extensionToken">
		/// Root JSON token for KHR_draco_mesh_compression extension.
		/// </param>
		/// <returns>
		/// C# object (`KHR_draco_mesh_compressionExtension`) mirroring
		/// JSON content of KHR_draco_mesh_compression extension.
		/// </returns>
        public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
		{
			if (extensionToken == null)
				return null;

	        // Get the value of the "bufferView" JSON property, which provides the
	        // index of the glTF buffer view that contains the Draco-compressed
	        // mesh data.

            var bufferViewToken = extensionToken.Value["bufferView"];
            if (bufferViewToken == null)
                return null;

            var bufferView = bufferViewToken.DeserializeAsInt();

            // Parse the "attributes" JSON object, which maps
            // the various mesh attributes (POSITION, NORMAL, etc.) to
            // their corresponding IDs in the Draco-compressed mesh data.
            // (See [1] for further info.)
            //
            // Note: DracoUnity does not require ID mappings for any
            // attributes except JOINT and WEIGHT when decompressing the Draco mesh
            // data. That's why I'm only loading the JOINT and WEIGHT attribute
            // mappings here. I'm not sure how DracoUnity figures the rest of the
            // ID mappings for POSITION, NORMAL, etc. -- perhaps the ID mappings are
            // standardized or the Draco C++ library allows accessing the
            // attributes by name.
            //
	        // [1] https://github.com/KhronosGroup/glTF/blob/master/extensions/2.0/Khronos/KHR_draco_mesh_compression/README.md

            var attributesToken = extensionToken.Value["attributes"];
            if (attributesToken == null || attributesToken.Type != JTokenType.Object)
	            return null;

            var attributesObject = (JObject) attributesToken;
            var attributes = new Dictionary<string, AccessorId>();

            var jointsProperty = attributesObject.Property(SemanticProperties.JOINT);
            if (jointsProperty != null)
            {
	            attributes.Add(
		            SemanticProperties.JOINT,
		            new AccessorId {
			            Id = jointsProperty.Value.DeserializeAsInt(),
			            Root = root
		            });
            }

            var weightsProperty = attributesObject.Property(SemanticProperties.WEIGHT);
            if (weightsProperty != null)
            {
	            attributes.Add(
		            SemanticProperties.WEIGHT,
		            new AccessorId {
			            Id = weightsProperty.Value.DeserializeAsInt(),
			            Root = root
		            });
            }

            return new KHR_draco_mesh_compressionExtension(bufferView, attributes);
        }
    }
}