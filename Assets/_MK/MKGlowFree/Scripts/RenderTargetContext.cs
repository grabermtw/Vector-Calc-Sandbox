//////////////////////////////////////////////////////
// MK Glow RenderTargetContext 	    	    	    //
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de | www.michaelkremmel.store //
// Copyright © 2019 All rights reserved.            //
//////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MK.Glow
{
    //To reduce garbage collection this part is hardcoded
    /// <summary>
    /// Render targets based on a given render context
    /// </summary>
	internal static class RenderTargetContext
	{
		private static int _renderTargetCount;
		#if UNITY_2018_3_OR_NEWER
        private static RenderTargetBinding[] _mrtBindings = new RenderTargetBinding[6]
        {
            new RenderTargetBinding
            (
                new RenderTargetIdentifier[1], 
                new RenderBufferLoadAction[1]{RenderBufferLoadAction.DontCare},
                new RenderBufferStoreAction[1]{RenderBufferStoreAction.Store},
                0, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
            ),
            new RenderTargetBinding
            (
                new RenderTargetIdentifier[2], 
                new RenderBufferLoadAction[2]{RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare},
                new RenderBufferStoreAction[2]{RenderBufferStoreAction.Store, RenderBufferStoreAction.Store},
                0, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
            ),
            new RenderTargetBinding
            (
                new RenderTargetIdentifier[3], 
                new RenderBufferLoadAction[3]{RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare},
                new RenderBufferStoreAction[3]{RenderBufferStoreAction.Store, RenderBufferStoreAction.Store, RenderBufferStoreAction.Store},
                0, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
            ),
            new RenderTargetBinding
            (
                new RenderTargetIdentifier[4], 
                new RenderBufferLoadAction[4]{RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare},
                new RenderBufferStoreAction[4]{RenderBufferStoreAction.Store, RenderBufferStoreAction.Store, RenderBufferStoreAction.Store, RenderBufferStoreAction.Store},
                0, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
            ),
            new RenderTargetBinding
            (
                new RenderTargetIdentifier[5], 
                new RenderBufferLoadAction[5]{RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare},
                new RenderBufferStoreAction[5]{RenderBufferStoreAction.Store, RenderBufferStoreAction.Store, RenderBufferStoreAction.Store, RenderBufferStoreAction.Store, RenderBufferStoreAction.Store},
                0, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
            ),
            new RenderTargetBinding
            (
                new RenderTargetIdentifier[6], 
                new RenderBufferLoadAction[6]{RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare},
                new RenderBufferStoreAction[6]{RenderBufferStoreAction.Store, RenderBufferStoreAction.Store, RenderBufferStoreAction.Store, RenderBufferStoreAction.Store, RenderBufferStoreAction.Store, RenderBufferStoreAction.Store},
                0, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
            )
        };

        internal static void SetRenderTargetContext(this CommandBuffer cmd, List<RenderTargetIdentifier> renderTargets)
		{
			_renderTargetCount = renderTargets.Count - 1;
			for(int i = 0; i <= _renderTargetCount; i++)
            {
                _mrtBindings[_renderTargetCount].colorRenderTargets[i] = renderTargets[i];
            }
            _mrtBindings[_renderTargetCount].depthRenderTarget = _mrtBindings[_renderTargetCount].colorRenderTargets[0];
            cmd.SetRenderTarget(_mrtBindings[_renderTargetCount]);
		}
        #else
        private static RenderTargetIdentifier[][] _mrtBindings = new RenderTargetIdentifier[6][]
        {
            new RenderTargetIdentifier[1]{new RenderTargetIdentifier()},
            new RenderTargetIdentifier[2]{new RenderTargetIdentifier(), new RenderTargetIdentifier()},
            new RenderTargetIdentifier[3]{new RenderTargetIdentifier(), new RenderTargetIdentifier(), new RenderTargetIdentifier()},
            new RenderTargetIdentifier[4]{new RenderTargetIdentifier(), new RenderTargetIdentifier(), new RenderTargetIdentifier(), new RenderTargetIdentifier()},
            new RenderTargetIdentifier[5]{new RenderTargetIdentifier(), new RenderTargetIdentifier(), new RenderTargetIdentifier(), new RenderTargetIdentifier(), new RenderTargetIdentifier()},
            new RenderTargetIdentifier[6]{new RenderTargetIdentifier(), new RenderTargetIdentifier() ,new RenderTargetIdentifier() ,new RenderTargetIdentifier() ,new RenderTargetIdentifier(), new RenderTargetIdentifier()}
        };

		internal static void SetRenderTargetContext(this CommandBuffer cmd, List<RenderTargetIdentifier> renderTargets)
		{
			_renderTargetCount = renderTargets.Count - 1;
			for(int i = 0; i <= _renderTargetCount; i++)
            {
                _mrtBindings[_renderTargetCount][i] = renderTargets[i];
            }
            cmd.SetRenderTarget(_mrtBindings[_renderTargetCount], _mrtBindings[_renderTargetCount][0]);
		}
        #endif
	}
}
