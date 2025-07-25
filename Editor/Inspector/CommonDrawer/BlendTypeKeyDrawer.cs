using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using net.rs64.TexTransCoreEngineForUnity;

namespace net.rs64.TexTransTool.Editor
{
    [CustomPropertyDrawer(typeof(BlendTypeKeyAttribute))]
    internal class BlendTypeKeyDrawer : PropertyDrawer
    {
        static string s_nowLangCode;
        static GUIContent[] s_blendTypeKeyContents;
        static string[] s_blendTypeKeys;
        public override void OnGUI(Rect rect, SerializedProperty serializedProperty, GUIContent label)
        {
            DrawBlendModeKey(rect, serializedProperty, label);
        }

        public static void DrawBlendModeKey(Rect rect, SerializedProperty serializedProperty, GUIContent label)
        {
            if (s_blendTypeKeyContents == null || s_nowLangCode != TTTGlobalConfig.instance.Language)
            {
                if (ComputeObjectUtility.BlendingObject == null) { return; }//次フレームを待つ
                var langCode = TTTGlobalConfig.instance.Language;

                var stdKeys = new string[]{
                "Clip/Normal",

                "Clip/Mul",
                "Clip/ColorBurn",
                "Clip/LinearBurn",
                "Clip/DarkenOnly",
                "Clip/DarkenColorOnly",

                "Clip/Screen",
                "Clip/ColorDodge",
                "Clip/ColorDodgeGlow",
                "Clip/Addition",
                "Clip/AdditionGlow",
                "Clip/LightenOnly",
                "Clip/LightenColorOnly",

                "Clip/Overlay",
                "Clip/SoftLight",
                "Clip/HardLight",
                "Clip/VividLight",
                "Clip/LinearLight",
                "Clip/PinLight",
                "Clip/HardMix",

                "Clip/Difference",
                "Clip/Exclusion",
                "Clip/Subtract",
                "Clip/Divide",

                "Clip/Hue",
                "Clip/Saturation",
                "Clip/Color",
                "Clip/Luminosity",

                //特殊な色合成をしない系
                "Normal",//通常
                "Dissolve",//ディザ合成
                "NotBlend",//ほぼTTTの内部処理用の上のレイヤーで置き換えるもの

                //暗くする系
                "Mul",//乗算
                "ColorBurn",//焼きこみカラー
                "LinearBurn",//焼きこみ(リニア)
                "DarkenOnly",//比較(暗)
                "DarkenColorOnly",//カラー比較(暗)

                //明るくする系
                "Screen",//スクリーン
                "ColorDodge",//覆い焼きカラー
                "ColorDodgeGlow",//覆い焼き(発光)
                "Addition",//加算-覆い焼き(リニア)
                "AdditionGlow",//加算(発光)
                "LightenOnly",//比較(明)
                "LightenColorOnly",//カラー比較(明)

                //ライト系
                "Overlay",//オーバーレイ
                "SoftLight",//ソフトライト
                "HardLight",//ハードライト
                "VividLight",//ビビッドライト
                "LinearLight",//リニアライト
                "PinLight",//ピンライト
                "HardMix",//ハードミックス

                //算術系
                "Difference",//差の絶対値
                "Exclusion",//除外
                "Subtract",//減算
                "Divide",//除算

                //視覚的な色調置き換え系
                "Hue",//色相
                "Saturation",//彩度
                "Color",//カラー
                "Luminosity",//輝度
                };
                var stdHash = stdKeys.ToHashSet();
                s_blendTypeKeys = ComputeObjectUtility.BlendingObject.Keys.Where(i => stdHash.Contains(i) is false).Concat(stdKeys).ToHashSet().ToArray();
                s_blendTypeKeyContents = s_blendTypeKeys.Select(GetLocale).ToArray();

                // Debug.Log(string.Join("\n", s_blendTypeKeys.Select(i => i + ":" + string.Join("-", UTF8Encoding.UTF8.GetBytes(i)))));

                GUIContent GetLocale(string key)
                {
                    var text = ComputeObjectUtility.BlendingObject.GetValueOrDefault(key)?.Locales?.FirstOrDefault(i => i.LangCode == langCode)?.DisplayName;
                    text ??= key;
                    return new(text);
                }
            }

            var sTarget = serializedProperty;
            var propLabel = EditorGUI.BeginProperty(rect, label, serializedProperty);

            var keyName = sTarget.stringValue;
            var blKeySelectIndex = Array.IndexOf(s_blendTypeKeys, keyName);
            if (sTarget.hasMultipleDifferentValues) { blKeySelectIndex = -1; }
            blKeySelectIndex = EditorGUI.Popup(rect, propLabel, blKeySelectIndex, s_blendTypeKeyContents);
            if (0 <= blKeySelectIndex && blKeySelectIndex < s_blendTypeKeyContents.Length) { sTarget.stringValue = s_blendTypeKeys[blKeySelectIndex]; }

            EditorGUI.EndProperty();
        }

        [InitializeOnLoadMethod]
        static void RegisterCallBack()
        {
            ComputeObjectUtility.InitBlendShadersCallBack += () =>
            {
                s_blendTypeKeyContents = null;
                s_blendTypeKeys = null;
            };
        }
    }
}
