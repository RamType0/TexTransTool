#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace net.rs64.TexTransTool.Decal.Curve
{
    internal abstract class CurveDecal : AbstractDecal
    {
        public float Size = 0.5f;
        public uint LoopCount = 1;
        public float OutOfRangeOffset = 0f;
        public bool IsTextureWarp = true;
        public Vector2 TextureWarpRange = new Vector2(0, 0.05f);
        public List<CurveSegment> Segments = new List<CurveSegment>();
        public bool DrawGizmoAlways = false;
        public bool UseFirstAndEnd = false;
        public Texture2D DecalTexture;
        public Texture2D FirstTexture;
        public Texture2D EndTexture;
        public float CurveStartOffset;

        public bool IsPossibleSegments => Segments.Count > 1 && !Segments.Any(i => i == null);
        public override bool IsPossibleApply => DecalTexture != null && TargetRenderers.Any(i => i != null) && (UseFirstAndEnd ? FirstTexture != null && EndTexture != null : true) && IsPossibleSegments;




    }
}
#endif
