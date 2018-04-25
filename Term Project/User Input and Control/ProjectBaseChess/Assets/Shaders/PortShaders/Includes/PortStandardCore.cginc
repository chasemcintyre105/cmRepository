// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#ifndef PORT_STANDARD_CORE_INCLUDED
#define PORT_STANDARD_CORE_INCLUDED

#include "UnityCG.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityStandardConfig.cginc"
#include "Assets/Shaders/PortShaders/Includes/PortStandardInput.cginc"
#include "UnityPBSLighting.cginc"
#include "UnityStandardUtils.cginc"
#include "UnityStandardBRDF.cginc"

#include "AutoLight.cginc"

uniform float3 _SunDirection;

uniform int _DissolveMode;
sampler2D _DissolveMask;
float _Dissolve;
float _BorderSize;

fixed4 _GlowColor = fixed4(0,1,0,1);
float _GlowPower = 0;
float _GlowSpeed = 3.0;
uniform float _GlowStatic = 1;

#if _SSS
	sampler2D _DepthMap;

	float _TranslucencyOffset;
	float _TranslucencyPower;
	float _TranslucencyRadius;
	float _SSSScale;
	sampler2D _BRDF;
#endif
    
#if _ADD_DIRECTIONAL_LAYER
	sampler2D _LayerTex;
    sampler2D _LayerBump;
	float _LayerBumpStrength;
	float _LayerScale;
	float _LayerSmooth;
    float _LayerStrength;
    float3 _LayerDirection;
    float _LayerDepth;
#endif

#if _COLORREPLACEMENT
	sampler2D _ReplacementMap;
	fixed4 _PrimaryColor;
	fixed4 _SecondaryColor;
	fixed4 _TertiaryColor;

	fixed4 _PrimarySpecular;
	fixed4 _SecondarySpecular;
	fixed4 _TertiarySpecular;
#endif


#define Pi 3.14159265358979323846
#define OneOnLN2_x6 8.656170
#if _ANISO
	float _AnisoOffset, _SpecularMultiplier;
	fixed4 _AnisoSpecColor;
	fixed _Gloss;
	sampler2D _AnisoSpecMap;
	float4 _AnisoSpecMap_ST;
#endif

#if _COLORADJUSTMENT
	float _SaturationAmount;
	float _BrightnessAmount;
	float _ContrastAmount;

	float3 ContrastSaturationBrightness(float3 color, float brt, float sat, float con)
	{
		//RGB Color Channels
		float AvgLumR = 0.5;
		float AvgLumG = 0.5;
		float AvgLumB = 0.5;

		  //Luminace Coefficients for brightness of image
		float3 LuminaceCoeff = float3(0.2125, 0.7154, 0.0721);

		//Brigntess calculations
		float3 AvgLumin = float3(AvgLumR, AvgLumG, AvgLumB);
		float3 brtColor = color * brt;
		float intensityf = dot(brtColor, LuminaceCoeff);
		float3 intensity = float3(intensityf, intensityf, intensityf);

		//Saturation calculation
		float3 satColor = lerp(intensity, brtColor, sat);

		//Contrast calculations
		float3 conColor = lerp(AvgLumin, satColor, con);

		return conColor;
	}
#endif

//-------------------------------------------------------------------------------------
// counterpart for NormalizePerPixelNormal
// skips normalization per-vertex and expects normalization to happen per-pixel
half3 NormalizePerVertexNormal (half3 n)
{
	#if (SHADER_TARGET < 30)
		return normalize(n);
	#else
		return n; // will normalize per-pixel instead
	#endif
}

half3 NormalizePerPixelNormal (half3 n)
{
	#if (SHADER_TARGET < 30)
		return n;
	#else
		return normalize(n);
	#endif
}

//-------------------------------------------------------------------------------------
UnityLight MainLight (half3 normalWorld)
{
	UnityLight l;
	#ifdef LIGHTMAP_OFF
		
		l.color = _LightColor0.rgb;
		l.dir = _WorldSpaceLightPos0.xyz;
		l.ndotl = LambertTerm (normalWorld, l.dir);
	#else
		// no light specified by the engine
		// analytical light might be extracted from Lightmap data later on in the shader depending on the Lightmap type
		l.color = half3(0.f, 0.f, 0.f);
		l.ndotl  = 0.f;
		l.dir = half3(0.f, 0.f, 0.f);
	#endif

	return l;
}

