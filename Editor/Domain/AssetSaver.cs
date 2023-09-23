#if UNITY_EDITOR
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace net.rs64.TexTransTool
{
    public class AssetSaver : IAssetSaver
    {
        [NotNull] public AvatarDomainAsset Asset;

        public AssetSaver(Object container = null)
        {
            if (container == null)
            {
                Asset = ScriptableObject.CreateInstance<AvatarDomainAsset>();
                AssetDatabase.CreateAsset(Asset, AssetSaveHelper.GenerateAssetPath("AvatarDomainAsset", ".asset", AssetSaveHelper.SaveType.AvatarDomainAssets));
            }
            else
            {
                Asset = ScriptableObject.CreateInstance<AvatarDomainAsset>();
                Asset.OverrideContainer = container;
                Asset.name = "net.rs64.TexTransTool.AssetContainer";
                Asset.AddSubObject(Asset);
            }
        }

        public void TransferAsset(UnityEngine.Object UnityObject)
        {
            Asset.AddSubObject(UnityObject);
        }
    }
}
#endif