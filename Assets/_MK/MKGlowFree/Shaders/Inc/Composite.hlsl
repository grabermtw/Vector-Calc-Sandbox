//////////////////////////////////////////////////////
// MK Glow Composite								//
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de | www.michaelkremmel.store //
// Copyright © 2019 All rights reserved.            //
//////////////////////////////////////////////////////
#ifndef MK_GLOW_COMPOSITE
	#define MK_GLOW_COMPOSITE
	
	#include "../Inc/Common.hlsl"

	UNIFORM_SAMPLER_AND_TEXTURE_2D(_SourceTex)

	UNIFORM_SAMPLER_AND_TEXTURE_2D(_BloomTex)
	uniform float2 _BloomTex_TexelSize;
	uniform half _BloomSpread;
	uniform half _BloomIntensity;
	uniform float _BloomBlooming;

		#define VertGeoOutput VertGeoOutputAdvanced

		VertGeoOutput vert (VertexInputOnlyPosition i0)
		{
			VertGeoOutput o;
			UNITY_SETUP_INSTANCE_ID(i0);
			UNITY_INITIALIZE_OUTPUT(VertGeoOutput, o);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

			o.pos = TransformMeshPos(i0.vertex);

			o.uv0.xy = SetMeshUV(o.pos.xy);
			o.uv0.zw = BLOOM_TEXEL_SIZE * _BloomSpread;

			return o;
		}

	#define HEADER half4 frag (VertGeoOutput o) : SV_Target
	HEADER
	{
		#include "CompositeSample.hlsl"
	}
#endif