UnityLight AdditiveLight (half3 normalWorld, half3 lightDir, half atten)
{
	UnityLight l;

	l.color = _LightColor0.rgb;
	l.dir = lightDir;
	#ifndef USING_DIRECTIONAL_LIGHT
		l.dir = NormalizePerPixelNormal(l.dir);
	#endif
	l.ndotl = LambertTerm (normalWorld, l.dir);

	// shadow the light
	l.color *= atten;
	return l;
}

UnityLight DummyLight (half3 normalWorld)
{
	UnityLight l;
	l.color = 0;
	l.dir = half3 (0,1,0);
	l.ndotl = LambertTerm (normalWorld, l.dir);
	return l;
}

UnityIndirect ZeroIndirect ()
{
	UnityIndirect ind;
	ind.diffuse = 0;
	ind.specular = 0;
	return ind;
}

//-------------------------------------------------------------------------------------
// Common fragment setup
half3 WorldNormal(half4 tan2world[3])
{
	return normalize(tan2world[2].xyz);
}

#ifdef _TANGENT_TO_WORLD
	half3x3 ExtractTangentToWorldPerPixel(half4 tan2world[3])
	{
		half3 t = tan2world[0].xyz;
		half3 b = tan2world[1].xyz;
		half3 n = tan2world[2].xyz;

	#if UNITY_TANGENT_ORTHONORMALIZE
		n = NormalizePerPixelNormal(n);

		// ortho-normalize Tangent
		t = normalize (t - n * dot(t, n));

		// recalculate Binormal
		half3 newB = cross(n, t);
		b = newB * sign (dot (newB, b));
	#endif

		return half3x3(t, b, n);
	}
#else
	half3x3 ExtractTangentToWorldPerPixel(half4 tan2world[3])
	{
		return half3x3(0,0,0,0,0,0,0,0,0);
	}
#endif

#ifdef _PARALLAXMAP
	#define IN_VIEWDIR4PARALLAX(i) NormalizePerPixelNormal(half3(i.tangentToWorldAndParallax[0].w,i.tangentToWorldAndParallax[1].w,i.tangentToWorldAndParallax[2].w))
	#define IN_VIEWDIR4PARALLAX_FWDADD(i) NormalizePerPixelNormal(i.viewDirForParallax.xyz)
#else
	#define IN_VIEWDIR4PARALLAX(i) half3(0,0,0)
	#define IN_VIEWDIR4PARALLAX_FWDADD(i) half3(0,0,0)
#endif

#if UNITY_SPECCUBE_BOX_PROJECTION
	#define IN_WORLDPOS(i) i.posWorld
#else
	#define IN_WORLDPOS(i) half3(0,0,0)
#endif

#define IN_LIGHTDIR_FWDADD(i) half3(i.tangentToWorldAndLightDir[0].w, i.tangentToWorldAndLightDir[1].w, i.tangentToWorldAndLightDir[2].w)

#define FRAGMENT_SETUP(x) FragmentCommonData x = \
	FragmentSetup(i.tex, i.eyeVec, WorldNormal(i.tangentToWorldAndParallax), IN_VIEWDIR4PARALLAX(i), ExtractTangentToWorldPerPixel(i.tangentToWorldAndParallax), IN_WORLDPOS(i));

#define FRAGMENT_SETUP_FWDADD(x) FragmentCommonData x = \
	FragmentSetup(i.tex, i.eyeVec, WorldNormal(i.tangentToWorldAndLightDir), IN_VIEWDIR4PARALLAX_FWDADD(i), ExtractTangentToWorldPerPixel(i.tangentToWorldAndLightDir), half3(0,0,0));

struct FragmentCommonData
{
	half3 diffColor, specColor;
	// Note: oneMinusRoughness & oneMinusReflectivity for optimization purposes, mostly for DX9 SM2.0 level.
	// Most of the math is being done on these (1-x) values, and that saves a few precious ALU slots.
	half oneMinusReflectivity, oneMinusRoughness;
	half3 normalWorld, eyeVec, posWorld;
	half alpha;
};

