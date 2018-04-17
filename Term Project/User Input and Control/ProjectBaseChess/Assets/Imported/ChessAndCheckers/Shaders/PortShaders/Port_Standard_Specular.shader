Shader "Portalarium/Standard (Specular Setup)" 
{   
	Properties 
	{ 
		[LM_Albedo] [LM_Transparency] _Color("Color", Color) = (1,1,1,1)	
		[LM_MasterTilingOffset] [LM_Albedo] _MainTex("Albedo", 2D) = "white" {}
		
		[LM_TransparencyCutOff] _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		[LM_Glossiness] _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.0
		[LM_Specular] _SpecColor("Specular", Color) = (0.2,0.2,0.2)	
		[LM_Specular] [LM_Glossiness] _SpecGlossMap("Specular", 2D) = "white" {}
		  
		 _BumpScale("Scale", Float) = 1.0
		[LM_NormalMap] _BumpMap("Normal Map", 2D) = "bump" {}

		_Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
		_ParallaxMap ("Height Map", 2D) = "black" {}

		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		[LM_Emission] _EmissionColor("Color", Color) = (0,0,0)
		[LM_Emission] _EmissionMap("Emission", 2D) = "white" {}
		   
		_DetailMask("Detail Mask", 2D) = "white" {}
		  
		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}
		_DetailAlbedoStrength ("Detail Albedo Strength", Range(0.0, 1.0)) = 1
		_DetailAlbedoDodgeStrength ("Dodge Strength", Range(0.0, 0.5)) = .4
 
		_ReplacementMap("Replacement Map (B)", 2D) = "black" {}
		_PrimaryColor("Primary Color", Color) = (1.0, 0.0, 0.0, 0)
		_SecondaryColor("Secondary Color", Color) = (0.0, 1.0, 0.0, 0)
		_TertiaryColor("Tertiary Color", Color) = (0.0, 0.0, 1.0, 0)
		   
		_PrimarySpecular("Primary Specular", Color) = (0.0, 0.0, 0.0, 0.5)
		_SecondarySpecular("Secondary Specular", Color) = (0.0, 0.0, 0.0, 0.5)
		_TertiarySpecular("Tertiary Specular", Color) = (0.0, 0.0, 0.0, 0.5)
		  
		_Dissolve("Dissolve Amount", Range(0.0, 1.0)) = 0.0
		_BorderSize("Border Percent", Range(0.0,0.1)) = 0.01

		_SaturationAmount("Saturation Amount", Range(0.0, 2.0)) = 1.0
		_BrightnessAmount("Brightness Amount", Range(0.0, 2.0)) = 1.0
		_ContrastAmount("Contrast Amount", Range(0.0,2.0)) = 1.0

		_SSSScale("SSS Scale", Float) = 1
		_BRDF("BRDF Ramp", 2D) = "black" {}
		_DepthMap("Depth Map", 2D) = "black" {}

		_TranslucencyOffset ("Translucency Offset", Float) = 0.0
		_TranslucencyPower ("Translucency Power", Float) = 1
		_TranslucencyRadius ("Translucency Radius", Range(0,1)) = 1
		 
		_LayerTex ("Layer (RGB)", 2D) = "white" {}
        _LayerBump ("Layer Normal", 2D) ="bump" {}
		_LayerBumpStrength ("Layer Normal Strength", Float) = 1
        _LayerStrength ("Layer Strength", Range(0, 1)) = 0
        _LayerScale ("Layer Scale", Float) = 1
		_LayerSmooth ("Layer Smooth", Range(0.01, 0.4)) = 0.2
        _LayerDirection ("Layer Direction", Vector) = (0, 1, 0)
            
		_AnisoSpecMap("Anisotropic Specular Map", 2D) = "black" {}
		_AnisoOffset ("Anisotropic Highlight Offset", Range(-1,1)) = 0.0
		_AnisoSpecColor ("Anisotropic Specular Color", Color) = (1,1,1,1)
		   
		_GlowColor ("Glow Color", Color) = (0,1,0,1)
		_GlowPower ("Glow Power", Range(0.0,8.0)) = 0.0
		_GlowSpeed ("Glow Speed", Range(0.01,8.0)) = 1.0 
		_GlowStatic ("Glow Static", Float) = 1.0
		     
		[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0

		[HideInInspector] _VertColor("_VertColor", Int) = 0
		[HideInInspector] _Skin("_Skin", Int) = 0
		[HideInInspector] _ColorAdjustment("_ColorAdjustment", Int) = 0
		[HideInInspector] _ColorReplacement("_ColorReplacement", Int) = 0
		[HideInInspector] _SSS("_SSS", Int) = 0
		[HideInInspector] _AddDirectionalLayer("_AddDirectionalLayer", Int) = 0

		[HideInInspector] _Translucency("_Translucency", Int) = 0
		[Enum(None,0,Front,1,Back,2)] _CullMode("Culling Mode", Int) = 2
		[Enum(None,0,Dissolve,1,Reverse,2)] _DissolveMode("Dissolve Mode", Int) = 0
		[Enum(Standard,0,Anisotropic,1)] _SpecularMode("Specular Mode", Int) = 0
		[Enum(Multiply X2,0,Multiply,1,Add,2,Lerp,3,Dodge,4)] _DetailMode("Detail Mode", Int) = 0
		 
		// UI-only data
		[KeywordEnum(None, Realtime, Baked)]  _Lightmapping ("GI", Int) = 1
		[HideInInspector] _EmissionScaleUI("Scale", Float) = 0.0
		[HideInInspector] _EmissionColorUI("Color", Color) = (1,1,1)
		 
		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	} 

	
	CGINCLUDE
		#define UNITY_SETUP_BRDF_INPUT SpecularSetup
		#define _NORMALMAP 1
		#define _PARALLAXMAP 1
	ENDCG

	SubShader
	{
		Tags { "Queue"="Geometry" "PerformanceChecks"="False" "RenderType"="Opaque"}
		LOD 300
	
		Cull [_CullMode]

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			CGPROGRAM
			#pragma target 3.0
			#pragma only_renderers d3d9 d3d11 opengl
			
			// -------------------------------------
					
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON 
			#pragma shader_feature _FEATHER_ON
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2 _DETAIL_MUL _DETAIL_ADD _DETAIL_LERP _DETAIL_DODGE
			#pragma shader_feature _COLORREPLACEMENT
			
			#pragma multi_compile_fwdbase
				
			#pragma vertex vertForwardBase
			#pragma fragment fragForwardBase

			#include "Includes/PortStandardCore.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			Blend [_SrcBlend] One
			Fog { Color (0,0,0,0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
			#pragma target 3.0
			#pragma only_renderers d3d9 d3d11 opengl

			// -------------------------------------

			
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON 
			#pragma shader_feature _FEATHER_ON
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2 _DETAIL_MUL _DETAIL_ADD _DETAIL_LERP _DETAIL_DODGE
			#pragma shader_feature _COLORREPLACEMENT
			
			#pragma multi_compile_fwdadd_fullshadows
			
			#pragma vertex vertForwardAdd
			#pragma fragment fragForwardAdd

			#include "Includes/PortStandardCore.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			
			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma target 3.0
			#pragma only_renderers d3d9 d3d11 opengl
			
			// -------------------------------------


			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma multi_compile_shadowcaster

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "Includes/PortStandardShadow.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Deferred pass
		Pass
		{
			Name "DEFERRED"
			Tags { "LightMode" = "Deferred" }

			CGPROGRAM
			#pragma target 3.0
			#pragma only_renderers d3d9 d3d11 opengl
			
			 
			// -------------------------------------

			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2 _DETAIL_MUL _DETAIL_ADD _DETAIL_LERP _DETAIL_DODGE
			#pragma shader_feature _VERTEXCOLOR
			#pragma shader_feature _ANISO
			#pragma shader_feature _COLORREPLACEMENT
			#pragma shader_feature _COLORADJUSTMENT
			#pragma shader_feature _ _ADD_DIRECTIONAL_LAYER
			 
			#pragma multi_compile ___ UNITY_HDR_ON
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			
			#pragma vertex vertDeferred
			#pragma fragment fragDeferred

			#include "Includes/PortStandardCore.cginc"

			ENDCG
		}

		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		Pass
		{
			Name "META"  
			Tags { "LightMode"="Meta" }

			Cull Off

			CGPROGRAM
			#pragma vertex vert_meta 
			#pragma fragment frag_meta
			 
			#pragma shader_feature _EMISSION
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2 _DETAIL_MUL _DETAIL_ADD _DETAIL_LERP _DETAIL_DODGE

			#include "UnityStandardMeta.cginc"
			ENDCG
		}
	}

	CustomEditor "PortStandardShaderGUI"
}