using UnityEditor;
using UnityEngine;

namespace Beatrate.AnimationBaker.EditorSide
{
	public class SerializedBakedAnimationClip
	{
		public SerializedObject SerializedObject;
		public SerializedProperty SourceClip;
		public SerializedProperty TransformBindings;
	}

	[CustomEditor(typeof(BakedAnimationClip))]
    public class BakedAnimationClipEditor : Editor
	{

		private SerializedBakedAnimationClip properties = null;

		public void OnEnable()
		{
			properties = new SerializedBakedAnimationClip
			{
				SerializedObject = serializedObject,
				SourceClip = serializedObject.FindProperty("sourceClip"),
				TransformBindings = serializedObject.FindProperty("transformBindings")
			};
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(properties.SourceClip);
			EditorGUILayout.LabelField($"Transform bindings:{properties.TransformBindings.arraySize}");

			bool canBake = properties.SourceClip.objectReferenceValue != null;

			serializedObject.ApplyModifiedProperties();

			if(!canBake)
			{
				GUI.enabled = false;
			}
			if(GUILayout.Button("Bake") && canBake)
			{
				AnimationBaker.Bake((BakedAnimationClip)target, properties);
			}

			if(!canBake)
			{
				GUI.enabled = true;
			}
		}
	}
}