#ifndef UNITY_SETUP_BRDF_INPUT
	#define UNITY_SETUP_BRDF_INPUT SpecularSetup
#endif

inline half ApplyGammaToLinear(half sRGB) { return sRGB * (sRGB * (sRGB * 0.305306011 + 0.682171111) + 0.012522878); }
inline FragmentCommonData SpecularSetup (float4 i_tex)
{
	half4 specGloss = SpecularGloss(i_tex.xy);
	half3 specColor = specGloss.rgb;
	half oneMinusRoughness = ApplyGammaToLinear(specGloss.a);
	half oneMinusReflectivity;

	half3 diffColor = Albedo(i_tex);
	
	#if _COLORREPLACEMENT
		float4 replacement = tex2D(_ReplacementMap, i_tex.xy);

		float rComponent = diffColor.rgb.r * replacement.b;
		float gComponent = diffColor.rgb.g * replacement.b;
		float bComponent = diffColor.rgb.b * replacement.b;


		diffColor.rgb += (rComponent * _PrimaryColor.rgb);
		diffColor.rgb += (gComponent * _SecondaryColor.rgb);
		diffColor.rgb += (bComponent * _TertiaryColor.rgb);

		diffColor.r -= rComponent;
		diffColor.g -= gComponent;
		diffColor.b -= bComponent;

		oneMinusRoughness += rComponent * (_PrimarySpecular.a - 0.5) * 2;
		oneMinusRoughness += gComponent * (_SecondarySpecular.a - 0.5) * 2;
		oneMinusRoughness += bComponent * (_TertiarySpecular.a - 0.5) * 2;
	#endif


	diffColor = EnergyConservationBetweenDiffuseAndSpecular (diffColor, specColor, /*out*/ oneMinusReflectivity);
	
	#if _COLORREPLACEMENT
		specColor.rgb += rComponent * _PrimarySpecular;
		specColor.rgb += gComponent * _SecondarySpecular;
		specColor.rgb += bComponent * _TertiarySpecular;
	#endif

	FragmentCommonData o = (FragmentCommonData)0;
	o.diffColor = diffColor;
	o.specColor = specColor;
	o.oneMinusReflectivity = oneMinusReflectivity;
	o.oneMinusRoughness = oneMinusRoughness;
	return o;
}

