﻿#if (UNITY_EDITOR && VRC_BASE && !NDMF)
using UnityEditor;
using UnityEngine;
using VRC.SDKBase.Editor.BuildPipeline;

namespace net.rs64.TexTransTool.Build.VRC
{

    [InitializeOnLoad]
    internal class VRCAvatarCallBackToProcessAvatar : IVRCSDKPreprocessAvatarCallback, IVRCSDKPostprocessAvatarCallback
    {
        public int callbackOrder => -2048;//この値についてはもうすこし考えるべきだが -1024で IEditorOnlyは消滅するらしい。

        public bool OnPreprocessAvatar(GameObject avatarGameObject)
        {
            return AvatarBuildUtils.ProcessAvatar(avatarGameObject, null, true);
        }
        public void OnPostprocessAvatar()
        {
            AssetSaveHelper.IsTemporary = false;
        }

    }
}
#endif
