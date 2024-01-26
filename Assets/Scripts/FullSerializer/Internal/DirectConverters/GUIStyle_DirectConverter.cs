using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	public class GUIStyle_DirectConverter : fsDirectConverter<GUIStyle>
	{
		protected override FsResult DoSerialize(GUIStyle model, Dictionary<string, fsData> serialized)
		{
			FsResult success = FsResult.Success;
			success += SerializeMember(serialized, null, "active", model.active);
			success += SerializeMember(serialized, null, "alignment", model.alignment);
			success += SerializeMember(serialized, null, "border", model.border);
			success += SerializeMember(serialized, null, "clipping", model.clipping);
			success += SerializeMember(serialized, null, "contentOffset", model.contentOffset);
			success += SerializeMember(serialized, null, "fixedHeight", model.fixedHeight);
			success += SerializeMember(serialized, null, "fixedWidth", model.fixedWidth);
			success += SerializeMember(serialized, null, "focused", model.focused);
			success += SerializeMember(serialized, null, "font", model.font);
			success += SerializeMember(serialized, null, "fontSize", model.fontSize);
			success += SerializeMember(serialized, null, "fontStyle", model.fontStyle);
			success += SerializeMember(serialized, null, "hover", model.hover);
			success += SerializeMember(serialized, null, "imagePosition", model.imagePosition);
			success += SerializeMember(serialized, null, "margin", model.margin);
			success += SerializeMember(serialized, null, "name", model.name);
			success += SerializeMember(serialized, null, "normal", model.normal);
			success += SerializeMember(serialized, null, "onActive", model.onActive);
			success += SerializeMember(serialized, null, "onFocused", model.onFocused);
			success += SerializeMember(serialized, null, "onHover", model.onHover);
			success += SerializeMember(serialized, null, "onNormal", model.onNormal);
			success += SerializeMember(serialized, null, "overflow", model.overflow);
			success += SerializeMember(serialized, null, "padding", model.padding);
			success += SerializeMember(serialized, null, "richText", model.richText);
			success += SerializeMember(serialized, null, "stretchHeight", model.stretchHeight);
			success += SerializeMember(serialized, null, "stretchWidth", model.stretchWidth);
			return success + SerializeMember(serialized, null, "wordWrap", model.wordWrap);
		}

		protected override FsResult DoDeserialize(Dictionary<string, fsData> data, ref GUIStyle model)
		{
			FsResult success = FsResult.Success;
			GUIStyleState value = model.active;
			success += DeserializeMember(data, null, "active", out value);
			model.active = value;
			TextAnchor value2 = model.alignment;
			success += DeserializeMember(data, null, "alignment", out value2);
			model.alignment = value2;
			RectOffset value3 = model.border;
			success += DeserializeMember(data, null, "border", out value3);
			model.border = value3;
			TextClipping value4 = model.clipping;
			success += DeserializeMember(data, null, "clipping", out value4);
			model.clipping = value4;
			Vector2 value5 = model.contentOffset;
			success += DeserializeMember(data, null, "contentOffset", out value5);
			model.contentOffset = value5;
			float value6 = model.fixedHeight;
			success += DeserializeMember(data, null, "fixedHeight", out value6);
			model.fixedHeight = value6;
			float value7 = model.fixedWidth;
			success += DeserializeMember(data, null, "fixedWidth", out value7);
			model.fixedWidth = value7;
			GUIStyleState value8 = model.focused;
			success += DeserializeMember(data, null, "focused", out value8);
			model.focused = value8;
			Font value9 = model.font;
			success += DeserializeMember(data, null, "font", out value9);
			model.font = value9;
			int value10 = model.fontSize;
			success += DeserializeMember(data, null, "fontSize", out value10);
			model.fontSize = value10;
			FontStyle value11 = model.fontStyle;
			success += DeserializeMember(data, null, "fontStyle", out value11);
			model.fontStyle = value11;
			GUIStyleState value12 = model.hover;
			success += DeserializeMember(data, null, "hover", out value12);
			model.hover = value12;
			ImagePosition value13 = model.imagePosition;
			success += DeserializeMember(data, null, "imagePosition", out value13);
			model.imagePosition = value13;
			RectOffset value14 = model.margin;
			success += DeserializeMember(data, null, "margin", out value14);
			model.margin = value14;
			string value15 = model.name;
			success += DeserializeMember(data, null, "name", out value15);
			model.name = value15;
			GUIStyleState value16 = model.normal;
			success += DeserializeMember(data, null, "normal", out value16);
			model.normal = value16;
			GUIStyleState value17 = model.onActive;
			success += DeserializeMember(data, null, "onActive", out value17);
			model.onActive = value17;
			GUIStyleState value18 = model.onFocused;
			success += DeserializeMember(data, null, "onFocused", out value18);
			model.onFocused = value18;
			GUIStyleState value19 = model.onHover;
			success += DeserializeMember(data, null, "onHover", out value19);
			model.onHover = value19;
			GUIStyleState value20 = model.onNormal;
			success += DeserializeMember(data, null, "onNormal", out value20);
			model.onNormal = value20;
			RectOffset value21 = model.overflow;
			success += DeserializeMember(data, null, "overflow", out value21);
			model.overflow = value21;
			RectOffset value22 = model.padding;
			success += DeserializeMember(data, null, "padding", out value22);
			model.padding = value22;
			bool value23 = model.richText;
			success += DeserializeMember(data, null, "richText", out value23);
			model.richText = value23;
			bool value24 = model.stretchHeight;
			success += DeserializeMember(data, null, "stretchHeight", out value24);
			model.stretchHeight = value24;
			bool value25 = model.stretchWidth;
			success += DeserializeMember(data, null, "stretchWidth", out value25);
			model.stretchWidth = value25;
			bool value26 = model.wordWrap;
			success += DeserializeMember(data, null, "wordWrap", out value26);
			model.wordWrap = value26;
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return new GUIStyle();
		}
	}
}
