#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using UnityEditor;
using net.rs64.TexTransTool.Decal;

namespace net.rs64.TexTransTool.Editor.Decal
{

    [CustomEditor(typeof(SimpleDecal), true)]
    public class SimpleDecalEditor : UnityEditor.Editor
    {
        bool FoldoutOption;
        public override void OnInspectorGUI()
        {
            var This_S_Object = serializedObject;
            var ThisObject = target as SimpleDecal;

            EditorGUI.BeginDisabledGroup(PreviewContext.IsPreviewing(ThisObject));

            AbstractDecalEditor.DrawerDecalEditor(This_S_Object);

            EditorGUILayout.LabelField("ScaleSettings", EditorStyles.boldLabel);
            EditorGUI.indentLevel += 1;
            var s_Scale = This_S_Object.FindProperty("Scale");
            var s_FixedAspect = This_S_Object.FindProperty("FixedAspect");
            AbstractDecalEditor.DrawerScaleEditor(ThisObject, This_S_Object, s_Scale, s_FixedAspect);

            var s_MaxDistance = This_S_Object.FindProperty("MaxDistance");
            TextureTransformerEditor.DrawerProperty(s_MaxDistance, (float MaxDistanceValue) =>
            {
                Undo.RecordObject(ThisObject, "ApplyScale - MaxDistance");
                ThisObject.MaxDistance = MaxDistanceValue;
                ThisObject.ScaleApply();
            });
            EditorGUI.indentLevel -= 1;


            EditorGUILayout.LabelField("CullingSettings", EditorStyles.boldLabel);
            EditorGUI.indentLevel += 1;

            var s_PolygonCulling = This_S_Object.FindProperty("PolygonCulling");
            EditorGUILayout.PropertyField(s_PolygonCulling, new GUIContent("Polygon Culling"));

            var s_SideCulling = This_S_Object.FindProperty("SideCulling");
            EditorGUILayout.PropertyField(s_SideCulling, new GUIContent("Side Culling"));

            var s_IslandCulling = This_S_Object.FindProperty("IslandCulling");
            EditorGUILayout.PropertyField(s_IslandCulling);
            if (s_IslandCulling.boolValue)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.LabelField("IslandSelectorPos");
                EditorGUI.indentLevel += 1;
                var s_IslandSelectorPos = This_S_Object.FindProperty("IslandSelectorPos");
                var s_IslandSelectorPosX = s_IslandSelectorPos.FindPropertyRelative("x");
                var s_IslandSelectorPosY = s_IslandSelectorPos.FindPropertyRelative("y");
                EditorGUILayout.Slider(s_IslandSelectorPosX, 0, 1, new GUIContent("x"));
                EditorGUILayout.Slider(s_IslandSelectorPosY, 0, 1, new GUIContent("y"));
                EditorGUI.indentLevel -= 1;
                var s_IslandSelectorRange = This_S_Object.FindProperty("IslandSelectorRange");
                EditorGUILayout.Slider(s_IslandSelectorRange, 0, 1);
                EditorGUI.indentLevel -= 1;
            }
            EditorGUI.indentLevel -= 1;


            FoldoutOption = EditorGUILayout.Foldout(FoldoutOption, "Advanced Option");
            if (FoldoutOption)
            {
                EditorGUI.indentLevel += 1;

                var s_FastMode = This_S_Object.FindProperty("FastMode");
                EditorGUILayout.PropertyField(s_FastMode, new GUIContent("FastMode"));

                var s_Padding = This_S_Object.FindProperty("Padding");
                EditorGUILayout.PropertyField(s_Padding, new GUIContent("Padding"));

                EditorGUI.indentLevel -= 1;
            }


            EditorGUI.EndDisabledGroup();
            AbstractDecalEditor.DrawerRealTimePreviewEditor(ThisObject);
            EditorGUI.BeginDisabledGroup(RealTimePreviewManager.instance.RealTimePreviews.ContainsKey(ThisObject));
            PreviewContext.instance.DrawApplyAndRevert(ThisObject);
            EditorGUI.EndDisabledGroup();

            This_S_Object.ApplyModifiedProperties();
        }


        public static void DrawerSummary(SimpleDecal target)
        {
            var s_obj = new SerializedObject(target);
            var s_TargetRenderers = s_obj.FindProperty("TargetRenderers");
            TextureTransformerEditor.DrawerTargetRenderersSummary(s_TargetRenderers);
            var s_DecalTexture = s_obj.FindProperty("DecalTexture");
            TextureTransformerEditor.DrawerObjectReference<Texture2D>(s_DecalTexture);

            s_obj.ApplyModifiedProperties();
        }


    }


}
#endif
