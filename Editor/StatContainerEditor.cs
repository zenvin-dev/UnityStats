using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Zenvin.Stats {
	[CustomEditor (typeof (StatContainer))]
	internal class StatContainerEditor : Editor {
		private readonly Dictionary<StatInstance, InstanceEditor> editors = new Dictionary<StatInstance, InstanceEditor> ();
		private readonly CreateInstanceData createInstanceData = new CreateInstanceData ();

		private GUIStyle dropZoneStyle;
		private GUIStyle DropZoneStyle => dropZoneStyle ??= new GUIStyle (EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter };

		private GUIStyle foldoutStyle;
		private GUIStyle FoldoutStyle => foldoutStyle ??= new GUIStyle (EditorStyles.foldout) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold };

		private GUIContent removeContent;
		private GUIContent RemoveContent => removeContent ??= EditorGUIUtility.IconContent ("d_Toolbar Minus");

		private GUIStyle removeStyle;
		private GUIStyle RemoveStyle => removeStyle ??= new GUIStyle (EditorStyles.label) { padding = new RectOffset(), margin = new RectOffset() };


		public override void OnInspectorGUI () {
			var container = target as StatContainer;

			EditorGUILayout.LabelField ("Stats", EditorStyles.miniLabel);
			DrawStatList (container);

			GUILayout.Space (7);

			var dropRect = EditorGUILayout.GetControlRect (GUILayout.Height (EditorGUIUtility.singleLineHeight * 2));
			GUI.Box (dropRect, "Drop Stat object to add Instance", DropZoneStyle);
			HandleDragAndDrop (container, dropRect);
		}


		private void DrawStatList (StatContainer container) {
			for (int i = 0; i < container.stats.Count; i++) {
				var instance = container.stats[i];
				if (instance == null || !instance.IsValid) {
					container.RemoveStat (i);
					i--;
					continue;
				}

				var editor = GetEditor (instance);
				editor.Expanded = DrawStatHeader (instance, editor.Expanded, out var remove);
				if (editor.Expanded) {
					editor.Editor.DrawDefaultInspector ();
				}

				GetHeaderRect (1f, false, out _, out var separatorRect, out _);
				EditorGUI.DrawRect (separatorRect, new Color32 (26, 26, 26, 255));

				if (remove) {
					container.RemoveStat (i);
					i--;
					continue;
				}
			}
		}

		private bool DrawStatHeader (StatInstance instance, bool expanded, out bool remove) {
			var size = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			GetHeaderRect (
				size,
				false,
				out var controlRect,
				out var backgroundRect,
				out var accentRect
			);
			var stat = instance.GetStat ();

			EditorGUI.DrawRect (backgroundRect, new Color (0.3f, 0.3f, 0.3f, 0.7f));
			EditorGUI.DrawRect (accentRect, stat.AccentColor);

			var instanceText = instance.ToString ();
			var text = string.IsNullOrEmpty (instanceText) ? stat.Identifier : $"{stat.Identifier} [{instanceText}]";

			var foldRect = new Rect (controlRect);
			foldRect.width -= size;
			var btnRect = new Rect (controlRect);
			btnRect.x += foldRect.width;
			btnRect.width = size;

			remove = GUI.Button (btnRect, RemoveContent, RemoveStyle);
			return EditorGUI.Foldout (foldRect, expanded, new GUIContent (text), true, FoldoutStyle);
		}

		private void GetHeaderRect (float height, bool accentRight, out Rect controlRect, out Rect backgroundRect, out Rect accentRect) {
			controlRect = GUILayoutUtility.GetRect (EditorGUIUtility.currentViewWidth, height);

			backgroundRect = new Rect (controlRect);
			backgroundRect.x -= 18f;
			backgroundRect.width += 22f;

			const float accentWidth = 4f;
			accentRect = new Rect (backgroundRect);
			if (accentRight) {
				accentRect.x += accentRect.width - accentWidth;
			}
			accentRect.width = accentWidth;
		}

		private void HandleDragAndDrop (StatContainer container, Rect dropRect) {
			var evt = Event.current;
			if (evt.type != EventType.DragExited)
				return;
			if (!dropRect.Contains (evt.mousePosition))
				return;

			createInstanceData.Container = container;
			createInstanceData.Stat = DragAndDrop.objectReferences.FirstOrDefault (o => o is Stat) as Stat;

			if (createInstanceData.Stat != null) {
				HandleAddInstance ();
			}
		}

		private void HandleAddInstance () {
			if (createInstanceData.Container.ContainsStat (createInstanceData.Stat)) {
				ExpandOnly (createInstanceData.Stat);
				return;
			}

			var menu = new GenericMenu ();

			var statType = createInstanceData.Stat.GetValueType ();
			var baseType = typeof (StatInstance<>).MakeGenericType (statType);
			var types = TypeCache.GetTypesDerivedFrom (baseType);
			var foundTypes = 0;
			Type singleType = null;

			menu.AddDisabledItem (new GUIContent ($"Choose Instance type for {createInstanceData.Stat.Identifier}"));
			foreach (var type in types) {
				if (!type.IsAbstract && !type.ContainsGenericParameters) {
					menu.AddItem (new GUIContent ($"{type.Namespace}/{type.Name}"), false, CreateInstanceMenuHandler, type);
					foundTypes++;
					singleType = type;
				}
			}

			if (foundTypes == 1) {
				createInstanceData.Container.AddStat (createInstanceData.Stat, singleType);
			} else if (foundTypes > 1) {
				menu.ShowAsContext ();
			} else {
				Debug.Log ($"Did not find any types for stat type '{statType}'.");
			}
		}

		private void CreateInstanceMenuHandler (object userData) {
			if (userData is Type type) {
				createInstanceData.Container.AddStat (createInstanceData.Stat, type);
			}
		}

		private InstanceEditor GetEditor (StatInstance instance) {
			if (editors.TryGetValue (instance, out var editor)) {
				return editor;
			}

			editor = new InstanceEditor () { Editor = CreateEditor (instance), Expanded = false, };
			editors[instance] = editor;
			return editor;
		}

		private void ExpandOnly (Stat stat) {
			var container = target as StatContainer;

			foreach (var instance in container.stats) {
				var editor = GetEditor (instance);
				editor.Expanded = instance.GetStat () == stat;
			}
		}


		private class InstanceEditor {
			public bool Expanded { get; set; }
			public Editor Editor { get; set; }
		}

		private class CreateInstanceData {
			public StatContainer Container { get; set; }
			public Stat Stat { get; set; }
		}
	}
}
