#nullable enable
using System.Collections;
using net.rs64.TexTransTool.Utils;
using net.rs64.TexTransTool.UVIsland;
using UnityEngine;
using UnityEngine.Profiling;

namespace net.rs64.TexTransTool.IslandSelector
{
    [AddComponentMenu(TexTransBehavior.TTTName + "/" + MenuPath)]
    public class IslandSelectorXOR : AbstractIslandSelector, ITexTransToolStableComponent
    {
        internal const string ComponentName = "TTT IslandSelectorXOR";
        internal const string MenuPath = FoldoutName + "/" + ComponentName;
        public int StabilizeSaveDataVersion => TTTDataVersion_0_10_X;

        internal override void LookAtCalling(ILookingObject looker) { LookAtChildren(this, looker); }
        internal override BitArray IslandSelectImpl(IslandSelectorContext ctx)
        {
            BitArray? bitArray = null;
            foreach (var islandSelector in transform.GetChildeComponent<AbstractIslandSelector>())
            {
                if (ctx.Targeting.IsActive(islandSelector.gameObject) is false) { continue; }
                Profiler.BeginSample(islandSelector.GetType().Name);
                var selectBit = islandSelector.IslandSelect(ctx);
                Profiler.EndSample();
                if (bitArray is null) { bitArray = selectBit; continue; }
                bitArray.Xor(selectBit);
            }
            bitArray ??= new(ctx.Islands.Length);
            return bitArray;
        }
        internal override void OnDrawGizmosSelected() { foreach (var islandSelector in transform.GetChildeComponent<AbstractIslandSelector>()) { islandSelector.OnDrawGizmosSelected(); } }
        internal override bool IsExperimental => false;
    }
}
