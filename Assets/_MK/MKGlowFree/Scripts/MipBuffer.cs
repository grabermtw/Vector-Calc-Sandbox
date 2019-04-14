//////////////////////////////////////////////////////
// MK Glow Mip Buffer	    	    	       		//
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
	#if UNITY_2018_3_OR_NEWER
	using XRSettings = UnityEngine.XR.XRSettings;
	#endif

	/// <summary>
	/// Renderbuffer based on a mip setup
	/// </summary>
	internal sealed class MipBuffer
	{
		private int[] _identifiers = new int[PipelineProperties.renderBufferSize];
		internal int[] identifiers { get{ return _identifiers; } }

		private RenderTargetIdentifier[] _renderTargets = new RenderTargetIdentifier[PipelineProperties.renderBufferSize];
		internal RenderTargetIdentifier[] renderTargets { get { return _renderTargets; } }

		public MipBuffer(string name)
		{
			for(int i = 0; i < PipelineProperties.renderBufferSize; i++)
			{
				_identifiers[i] = Shader.PropertyToID(name + i);
				#if UNITY_2018_2_OR_NEWER
				_renderTargets[i] = new RenderTargetIdentifier(_identifiers[i], 0, CubemapFace.Unknown, -1);
				#else
				_renderTargets[i] = new RenderTargetIdentifier(_identifiers[i]);
				#endif
			}
		}

		/// <summary>
		/// Create a specific level of the the buffer
		/// </summary>
		/// <param name="renderContext"></param>
		/// <param name="level"></param>
		/// <param name="cmd"></param>
		/// <param name="format"></param>
		/// <param name="useComputeShaders"></param>
		internal void CreateTemporary(RenderContext[] renderContext, int level, CommandBuffer cmd, RenderTextureFormat format)
		{
			#if UNITY_2017_1_OR_NEWER
			cmd.GetTemporaryRT(_identifiers[level], renderContext[level].descriptor, FilterMode.Bilinear);
			#else
			cmd.GetTemporaryRT(_identifiers[level], renderContext[level].width, renderContext[level].height, 0, FilterMode.Bilinear, format, RenderTextureReadWrite.Default, 1, false);
			#endif
		}

		/// <summary>
		/// Clear a specific level of the buffer
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="level"></param>
		internal void ClearTemporary(CommandBuffer cmd, int level)
		{
			//if(cmd != null)
			cmd.ReleaseTemporaryRT(_identifiers[level]);
		}
	}
}