// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Scripting;
using UnityEngine.Bindings;
using System.Collections.Generic;

[assembly: InternalsVisibleTo("Unity.RenderPipelines.GPUDriven.Runtime")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.Core.Editor.Tests")]

namespace UnityEngine.Rendering
{
    delegate void GPUDrivenLODGroupDataCallback(in GPUDrivenLODGroupData lodGroupData);
    delegate void GPUDrivenLODGroupDataNativeCallback(in GPUDrivenLODGroupDataNative lodGroupDataNative, GPUDrivenLODGroupDataCallback callback);
    delegate void GPUDrivenRendererDataCallback(in GPUDrivenRendererGroupData rendererData, IList<Mesh> meshes, IList<Material> materials);
    delegate void GPUDrivenRendererDataNativeCallback(in GPUDrivenRendererGroupDataNative rendererDataNative, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback callback);
    delegate void GPUDrivenSpeedTreeWindDataCallback(in GPUDrivenSpeedTreeWindData speedTreeWindData);
    delegate void GPUDrivenSpeedTreeWindDataNativeCallback(in GPUDrivenSpeedTreeWindDataNative speedTreeWindDataNative, GPUDrivenSpeedTreeWindDataCallback callback);

    [RequiredByNativeCode]
    internal static class GPUDrivenCallbacks
    {
        [RequiredByNativeCode(GenerateProxy = true)]
        public static void InvokeGPUDrivenLODGroupDataNativeCallback(
            GPUDrivenLODGroupDataNativeCallback callback,
            in GPUDrivenLODGroupDataNative lodGroupDataNative,
            GPUDrivenLODGroupDataCallback target)
        {
            callback.Invoke(lodGroupDataNative, target);
        }

        [RequiredByNativeCode(GenerateProxy = true)]
        public static void InvokeGPUDrivenRendererDataNativeCallback(
            GPUDrivenRendererDataNativeCallback callback,
            in GPUDrivenRendererGroupDataNative rendererDataNative,
            List<Mesh> meshes, List<Material> materials,
            GPUDrivenRendererDataCallback target)
        {
            callback.Invoke(rendererDataNative, meshes, materials, target);
        }

        [RequiredByNativeCode(GenerateProxy = true)]
        public static void InvokeGPUDrivenSpeedTreeWindDataNativeCallback(
            GPUDrivenSpeedTreeWindDataNativeCallback callback,
            in GPUDrivenSpeedTreeWindDataNative speedTreeWindDataNative,
            GPUDrivenSpeedTreeWindDataCallback target)
        {
            callback.Invoke(speedTreeWindDataNative, target);
        }
    }

    [RequiredByNativeCode]
    [NativeHeader("Runtime/Camera/GPUDrivenProcessor.h")]
    internal class GPUDrivenProcessor
    {
        internal IntPtr m_Ptr;

        internal List<Mesh> scratchMeshes { get; private set; }
        internal List<Material> scratchMaterials { get; private set; }

        public GPUDrivenProcessor()
        {
            m_Ptr = Internal_Create();

            scratchMeshes = new List<Mesh>();
            scratchMaterials = new List<Material>();
        }

        ~GPUDrivenProcessor()
        {
            Destroy();
        }

        public void Dispose()
        {
            scratchMeshes = null;
            scratchMaterials = null;

            Destroy();
            GC.SuppressFinalize(this);
        }

        private void Destroy()
        {
            if (m_Ptr != IntPtr.Zero)
            {
                Internal_Destroy(m_Ptr);
                m_Ptr = IntPtr.Zero;
            }
        }

        private static extern IntPtr Internal_Create();

        private static extern void Internal_Destroy(IntPtr ptr);

        private static unsafe GPUDrivenRendererDataNativeCallback s_NativeRendererCallback = (in GPUDrivenRendererGroupDataNative nativeData, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback callback) =>
        {
            var rendererGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.rendererGroupID, nativeData.rendererGroupCount, Allocator.Invalid);
            var localBounds = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Bounds>(nativeData.localBounds, nativeData.rendererGroupCount, Allocator.Invalid);
            var lightmapScaleOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.lightmapScaleOffset, nativeData.rendererGroupCount, Allocator.Invalid);
            var gameObjectLayer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.gameObjectLayer, nativeData.rendererGroupCount, Allocator.Invalid);
            var renderingLayerMask = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<uint>(nativeData.renderingLayerMask, nativeData.rendererGroupCount, Allocator.Invalid);
            var lodGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.lodGroupID, nativeData.rendererGroupCount, Allocator.Invalid);
            var lightmapIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.motionVecGenMode, nativeData.rendererGroupCount, Allocator.Invalid);
            var packedRendererData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenPackedRendererData>(nativeData.packedRendererData, nativeData.rendererGroupCount, Allocator.Invalid);
            var rendererPriority = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.rendererPriority, nativeData.rendererGroupCount, Allocator.Invalid);
            var meshIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.meshIndex, nativeData.rendererGroupCount, Allocator.Invalid);
            var subMeshStartIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>(nativeData.subMeshStartIndex, nativeData.rendererGroupCount, Allocator.Invalid);
            var materialsOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.materialsOffset, nativeData.rendererGroupCount, Allocator.Invalid);
            var materialsCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>(nativeData.materialsCount, nativeData.rendererGroupCount, Allocator.Invalid);
            var instancesOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(null, 0, Allocator.Invalid);
            var instancesCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(null, 0, Allocator.Invalid);

            var invalidRendererGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.invalidRendererGroupID, nativeData.invalidRendererGroupIDCount, Allocator.Invalid);

            var localToWorldMatrix = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(nativeData.localToWorldMatrix, nativeData.rendererGroupCount, Allocator.Invalid);
            var prevLocalToWorldMatrix = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(nativeData.prevLocalToWorldMatrix, nativeData.rendererGroupCount, Allocator.Invalid);
            var rendererGroupIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(null, 0, Allocator.Invalid);

            var meshID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.meshID, nativeData.meshCount, Allocator.Invalid);
            var subMeshCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>(nativeData.subMeshCount, nativeData.meshCount, Allocator.Invalid);
            var subMeshDescOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.subMeshDescOffset, nativeData.meshCount, Allocator.Invalid);

            var subMeshDesc = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<SubMeshDescriptor>(nativeData.subMeshDesc, nativeData.subMeshDescCount, Allocator.Invalid);

            var materialIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.materialIndex, nativeData.materialIndexCount, Allocator.Invalid);

            var materialID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.materialID, nativeData.materialCount, Allocator.Invalid);
            var isTransparent = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<bool>(nativeData.isTransparent, nativeData.materialCount, Allocator.Invalid);
            var isMotionVectorsPassEnabled = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<bool>(nativeData.isMotionVectorsPassEnabled, nativeData.materialCount, Allocator.Invalid);
            var materialFilterFlags = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.materialFilterFlags, nativeData.materialCount, Allocator.Invalid);

            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref rendererGroupID, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref localBounds, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref lightmapScaleOffset, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref gameObjectLayer, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref renderingLayerMask, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref lodGroupID, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref lightmapIndex, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref packedRendererData, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref rendererPriority, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref meshIndex, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref subMeshStartIndex, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref materialsOffset, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref materialsCount, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref instancesOffset, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref instancesCount, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref invalidRendererGroupID, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref localToWorldMatrix, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref prevLocalToWorldMatrix, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref rendererGroupIndex, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref meshID, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref subMeshCount, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref subMeshDescOffset, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref subMeshDesc, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref materialIndex, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref materialID, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref isTransparent, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref isMotionVectorsPassEnabled, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref materialFilterFlags, AtomicSafetyHandle.Create());
            GPUDrivenRendererGroupData data = new GPUDrivenRendererGroupData
            {
                rendererGroupID = rendererGroupID,
                localBounds = localBounds,
                lightmapScaleOffset = lightmapScaleOffset,
                gameObjectLayer = gameObjectLayer,
                renderingLayerMask = renderingLayerMask,
                lodGroupID = lodGroupID,
                lightmapIndex = lightmapIndex,
                packedRendererData = packedRendererData,
                rendererPriority = rendererPriority,
                meshIndex = meshIndex,
                subMeshStartIndex = subMeshStartIndex,
                materialsOffset = materialsOffset,
                materialsCount = materialsCount,
                instancesOffset = instancesOffset,
                instancesCount = instancesCount,
                invalidRendererGroupID = invalidRendererGroupID,
                localToWorldMatrix = localToWorldMatrix,
                prevLocalToWorldMatrix = prevLocalToWorldMatrix,
                rendererGroupIndex = rendererGroupIndex,
                meshID = meshID,
                subMeshCount = subMeshCount,
                subMeshDescOffset = subMeshDescOffset,
                subMeshDesc = subMeshDesc,
                materialIndex = materialIndex,
                materialID = materialID,
                isTransparent = isTransparent,
                isMotionVectorsPassEnabled = isMotionVectorsPassEnabled,
                materialFilterFlags = materialFilterFlags
            };

            callback(data, meshes, materials);

            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(rendererGroupID));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(localBounds));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(lightmapScaleOffset));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(gameObjectLayer));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(renderingLayerMask));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(lodGroupID));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(lightmapIndex));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(packedRendererData));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(rendererPriority));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(meshIndex));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(subMeshStartIndex));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(materialsOffset));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(materialsCount));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(instancesOffset));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(instancesCount));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(invalidRendererGroupID));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(localToWorldMatrix));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(prevLocalToWorldMatrix));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(rendererGroupIndex));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(meshID));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(subMeshCount));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(subMeshDescOffset));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(subMeshDesc));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(materialIndex));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(materialID));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(isTransparent));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(isMotionVectorsPassEnabled));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(materialFilterFlags));
        };

        private static unsafe GPUDrivenLODGroupDataNativeCallback s_NativeLODGroupCallback = (in GPUDrivenLODGroupDataNative nativeData, GPUDrivenLODGroupDataCallback callback) =>
        {
            var lodGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.lodGroupID, nativeData.lodGroupCount, Allocator.Invalid);
            var lodOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.lodOffset, nativeData.lodGroupCount, Allocator.Invalid);
            var lodCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.lodCount, nativeData.lodGroupCount, Allocator.Invalid);
            var fadeMode = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<LODFadeMode>(nativeData.fadeMode, nativeData.lodGroupCount, Allocator.Invalid);
            var worldSpaceReferencePoint = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(nativeData.worldSpaceReferencePoint, nativeData.lodGroupCount, Allocator.Invalid);
            var worldSpaceSize = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(nativeData.worldSpaceSize, nativeData.lodGroupCount, Allocator.Invalid);
            var renderersCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>(nativeData.renderersCount, nativeData.lodGroupCount, Allocator.Invalid);
            var lastLODIsBillboard = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<bool>(nativeData.lastLODIsBillboard, nativeData.lodGroupCount, Allocator.Invalid);

            var invalidLODGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.invalidLODGroupID, nativeData.invalidLODGroupCount, Allocator.Invalid);

            var lodRenderersCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>(nativeData.lodRenderersCount, nativeData.lodDataCount, Allocator.Invalid);
            var lodScreenRelativeTransitionHeight = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(nativeData.lodScreenRelativeTransitionHeight, nativeData.lodDataCount, Allocator.Invalid);
            var lodFadeTransitionWidth = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(nativeData.lodFadeTransitionWidth, nativeData.lodDataCount, Allocator.Invalid);

            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref lodGroupID, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref lodOffset, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref lodCount, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref fadeMode, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref worldSpaceReferencePoint, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref worldSpaceSize, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref renderersCount, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref lastLODIsBillboard, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref invalidLODGroupID, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref lodRenderersCount, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref lodScreenRelativeTransitionHeight, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref lodFadeTransitionWidth, AtomicSafetyHandle.Create());
            GPUDrivenLODGroupData data = new GPUDrivenLODGroupData
            {
                lodGroupID = lodGroupID,
                lodOffset = lodOffset,
                lodCount = lodCount,
                fadeMode = fadeMode,
                worldSpaceReferencePoint = worldSpaceReferencePoint,
                worldSpaceSize = worldSpaceSize,
                renderersCount = renderersCount,
                lastLODIsBillboard = lastLODIsBillboard,
                invalidLODGroupID = invalidLODGroupID,
                lodRenderersCount = lodRenderersCount,
                lodScreenRelativeTransitionHeight = lodScreenRelativeTransitionHeight,
                lodFadeTransitionWidth = lodFadeTransitionWidth
            };

            callback(data);

            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(lodGroupID));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(lodOffset));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(lodCount));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(fadeMode));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(worldSpaceReferencePoint));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(worldSpaceSize));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(renderersCount));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(invalidLODGroupID));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(lodRenderersCount));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(lastLODIsBillboard));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(lodScreenRelativeTransitionHeight));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(lodFadeTransitionWidth));
        };

        private static unsafe GPUDrivenSpeedTreeWindDataNativeCallback s_NativeSpeedTreeWindCallback = (in GPUDrivenSpeedTreeWindDataNative nativeData, GPUDrivenSpeedTreeWindDataCallback callback) =>
        {
            var instance = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.instance, nativeData.count, Allocator.Invalid);
            var windVector = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windVector, nativeData.count, Allocator.Invalid);
            var windGlobal = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windGlobal, nativeData.count, Allocator.Invalid);
            var windBranchAdherences = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windBranchAdherences, nativeData.count, Allocator.Invalid);
            var windBranch = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windBranch, nativeData.count, Allocator.Invalid);
            var windBranchTwitch = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windBranchTwitch, nativeData.count, Allocator.Invalid);
            var windBranchWhip = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windBranchWhip, nativeData.count, Allocator.Invalid);
            var windBranchAnchor = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windBranchAnchor, nativeData.count, Allocator.Invalid);
            var windTurbulences = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windTurbulences, nativeData.count, Allocator.Invalid);
            var windLeaf1Ripple = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windLeaf1Ripple, nativeData.count, Allocator.Invalid);
            var windLeaf1Tumble = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windLeaf1Tumble, nativeData.count, Allocator.Invalid);
            var windLeaf1Twitch = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windLeaf1Twitch, nativeData.count, Allocator.Invalid);
            var windLeaf2Ripple = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windLeaf2Ripple, nativeData.count, Allocator.Invalid);
            var windLeaf2Tumble = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windLeaf2Tumble, nativeData.count, Allocator.Invalid);
            var windLeaf2Twitch = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windLeaf2Twitch, nativeData.count, Allocator.Invalid);
            var windFrondRipple = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windFrondRipple, nativeData.count, Allocator.Invalid);
            var windAnimation = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.windAnimation, nativeData.count, Allocator.Invalid);

            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref instance, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windVector, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windGlobal, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windBranchAdherences, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windBranch, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windBranchTwitch, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windBranchWhip, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windBranchAnchor, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windTurbulences, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windLeaf1Ripple, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windLeaf1Tumble, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windLeaf1Twitch, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windLeaf2Ripple, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windLeaf2Tumble, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windLeaf2Twitch, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windFrondRipple, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref windAnimation, AtomicSafetyHandle.Create());
            GPUDrivenSpeedTreeWindData data = new GPUDrivenSpeedTreeWindData
            {
                history = nativeData.history,
                instance = instance,
                windVector = windVector,
                windGlobal = windGlobal,
                windBranchAdherences = windBranchAdherences,
                windBranch = windBranch,
                windBranchTwitch = windBranchTwitch,
                windBranchWhip = windBranchWhip,
                windBranchAnchor = windBranchAnchor,
                windTurbulences = windTurbulences,
                windLeaf1Ripple = windLeaf1Ripple,
                windLeaf1Tumble = windLeaf1Tumble,
                windLeaf1Twitch = windLeaf1Twitch,
                windLeaf2Ripple = windLeaf2Ripple,
                windLeaf2Tumble = windLeaf2Tumble,
                windLeaf2Twitch = windLeaf2Twitch,
                windFrondRipple = windFrondRipple,
                windAnimation = windAnimation
            };

            callback(data);

            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(instance));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windVector));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windGlobal));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windBranchAdherences));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windBranch));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windBranchTwitch));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windBranchWhip));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windBranchAnchor));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windTurbulences));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windLeaf1Ripple));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windLeaf1Tumble));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windLeaf1Twitch));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windLeaf2Ripple));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windLeaf2Tumble));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windLeaf2Twitch));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windFrondRipple));
            AtomicSafetyHandle.Release(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(windAnimation));
        };

        private extern void EnableGPUDrivenRenderingAndDispatchRendererData(ReadOnlySpan<int> renderersID, GPUDrivenRendererDataNativeCallback callback, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback param);
        public void EnableGPUDrivenRenderingAndDispatchRendererData(ReadOnlySpan<int> renderersID, GPUDrivenRendererDataCallback callback)
        {
            scratchMeshes.Clear();
            scratchMaterials.Clear();
            EnableGPUDrivenRenderingAndDispatchRendererData(renderersID, s_NativeRendererCallback, scratchMeshes, scratchMaterials, callback);
        }
        public extern void DisableGPUDrivenRendering(ReadOnlySpan<int> renderersID);

        private extern void DispatchLODGroupData(ReadOnlySpan<int> lodGroupID, GPUDrivenLODGroupDataNativeCallback callback, GPUDrivenLODGroupDataCallback param);
        public void DispatchLODGroupData(ReadOnlySpan<int> lodGroupID, GPUDrivenLODGroupDataCallback callback)
        {
            DispatchLODGroupData(lodGroupID, s_NativeLODGroupCallback, callback);
        }

        private extern void DispatchSpeedTreeWindData(ReadOnlySpan<int> renderersID, ReadOnlySpan<int> instances, bool history, GPUDrivenSpeedTreeWindDataNativeCallback callback, GPUDrivenSpeedTreeWindDataCallback param);
        public void DispatchSpeedTreeWindData(ReadOnlySpan<int> renderersID, ReadOnlySpan<int> instances, bool history, GPUDrivenSpeedTreeWindDataCallback callback)
        {
            DispatchSpeedTreeWindData(renderersID, instances, history, s_NativeSpeedTreeWindCallback, callback);
        }

        public extern bool enablePartialRendering { set; get; }
        public extern bool enableMaterialFilters { set; get; }
        public extern void AddMaterialFilters([NotNull] GPUDrivenMaterialFilterEntry[] filters);
        public extern void ClearMaterialFilters();
        public extern int GetMaterialFilterFlags(Material material);

        internal static class BindingsMarshaller
        {
            public static IntPtr ConvertToNative(UnityEngine.Rendering.GPUDrivenProcessor obj) => obj.m_Ptr;
        }
    }

    [UsedByNativeCode]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct GPUDrivenRendererGroupDataNative
    {
        public int* rendererGroupID;
        public Bounds* localBounds;
        public Vector4* lightmapScaleOffset;
        public int* gameObjectLayer;
        public uint* renderingLayerMask;
        public int* lodGroupID;
        public MotionVectorGenerationMode* motionVecGenMode;
        public GPUDrivenPackedRendererData* packedRendererData;
        public int* rendererPriority;
        public int* meshIndex;
        public short* subMeshStartIndex;
        public int* materialsOffset;
        public short* materialsCount;
        public int* instancesOffset;
        public int* instancesCount;
        public int rendererGroupCount;

        public int* invalidRendererGroupID;
        public int invalidRendererGroupIDCount;

        public Matrix4x4* localToWorldMatrix;
        public Matrix4x4* prevLocalToWorldMatrix;
        public int* rendererGroupIndex;
        public int instanceCount;

        public int* meshID;
        public short* subMeshCount;
        public int* subMeshDescOffset;
        public int meshCount;

        public SubMeshDescriptor* subMeshDesc;
        public int subMeshDescCount;

        public int* materialIndex;
        public int materialIndexCount;

        public int* materialID;
        public bool* isTransparent;
        public bool* isMotionVectorsPassEnabled;
        public int* materialFilterFlags;
        public int materialCount;
    }

    [UsedByNativeCode]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct GPUDrivenLODGroupDataNative
    {
        public int* lodGroupID;
        public int* lodOffset;
        public int* lodCount;
        public LODFadeMode* fadeMode;
        public Vector3* worldSpaceReferencePoint;
        public float* worldSpaceSize;
        public short* renderersCount;
        public bool* lastLODIsBillboard;
        public int lodGroupCount;

        public int* invalidLODGroupID;
        public int invalidLODGroupCount;

        public short* lodRenderersCount;
        public float* lodScreenRelativeTransitionHeight;
        public float* lodFadeTransitionWidth;
        public int lodDataCount;
    }

    [UsedByNativeCode]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct GPUDrivenSpeedTreeWindDataNative
    {
        public bool history;
        public int* instance;
        public Vector4* windVector;
        public Vector4* windGlobal;
        public Vector4* windBranchAdherences;
        public Vector4* windBranch;
        public Vector4* windBranchTwitch;
        public Vector4* windBranchWhip;
        public Vector4* windBranchAnchor;
        public Vector4* windTurbulences;
        public Vector4* windLeaf1Ripple;
        public Vector4* windLeaf1Tumble;
        public Vector4* windLeaf1Twitch;
        public Vector4* windLeaf2Ripple;
        public Vector4* windLeaf2Tumble;
        public Vector4* windLeaf2Twitch;
        public Vector4* windFrondRipple;
        public Vector4* windAnimation;
        public int count;
    };

    [UsedByNativeCode]
    [StructLayout(LayoutKind.Sequential)]
    internal struct GPUDrivenPackedRendererData
    {
        uint data;

        public bool receiveShadows => (data & 1) != 0;
        public bool staticShadowCaster => (data & 1 << 1) != 0;
        public byte lodMask => (byte)(data >> 2 & 0xFF);
        public ShadowCastingMode shadowCastingMode => (ShadowCastingMode)(data >> 10 & 0x3);
        public LightProbeUsage lightProbeUsage => (LightProbeUsage)(data >> 12 & 0x7);
        public MotionVectorGenerationMode motionVecGenMode => (MotionVectorGenerationMode)(data >> 15 & 0x3);
        public bool isPartOfStaticBatch => (data & 1 << 17) != 0;
        public bool movedCurrentFrame => (data & 1 << 18) != 0;
        public bool hasTree => (data & 1 << 19) != 0;
        public bool smallMeshCulling => (data & 1 << 20) != 0;
        public bool supportsIndirect => (data & 1 << 21) != 0;

        public GPUDrivenPackedRendererData()
        {
            data = 0;
        }

        public GPUDrivenPackedRendererData(bool receiveShadows, bool staticShadowCaster, byte lodMask, ShadowCastingMode shadowCastingMode, LightProbeUsage lightProbeUsage,
            MotionVectorGenerationMode motionVecGenMode, bool isPartOfStaticBatch, bool movedCurrentFrame, bool hasTree, bool smallMeshCulling, bool supportsIndirect)
        {
            data = receiveShadows ? 1u : 0u;
            data |= staticShadowCaster ? 1u << 1 : 0u;
            data |= (uint)lodMask << 2;
            data |= (uint)shadowCastingMode << 10;
            data |= (uint)lightProbeUsage << 12;
            data |= (uint)motionVecGenMode << 15;
            data |= isPartOfStaticBatch ? 1u << 17 : 0u;
            data |= movedCurrentFrame ? 1u << 18 : 0u;
            data |= hasTree ? 1u << 19 : 0u;
            data |= smallMeshCulling ? 1u << 20 : 0u;
            data |= supportsIndirect ? 1u << 21 : 0u;
        }
    }

    internal enum GPUDrivenBitOpType
    {
        And,
        Or
    }

    [UsedByNativeCode]
    [StructLayout(LayoutKind.Sequential)]
    internal struct GPUDrivenMaterialFilterEntry
    {
        public GPUDrivenBitOpType op;
        public int minQueueValue;
        public int maxQueueValue;
        public ShaderTagId keyTag;
        public ShaderTagId valueTag;
        public int flags;
        public string keyword;
    }

    internal struct GPUDrivenRendererGroupData
    {
        /// <summary>
        /// Renderer Group data.
        /// </summary>
        // RendererGroupID can be either an InstanceID (for example acquired from MeshRenderer.GetComponentID()) or custom generated integer ID.
        // InstanceIDs are always even numbers in Unity so we can mix them with custom generated integers as long as we generate odd numbers.
        // These unique RendererGroupIDs are used to define instances that belong to a certain MeshRenderer or other custom instances group.
        public NativeArray<int> rendererGroupID;
        public NativeArray<Bounds> localBounds;
        public NativeArray<Vector4> lightmapScaleOffset;
        public NativeArray<int> gameObjectLayer;
        public NativeArray<uint> renderingLayerMask;
        public NativeArray<int> lodGroupID;
        public NativeArray<int> lightmapIndex;
        public NativeArray<GPUDrivenPackedRendererData> packedRendererData;
        public NativeArray<int> rendererPriority;
        public NativeArray<int> meshIndex;
        public NativeArray<short> subMeshStartIndex;
        public NativeArray<int> materialsOffset;
        public NativeArray<short> materialsCount;
        // Used for indexing multiple instances per a renderer group.
        public NativeArray<int> instancesOffset;
        public NativeArray<int> instancesCount;

        /// <summary>
        /// Invalid or disabled Render Group IDs.
        /// </summary>
        public NativeArray<int> invalidRendererGroupID;

        /// <summary>
        /// Instance data. Indexed by instancesOffset and instancesCount. Or directly if instancesOffset and instancesCount are empty.
        /// </summary>
        public NativeArray<Matrix4x4> localToWorldMatrix;
        public NativeArray<Matrix4x4> prevLocalToWorldMatrix;
        // Used for mapping instances back to renderer groups if multiple instances per renderer group are used.
        public NativeArray<int> rendererGroupIndex;

        /// <summary>
        /// Mesh data. Indexed by meshIndex.
        /// </summary>
        public NativeArray<int> meshID;
        public NativeArray<short> subMeshCount;
        public NativeArray<int> subMeshDescOffset;

        /// <summary>
        /// SubMesh Descriptor data. Indexed by subMeshCount and subMeshDescOffset.
        /// </summary>
        public NativeArray<SubMeshDescriptor> subMeshDesc;

        /// <summary>
        /// Material data indices. Indexed by materialsOffset and materialsCount.
        /// </summary>
        public NativeArray<int> materialIndex;

        /// <summary>
        /// Material data. Indexed by materialIndex.
        /// </summary>
        public NativeArray<int> materialID;
        public NativeArray<bool> isTransparent;
        public NativeArray<bool> isMotionVectorsPassEnabled;
        public NativeArray<int> materialFilterFlags;
    }

    internal struct GPUDrivenLODGroupData
    {
        /// <summary>
        /// LODGroup data.
        /// </summary>
        public NativeArray<int> lodGroupID;
        public NativeArray<int> lodOffset;
        public NativeArray<int> lodCount;
        public NativeArray<LODFadeMode> fadeMode;
        public NativeArray<Vector3> worldSpaceReferencePoint;
        public NativeArray<float> worldSpaceSize;
        public NativeArray<short> renderersCount;
        public NativeArray<bool> lastLODIsBillboard;

        /// <summary>
        /// Invalid or disabled LODGroup IDs.
        /// </summary>
        public NativeArray<int> invalidLODGroupID;

        /// <summary>
        /// LOD Group data. Indexed by lodOffset and lodCount.
        /// </summary>
        public NativeArray<short> lodRenderersCount;
        public NativeArray<float> lodScreenRelativeTransitionHeight;
        public NativeArray<float> lodFadeTransitionWidth;
    }

    internal struct GPUDrivenSpeedTreeWindData
    {
        /// <summary>
        /// SpeedTree wind data.
        /// </summary>
        public bool history;
        public NativeArray<int> instance;
        public NativeArray<Vector4> windVector;
        public NativeArray<Vector4> windGlobal;
        public NativeArray<Vector4> windBranchAdherences;
        public NativeArray<Vector4> windBranch;
        public NativeArray<Vector4> windBranchTwitch;
        public NativeArray<Vector4> windBranchWhip;
        public NativeArray<Vector4> windBranchAnchor;
        public NativeArray<Vector4> windTurbulences;
        public NativeArray<Vector4> windLeaf1Ripple;
        public NativeArray<Vector4> windLeaf1Tumble;
        public NativeArray<Vector4> windLeaf1Twitch;
        public NativeArray<Vector4> windLeaf2Ripple;
        public NativeArray<Vector4> windLeaf2Tumble;
        public NativeArray<Vector4> windLeaf2Twitch;
        public NativeArray<Vector4> windFrondRipple;
        public NativeArray<Vector4> windAnimation;
    };
}