inline FragmentCommonData MetallicSetup (float4 i_tex)
{
	half2 metallicGloss = MetallicGloss(i_tex.xy);
	half metallic = metallicGloss.x;
	half oneMinusRoughness = metallicGloss.y;

	half oneMinusReflectivity;
	half3 specColor;
	half3 diffColor = Albedo(i_tex);

	#if _COLORREPLACEMENT
		float4 replacement = tex2D(_ReplacementMap, i_tex.xy);

		float rComponent = diffColor.rgb.r * replacement.b;
		float gComponent = diffColor.rgb.g * replacement.b;
		float bComponent = diffColor.rgb.b * replacement.b;


		diffColor.rgb += (rComponent * _PrimaryColor.rgb);
		diffColor.rgb += (gComponent * _SecondaryColor.rgb);
		diffColor.rgb += (bComponent * _TertiaryColor.rgb);

		diffColor.r -= rComponent;
		diffColor.g -= gComponent;
		diffColor.b -= bComponent;

		metallic += rComponent * (_PrimarySpecular.a - 0.5) * 2;
		metallic += gComponent * (_SecondarySpecular.a - 0.5) * 2;
		metallic += bComponent * (_TertiarySpecular.a - 0.5) * 2;
	#endif


	diffColor = DiffuseAndSpecularFromMetallic (diffColor, metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

	#if _COLORREPLACEMENT
		specColor.rgb += rComponent * _PrimarySpecular;
		specColor.rgb += gComponent * _SecondarySpecular;
		specColor.rgb += bComponent * _TertiarySpecular;
	#endif

	FragmentCommonData o = (FragmentCommonData)0;
	o.diffColor = diffColor;
	o.specColor = specColor;
	o.oneMinusReflectivity = oneMinusReflectivity;
	o.oneMinusRoughness = oneMinusRoughness;
	return o;
} 

inline FragmentCommonData FragmentSetup (float4 i_tex, half3 i_eyeVec, half3 i_normalWorld, half3 i_viewDirForParallax, half3x3 i_tanToWorld, half3 i_posWorld)
{
	i_tex = Parallax(i_tex, i_viewDirForParallax);

	half alpha = Alpha(i_tex.xy);
	half dissolve = 0;
	if(_DissolveMode == 1)
	{
		dissolve = tex2D(_DissolveMask, i_tex.xy).r;
		clip(dissolve - _Dissolve);
	}
	else if( _DissolveMode == 2)
	{
		dissolve = 1 - tex2D(_DissolveMask, i_tex.xy).r;
		clip(dissolve - (1-_Dissolve));
	}
	#if defined(_ALPHATEST_ON)
		clip (alpha - _Cutoff);
	#endif

	#if defined(_FEATHER_ON)
		clip ((1-alpha) - _Cutoff);
		clip ((alpha) - 0.1);
	#endif

	#ifdef _NORMALMAP
		half3 normalWorld = NormalizePerPixelNormal(mul(NormalInTangentSpace(i_tex), i_tanToWorld)); // @TODO: see if we can squeeze this normalize on SM2.0 as well
	#else
		// Should get compiled out, isn't being used in the end.
	 	half3 normalWorld = i_normalWorld;
	#endif

	half3 eyeVec = i_eyeVec;
	eyeVec = NormalizePerPixelNormal(eyeVec);


	FragmentCommonData o = UNITY_SETUP_BRDF_INPUT (i_tex);
	o.normalWorld = normalWorld;
	o.eyeVec = eyeVec;
	o.posWorld = i_posWorld;

	if( _DissolveMode == 2)
	{
		half3 border = 0;
		if (((dissolve - _Dissolve) < _BorderSize))
		{
			o.alpha = 1;
			o.diffColor += (abs((dissolve - _Dissolve)) / _BorderSize) * o.diffColor.rgb;
		}
	}

	#ifdef _ADD_DIRECTIONAL_LAYER
        // Snow mask
        half sm = dot(o.normalWorld, _LayerDirection);
        sm = pow(0.5 * sm + 0.5, 2.0);
        
		half inverseStrength = 1.0f - _LayerStrength;
        if (sm >= inverseStrength)
        {
			half4 layerDiffuse = tex2D(_LayerTex, i_tex.xy * _LayerScale);
			
			half layerStrength = saturate((sm - inverseStrength) / _LayerSmooth);

			o.diffColor = o.diffColor * (1.0 - layerStrength) + layerDiffuse * layerStrength;
			o.normalWorld = normalize(o.normalWorld + NormalizePerPixelNormal(mul(UnpackNormal(tex2D(_LayerBump, i_tex.xy * _LayerScale)), i_tanToWorld)) * _LayerBumpStrength * layerStrength);
        }
	#endif


	// NOTE: shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
	o.diffColor = PreMultiplyAlpha (o.diffColor, alpha, o.oneMinusReflectivity, /*out*/ o.alpha);

	#if _COLORADJUSTMENT
		o.diffColor = ContrastSaturationBrightness(o.diffColor, _BrightnessAmount, _SaturationAmount, _ContrastAmount);
	#endif

	return o;
}

inline UnityGI FragmentGI (
	float3 posWorld, 
	half occlusion, half4 i_ambientOrLightmapUV, half atten, half oneMinusRoughness, half3 normalWorld, half3 eyeVec,
	UnityLight light)
{
	UnityGIInput d;
	d.light = light;
	d.worldPos = posWorld;
	d.worldViewDir = -eyeVec;
	d.atten = atten;
	#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
		d.ambient = 0;
		d.lightmapUV = i_ambientOrLightmapUV;
	#else
		d.ambient = i_ambientOrLightmapUV.rgb;
		d.lightmapUV = 0;
	#endif
	d.boxMax[0] = unity_SpecCube0_BoxMax;
	d.boxMin[0] = unity_SpecCube0_BoxMin;
	d.probePosition[0] = unity_SpecCube0_ProbePosition;
	d.probeHDR[0] = unity_SpecCube0_HDR;

	d.boxMax[1] = unity_SpecCube1_BoxMax;
	d.boxMin[1] = unity_SpecCube1_BoxMin;
	d.probePosition[1] = unity_SpecCube1_ProbePosition;
	d.probeHDR[1] = unity_SpecCube1_HDR;

	return UnityGlobalIllumination (
		d, occlusion, oneMinusRoughness, normalWorld);
}

//-------------------------------------------------------------------------------------
half4 OutputForward (half4 output, half alphaFromSurface)
{
	#if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
		output.a = alphaFromSurface;
	#else
		UNITY_OPAQUE_ALPHA(output.a);
	#endif
	return output;
}

// ------------------------------------------------------------------
//  Base forward pass (directional light, emission, lightmaps, ...)

struct VertexOutputForwardBase
{
	float4 pos							: SV_POSITION;
	float4 tex							: TEXCOORD0;
	half3 eyeVec 						: TEXCOORD1;
	half4 tangentToWorldAndParallax[3]	: TEXCOORD2;	// [3x3:tangentToWorld | 1x3:viewDirForParallax]
	half4 ambientOrLightmapUV			: TEXCOORD5;	// SH or Lightmap UV
	SHADOW_COORDS(6)
	UNITY_FOG_COORDS(7)

	// next ones would not fit into SM2.0 limits, but they are always for SM3.0+
	#if UNITY_SPECCUBE_BOX_PROJECTION
		float3 posWorld					: TEXCOORD8;
	#endif
	#if _VERTEXCOLOR
		fixed4 color : TEXCOORD9;
	#endif
};

VertexOutputForwardBase vertForwardBase (VertexInput v)
{
	VertexOutputForwardBase o;
	UNITY_INITIALIZE_OUTPUT(VertexOutputForwardBase, o);

	float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
	#if UNITY_SPECCUBE_BOX_PROJECTION
		o.posWorld = posWorld.xyz;
	#endif
	o.pos = UnityObjectToClipPos(v.vertex);
	o.tex = TexCoords(v);
	o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
	float3 normalWorld = UnityObjectToWorldNormal(v.normal);
	#ifdef _TANGENT_TO_WORLD
		float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

		float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
		o.tangentToWorldAndParallax[0].xyz = tangentToWorld[0];
		o.tangentToWorldAndParallax[1].xyz = tangentToWorld[1];
		o.tangentToWorldAndParallax[2].xyz = tangentToWorld[2];
	#else
		o.tangentToWorldAndParallax[0].xyz = 0;
		o.tangentToWorldAndParallax[1].xyz = 0;
		o.tangentToWorldAndParallax[2].xyz = normalWorld;
	#endif
	//We need this for shadow receving
	TRANSFER_SHADOW(o);

	// Static lightmaps
	#ifndef LIGHTMAP_OFF
		o.ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
		o.ambientOrLightmapUV.zw = 0;
	// Sample light probe for Dynamic objects only (no static or dynamic lightmaps)
	#elif UNITY_SHOULD_SAMPLE_SH
		#if UNITY_SAMPLE_FULL_SH_PER_PIXEL
			o.ambientOrLightmapUV.rgb = 0;
		#elif (SHADER_TARGET < 30)
			o.ambientOrLightmapUV.rgb = ShadeSH9(half4(normalWorld, 1.0));
		#else
			// Optimization: L2 per-vertex, L0..L1 per-pixel
			o.ambientOrLightmapUV.rgb = ShadeSH3Order(half4(normalWorld, 1.0));
		#endif
		// Add approximated illumination from non-important point lights
		#ifdef VERTEXLIGHT_ON
			o.ambientOrLightmapUV.rgb += Shade4PointLights (
				unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
				unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
				unity_4LightAtten0, posWorld, normalWorld);
		#endif
	#endif

	#ifdef DYNAMICLIGHTMAP_ON
		o.ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
	#endif
	
	#ifdef _PARALLAXMAP
		TANGENT_SPACE_ROTATION;
		half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
		o.tangentToWorldAndParallax[0].w = viewDirForParallax.x;
		o.tangentToWorldAndParallax[1].w = viewDirForParallax.y;
		o.tangentToWorldAndParallax[2].w = viewDirForParallax.z;
	#endif
	
	UNITY_TRANSFER_FOG(o,o.pos);

	#if _VERTEXCOLOR
		o.color = (v.color);
	#endif

	return o;
}

half4 fragForwardBase (VertexOutputForwardBase i) : SV_Target
{
	FRAGMENT_SETUP(s)
	UnityLight mainLight = MainLight (s.normalWorld);
	half atten = SHADOW_ATTENUATION(i);
	
	half occlusion = Occlusion(i.tex.xy);
	#if _VERTEXCOLOR
		s.diffColor *= i.color;
		s.specColor *= i.color;
		occlusion *= Luminance(i.color);
		s.alpha *= i.color.a;
	#endif
	UnityGI gi = FragmentGI (
		s.posWorld, occlusion, i.ambientOrLightmapUV, atten, s.oneMinusRoughness, s.normalWorld, s.eyeVec, mainLight);

	half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
	c.rgb += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, occlusion, gi);
	c.rgb += Emission(i.tex.xy);

	UNITY_APPLY_FOG(i.fogCoord, c.rgb);
	return OutputForward (c, s.alpha);
}

