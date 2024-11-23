using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class PinTemplate
	{
		[ProtoMember(1)]
		public string TemplateName = string.Empty;

		[ProtoMember(2)]
		public Vector2 Position;

		[ProtoMember(3)]
		public Vector2 PositionOffset;

		[ProtoMember(4)]
		public string LoadingScreen = string.Empty;

		[ProtoMember(5, IsRequired = true)]
		public bool ProgressIndicator = true;

		[ProtoMember(6, IsRequired = true)]
		public bool ShowSelectionArrow = true;

		[ProtoMember(7)]
		public Dictionary<string, TextureDetail> Textures = new Dictionary<string, TextureDetail>();

#if UNITY_EDITOR
	    //public StringTextureDetailsDictionary _texturesSerializable =
	    //    StringTextureDetailsDictionary.New<StringTextureDetailsDictionary>();

        public PinDetail.StringTextureDetailsDictionaryV2 _texturesSerializableV2 =
            new PinDetail.StringTextureDetailsDictionaryV2();
        public void DoSerializationStaff()
	    {
	        //foreach (var textureDetail in Textures)
	        //{
	        //    _texturesSerializable.dictionary.Add(textureDetail.Key, textureDetail.Value);
	        //}

            foreach (var textureDetail in Textures)
            {
                _texturesSerializableV2.Add(textureDetail.Key, textureDetail.Value);
            }
        }

	    public void DoDeSerializationStaff()
	    {
	        Textures = new Dictionary<string, TextureDetail>();

	        //foreach (var textureDetail in _texturesSerializable.dictionary)
	        //{
	        //    Textures.Add(textureDetail.Key, textureDetail.Value);
	        //}

            foreach (var textureDetail in _texturesSerializableV2)
            {
                Textures.Add(textureDetail.Key, textureDetail.Value);
            }
        }
#endif
    }
}
