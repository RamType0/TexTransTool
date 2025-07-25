using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace net.rs64.TexTransTool.Utils
{
    internal static class GradientTempTexture
    {
        static Texture2D s_GradientTempTexture;
        internal static Texture2D Get(Gradient gradient, float alpha)
        {
            if (s_GradientTempTexture == null)
            {
                s_GradientTempTexture = new Texture2D(256, 1, TextureFormat.RGBA32, false);
                s_GradientTempTexture.wrapMode = TextureWrapMode.Clamp;
            }


            using (var colorArray = new NativeArray<Color32>(256, Allocator.Temp, NativeArrayOptions.UninitializedMemory))
            {
                var writeSpan = colorArray.AsSpan();
                for (var i = 0; i < writeSpan.Length; i++)
                {
                    const float UNorm8ToFloat = 1 / 255f;
                    var col = gradient.Evaluate(i * UNorm8ToFloat);
                    col.a *= alpha;
                    writeSpan[i] = col;
                }

                s_GradientTempTexture.LoadRawTextureData(colorArray);
            }
            s_GradientTempTexture.Apply(true);

            return s_GradientTempTexture;
        }

    }
}
