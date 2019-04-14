//////////////////////////////////////////////////////
// MK Glow Effect	    			                //
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de | www.michaelkremmel.store //
// Copyright © 2019 All rights reserved.            //
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

namespace MK.Glow
{
    using ShaderProperties = PipelineProperties.ShaderProperties;
    #if UNITY_2018_3_OR_NEWER
    using XRSettings = UnityEngine.XR.XRSettings;
    #endif

    internal sealed class Effect
    {
        internal Effect()
        {
            _resources = MK.Glow.Resources.LoadResourcesAsset();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Members
        /////////////////////////////////////////////////////////////////////////////////////////////
        //always needed parameters - static
        private static MK.Glow.Resources _resources;
        private static readonly Vector2 _selectiveWorkflowThreshold = new Vector2(0.1f, 10);

        //Selective rendering objects
        private static readonly string _selectiveReplacementTag = "RenderType";
        private static readonly string _selectiveGlowCameraObjectName = "selectiveGlowCameraObject";
        private GameObject _selectiveGlowCameraObject;
        private Camera _selectiveGlowCamera;

        //Renderbuffers
        private CommandBuffer _commandBuffer;
        private RenderTexture _selectiveRenderTexture;
        private MipBuffer _sourceBuffer;
		private MipBuffer _bloomDownsampleBuffer, _bloomUpsampleBuffer;

        private RenderTargetIdentifier _sourceFrameBuffer, _destinationFrameBuffer;
        private RenderTargetIdentifier sourceFrameBuffer
        {
            get 
            {
                return _settings.workflow == Workflow.Selective && _settings.debugView != DebugView.None ? _selectiveRenderTexture : _sourceFrameBuffer;
            }
        }

        //Runtime needed
        private Keyword[] _shaderKeywords = new Keyword[] 
        {
            new Keyword("_EMISSION", false),
            new Keyword("_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", false), //No Keyword will be set
            new Keyword("EDITOR_VISUALIZATION", false)
        };

        //Lists
        private List<RenderTargetIdentifier> _renderTargetsBundle;
        private List<MaterialKeywords> _renderKeywordsBundle;

        //Rendering dependent
        private int _bloomIterations, _currentRenderIndex;
        private float bloomUpsampleSpread;
        private RenderTextureFormat _renderTextureFormat;
        internal RenderTextureFormat renderTextureFormat { get{ return _renderTextureFormat; } }
        private RenderContext[] _sourceContext, _renderContext;
        private RenderContext _selectiveRenderContext;
        private Camera _renderingCamera;

        //Materials
        private Material _renderMaterialNoGeometry;

        //Settings
        private Settings _settings = new Settings();

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Unity MonoBehavior Messages
        /////////////////////////////////////////////////////////////////////////////////////////////
        internal void Enable()
        {
            _sourceContext = new RenderContext[1]{new RenderContext()};
            _renderContext = new RenderContext[PipelineProperties.renderBufferSize];
            for(int i = 0; i < PipelineProperties.renderBufferSize; i++)
                _renderContext[i] = new RenderContext();
            _selectiveRenderContext = new RenderContext();

            _renderMaterialNoGeometry = new Material(_resources.sm40Shader) { hideFlags = HideFlags.HideAndDontSave };

            _renderTargetsBundle = new List<RenderTargetIdentifier>();
            _renderKeywordsBundle = new List<MaterialKeywords>();

            //create buffers
            _sourceBuffer = new MipBuffer(PipelineProperties.CommandBufferProperties.sourceBuffer);
            _bloomDownsampleBuffer = new MipBuffer(PipelineProperties.CommandBufferProperties.bloomDownsampleBuffer);
            _bloomUpsampleBuffer = new MipBuffer(PipelineProperties.CommandBufferProperties.bloomUpsampleBuffer);
        }

        internal void Disable()
        {            
            _currentRenderIndex = 0;
            _renderTargetsBundle.Clear();
            _renderKeywordsBundle.Clear();

            MonoBehaviour.DestroyImmediate(_selectiveGlowCamera);
            MonoBehaviour.DestroyImmediate(_selectiveGlowCameraObject);
            MonoBehaviour.DestroyImmediate(_renderMaterialNoGeometry);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // RenderBuffers
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Prepare Scattering parameters fora given Scattering value
        /// </summary>
        /// <param name="Scattering"></param>
        /// <param name="scale"></param>
        /// <param name="iterations"></param>
        /// <param name="spread"></param>
        private void PrepareScattering(float Scattering, float scale, ref int iterations, ref float spread)
        {
            float scaledIterations = scale + Mathf.Clamp(Scattering, 1f, 10.0f) - 10.0f;
            iterations = Mathf.Max(Mathf.FloorToInt(scaledIterations), 1);
            spread = scaledIterations > 1 ? 0.5f + scaledIterations - iterations : 0.5f;
        }

        /// <summary>
        /// Create renderbuffers
        /// </summary>
        private void UpdateRenderBuffers()
        {
            RenderDimension renderDimension = new RenderDimension(_renderingCamera.pixelWidth, _renderingCamera.pixelHeight);
            _sourceContext[0].UpdateRenderContext(_renderingCamera, _renderTextureFormat, 0, renderDimension);
            _sourceContext[0].SinglePassStereoAdjustWidth(_renderingCamera.stereoEnabled);
            renderDimension = _sourceContext[0].renderDimension;

            renderDimension.width /= 2;
            renderDimension.height /= 2;

            float sizeScale = Mathf.Log(Mathf.FloorToInt(Mathf.Max(renderDimension.width, renderDimension.height)), 2.0f);
            
            PrepareScattering(_settings.bloomScattering, sizeScale, ref _bloomIterations, ref bloomUpsampleSpread);

            _renderingCamera.UpdateMipRenderContext(_renderContext, renderDimension, _bloomIterations + 1, _renderTextureFormat, 0);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Selective glow setup
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// selective replacement shader rendering camera forthe glow
        /// </summary>
        private GameObject selectiveGlowCameraObject
        {
            get
            {
                if(!_selectiveGlowCameraObject)
                {
                    _selectiveGlowCameraObject = new GameObject(_selectiveGlowCameraObjectName);
                    _selectiveGlowCameraObject.AddComponent<Camera>();
                    _selectiveGlowCameraObject.hideFlags = HideFlags.HideAndDontSave;
                }
                return _selectiveGlowCameraObject;
            }
        }

        /// <summary>
        /// selective replacement shader rendering camera forthe glow
        /// </summary>
        private Camera selectiveGlowCamera
        {
            get
            {
                if(_selectiveGlowCamera == null)
                {
                    _selectiveGlowCamera = selectiveGlowCameraObject.GetComponent<Camera>();
                    _selectiveGlowCamera.hideFlags = HideFlags.HideAndDontSave;
                    _selectiveGlowCamera.enabled = false;
                }
                return _selectiveGlowCamera;
            }
        }

        /// <summary>
        /// Prepare replacement rendering camera forthe selective glow
        /// </summary>
        private void SetupSelectiveGlowCamera()
        {
            selectiveGlowCamera.CopyFrom(_renderingCamera);
            selectiveGlowCamera.targetTexture = _selectiveRenderTexture;
            selectiveGlowCamera.clearFlags = CameraClearFlags.SolidColor;
            selectiveGlowCamera.rect = new Rect(0,0, 1,1);
            selectiveGlowCamera.backgroundColor = new Color(0, 0, 0, 1);
            selectiveGlowCamera.cullingMask = _settings.selectiveRenderLayerMask;
            selectiveGlowCamera.renderingPath = RenderingPath.VertexLit;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // CommandBuffer creation
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Enable or disable all supported / unsupported shaders based on the platform
        /// </summary>
        private void CheckFeatureSupport()
        {
            _renderTextureFormat = Compatibility.CheckSupportedRenderTextureFormat();
        }
    
        /// <summary>
        /// Renders the effect from source into destination buffer
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        internal void Build(RenderTargetIdentifier source, RenderTargetIdentifier destination, Settings settings, CommandBuffer cmd, Camera renderingCamera)
        {
            _commandBuffer = cmd;
            _settings = settings;
            _renderingCamera = renderingCamera;

            _commandBuffer.BeginSample(PipelineProperties.CommandBufferProperties.samplePrepare);
            
            CheckFeatureSupport();

            _sourceFrameBuffer = source;
            #if UNITY_2018_2_OR_NEWER
            _destinationFrameBuffer = new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1);
            #else
            _destinationFrameBuffer = destination;
            #endif

            UpdateRenderBuffers();
            _commandBuffer.EndSample(PipelineProperties.CommandBufferProperties.samplePrepare);

            //Prepare for selective glow
            if(_settings.workflow == Workflow.Selective)
            {
                _commandBuffer.BeginSample(PipelineProperties.CommandBufferProperties.sampleReplacement);
                _selectiveRenderContext.UpdateRenderContext(_renderingCamera, _renderTextureFormat, 16, _sourceContext[0].renderDimension);
                _selectiveRenderTexture = PipelineExtensions.GetTemporary(_selectiveRenderContext, _renderTextureFormat);
                SetupSelectiveGlowCamera();
                selectiveGlowCamera.RenderWithShader(_resources.selectiveRenderShader, _selectiveReplacementTag);
                _commandBuffer.EndSample(PipelineProperties.CommandBufferProperties.sampleReplacement);
            }

            _commandBuffer.BeginSample(PipelineProperties.CommandBufferProperties.sampleSetup);
            UpdateConstantBuffers();
            _commandBuffer.EndSample(PipelineProperties.CommandBufferProperties.sampleSetup);
            
            PreSample();
            Downsample();
            Upsample();
            Composite();
        }

        /// <summary>
        /// Update the profile based on the user input
        /// </summary>
        private void UpdateConstantBuffers()
        {      
            //Bloom
            SetFloat(PipelineProperties.ShaderProperties.bloomIntensity, ConvertGammaValue(_settings.bloomIntensity));
            SetFloat(PipelineProperties.ShaderProperties.bloomSpread, bloomUpsampleSpread);
            SetFloat(PipelineProperties.ShaderProperties.bloomSpread, bloomUpsampleSpread);

            SetVector(PipelineProperties.ShaderProperties.bloomThreshold, _settings.workflow == Workflow.Selective ? _selectiveWorkflowThreshold : new Vector2(ConvertGammaValue(_settings.bloomThreshold.minValue), ConvertGammaValue(_settings.bloomThreshold.maxValue)));
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Commandbuffer helpers
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Set a specific keyword for the pixelshader
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="enable"></param>
        private void SetKeyword(MaterialKeywords keyword, bool enable)
        {
            //For now disable check if da keyword is already set
            //to make sure the cmd is always correctly setuped
            //if(_shaderKeywords[(int)keyword].enabled != enable)
            {
                _commandBuffer.SetKeyword(_shaderKeywords[(int)keyword].name, enable);
                _shaderKeywords[(int)keyword].enabled = enable;
            }
        }

        /// <summary>
        /// Convert an angle (degree) to a Vector2 direction
        /// </summary>
        /// <returns></returns>
        private Vector2 AngleToDirection(float angleDegree)
        {
            return new Vector2(Mathf.Sin(angleDegree * Mathf.Deg2Rad), Mathf.Cos(angleDegree * Mathf.Deg2Rad));
        }

        /// <summary>
        /// get a threshold value based on current color space
        /// </summary>
        private float ConvertGammaValue(float gammaSpacedValue)
        {
            if(QualitySettings.activeColorSpace == ColorSpace.Linear)
            {
                return Mathf.GammaToLinearSpace(gammaSpacedValue);
            }
            else
                return gammaSpacedValue;
        }

        /// <summary>
        /// get a threshold value based on current color space
        /// </summary>
        private Vector4 ConvertGammaValue(Vector4 gammaSpacedVector)
        {
            if(QualitySettings.activeColorSpace == ColorSpace.Linear)
            {
                gammaSpacedVector.x = ConvertGammaValue(gammaSpacedVector.x);
                gammaSpacedVector.y = ConvertGammaValue(gammaSpacedVector.y);
                gammaSpacedVector.z = ConvertGammaValue(gammaSpacedVector.z);
                gammaSpacedVector.w = ConvertGammaValue(gammaSpacedVector.w);
                return gammaSpacedVector;
            }
            else
                return gammaSpacedVector;
        }
        
        /// <summary>
        /// Update the renderindex (pass) forthe next Draw
        /// </summary>
        /// <param name="v"></param>
        private void UpdateRenderIndex(int v)
        {
            _currentRenderIndex = v;
        }

        /// <summary>
        /// Auto set a float value on the renderpipeline
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        private void SetFloat(ShaderProperties.CBufferProperty property, float value)
        {
            _commandBuffer.SetGlobalFloat(property.id, value);    
        }

        /// <summary>
        /// Auto set a vector value on the renderpipeline
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        private void SetVector(ShaderProperties.CBufferProperty property, Vector4 value)
        {
            _commandBuffer.SetGlobalVector(property.id, value);
        }

        /// <summary>
        /// Auto set a vector value on the renderpipeline
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        private void SetVector(ShaderProperties.CBufferProperty property, Vector3 value)
        {
            _commandBuffer.SetGlobalVector(property.id, value);
        }

        /// <summary>
        /// Auto set a vector value on the renderpipeline
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        private void SetVector(ShaderProperties.CBufferProperty property, Vector2 value)
        {
            _commandBuffer.SetGlobalVector(property.id, value);
        }

        /// <summary>
        /// Auto set a texture on the renderpipeline, 
        /// always update the computeKernelIndexBuffer before using this to get the correct variant while using compute shaders
        /// </summary>
        /// <param name="property"></param>
        /// <param name="rti"></param>
        /// <param name="forcePixelShader"></param>
        private void SetTexture(ShaderProperties.DefaultProperty property, RenderTargetIdentifier rti)
        {
            _commandBuffer.SetGlobalTexture(property.id, rti);
        }
        
        /// <summary>
        /// Setup for the next draw command
        /// </summary>
        /// <param name="variant"></param>
        /// <param name="renderDimension"></param>
        /// <param name="forcePixelShader"></param>
        private void PrepareDraw(int variant)
        {
            UpdateRenderIndex(variant);
            foreach(MaterialKeywords kw in _renderKeywordsBundle)
                SetKeyword(kw, true);
            _renderKeywordsBundle.Clear();
        }

        /// <summary>
        /// Draw into a destination framebuffer based on shadertype
        /// Always prepare for drawing using the PrepareDraw command
        /// </summary>
        /// <param name="forcePixelShader"></param>
        private void Draw(bool forcePixelShader = false)
        {
            _commandBuffer.Draw(_renderTargetsBundle, _renderMaterialNoGeometry, _currentRenderIndex);
            _renderTargetsBundle.Clear();
        } 

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Sampling
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Disable debug Keywords
        /// </summary>
        private void DisableDebugKeywords()
        {
            SetKeyword(MaterialKeywords.DebugRawBloom, false);
            SetKeyword(MaterialKeywords.DebugBloom, false);
            SetKeyword(MaterialKeywords.DebugComposite, false);
        }
        
        /// <summary>
        /// Pre sample the glow map
        /// </summary>
        private void PreSample()
        {
            _commandBuffer.BeginSample(PipelineProperties.CommandBufferProperties.samplePreSample);

            _bloomDownsampleBuffer.CreateTemporary(_renderContext, 0, _commandBuffer, _renderTextureFormat);
            _renderTargetsBundle.Add(_bloomDownsampleBuffer.renderTargets[0]);

            PrepareDraw
            (   
                (int)ShaderRenderPass.Presample
            );

            SetTexture(PipelineProperties.ShaderProperties.sourceTex, _settings.workflow == Workflow.Luminance ? sourceFrameBuffer : _selectiveRenderTexture);

            Draw();

            if(_settings.workflow == Workflow.Selective)
                RenderTexture.ReleaseTemporary(_selectiveRenderTexture);

            _commandBuffer.EndSample(PipelineProperties.CommandBufferProperties.samplePreSample);
        }

        /// <summary>
        /// Downsample the glow map
        /// </summary>
        private void Downsample()
        {
            _commandBuffer.BeginSample(PipelineProperties.CommandBufferProperties.sampleDownsample);

            for(int i = 0; i < _bloomIterations; i++)
            {
                _bloomDownsampleBuffer.CreateTemporary(_renderContext, i + 1, _commandBuffer, _renderTextureFormat);
                _renderTargetsBundle.Add(_bloomDownsampleBuffer.renderTargets[i + 1]);

                PrepareDraw
                (   
                    (int)ShaderRenderPass.Downsample
                );
                    
                SetTexture(PipelineProperties.ShaderProperties.bloomTex, _bloomDownsampleBuffer.identifiers[i]);

                Draw();
            }
            _commandBuffer.EndSample(PipelineProperties.CommandBufferProperties.sampleDownsample);
        }


        /// <summary>
        /// Upsample the glow map
        /// </summary>
        private void Upsample()
        {
            _commandBuffer.BeginSample(PipelineProperties.CommandBufferProperties.sampleUpsample);

            for(int i = _bloomIterations; i > 0; i--)
            {
                _bloomUpsampleBuffer.CreateTemporary(_renderContext, i - 1, _commandBuffer, _renderTextureFormat);
                _renderTargetsBundle.Add(_bloomUpsampleBuffer.renderTargets[i - 1]);

                PrepareDraw
                (   
                    (int)ShaderRenderPass.Upsample
                );

                SetTexture(PipelineProperties.ShaderProperties.higherMipBloomTex, _bloomDownsampleBuffer.identifiers[i - 1]);
                SetTexture(PipelineProperties.ShaderProperties.bloomTex, (i >= _bloomIterations) ? _bloomDownsampleBuffer.identifiers[i] : _bloomUpsampleBuffer.identifiers[i]);

                Draw();

                if(i >= _bloomIterations)
                    _bloomDownsampleBuffer.ClearTemporary(_commandBuffer, i);
                else
                {
                    _bloomDownsampleBuffer.ClearTemporary(_commandBuffer, i);
                    _bloomUpsampleBuffer.ClearTemporary(_commandBuffer, i);
                }
            }

            _bloomDownsampleBuffer.ClearTemporary(_commandBuffer, 0);

            _commandBuffer.EndSample(PipelineProperties.CommandBufferProperties.sampleUpsample);
        }

        /// <summary>
        /// Precomposite of the glow map
        /// </summary>
        private void Composite()
        {
            _commandBuffer.BeginSample(PipelineProperties.CommandBufferProperties.sampleComposite);

            int renderpass;
            
            switch(_settings.debugView)
            {
                case DebugView.RawBloom:
                    _renderKeywordsBundle.Add(MaterialKeywords.DebugRawBloom);
                    renderpass = (int)ShaderRenderPass.Debug;
                break;
                case DebugView.Bloom:
                    _renderKeywordsBundle.Add(MaterialKeywords.DebugBloom);
                    renderpass = (int)ShaderRenderPass.Debug;
                break;
                default:
                    renderpass = (int)ShaderRenderPass.Composite;
                break;
            }

            PrepareDraw
            (   
                renderpass
            );

            SetTexture(PipelineProperties.ShaderProperties.sourceTex, sourceFrameBuffer);
            SetTexture(PipelineProperties.ShaderProperties.bloomTex, _bloomUpsampleBuffer.identifiers[0]);

            _renderTargetsBundle.Add(_destinationFrameBuffer);

            Draw();

            _sourceBuffer.ClearTemporary(_commandBuffer, 0);
            _bloomUpsampleBuffer.ClearTemporary(_commandBuffer, 0);

            DisableDebugKeywords();

            _commandBuffer.EndSample(PipelineProperties.CommandBufferProperties.sampleComposite);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Enum / structs used for rendering
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// 
        /// <summary>
        /// Rendering passes for shaders
        /// </summary>
        internal enum ShaderRenderPass
        {
            Copy = 0,
            Presample = 1,
            Downsample = 2,
            Upsample = 3,
            Composite = 4,
            Debug = 5
        }

        /// <summary>
        /// Material keywords represented in the keyword holder
        /// </summary>
        internal enum MaterialKeywords
        {
            DebugRawBloom = 0,
            DebugBloom = 1,
            DebugComposite = 2
        }
        
        /// <summary>
        /// Keyword represented as with state
        /// </summary>
        internal struct Keyword
        {
            internal string name;
            internal bool enabled;

            internal Keyword(string name, bool enabled)
            {
                this.name = name;
                this.enabled = enabled;
            }
        }
    }
}