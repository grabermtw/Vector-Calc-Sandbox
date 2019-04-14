//////////////////////////////////////////////////////
// MK Glow 	    	    	                        //
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de | www.michaelkremmel.store //
// Copyright © 2019 All rights reserved.            //
//////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MK.Glow.Legacy
{
	#if UNITY_2018_3_OR_NEWER
        [ExecuteAlways]
    #else
        [ExecuteInEditMode]
    #endif
    [DisallowMultipleComponent]
    [ImageEffectAllowedInSceneView]
    [RequireComponent(typeof(Camera))]
	public class MKGlow : MonoBehaviour
	{
        #if UNITY_EDITOR
        public bool showEditorMainBehavior = true;
		public bool showEditorBloomBehavior;
		public bool showEditorLensSurfaceBehavior;
		public bool showEditorLensFlareBehavior;
		public bool showEditorGlareBehavior;
        #endif

        private CommandBuffer _commandBuffer;

        //Main
        public DebugView debugView = MK.Glow.DebugView.None;
        public Workflow workflow = MK.Glow.Workflow.Luminance;
        public LayerMask selectiveRenderLayerMask = -1;

        //Bloom
        [MK.Glow.MinMaxRange(0, 10)]
        public MinMaxRange bloomThreshold = new MinMaxRange(1.0f, 10f);
        [Range(1f, 10f)]
		public float bloomScattering = 7f;
		public float bloomIntensity = 1f;

        private RenderTexture _tmpTexture;
        private RenderContext _tmpContext;
        private RenderDimension _tmpRenderDimension = new RenderDimension();

        private Effect effect;

		private Camera RenderingCamera
		{
			get { return GetComponent<Camera>(); }
		}

		public void OnEnable()
		{
            _commandBuffer = new CommandBuffer { name = PipelineProperties.CommandBufferProperties.commandBufferName };

            _tmpContext = new RenderContext();

            effect = new Effect();

			effect.Enable();
		}

		public void OnDisable()
		{
			effect.Disable();
		}

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if(workflow == Workflow.Selective && PipelineProperties.xrEnabled)
            {
                Graphics.Blit(source, destination);
                return;
            }
            
            _commandBuffer.Clear();
            
            _tmpRenderDimension.width = RenderingCamera.pixelWidth;
            _tmpRenderDimension.height = RenderingCamera.pixelHeight;
            _tmpContext.UpdateRenderContext(RenderingCamera, effect.renderTextureFormat, 0, _tmpRenderDimension);
            _tmpTexture = PipelineExtensions.GetTemporary(_tmpContext, effect.renderTextureFormat);
			effect.Build(source, _tmpTexture, this, _commandBuffer, RenderingCamera);
            
            Graphics.ExecuteCommandBuffer(_commandBuffer);

            //Additional copy is need when rendering using commandbuffer and OnRenderImage to avoid warnings / flipped image
            //For now stick with the builtin blit, this could be optimized some day by allowing compute shaders in the final glow pass
            //and using pixel shaders for the final blit
            Graphics.Blit(_tmpTexture, destination);

            RenderTexture.ReleaseTemporary(_tmpTexture);
        }
	}
}