// ------------------------------------------------------------------
//  Additive forward pass (one light per pass)
struct VertexOutputForwardAdd
{
	float4 pos							: SV_POSITION;
	float4 tex							: TEXCOORD0;
	half3 eyeVec 						: TEXCOORD1;
	half4 tangentToWorldAndLightDir[3]	: TEXCOORD2;	// [3x3:tangentToWorld | 1x3:lightDir]
	LIGHTING_COORDS(5,6)
	UNITY_FOG_COORDS(7)

	// next ones would not fit into SM2.0 limits, but they are always for SM3.0+
	#if defined(_PARALLAXMAP)
	half3 viewDirForParallax			: TEXCOORD8;
	#endif
};

VertexOutputForwardAdd vertForwardAdd (VertexInput v)
{
	VertexOutputForwardAdd o;
	UNITY_INITIALIZE_OUTPUT(VertexOutputForwardAdd, o);

	float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
	o.pos = UnityObjectToClipPos(v.vertex);
	o.tex = TexCoords(v);
	o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
	float3 normalWorld = UnityObjectToWorldNormal(v.normal);
	#ifdef _TANGENT_TO_WORLD
		float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

		float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
		o.tangentToWorldAndLightDir[0].xyz = tangentToWorld[0];
		o.tangentToWorldAndLightDir[1].xyz = tangentToWorld[1];
		o.tangentToWorldAndLightDir[2].xyz = tangentToWorld[2];
	#else
		o.tangentToWorldAndLightDir[0].xyz = 0;
		o.tangentToWorldAndLightDir[1].xyz = 0;
		o.tangentToWorldAndLightDir[2].xyz = normalWorld;
	#endif
	//We need this for shadow receving
	TRANSFER_VERTEX_TO_FRAGMENT(o);

	float3 lightDir = _WorldSpaceLightPos0.xyz - posWorld.xyz * _WorldSpaceLightPos0.w;
	#ifndef USING_DIRECTIONAL_LIGHT
		lightDir = NormalizePerVertexNormal(lightDir);
	#endif
	o.tangentToWorldAndLightDir[0].w = lightDir.x;
	o.tangentToWorldAndLightDir[1].w = lightDir.y;
	o.tangentToWorldAndLightDir[2].w = lightDir.z;

	#ifdef _PARALLAXMAP
		TANGENT_SPACE_ROTATION;
		o.viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
	#endif
	
	UNITY_TRANSFER_FOG(o,o.pos);
	return o;
}

