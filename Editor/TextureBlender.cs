#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using net.rs64.TexTransCore.TransTextureCore;
namespace net.rs64.TexTransTool
{
    [AddComponentMenu("TexTransTool/TTT TextureBlender")]
    public class TextureBlender : TextureTransformer
    {
        public Renderer TargetRenderer;
        public int MaterialSelect = 0;
        public Texture2D BlendTexture;
        public Color Color = Color.white;
        public BlendType BlendType = BlendType.Normal;
        public PropertyName TargetPropertyName;


        public override List<Renderer> GetRenderers => new List<Renderer>() { TargetRenderer };

        public override bool IsPossibleApply => TargetRenderer != null && BlendTexture != null;

        public override TexTransPhase PhaseDefine => TexTransPhase.BeforeUVModification;

        public override void Apply(IDomain Domain)
        {
            if (!IsPossibleApply) return;

            var DistMaterials = TargetRenderer.sharedMaterials;

            if (DistMaterials.Length <= MaterialSelect) return;
            var DistMat = DistMaterials[MaterialSelect];

            var DistTex = DistMat.GetTexture(TargetPropertyName) as Texture2D;
            var AddTex = TextureLayerUtil.CreateMultipliedRenderTexture(BlendTexture, Color);
            if (DistTex == null)
            {
                return;
            }

            Domain.AddTextureStack(DistTex, new TextureLayerUtil.BlendTextures(AddTex, BlendType));
        }
    }
}
#endif
