#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using net.rs64.TexTransCore.Decal;
using net.rs64.TexTransCore.TransTextureCore;
using net.rs64.TexTransTool.Utils;
using net.rs64.TexTransCore.TransTextureCore.TransCompute;

namespace net.rs64.TexTransTool.Decal
{
    public abstract class AbstractSingleDecal<SpaceConverter> : AbstractDecal
    where SpaceConverter : DecalUtility.IConvertSpace
    {
        public Texture2D DecalTexture;
        public override bool IsPossibleApply => TargetRenderers.Any(i => i != null);
        public abstract SpaceConverter GetSpaceConverter { get; }
        public abstract DecalUtility.ITrianglesFilter<SpaceConverter> GetTriangleFilter { get; }

        public override Dictionary<Texture2D, Texture> CompileDecal()
        {
            var mulDecalTexture = DecalTexture != null ? (Texture)TextureLayerUtil.CreateMultipliedRenderTexture(DecalTexture, Color) : TextureLayerUtil.CreateColorTex(Color);
            var decalCompiledTextures = new Dictionary<Texture2D, Texture>();
            if (FastMode)
            {
                var decalCompiledRenderTextures = new Dictionary<Texture2D, RenderTexture>();
                foreach (var renderer in TargetRenderers)
                {
                    DecalUtility.CreateDecalTexture(
                        renderer,
                        decalCompiledRenderTextures,
                        mulDecalTexture,
                        GetSpaceConverter,
                        GetTriangleFilter,
                        TargetPropertyName,
                        GetTextureWarp,
                        Padding
                    );
                }

                foreach (var texture in decalCompiledRenderTextures)
                {
                    decalCompiledTextures.Add(texture.Key, texture.Value);
                }
            }
            else
            {
                Texture2D mulDecalTexture2D = null;
                switch (mulDecalTexture)
                {
                    case RenderTexture renderTexture:
                        {
                            mulDecalTexture2D = renderTexture.CopyTexture2D();
                            break;
                        }
                    case Texture2D texture2D:
                        {
                            mulDecalTexture2D = texture2D;
                            break;
                        }
                }
                var mulDecalTexTowDimensionMap = new TwoDimensionalMap<Color>(mulDecalTexture2D.GetPixels(), new Vector2Int(mulDecalTexture2D.width, mulDecalTexture2D.height));
                var TransTextureCompute = TransMapper.TransTextureCompute;
                List<Dictionary<Texture2D, List<TwoDimensionalMap<Color>>>> DecalsCompileTexListDict = new List<Dictionary<Texture2D, List<TwoDimensionalMap<Color>>>>();
                foreach (var renderer in TargetRenderers)
                {
                    var DecalsCompile = DecalUtility.CreateDecalTextureCS(
                        TransTextureCompute,
                        renderer,
                        mulDecalTexTowDimensionMap,
                        GetSpaceConverter,
                        GetTriangleFilter,
                        TargetPropertyName,
                        GetTextureWarp,
                        Padding
                    );
                    DecalsCompileTexListDict.Add(DecalsCompile);
                }

                var zipDict = CollectionsUtility.ZipToDictionaryOnList(DecalsCompileTexListDict);

                var blendTextureCS = TransMapper.BlendTextureCS;
                foreach (var texture in zipDict)
                {
                    var blendColorMap = TextureLayerUtil.BlendTextureUseComputeShader(blendTextureCS, texture.Value, BlendType.AlphaLerp);
                    var blendTexture = new Texture2D(blendColorMap.MapSize.x, blendColorMap.MapSize.y);
                    blendTexture.SetPixels(blendColorMap.Array);
                    blendTexture.Apply();
                    decalCompiledTextures.Add(texture.Key, blendTexture);
                }
            }


            return decalCompiledTextures;
        }

        public virtual void ScaleApply() { throw new NotImplementedException(); }

        public void ScaleApply(Vector3 Scale, bool FixedAspect)
        {
            if (DecalTexture != null && FixedAspect)
            {
                transform.localScale = new Vector3(Scale.x, Scale.x * ((float)DecalTexture.height / (float)DecalTexture.width), Scale.z);
            }
            else
            {
                transform.localScale = Scale;
            }
        }
    }
}



#endif