half4 fragForwardAdd (VertexOutputForwardAdd i) : SV_Target
{
	FRAGMENT_SETUP_FWDADD(s)

	UnityLight light = AdditiveLight (s.normalWorld, IN_LIGHTDIR_FWDADD(i), LIGHT_ATTENUATION(i));
	UnityIndirect noIndirect = ZeroIndirect ();

	half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, light, noIndirect);
	
	UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0,0,0,0)); // fog towards black in additive pass
	return OutputForward (c, s.alpha);
}

// ------------------------------------------------------------------
//  Deferred pass

struct VertexOutputDeferred
{
	float4 pos							: SV_POSITION;
	float4 tex							: TEXCOORD0;
	half3 eyeVec 						: TEXCOORD1;
	half4 tangentToWorldAndParallax[3]	: TEXCOORD2;	// [3x3:tangentToWorld | 1x3:viewDirForParallax]
	half4 ambientOrLightmapUV			: TEXCOORD5;	// SH or Lightmap UVs			
	#if UNITY_SPECCUBE_BOX_PROJECTION
		float3 posWorld						: TEXCOORD6;
	#endif
	#if _VERTEXCOLOR
		fixed4 color : TEXCOORD7;
	#endif
	#if _ANISO
		float2 tex2 : TEXCOORD8;
	#endif
};


VertexOutputDeferred vertDeferred (VertexInput v)
{
	VertexOutputDeferred o;
	UNITY_INITIALIZE_OUTPUT(VertexOutputDeferred, o);

	float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
	#if UNITY_SPECCUBE_BOX_PROJECTION
		o.posWorld = posWorld;
	#endif
	o.pos = UnityObjectToClipPos(v.vertex);
	o.tex = TexCoords(v);

	#if _ANISO
		o.tex2.xy = TRANSFORM_TEX(v.uv3, _AnisoSpecMap); // Always source from uv0
	#endif
	o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
	float3 normalWorld = UnityObjectToWorldNormal(v.normal);
	#ifdef _TANGENT_TO_WORLD
		float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

		float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
		o.tangentToWorldAndParallax[0].xyz = tangentToWorld[0];
		o.tangentToWorldAndParallax[1].xyz = tangentToWorld[1];
		o.tangentToWorldAndParallax[2].xyz = tangentToWorld[2];
	#else
		o.tangentToWorldAndParallax[0].xyz = 0;
		o.tangentToWorldAndParallax[1].xyz = 0;
		o.tangentToWorldAndParallax[2].xyz = normalWorld;
	#endif

	#ifndef LIGHTMAP_OFF
		o.ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
		o.ambientOrLightmapUV.zw = 0;
	#elif UNITY_SHOULD_SAMPLE_SH
		#if (SHADER_TARGET < 30)
			o.ambientOrLightmapUV.rgb = ShadeSH9(half4(normalWorld, 1.0));
		#else
			// Optimization: L2 per-vertex, L0..L1 per-pixel
			o.ambientOrLightmapUV.rgb = ShadeSH3Order(half4(normalWorld, 1.0));
		#endif
	#endif
	
	#ifdef DYNAMICLIGHTMAP_ON
		o.ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
	#endif
	
	#ifdef _PARALLAXMAP
		TANGENT_SPACE_ROTATION;
		half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
		o.tangentToWorldAndParallax[0].w = viewDirForParallax.x;
		o.tangentToWorldAndParallax[1].w = viewDirForParallax.y;
		o.tangentToWorldAndParallax[2].w = viewDirForParallax.z;
	#endif
  
	#if _VERTEXCOLOR
		o.color = (v.color);
	#endif

	return o;
}

void fragDeferred (
	VertexOutputDeferred i,
	out half4 outDiffuse : SV_Target0,			// RT0: diffuse color (rgb), occlusion (a)
	out half4 outSpecSmoothness : SV_Target1,	// RT1: spec color (rgb), smoothness (a)
	out half4 outNormal : SV_Target2,			// RT2: normal (rgb), --unused, very low precision-- (a) 
	out half4 outEmission : SV_Target3			// RT3: emission (rgb), --unused-- (a)
)
{
	#if (SHADER_TARGET < 30)
		outDiffuse = 1;
		outSpecSmoothness = 1;
		outNormal = 0;
		outEmission = 0;
		return;
	#endif

	FRAGMENT_SETUP(s)

	// no analytic lights in this pass
	UnityLight dummyLight = DummyLight (s.normalWorld);
	half atten = 1;

	// only GI
	half occlusion = Occlusion(i.tex.xy);
	#if _VERTEXCOLOR
		s.diffColor *= i.color;
		s.specColor *= i.color;
		occlusion *= Luminance(i.color);
	#endif
	UnityGI gi = FragmentGI (
		s.posWorld, occlusion, i.ambientOrLightmapUV, atten, s.oneMinusRoughness, s.normalWorld, s.eyeVec, dummyLight);

	half3 color = 0;

	#if _ANISO
		//	Hairlighting
		fixed3 specGloss = tex2D(_AnisoSpecMap, i.tex2.xy).rgb;
		fixed3 h = normalize(normalize(gi.light.dir) + normalize(-s.eyeVec.xyz));
		fixed HdotA = saturate(dot(s.normalWorld, h));
		float aniso = saturate((sin(radians((HdotA + _AnisoOffset) * 180)) + 1) * 0.5);
		float spec = aniso * specGloss.r;
				
		s.oneMinusRoughness = lerp(s.oneMinusRoughness, spec, specGloss.b);
		s.specColor = lerp(s.specColor, _AnisoSpecColor, specGloss.b);

	#endif

	half3 pbsColor = UNITY_BRDF_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect).rgb;
	half3 giColor = UNITY_BRDF_GI(s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, occlusion, gi);
	color += pbsColor + giColor;
	
	#if _SSS
		half3 depthMap = tex2D(_DepthMap, i.tex.xy).rgb;
		
		float2 brdfUV;
		float NdotV = dot(s.normalWorld, -s.eyeVec);
		float diffuseTerm = gi.light.ndotl * 0.5 + 0.5;
		float3 scatter = tex2D(_BRDF, float2(diffuseTerm, NdotV)).rgb;

		color += scatter * diffuseTerm * _SSSScale * pbsColor * occlusion * depthMap.r;

		half depth = 1 - depthMap.b;

		depth = saturate((depth + _TranslucencyOffset));

		float scale = depth / _TranslucencyRadius;
		float d = scale * depth;
		half translucencyStrength = (1 - depth) * _TranslucencyPower;


		// Could use a lookup map for this, but it's actually faster to compute on my GTX460
		// half3 translucencyProfile = tex2D(_LookupTranslucency, half2(d, 0)).rgb;

		float dd = -d * d;
		half3 translucencyProfile =
			float3(0.233, 0.455, 0.649) * exp(dd / 0.0064) +
			float3(0.1, 0.336, 0.344) * exp(dd / 0.0484) +
			float3(0.118, 0.198, 0.0)   * exp(dd / 0.187) +
			float3(0.113, 0.007, 0.007) * exp(dd / 0.567) +
			float3(0.358, 0.004, 0.0)   * exp(dd / 1.99) +
			float3(0.078, 0.0, 0.0)   * exp(dd / 7.41);

		color += translucencyStrength * translucencyProfile * saturate((1 - gi.light.ndotl)*(1- dot(gi.light.dir,-s.eyeVec)) * _TranslucencyRadius) * pbsColor;
	#endif

	#ifdef _EMISSION
		color += Emission (i.tex.xy);
	#endif

	#ifndef UNITY_HDR_ON
		color.rgb = exp2(-color.rgb);
	#endif

	if(_GlowPower>0)
	{
		half rim = 1.0 - saturate(dot (-s.eyeVec, s.normalWorld));
		half glowPower = pow(rim, _GlowPower) * lerp(abs(sin(_Time.w * _GlowSpeed)), 1, _GlowStatic);
		half3 glowColor = _GlowColor.rgb * glowPower;
		color += glowColor;

		s.diffColor = (s.diffColor * (1 - glowPower));
	}

	outDiffuse = half4(s.diffColor, occlusion);
	outSpecSmoothness = half4(s.specColor, s.oneMinusRoughness);
	outNormal = half4(s.normalWorld*0.5+0.5,1);
	outEmission = half4(color, 1);
}					
			
#endif // UNITY_STANDARD_CORE_INCLUDED
