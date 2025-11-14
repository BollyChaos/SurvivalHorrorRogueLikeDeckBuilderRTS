// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "challenge/Foil"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin]_ColorBrackground("Color Brackground", Color) = (0,0,0,0)
		_Colorgradientsurface1("Color gradient surface 1", Color) = (1,0,0.06209469,0)
		_Colorgradientsurface2("Color gradient surface 2", Color) = (0,0.06287718,1,0)
		_Colorborde("Color borde", Color) = (0.08627451,0.08627451,0.08627451,1)
		_Parallax("Parallax", Float) = -1.17
		_Parallaxrainbow("Parallax rainbow", Float) = -1.78
		_stepShiny("step Shiny", Float) = 0.03
		_shinyscale("shiny scale", Float) = 32.9
		_TilingandoffsetShiny("Tiling and offset Shiny", Vector) = (0.1,0.76,-1.055,0)
		_TilingandOffsetrainbow("Tiling and Offset rainbow", Vector) = (0.22,0.99,0.33,0)
		_tilingandoffsetglare("tiling and offset glare", Vector) = (1,1,0,0)
		_Powerrainbowmask("Power rainbow mask", Float) = 9
		_Emission("Emission", Float) = 40
		_Bordermultiplier("Border multiplier", Float) = 7.4
		_Angleglare("Angle glare", Float) = -2.18
		_frequencyglare("frequency glare", Float) = 0.39
		_glareopacity("glare opacity", Float) = 0.025
		_distortion("distortion", Float) = 1.8
		_Scalesurface("Scale surface", Float) = 2.95
		_surface("surface", Float) = 0.14
		_powersurface("power surface", Float) = 3.16
		_speedsurface("speed surface", Vector) = (0.57,1.44,0.8,0.49)
		_Border("Border", Vector) = (0.98,0.985,0,0)
		_smoothnessborder("smoothness border", Float) = 0.68
		_TilingandOffsetWindow("Tiling and Offset Window", Vector) = (0.885,0.01,0,-0.2)
		_TilingandOffsetTexto("Tiling and Offset Texto", Vector) = (0.75,0.25,0,0.22)
		_Pentagono1("Pentagono 1", Vector) = (5.32,9.43,-4.17,-0.14)
		_Pentagono2("Pentagono 2", Vector) = (5.32,9.43,-0.17,-0.14)
		_Widthtexto("Width texto", Vector) = (0.017,0.01,0,0)
		_Patternintextarea("Pattern in text area", Range( 0 , 1)) = 0.894
		_ColorrectanguloTexto("Color rectangulo Texto", Color) = (0,0.1137255,0.2627451,1)
		_smoothnesstextarea("smoothness text area", Range( 0 , 1)) = 0
		_starsintextarea("stars in text area", Range( 0 , 1)) = 0.98
		_tilingandoffsetrectangulotitulo("tiling and offset rectangulo titulo", Vector) = (1,1,0,-0.34)
		[ASEEnd]_speedGlare("speed Glare", Float) = 3

		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry+1" }
		Cull Back
		AlphaToMask Off
		HLSLINCLUDE
		#pragma target 2.0

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS

		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			Stencil
			{
				Ref 5
				Comp NotEqual
				Pass Keep
				Fail Keep
				ZFail Keep
			}

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#define _RECEIVE_SHADOWS_OFF 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999
			#define ASE_USING_SAMPLING_MACROS 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorBrackground;
			float4 _tilingandoffsetglare;
			float4 _TilingandOffsetrainbow;
			float4 _Colorgradientsurface2;
			float4 _Colorgradientsurface1;
			float4 _ColorrectanguloTexto;
			float4 _speedsurface;
			float4 _tilingandoffsetrectangulotitulo;
			float4 _TilingandoffsetShiny;
			float4 _TilingandOffsetTexto;
			float4 _Pentagono2;
			float4 _Pentagono1;
			float4 _TilingandOffsetWindow;
			float4 _Border;
			float4 _Colorborde;
			float2 _Widthtexto;
			float _Bordermultiplier;
			float _Parallax;
			float _Parallaxrainbow;
			float _starsintextarea;
			float _stepShiny;
			float _shinyscale;
			float _Powerrainbowmask;
			float _Emission;
			float _Patternintextarea;
			float _Angleglare;
			float _speedGlare;
			float _frequencyglare;
			float _smoothnesstextarea;
			float _surface;
			float _powersurface;
			float _Scalesurface;
			float _distortion;
			float _glareopacity;
			float _smoothnessborder;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			

			float2 UnityGradientNoiseDir( float2 p )
			{
				p = fmod(p , 289);
				float x = fmod((34 * p.x + 1) * p.x , 289) + p.y;
				x = fmod( (34 * x + 1) * x , 289);
				x = frac( x / 41 ) * 2 - 1;
				return normalize( float2(x - floor(x + 0.5 ), abs( x ) - 0.5 ) );
			}
			
			float UnityGradientNoise( float2 UV, float Scale )
			{
				float2 p = UV * Scale;
				float2 ip = floor( p );
				float2 fp = frac( p );
				float d00 = dot( UnityGradientNoiseDir( ip ), fp );
				float d01 = dot( UnityGradientNoiseDir( ip + float2( 0, 1 ) ), fp - float2( 0, 1 ) );
				float d10 = dot( UnityGradientNoiseDir( ip + float2( 1, 0 ) ), fp - float2( 1, 0 ) );
				float d11 = dot( UnityGradientNoiseDir( ip + float2( 1, 1 ) ), fp - float2( 1, 1 ) );
				fp = fp * fp * fp * ( fp * ( fp * 6 - 15 ) + 10 );
				return lerp( lerp( d00, d01, fp.y ), lerp( d10, d11, fp.y ), fp.x ) + 0.5;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
					float2 voronoihash51( float2 p )
					{
						p = p - 151 * floor( p / 151 );
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi51( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash51( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord7.xy = v.texcoord.xy;
				o.ase_texcoord7.zw = v.texcoord1.xyzw.xy;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag ( VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 appendResult308 = (float2(_Border.z , _Border.w));
				float2 texCoord307 = IN.ase_texcoord7.xy * float2( 1,1 ) + appendResult308;
				float temp_output_2_0_g45 = _Border.x;
				float temp_output_3_0_g45 = _Border.y;
				float2 appendResult21_g45 = (float2(temp_output_2_0_g45 , temp_output_3_0_g45));
				float Radius25_g45 = max( min( min( abs( ( 0.02 * 2 ) ) , abs( temp_output_2_0_g45 ) ) , abs( temp_output_3_0_g45 ) ) , 1E-05 );
				float2 temp_cast_0 = (0.0).xx;
				float temp_output_30_0_g45 = ( length( max( ( ( abs( (texCoord307*2.0 + -1.0) ) - appendResult21_g45 ) + Radius25_g45 ) , temp_cast_0 ) ) / Radius25_g45 );
				float OuterEdge432 = saturate( ( ( 1.0 - temp_output_30_0_g45 ) / fwidth( temp_output_30_0_g45 ) ) );
				float2 appendResult291 = (float2(_TilingandOffsetWindow.z , _TilingandOffsetWindow.w));
				float2 texCoord302 = IN.ase_texcoord7.xy * float2( 1,1 ) + appendResult291;
				float temp_output_2_0_g50 = _TilingandOffsetWindow.x;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float temp_output_3_0_g50 = ( _TilingandOffsetWindow.x / ase_objectScale.z );
				float2 appendResult21_g50 = (float2(temp_output_2_0_g50 , temp_output_3_0_g50));
				float Radius25_g50 = max( min( min( abs( ( _TilingandOffsetWindow.y * 2 ) ) , abs( temp_output_2_0_g50 ) ) , abs( temp_output_3_0_g50 ) ) , 1E-05 );
				float2 temp_cast_1 = (0.0).xx;
				float temp_output_30_0_g50 = ( length( max( ( ( abs( (texCoord302*2.0 + -1.0) ) - appendResult21_g50 ) + Radius25_g50 ) , temp_cast_1 ) ) / Radius25_g50 );
				float WindowBorder435 = saturate( ( ( 1.0 - temp_output_30_0_g50 ) / fwidth( temp_output_30_0_g50 ) ) );
				float2 appendResult351 = (float2(_Pentagono1.x , _Pentagono1.y));
				float2 appendResult353 = (float2(_Pentagono1.z , _Pentagono1.w));
				float2 texCoord344 = IN.ase_texcoord7.xy * appendResult351 + appendResult353;
				float2 _Vector2 = float2(1,0.2);
				float temp_output_2_0_g49 = 5.0;
				float cosSides12_g49 = cos( ( PI / temp_output_2_0_g49 ) );
				float2 appendResult18_g49 = (float2(( _Vector2.x * cosSides12_g49 ) , ( _Vector2.x * cosSides12_g49 )));
				float2 break23_g49 = ( (texCoord344*2.0 + -1.0) / appendResult18_g49 );
				float polarCoords30_g49 = atan2( break23_g49.x , -break23_g49.y );
				float temp_output_52_0_g49 = ( TWO_PI / temp_output_2_0_g49 );
				float2 appendResult25_g49 = (float2(break23_g49.x , -break23_g49.y));
				float2 finalUVs29_g49 = appendResult25_g49;
				float temp_output_44_0_g49 = ( cos( ( ( floor( ( 0.5 + ( polarCoords30_g49 / temp_output_52_0_g49 ) ) ) * temp_output_52_0_g49 ) - polarCoords30_g49 ) ) * length( finalUVs29_g49 ) );
				float2 appendResult359 = (float2(_Pentagono2.x , _Pentagono2.y));
				float2 appendResult364 = (float2(_Pentagono2.z , _Pentagono2.w));
				float2 texCoord361 = IN.ase_texcoord7.xy * appendResult359 + appendResult364;
				float2 _Vector3 = float2(1,0.2);
				float temp_output_2_0_g47 = 5.0;
				float cosSides12_g47 = cos( ( PI / temp_output_2_0_g47 ) );
				float2 appendResult18_g47 = (float2(( _Vector3.x * cosSides12_g47 ) , ( _Vector3.x * cosSides12_g47 )));
				float2 break23_g47 = ( (texCoord361*2.0 + -1.0) / appendResult18_g47 );
				float polarCoords30_g47 = atan2( break23_g47.x , -break23_g47.y );
				float temp_output_52_0_g47 = ( TWO_PI / temp_output_2_0_g47 );
				float2 appendResult25_g47 = (float2(break23_g47.x , -break23_g47.y));
				float2 finalUVs29_g47 = appendResult25_g47;
				float temp_output_44_0_g47 = ( cos( ( ( floor( ( 0.5 + ( polarCoords30_g47 / temp_output_52_0_g47 ) ) ) * temp_output_52_0_g47 ) - polarCoords30_g47 ) ) * length( finalUVs29_g47 ) );
				float Pentagons437 = ( saturate( ( ( 1.0 - temp_output_44_0_g49 ) / fwidth( temp_output_44_0_g49 ) ) ) + saturate( ( ( 1.0 - temp_output_44_0_g47 ) / fwidth( temp_output_44_0_g47 ) ) ) );
				float2 appendResult365 = (float2(_TilingandOffsetTexto.z , _TilingandOffsetTexto.w));
				float2 texCoord367 = IN.ase_texcoord7.xy * float2( 1,1 ) + appendResult365;
				float temp_output_2_0_g48 = _TilingandOffsetTexto.x;
				float temp_output_3_0_g48 = _TilingandOffsetTexto.y;
				float2 appendResult21_g48 = (float2(temp_output_2_0_g48 , temp_output_3_0_g48));
				float Radius25_g48 = max( min( min( abs( ( 0.015 * 2 ) ) , abs( temp_output_2_0_g48 ) ) , abs( temp_output_3_0_g48 ) ) , 1E-05 );
				float2 temp_cast_2 = (0.0).xx;
				float temp_output_30_0_g48 = ( length( max( ( ( abs( (texCoord367*2.0 + -1.0) ) - appendResult21_g48 ) + Radius25_g48 ) , temp_cast_2 ) ) / Radius25_g48 );
				float temp_output_2_0_g34 = ( _TilingandOffsetTexto.x - _Widthtexto.x );
				float temp_output_3_0_g34 = ( _TilingandOffsetTexto.y - _Widthtexto.y );
				float2 appendResult21_g34 = (float2(temp_output_2_0_g34 , temp_output_3_0_g34));
				float Radius25_g34 = max( min( min( abs( ( 0.01 * 2 ) ) , abs( temp_output_2_0_g34 ) ) , abs( temp_output_3_0_g34 ) ) , 1E-05 );
				float2 temp_cast_3 = (0.0).xx;
				float temp_output_30_0_g34 = ( length( max( ( ( abs( (texCoord367*2.0 + -1.0) ) - appendResult21_g34 ) + Radius25_g34 ) , temp_cast_3 ) ) / Radius25_g34 );
				float Textrectangle381 = saturate( ( ( 1.0 - temp_output_30_0_g34 ) / fwidth( temp_output_30_0_g34 ) ) );
				float2 appendResult413 = (float2(_tilingandoffsetrectangulotitulo.z , _tilingandoffsetrectangulotitulo.w));
				float2 texCoord414 = IN.ase_texcoord7.xy * float2( 1,1 ) + appendResult413;
				float2 appendResult10_g35 = (float2(_tilingandoffsetrectangulotitulo.x , _tilingandoffsetrectangulotitulo.y));
				float2 temp_output_11_0_g35 = ( abs( (texCoord414*2.0 + -1.0) ) - appendResult10_g35 );
				float2 break16_g35 = ( 1.0 - ( temp_output_11_0_g35 / fwidth( temp_output_11_0_g35 ) ) );
				float biggertextRectangle440 = ( saturate( ( ( 1.0 - temp_output_30_0_g48 ) / fwidth( temp_output_30_0_g48 ) ) ) - saturate( ( Textrectangle381 - saturate( min( break16_g35.x , break16_g35.y ) ) ) ) );
				float Borders442 = ( ( 1.0 - OuterEdge432 ) + WindowBorder435 + Pentagons437 + biggertextRectangle440 );
				float2 texCoord193 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0.5,0.5 );
				float gradientNoise192 = UnityGradientNoise(texCoord193,9.07);
				float3 tanToWorld0 = float3( WorldTangent.x, WorldBiTangent.x, WorldNormal.x );
				float3 tanToWorld1 = float3( WorldTangent.y, WorldBiTangent.y, WorldNormal.y );
				float3 tanToWorld2 = float3( WorldTangent.z, WorldBiTangent.z, WorldNormal.z );
				float3 ase_tanViewDir =  tanToWorld0 * WorldViewDirection.x + tanToWorld1 * WorldViewDirection.y  + tanToWorld2 * WorldViewDirection.z;
				ase_tanViewDir = normalize(ase_tanViewDir);
				float2 texCoord212 = IN.ase_texcoord7.xy * float2( 1,1 ) + ( gradientNoise192 * ( ( ase_tanViewDir * _speedsurface.x ) + _speedsurface.y ) * _distortion ).xy;
				float simplePerlin2D191 = snoise( texCoord212*_Scalesurface );
				float temp_output_198_0 = saturate( pow( saturate( simplePerlin2D191 ) , _powersurface ) );
				float patternsurfacegrey270 = temp_output_198_0;
				float4 color354 = IsGammaSpace() ? float4(0.2641509,0.2641509,0.2641509,0) : float4(0.05672633,0.05672633,0.05672633,0);
				
				float4 lerpResult241 = lerp( _Colorgradientsurface1 , _Colorgradientsurface2 , simplePerlin2D191);
				float4 surface200 = ( ( lerpResult241 * temp_output_198_0 ) * 2.0 );
				float4 color388 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
				float patternintextarea391 = _Patternintextarea;
				float4 lerpResult389 = lerp( ( surface200 * _surface ) , color388 , ( Textrectangle381 * patternintextarea391 ));
				float2 appendResult113 = (float2(_tilingandoffsetglare.x , _tilingandoffsetglare.y));
				float2 appendResult114 = (float2(_tilingandoffsetglare.z , _tilingandoffsetglare.w));
				float temp_output_1_0_g37 = ( ( _TimeParameters.x / 2.0 ) * _frequencyglare );
				float2 texCoord102 = IN.ase_texcoord7.zw * appendResult113 + ( appendResult114 + ( ( ( temp_output_1_0_g37 - floor( ( temp_output_1_0_g37 + 0.5 ) ) ) * 2 ) * 2.0 * _speedGlare ) );
				float cos203 = cos( _Angleglare );
				float sin203 = sin( _Angleglare );
				float2 rotator203 = mul( texCoord102 - float2( 0.5,0.5 ) , float2x2( cos203 , -sin203 , sin203 , cos203 )) + float2( 0.5,0.5 );
				float2 appendResult10_g51 = (float2(0.5 , 20.0));
				float2 temp_output_11_0_g51 = ( abs( (rotator203*2.0 + -1.0) ) - appendResult10_g51 );
				float2 break16_g51 = ( 1.0 - ( temp_output_11_0_g51 / fwidth( temp_output_11_0_g51 ) ) );
				float Glare167 = ( saturate( min( break16_g51.x , break16_g51.y ) ) * _glareopacity );
				float2 appendResult60 = (float2(_TilingandoffsetShiny.x , _TilingandoffsetShiny.y));
				float2 appendResult61 = (float2(_TilingandoffsetShiny.z , _TilingandoffsetShiny.w));
				float2 texCoord11 = IN.ase_texcoord7.xy * appendResult60 + ( float3( appendResult61 ,  0.0 ) + ( _Parallax * ase_tanViewDir ) ).xy;
				float temp_output_1_0_g36 = ( texCoord11.x * 0.5 );
				float StarsMask173 = pow( saturate( ( ( abs( ( ( temp_output_1_0_g36 - floor( ( temp_output_1_0_g36 + 0.5 ) ) ) * 2 ) ) * 2 ) - 1.0 ) ) , _Powerrainbowmask );
				float time51 = 5.36;
				float2 voronoiSmoothId0 = 0;
				float2 texCoord50 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float2 coords51 = texCoord50 * _shinyscale;
				float2 id51 = 0;
				float2 uv51 = 0;
				float fade51 = 0.5;
				float voroi51 = 0;
				float rest51 = 0;
				for( int it51 = 0; it51 <8; it51++ ){
				voroi51 += fade51 * voronoi51( coords51, time51, id51, uv51, 0,voronoiSmoothId0 );
				rest51 += fade51;
				coords51 *= 2;
				fade51 *= 0.5;
				}//Voronoi51
				voroi51 /= rest51;
				float simplePerlin2D63 = snoise( texCoord50*_shinyscale );
				simplePerlin2D63 = simplePerlin2D63*0.5 + 0.5;
				float Stars170 = ( step( voroi51 , _stepShiny ) + step( simplePerlin2D63 , _stepShiny ) );
				float lerpResult401 = lerp( Stars170 , 0.0 , ( Textrectangle381 * _starsintextarea ));
				Gradient gradient67 = NewGradient( 0, 2, 2, float4( 0.5633206, 0, 1, 0 ), float4( 1, 0.554893, 0, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float2 appendResult84 = (float2(_TilingandOffsetrainbow.x , _TilingandOffsetrainbow.y));
				float2 appendResult85 = (float2(_TilingandOffsetrainbow.z , _TilingandOffsetrainbow.w));
				float2 texCoord70 = IN.ase_texcoord7.xy * appendResult84 + ( float3( appendResult85 ,  0.0 ) + ( _Parallaxrainbow * ase_tanViewDir ) ).xy;
				float4 Rainbow176 = SampleGradient( gradient67, texCoord70.x );
				float4 temp_output_218_0 = ( lerpResult389 + ( Glare167 + ( ( ( ( StarsMask173 * lerpResult401 ) * Rainbow176 ) * _Emission ) * OuterEdge432 ) ) );
				float4 lerpResult261 = lerp( temp_output_218_0 , ( temp_output_218_0 * _Bordermultiplier ) , Borders442);
				
				float lerpResult392 = lerp( patternsurfacegrey270 , _smoothnesstextarea , ( Textrectangle381 * patternintextarea391 ));
				float temp_output_272_0 = ( 1.0 - lerpResult392 );
				float lerpResult274 = lerp( temp_output_272_0 , ( temp_output_272_0 * _smoothnessborder ) , Borders442);
				
				float3 Albedo = ( _ColorBrackground + ( _Colorborde * saturate( ( Borders442 - patternsurfacegrey270 ) ) ) + ( Pentagons437 * color354 ) + ( Textrectangle381 * _ColorrectanguloTexto ) ).rgb;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = lerpResult261.rgb;
				float3 Specular = 0.5;
				float Metallic = lerpResult274;
				float Smoothness = lerpResult274;
				float Occlusion = 1;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				
				inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.clipPos);
				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);

				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif

				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif

				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal,0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#define _RECEIVE_SHADOWS_OFF 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999
			#define ASE_USING_SAMPLING_MACROS 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorBrackground;
			float4 _tilingandoffsetglare;
			float4 _TilingandOffsetrainbow;
			float4 _Colorgradientsurface2;
			float4 _Colorgradientsurface1;
			float4 _ColorrectanguloTexto;
			float4 _speedsurface;
			float4 _tilingandoffsetrectangulotitulo;
			float4 _TilingandoffsetShiny;
			float4 _TilingandOffsetTexto;
			float4 _Pentagono2;
			float4 _Pentagono1;
			float4 _TilingandOffsetWindow;
			float4 _Border;
			float4 _Colorborde;
			float2 _Widthtexto;
			float _Bordermultiplier;
			float _Parallax;
			float _Parallaxrainbow;
			float _starsintextarea;
			float _stepShiny;
			float _shinyscale;
			float _Powerrainbowmask;
			float _Emission;
			float _Patternintextarea;
			float _Angleglare;
			float _speedGlare;
			float _frequencyglare;
			float _smoothnesstextarea;
			float _surface;
			float _powersurface;
			float _Scalesurface;
			float _distortion;
			float _glareopacity;
			float _smoothnessborder;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			

			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}
		
		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#define _RECEIVE_SHADOWS_OFF 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999
			#define ASE_USING_SAMPLING_MACROS 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorBrackground;
			float4 _tilingandoffsetglare;
			float4 _TilingandOffsetrainbow;
			float4 _Colorgradientsurface2;
			float4 _Colorgradientsurface1;
			float4 _ColorrectanguloTexto;
			float4 _speedsurface;
			float4 _tilingandoffsetrectangulotitulo;
			float4 _TilingandoffsetShiny;
			float4 _TilingandOffsetTexto;
			float4 _Pentagono2;
			float4 _Pentagono1;
			float4 _TilingandOffsetWindow;
			float4 _Border;
			float4 _Colorborde;
			float2 _Widthtexto;
			float _Bordermultiplier;
			float _Parallax;
			float _Parallaxrainbow;
			float _starsintextarea;
			float _stepShiny;
			float _shinyscale;
			float _Powerrainbowmask;
			float _Emission;
			float _Patternintextarea;
			float _Angleglare;
			float _speedGlare;
			float _frequencyglare;
			float _smoothnesstextarea;
			float _surface;
			float _powersurface;
			float _Scalesurface;
			float _distortion;
			float _glareopacity;
			float _smoothnessborder;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			

			float2 UnityGradientNoiseDir( float2 p )
			{
				p = fmod(p , 289);
				float x = fmod((34 * p.x + 1) * p.x , 289) + p.y;
				x = fmod( (34 * x + 1) * x , 289);
				x = frac( x / 41 ) * 2 - 1;
				return normalize( float2(x - floor(x + 0.5 ), abs( x ) - 0.5 ) );
			}
			
			float UnityGradientNoise( float2 UV, float Scale )
			{
				float2 p = UV * Scale;
				float2 ip = floor( p );
				float2 fp = frac( p );
				float d00 = dot( UnityGradientNoiseDir( ip ), fp );
				float d01 = dot( UnityGradientNoiseDir( ip + float2( 0, 1 ) ), fp - float2( 0, 1 ) );
				float d10 = dot( UnityGradientNoiseDir( ip + float2( 1, 0 ) ), fp - float2( 1, 0 ) );
				float d11 = dot( UnityGradientNoiseDir( ip + float2( 1, 1 ) ), fp - float2( 1, 1 ) );
				fp = fp * fp * fp * ( fp * ( fp * 6 - 15 ) + 10 );
				return lerp( lerp( d00, d01, fp.y ), lerp( d10, d11, fp.y ), fp.x ) + 0.5;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord3.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord4.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord5.xyz = ase_worldBitangent;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_tangent = v.ase_tangent;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 appendResult308 = (float2(_Border.z , _Border.w));
				float2 texCoord307 = IN.ase_texcoord2.xy * float2( 1,1 ) + appendResult308;
				float temp_output_2_0_g45 = _Border.x;
				float temp_output_3_0_g45 = _Border.y;
				float2 appendResult21_g45 = (float2(temp_output_2_0_g45 , temp_output_3_0_g45));
				float Radius25_g45 = max( min( min( abs( ( 0.02 * 2 ) ) , abs( temp_output_2_0_g45 ) ) , abs( temp_output_3_0_g45 ) ) , 1E-05 );
				float2 temp_cast_0 = (0.0).xx;
				float temp_output_30_0_g45 = ( length( max( ( ( abs( (texCoord307*2.0 + -1.0) ) - appendResult21_g45 ) + Radius25_g45 ) , temp_cast_0 ) ) / Radius25_g45 );
				float OuterEdge432 = saturate( ( ( 1.0 - temp_output_30_0_g45 ) / fwidth( temp_output_30_0_g45 ) ) );
				float2 appendResult291 = (float2(_TilingandOffsetWindow.z , _TilingandOffsetWindow.w));
				float2 texCoord302 = IN.ase_texcoord2.xy * float2( 1,1 ) + appendResult291;
				float temp_output_2_0_g50 = _TilingandOffsetWindow.x;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float temp_output_3_0_g50 = ( _TilingandOffsetWindow.x / ase_objectScale.z );
				float2 appendResult21_g50 = (float2(temp_output_2_0_g50 , temp_output_3_0_g50));
				float Radius25_g50 = max( min( min( abs( ( _TilingandOffsetWindow.y * 2 ) ) , abs( temp_output_2_0_g50 ) ) , abs( temp_output_3_0_g50 ) ) , 1E-05 );
				float2 temp_cast_1 = (0.0).xx;
				float temp_output_30_0_g50 = ( length( max( ( ( abs( (texCoord302*2.0 + -1.0) ) - appendResult21_g50 ) + Radius25_g50 ) , temp_cast_1 ) ) / Radius25_g50 );
				float WindowBorder435 = saturate( ( ( 1.0 - temp_output_30_0_g50 ) / fwidth( temp_output_30_0_g50 ) ) );
				float2 appendResult351 = (float2(_Pentagono1.x , _Pentagono1.y));
				float2 appendResult353 = (float2(_Pentagono1.z , _Pentagono1.w));
				float2 texCoord344 = IN.ase_texcoord2.xy * appendResult351 + appendResult353;
				float2 _Vector2 = float2(1,0.2);
				float temp_output_2_0_g49 = 5.0;
				float cosSides12_g49 = cos( ( PI / temp_output_2_0_g49 ) );
				float2 appendResult18_g49 = (float2(( _Vector2.x * cosSides12_g49 ) , ( _Vector2.x * cosSides12_g49 )));
				float2 break23_g49 = ( (texCoord344*2.0 + -1.0) / appendResult18_g49 );
				float polarCoords30_g49 = atan2( break23_g49.x , -break23_g49.y );
				float temp_output_52_0_g49 = ( TWO_PI / temp_output_2_0_g49 );
				float2 appendResult25_g49 = (float2(break23_g49.x , -break23_g49.y));
				float2 finalUVs29_g49 = appendResult25_g49;
				float temp_output_44_0_g49 = ( cos( ( ( floor( ( 0.5 + ( polarCoords30_g49 / temp_output_52_0_g49 ) ) ) * temp_output_52_0_g49 ) - polarCoords30_g49 ) ) * length( finalUVs29_g49 ) );
				float2 appendResult359 = (float2(_Pentagono2.x , _Pentagono2.y));
				float2 appendResult364 = (float2(_Pentagono2.z , _Pentagono2.w));
				float2 texCoord361 = IN.ase_texcoord2.xy * appendResult359 + appendResult364;
				float2 _Vector3 = float2(1,0.2);
				float temp_output_2_0_g47 = 5.0;
				float cosSides12_g47 = cos( ( PI / temp_output_2_0_g47 ) );
				float2 appendResult18_g47 = (float2(( _Vector3.x * cosSides12_g47 ) , ( _Vector3.x * cosSides12_g47 )));
				float2 break23_g47 = ( (texCoord361*2.0 + -1.0) / appendResult18_g47 );
				float polarCoords30_g47 = atan2( break23_g47.x , -break23_g47.y );
				float temp_output_52_0_g47 = ( TWO_PI / temp_output_2_0_g47 );
				float2 appendResult25_g47 = (float2(break23_g47.x , -break23_g47.y));
				float2 finalUVs29_g47 = appendResult25_g47;
				float temp_output_44_0_g47 = ( cos( ( ( floor( ( 0.5 + ( polarCoords30_g47 / temp_output_52_0_g47 ) ) ) * temp_output_52_0_g47 ) - polarCoords30_g47 ) ) * length( finalUVs29_g47 ) );
				float Pentagons437 = ( saturate( ( ( 1.0 - temp_output_44_0_g49 ) / fwidth( temp_output_44_0_g49 ) ) ) + saturate( ( ( 1.0 - temp_output_44_0_g47 ) / fwidth( temp_output_44_0_g47 ) ) ) );
				float2 appendResult365 = (float2(_TilingandOffsetTexto.z , _TilingandOffsetTexto.w));
				float2 texCoord367 = IN.ase_texcoord2.xy * float2( 1,1 ) + appendResult365;
				float temp_output_2_0_g48 = _TilingandOffsetTexto.x;
				float temp_output_3_0_g48 = _TilingandOffsetTexto.y;
				float2 appendResult21_g48 = (float2(temp_output_2_0_g48 , temp_output_3_0_g48));
				float Radius25_g48 = max( min( min( abs( ( 0.015 * 2 ) ) , abs( temp_output_2_0_g48 ) ) , abs( temp_output_3_0_g48 ) ) , 1E-05 );
				float2 temp_cast_2 = (0.0).xx;
				float temp_output_30_0_g48 = ( length( max( ( ( abs( (texCoord367*2.0 + -1.0) ) - appendResult21_g48 ) + Radius25_g48 ) , temp_cast_2 ) ) / Radius25_g48 );
				float temp_output_2_0_g34 = ( _TilingandOffsetTexto.x - _Widthtexto.x );
				float temp_output_3_0_g34 = ( _TilingandOffsetTexto.y - _Widthtexto.y );
				float2 appendResult21_g34 = (float2(temp_output_2_0_g34 , temp_output_3_0_g34));
				float Radius25_g34 = max( min( min( abs( ( 0.01 * 2 ) ) , abs( temp_output_2_0_g34 ) ) , abs( temp_output_3_0_g34 ) ) , 1E-05 );
				float2 temp_cast_3 = (0.0).xx;
				float temp_output_30_0_g34 = ( length( max( ( ( abs( (texCoord367*2.0 + -1.0) ) - appendResult21_g34 ) + Radius25_g34 ) , temp_cast_3 ) ) / Radius25_g34 );
				float Textrectangle381 = saturate( ( ( 1.0 - temp_output_30_0_g34 ) / fwidth( temp_output_30_0_g34 ) ) );
				float2 appendResult413 = (float2(_tilingandoffsetrectangulotitulo.z , _tilingandoffsetrectangulotitulo.w));
				float2 texCoord414 = IN.ase_texcoord2.xy * float2( 1,1 ) + appendResult413;
				float2 appendResult10_g35 = (float2(_tilingandoffsetrectangulotitulo.x , _tilingandoffsetrectangulotitulo.y));
				float2 temp_output_11_0_g35 = ( abs( (texCoord414*2.0 + -1.0) ) - appendResult10_g35 );
				float2 break16_g35 = ( 1.0 - ( temp_output_11_0_g35 / fwidth( temp_output_11_0_g35 ) ) );
				float biggertextRectangle440 = ( saturate( ( ( 1.0 - temp_output_30_0_g48 ) / fwidth( temp_output_30_0_g48 ) ) ) - saturate( ( Textrectangle381 - saturate( min( break16_g35.x , break16_g35.y ) ) ) ) );
				float Borders442 = ( ( 1.0 - OuterEdge432 ) + WindowBorder435 + Pentagons437 + biggertextRectangle440 );
				float2 texCoord193 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0.5,0.5 );
				float gradientNoise192 = UnityGradientNoise(texCoord193,9.07);
				float3 ase_worldTangent = IN.ase_texcoord3.xyz;
				float3 ase_worldNormal = IN.ase_texcoord4.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord5.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_tanViewDir =  tanToWorld0 * ase_worldViewDir.x + tanToWorld1 * ase_worldViewDir.y  + tanToWorld2 * ase_worldViewDir.z;
				ase_tanViewDir = normalize(ase_tanViewDir);
				float2 texCoord212 = IN.ase_texcoord2.xy * float2( 1,1 ) + ( gradientNoise192 * ( ( ase_tanViewDir * _speedsurface.x ) + _speedsurface.y ) * _distortion ).xy;
				float simplePerlin2D191 = snoise( texCoord212*_Scalesurface );
				float temp_output_198_0 = saturate( pow( saturate( simplePerlin2D191 ) , _powersurface ) );
				float patternsurfacegrey270 = temp_output_198_0;
				float4 color354 = IsGammaSpace() ? float4(0.2641509,0.2641509,0.2641509,0) : float4(0.05672633,0.05672633,0.05672633,0);
				
				
				float3 Albedo = ( _ColorBrackground + ( _Colorborde * saturate( ( Borders442 - patternsurfacegrey270 ) ) ) + ( Pentagons437 * color354 ) + ( Textrectangle381 * _ColorrectanguloTexto ) ).rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
            ZTest LEqual
            ZWrite On

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#define _RECEIVE_SHADOWS_OFF 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999
			#define ASE_USING_SAMPLING_MACROS 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float3 worldNormal : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorBrackground;
			float4 _tilingandoffsetglare;
			float4 _TilingandOffsetrainbow;
			float4 _Colorgradientsurface2;
			float4 _Colorgradientsurface1;
			float4 _ColorrectanguloTexto;
			float4 _speedsurface;
			float4 _tilingandoffsetrectangulotitulo;
			float4 _TilingandoffsetShiny;
			float4 _TilingandOffsetTexto;
			float4 _Pentagono2;
			float4 _Pentagono1;
			float4 _TilingandOffsetWindow;
			float4 _Border;
			float4 _Colorborde;
			float2 _Widthtexto;
			float _Bordermultiplier;
			float _Parallax;
			float _Parallaxrainbow;
			float _starsintextarea;
			float _stepShiny;
			float _shinyscale;
			float _Powerrainbowmask;
			float _Emission;
			float _Patternintextarea;
			float _Angleglare;
			float _speedGlare;
			float _frequencyglare;
			float _smoothnesstextarea;
			float _surface;
			float _powersurface;
			float _Scalesurface;
			float _distortion;
			float _glareopacity;
			float _smoothnessborder;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			

			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal( v.ase_normal );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.worldNormal = normalWS;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif
				
				return float4(PackNormalOctRectEncode(TransformWorldToViewDir(IN.worldNormal, true)), 0.0, 0.0);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer" }
			
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			Stencil
			{
				Ref 5
				Comp NotEqual
				Pass Keep
				Fail Keep
				ZFail Keep
			}

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#define _RECEIVE_SHADOWS_OFF 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999
			#define ASE_USING_SAMPLING_MACROS 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			#pragma multi_compile _ _GBUFFER_NORMALS_OCT
			
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_GBUFFER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _ColorBrackground;
			float4 _tilingandoffsetglare;
			float4 _TilingandOffsetrainbow;
			float4 _Colorgradientsurface2;
			float4 _Colorgradientsurface1;
			float4 _ColorrectanguloTexto;
			float4 _speedsurface;
			float4 _tilingandoffsetrectangulotitulo;
			float4 _TilingandoffsetShiny;
			float4 _TilingandOffsetTexto;
			float4 _Pentagono2;
			float4 _Pentagono1;
			float4 _TilingandOffsetWindow;
			float4 _Border;
			float4 _Colorborde;
			float2 _Widthtexto;
			float _Bordermultiplier;
			float _Parallax;
			float _Parallaxrainbow;
			float _starsintextarea;
			float _stepShiny;
			float _shinyscale;
			float _Powerrainbowmask;
			float _Emission;
			float _Patternintextarea;
			float _Angleglare;
			float _speedGlare;
			float _frequencyglare;
			float _smoothnesstextarea;
			float _surface;
			float _powersurface;
			float _Scalesurface;
			float _distortion;
			float _glareopacity;
			float _smoothnessborder;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			

			float2 UnityGradientNoiseDir( float2 p )
			{
				p = fmod(p , 289);
				float x = fmod((34 * p.x + 1) * p.x , 289) + p.y;
				x = fmod( (34 * x + 1) * x , 289);
				x = frac( x / 41 ) * 2 - 1;
				return normalize( float2(x - floor(x + 0.5 ), abs( x ) - 0.5 ) );
			}
			
			float UnityGradientNoise( float2 UV, float Scale )
			{
				float2 p = UV * Scale;
				float2 ip = floor( p );
				float2 fp = frac( p );
				float d00 = dot( UnityGradientNoiseDir( ip ), fp );
				float d01 = dot( UnityGradientNoiseDir( ip + float2( 0, 1 ) ), fp - float2( 0, 1 ) );
				float d10 = dot( UnityGradientNoiseDir( ip + float2( 1, 0 ) ), fp - float2( 1, 0 ) );
				float d11 = dot( UnityGradientNoiseDir( ip + float2( 1, 1 ) ), fp - float2( 1, 1 ) );
				fp = fp * fp * fp * ( fp * ( fp * 6 - 15 ) + 10 );
				return lerp( lerp( d00, d01, fp.y ), lerp( d10, d11, fp.y ), fp.x ) + 0.5;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
					float2 voronoihash51( float2 p )
					{
						p = p - 151 * floor( p / 151 );
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi51( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash51( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord7.xy = v.texcoord.xy;
				o.ase_texcoord7.zw = v.texcoord1.xyzw.xy;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			FragmentOutput frag ( VertexOutput IN 
								#ifdef ASE_DEPTH_WRITE_ON
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								 )
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 appendResult308 = (float2(_Border.z , _Border.w));
				float2 texCoord307 = IN.ase_texcoord7.xy * float2( 1,1 ) + appendResult308;
				float temp_output_2_0_g45 = _Border.x;
				float temp_output_3_0_g45 = _Border.y;
				float2 appendResult21_g45 = (float2(temp_output_2_0_g45 , temp_output_3_0_g45));
				float Radius25_g45 = max( min( min( abs( ( 0.02 * 2 ) ) , abs( temp_output_2_0_g45 ) ) , abs( temp_output_3_0_g45 ) ) , 1E-05 );
				float2 temp_cast_0 = (0.0).xx;
				float temp_output_30_0_g45 = ( length( max( ( ( abs( (texCoord307*2.0 + -1.0) ) - appendResult21_g45 ) + Radius25_g45 ) , temp_cast_0 ) ) / Radius25_g45 );
				float OuterEdge432 = saturate( ( ( 1.0 - temp_output_30_0_g45 ) / fwidth( temp_output_30_0_g45 ) ) );
				float2 appendResult291 = (float2(_TilingandOffsetWindow.z , _TilingandOffsetWindow.w));
				float2 texCoord302 = IN.ase_texcoord7.xy * float2( 1,1 ) + appendResult291;
				float temp_output_2_0_g50 = _TilingandOffsetWindow.x;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float temp_output_3_0_g50 = ( _TilingandOffsetWindow.x / ase_objectScale.z );
				float2 appendResult21_g50 = (float2(temp_output_2_0_g50 , temp_output_3_0_g50));
				float Radius25_g50 = max( min( min( abs( ( _TilingandOffsetWindow.y * 2 ) ) , abs( temp_output_2_0_g50 ) ) , abs( temp_output_3_0_g50 ) ) , 1E-05 );
				float2 temp_cast_1 = (0.0).xx;
				float temp_output_30_0_g50 = ( length( max( ( ( abs( (texCoord302*2.0 + -1.0) ) - appendResult21_g50 ) + Radius25_g50 ) , temp_cast_1 ) ) / Radius25_g50 );
				float WindowBorder435 = saturate( ( ( 1.0 - temp_output_30_0_g50 ) / fwidth( temp_output_30_0_g50 ) ) );
				float2 appendResult351 = (float2(_Pentagono1.x , _Pentagono1.y));
				float2 appendResult353 = (float2(_Pentagono1.z , _Pentagono1.w));
				float2 texCoord344 = IN.ase_texcoord7.xy * appendResult351 + appendResult353;
				float2 _Vector2 = float2(1,0.2);
				float temp_output_2_0_g49 = 5.0;
				float cosSides12_g49 = cos( ( PI / temp_output_2_0_g49 ) );
				float2 appendResult18_g49 = (float2(( _Vector2.x * cosSides12_g49 ) , ( _Vector2.x * cosSides12_g49 )));
				float2 break23_g49 = ( (texCoord344*2.0 + -1.0) / appendResult18_g49 );
				float polarCoords30_g49 = atan2( break23_g49.x , -break23_g49.y );
				float temp_output_52_0_g49 = ( TWO_PI / temp_output_2_0_g49 );
				float2 appendResult25_g49 = (float2(break23_g49.x , -break23_g49.y));
				float2 finalUVs29_g49 = appendResult25_g49;
				float temp_output_44_0_g49 = ( cos( ( ( floor( ( 0.5 + ( polarCoords30_g49 / temp_output_52_0_g49 ) ) ) * temp_output_52_0_g49 ) - polarCoords30_g49 ) ) * length( finalUVs29_g49 ) );
				float2 appendResult359 = (float2(_Pentagono2.x , _Pentagono2.y));
				float2 appendResult364 = (float2(_Pentagono2.z , _Pentagono2.w));
				float2 texCoord361 = IN.ase_texcoord7.xy * appendResult359 + appendResult364;
				float2 _Vector3 = float2(1,0.2);
				float temp_output_2_0_g47 = 5.0;
				float cosSides12_g47 = cos( ( PI / temp_output_2_0_g47 ) );
				float2 appendResult18_g47 = (float2(( _Vector3.x * cosSides12_g47 ) , ( _Vector3.x * cosSides12_g47 )));
				float2 break23_g47 = ( (texCoord361*2.0 + -1.0) / appendResult18_g47 );
				float polarCoords30_g47 = atan2( break23_g47.x , -break23_g47.y );
				float temp_output_52_0_g47 = ( TWO_PI / temp_output_2_0_g47 );
				float2 appendResult25_g47 = (float2(break23_g47.x , -break23_g47.y));
				float2 finalUVs29_g47 = appendResult25_g47;
				float temp_output_44_0_g47 = ( cos( ( ( floor( ( 0.5 + ( polarCoords30_g47 / temp_output_52_0_g47 ) ) ) * temp_output_52_0_g47 ) - polarCoords30_g47 ) ) * length( finalUVs29_g47 ) );
				float Pentagons437 = ( saturate( ( ( 1.0 - temp_output_44_0_g49 ) / fwidth( temp_output_44_0_g49 ) ) ) + saturate( ( ( 1.0 - temp_output_44_0_g47 ) / fwidth( temp_output_44_0_g47 ) ) ) );
				float2 appendResult365 = (float2(_TilingandOffsetTexto.z , _TilingandOffsetTexto.w));
				float2 texCoord367 = IN.ase_texcoord7.xy * float2( 1,1 ) + appendResult365;
				float temp_output_2_0_g48 = _TilingandOffsetTexto.x;
				float temp_output_3_0_g48 = _TilingandOffsetTexto.y;
				float2 appendResult21_g48 = (float2(temp_output_2_0_g48 , temp_output_3_0_g48));
				float Radius25_g48 = max( min( min( abs( ( 0.015 * 2 ) ) , abs( temp_output_2_0_g48 ) ) , abs( temp_output_3_0_g48 ) ) , 1E-05 );
				float2 temp_cast_2 = (0.0).xx;
				float temp_output_30_0_g48 = ( length( max( ( ( abs( (texCoord367*2.0 + -1.0) ) - appendResult21_g48 ) + Radius25_g48 ) , temp_cast_2 ) ) / Radius25_g48 );
				float temp_output_2_0_g34 = ( _TilingandOffsetTexto.x - _Widthtexto.x );
				float temp_output_3_0_g34 = ( _TilingandOffsetTexto.y - _Widthtexto.y );
				float2 appendResult21_g34 = (float2(temp_output_2_0_g34 , temp_output_3_0_g34));
				float Radius25_g34 = max( min( min( abs( ( 0.01 * 2 ) ) , abs( temp_output_2_0_g34 ) ) , abs( temp_output_3_0_g34 ) ) , 1E-05 );
				float2 temp_cast_3 = (0.0).xx;
				float temp_output_30_0_g34 = ( length( max( ( ( abs( (texCoord367*2.0 + -1.0) ) - appendResult21_g34 ) + Radius25_g34 ) , temp_cast_3 ) ) / Radius25_g34 );
				float Textrectangle381 = saturate( ( ( 1.0 - temp_output_30_0_g34 ) / fwidth( temp_output_30_0_g34 ) ) );
				float2 appendResult413 = (float2(_tilingandoffsetrectangulotitulo.z , _tilingandoffsetrectangulotitulo.w));
				float2 texCoord414 = IN.ase_texcoord7.xy * float2( 1,1 ) + appendResult413;
				float2 appendResult10_g35 = (float2(_tilingandoffsetrectangulotitulo.x , _tilingandoffsetrectangulotitulo.y));
				float2 temp_output_11_0_g35 = ( abs( (texCoord414*2.0 + -1.0) ) - appendResult10_g35 );
				float2 break16_g35 = ( 1.0 - ( temp_output_11_0_g35 / fwidth( temp_output_11_0_g35 ) ) );
				float biggertextRectangle440 = ( saturate( ( ( 1.0 - temp_output_30_0_g48 ) / fwidth( temp_output_30_0_g48 ) ) ) - saturate( ( Textrectangle381 - saturate( min( break16_g35.x , break16_g35.y ) ) ) ) );
				float Borders442 = ( ( 1.0 - OuterEdge432 ) + WindowBorder435 + Pentagons437 + biggertextRectangle440 );
				float2 texCoord193 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0.5,0.5 );
				float gradientNoise192 = UnityGradientNoise(texCoord193,9.07);
				float3 tanToWorld0 = float3( WorldTangent.x, WorldBiTangent.x, WorldNormal.x );
				float3 tanToWorld1 = float3( WorldTangent.y, WorldBiTangent.y, WorldNormal.y );
				float3 tanToWorld2 = float3( WorldTangent.z, WorldBiTangent.z, WorldNormal.z );
				float3 ase_tanViewDir =  tanToWorld0 * WorldViewDirection.x + tanToWorld1 * WorldViewDirection.y  + tanToWorld2 * WorldViewDirection.z;
				ase_tanViewDir = normalize(ase_tanViewDir);
				float2 texCoord212 = IN.ase_texcoord7.xy * float2( 1,1 ) + ( gradientNoise192 * ( ( ase_tanViewDir * _speedsurface.x ) + _speedsurface.y ) * _distortion ).xy;
				float simplePerlin2D191 = snoise( texCoord212*_Scalesurface );
				float temp_output_198_0 = saturate( pow( saturate( simplePerlin2D191 ) , _powersurface ) );
				float patternsurfacegrey270 = temp_output_198_0;
				float4 color354 = IsGammaSpace() ? float4(0.2641509,0.2641509,0.2641509,0) : float4(0.05672633,0.05672633,0.05672633,0);
				
				float4 lerpResult241 = lerp( _Colorgradientsurface1 , _Colorgradientsurface2 , simplePerlin2D191);
				float4 surface200 = ( ( lerpResult241 * temp_output_198_0 ) * 2.0 );
				float4 color388 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
				float patternintextarea391 = _Patternintextarea;
				float4 lerpResult389 = lerp( ( surface200 * _surface ) , color388 , ( Textrectangle381 * patternintextarea391 ));
				float2 appendResult113 = (float2(_tilingandoffsetglare.x , _tilingandoffsetglare.y));
				float2 appendResult114 = (float2(_tilingandoffsetglare.z , _tilingandoffsetglare.w));
				float temp_output_1_0_g37 = ( ( _TimeParameters.x / 2.0 ) * _frequencyglare );
				float2 texCoord102 = IN.ase_texcoord7.zw * appendResult113 + ( appendResult114 + ( ( ( temp_output_1_0_g37 - floor( ( temp_output_1_0_g37 + 0.5 ) ) ) * 2 ) * 2.0 * _speedGlare ) );
				float cos203 = cos( _Angleglare );
				float sin203 = sin( _Angleglare );
				float2 rotator203 = mul( texCoord102 - float2( 0.5,0.5 ) , float2x2( cos203 , -sin203 , sin203 , cos203 )) + float2( 0.5,0.5 );
				float2 appendResult10_g51 = (float2(0.5 , 20.0));
				float2 temp_output_11_0_g51 = ( abs( (rotator203*2.0 + -1.0) ) - appendResult10_g51 );
				float2 break16_g51 = ( 1.0 - ( temp_output_11_0_g51 / fwidth( temp_output_11_0_g51 ) ) );
				float Glare167 = ( saturate( min( break16_g51.x , break16_g51.y ) ) * _glareopacity );
				float2 appendResult60 = (float2(_TilingandoffsetShiny.x , _TilingandoffsetShiny.y));
				float2 appendResult61 = (float2(_TilingandoffsetShiny.z , _TilingandoffsetShiny.w));
				float2 texCoord11 = IN.ase_texcoord7.xy * appendResult60 + ( float3( appendResult61 ,  0.0 ) + ( _Parallax * ase_tanViewDir ) ).xy;
				float temp_output_1_0_g36 = ( texCoord11.x * 0.5 );
				float StarsMask173 = pow( saturate( ( ( abs( ( ( temp_output_1_0_g36 - floor( ( temp_output_1_0_g36 + 0.5 ) ) ) * 2 ) ) * 2 ) - 1.0 ) ) , _Powerrainbowmask );
				float time51 = 5.36;
				float2 voronoiSmoothId0 = 0;
				float2 texCoord50 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float2 coords51 = texCoord50 * _shinyscale;
				float2 id51 = 0;
				float2 uv51 = 0;
				float fade51 = 0.5;
				float voroi51 = 0;
				float rest51 = 0;
				for( int it51 = 0; it51 <8; it51++ ){
				voroi51 += fade51 * voronoi51( coords51, time51, id51, uv51, 0,voronoiSmoothId0 );
				rest51 += fade51;
				coords51 *= 2;
				fade51 *= 0.5;
				}//Voronoi51
				voroi51 /= rest51;
				float simplePerlin2D63 = snoise( texCoord50*_shinyscale );
				simplePerlin2D63 = simplePerlin2D63*0.5 + 0.5;
				float Stars170 = ( step( voroi51 , _stepShiny ) + step( simplePerlin2D63 , _stepShiny ) );
				float lerpResult401 = lerp( Stars170 , 0.0 , ( Textrectangle381 * _starsintextarea ));
				Gradient gradient67 = NewGradient( 0, 2, 2, float4( 0.5633206, 0, 1, 0 ), float4( 1, 0.554893, 0, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float2 appendResult84 = (float2(_TilingandOffsetrainbow.x , _TilingandOffsetrainbow.y));
				float2 appendResult85 = (float2(_TilingandOffsetrainbow.z , _TilingandOffsetrainbow.w));
				float2 texCoord70 = IN.ase_texcoord7.xy * appendResult84 + ( float3( appendResult85 ,  0.0 ) + ( _Parallaxrainbow * ase_tanViewDir ) ).xy;
				float4 Rainbow176 = SampleGradient( gradient67, texCoord70.x );
				float4 temp_output_218_0 = ( lerpResult389 + ( Glare167 + ( ( ( ( StarsMask173 * lerpResult401 ) * Rainbow176 ) * _Emission ) * OuterEdge432 ) ) );
				float4 lerpResult261 = lerp( temp_output_218_0 , ( temp_output_218_0 * _Bordermultiplier ) , Borders442);
				
				float lerpResult392 = lerp( patternsurfacegrey270 , _smoothnesstextarea , ( Textrectangle381 * patternintextarea391 ));
				float temp_output_272_0 = ( 1.0 - lerpResult392 );
				float lerpResult274 = lerp( temp_output_272_0 , ( temp_output_272_0 * _smoothnessborder ) , Borders442);
				
				float3 Albedo = ( _ColorBrackground + ( _Colorborde * saturate( ( Borders442 - patternsurfacegrey270 ) ) ) + ( Pentagons437 * color354 ) + ( Textrectangle381 * _ColorrectanguloTexto ) ).rgb;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = lerpResult261.rgb;
				float3 Specular = 0.5;
				float Metallic = lerpResult274;
				float Smoothness = lerpResult274;
				float Occlusion = 1;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				BRDFData brdfData;
				InitializeBRDFData( Albedo, Metallic, Specular, Smoothness, Alpha, brdfData);
				half4 color;
				color.rgb = GlobalIllumination( brdfData, inputData.bakedGI, Occlusion, inputData.normalWS, inputData.viewDirectionWS);
				color.a = Alpha;

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;
				
					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;
				
					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );
				
							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif
				
				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;
				
					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
				
					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;
				
					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );
				
							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif
				
				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal, 0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif
				
				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif
				
				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif
				
				return BRDFDataToGbuffer(brdfData, inputData, Smoothness, Emission + color.rgb);
			}

			ENDHLSL
		}
		
	}
	/*ase_lod*/
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18910
230.4;73.6;1006;557.4;2682.871;786.9854;3.567529;True;False
Node;AmplifyShaderEditor.CommentaryNode;283;-4295.586,1295.752;Inherit;False;2901.491;1052.984;Sur;25;212;228;240;229;239;195;194;192;193;231;191;199;222;220;221;198;270;241;219;209;210;200;282;242;243;Surface Pattern;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;175;-3253.802,-24.93937;Inherit;False;1799.599;540.4216;;15;173;94;95;133;79;41;11;427;60;59;14;61;12;58;13;Stars Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;240;-4006.509,1562.204;Inherit;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;231;-4153.586,1413.775;Inherit;False;Constant;_Vector1;Vector 1;20;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;13;-3173.46,246.5667;Inherit;False;Property;_Parallax;Parallax;4;0;Create;True;0;0;0;False;0;False;-1.17;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;58;-3203.802,47.67158;Inherit;False;Property;_TilingandoffsetShiny;Tiling and offset Shiny;8;0;Create;True;0;0;0;False;0;False;0.1,0.76,-1.055,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;12;-3156.941,346.4824;Inherit;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector4Node;228;-4012.498,1710.864;Inherit;False;Property;_speedsurface;speed surface;21;0;Create;True;0;0;0;False;0;False;0.57,1.44,0.8,0.49;1,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;407;-1158.94,1737.089;Inherit;False;2304.935;940.5542;Comment;19;379;378;370;414;413;412;411;377;416;415;381;368;372;375;371;367;373;365;440;Text rectangle;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;61;-2923.405,121.6217;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;229;-3791.82,1626.056;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-2929.444,249.2246;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;370;-1060.84,1855.988;Inherit;False;Property;_TilingandOffsetTexto;Tiling and Offset Texto;25;0;Create;True;0;0;0;False;0;False;0.75,0.25,0,0.22;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;193;-3950.614,1366.814;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;60,60;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;169;-3652.447,-795.4771;Inherit;False;2173.808;629.6317;G;20;142;422;421;149;423;419;418;148;144;112;113;114;102;105;106;167;159;160;417;203;Glare;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;172;-2877.143,2449.875;Inherit;False;1502.91;727.6853;;9;50;55;51;63;53;52;65;66;170;Stars;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;195;-3580.812,1859.442;Inherit;False;Property;_distortion;distortion;17;0;Create;True;0;0;0;False;0;False;1.8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;239;-3638.768,1737.189;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;142;-3576.464,-505.7429;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;59;-2761.68,143.0252;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;422;-3562.065,-269.7479;Inherit;False;Constant;_Float2;Float 2;34;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;378;-814.629,2200.595;Inherit;False;Property;_Widthtexto;Width texto;28;0;Create;True;0;0;0;False;0;False;0.017,0.01;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector4Node;412;-832.3992,2399.44;Inherit;False;Property;_tilingandoffsetrectangulotitulo;tiling and offset rectangulo titulo;33;0;Create;True;0;0;0;True;0;False;1,1,0,-0.34;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;192;-3712.928,1364.752;Inherit;True;Gradient;False;True;2;0;FLOAT2;0,0;False;1;FLOAT;9.07;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;365;-649.9871,1959.555;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;60;-2932.465,25.0605;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;178;-3039.767,647.8474;Inherit;False;1600.244;574.7356;;13;70;67;68;7;0;83;85;84;86;73;72;71;176;Rainbow;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;194;-3409.233,1708.048;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;50;-2827.143,2673.305;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;408;-721.3057,887.7615;Inherit;False;1848.717;764.686;Comment;16;358;357;363;330;362;343;341;361;360;344;359;351;353;364;352;437;Pentagons;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;427;-2550.759,342.876;Inherit;False;Constant;_Float6;Float 6;35;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-2503.338,2692.251;Inherit;False;Property;_shinyscale;shiny scale;7;0;Create;True;0;0;0;False;0;False;32.9;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;421;-3372.065,-506.7478;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-2627.426,44.43671;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;375;-395.8936,2316.088;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;410;-252.6743,-102.0776;Inherit;False;1329.008;348.8181;Comment;11;306;311;307;308;256;432;6;2;4;3;5;Outer edge;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;367;-458.3894,1951.307;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;371;-343.7748,2139.831;Inherit;False;Constant;_Float13;Float 13;30;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;413;-525.2356,2523.765;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;373;-405.9414,2215.609;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;149;-3393.777,-399.236;Inherit;False;Property;_frequencyglare;frequency glare;15;0;Create;True;0;0;0;False;0;False;0.39;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-3187.778,-518.2359;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-2391.738,69.31453;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;212;-3247.935,1664.69;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;63;-2367.414,2879.56;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;199;-3181.276,1997.19;Inherit;False;Property;_Scalesurface;Scale surface;18;0;Create;True;0;0;0;False;0;False;2.95;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;51;-2307.679,2499.875;Inherit;True;0;0;1;0;8;True;151;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;5.36;False;2;FLOAT;8.58;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;72;-2976.934,941.3595;Inherit;False;Property;_Parallaxrainbow;Parallax rainbow;5;0;Create;True;0;0;0;False;0;False;-1.78;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;83;-2989.767,758.2733;Inherit;False;Property;_TilingandOffsetrainbow;Tiling and Offset rainbow;9;0;Create;True;0;0;0;False;0;False;0.22,0.99,0.33,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;53;-2267.88,2753.491;Inherit;False;Property;_stepShiny;step Shiny;6;0;Create;True;0;0;0;False;0;False;0.03;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;409;-469.1179,395.0901;Inherit;False;1559.068;456.0012;Comment;7;435;305;304;302;291;296;289;window border;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;256;-202.6743,-31.95261;Inherit;False;Property;_Border;Border;22;0;Create;True;0;0;0;False;0;False;0.98,0.985,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;71;-2964.43,1034.582;Inherit;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;414;-363.5057,2536.352;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;372;-94.45935,2176.741;Inherit;True;Rounded Rectangle;-1;;34;8679f72f5be758f47babb3ba1d5f51d3;0;4;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;352;-671.3057,937.7615;Inherit;False;Property;_Pentagono1;Pentagono 1;26;0;Create;True;0;0;0;False;0;False;5.32,9.43,-4.17,-0.14;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;358;-653.0618,1244.946;Inherit;False;Property;_Pentagono2;Pentagono 2;27;0;Create;True;0;0;0;False;0;False;5.32,9.43,-0.17,-0.14;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;85;-2709.267,840.0732;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;418;-2993.784,-519.5755;Inherit;True;Sawtooth Wave;-1;;37;289adb816c3ac6d489f255fc3caf5016;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;191;-2993.463,1701.573;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;3.88;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;79;-2253.137,70.24446;Inherit;True;Triangle Wave;-1;;36;51ec3c8d117f3ec4fa3742c3e00d535b;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;351;-262.1631,937.7615;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;423;-2951.782,-268.7765;Inherit;False;Property;_speedGlare;speed Glare;34;0;Create;True;0;0;0;False;0;False;3;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;112;-2970.683,-722.612;Inherit;False;Property;_tilingandoffsetglare;tiling and offset glare;10;0;Create;True;0;0;0;False;0;False;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;65;-2048.21,2861.549;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;52;-2075.473,2589.468;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;411;-107.0413,2404.942;Inherit;True;Rectangle;-1;;35;6b23e0c975270fb4084c354b2c83366a;0;3;1;FLOAT2;0,0;False;2;FLOAT;0.5;False;3;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;381;175.5757,2163.822;Inherit;False;Textrectangle;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;353;-389.3893,1078.642;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;289;-419.1179,457.2708;Inherit;False;Property;_TilingandOffsetWindow;Tiling and Offset Window;24;0;Create;True;0;0;0;False;0;False;0.885,0.01,0,-0.2;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;359;-302.6647,1274.222;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;364;-372.4553,1404.165;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;308;66.19489,-37.88962;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-2675.701,965.7954;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;84;-2712.367,751.1722;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;114;-2721.982,-630.0864;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;447;2298.145,454.09;Inherit;False;2036.963;1069.614;;30;402;406;405;171;404;174;401;177;54;69;96;97;433;266;168;201;218;383;382;206;208;391;388;207;385;389;273;444;124;261;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-2526.745,853.2003;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;222;-2724.818,1692.604;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;221;-2740.182,1790.036;Inherit;False;Property;_powersurface;power surface;20;0;Create;True;0;0;0;False;0;False;3.16;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-2137.101,289.9457;Inherit;False;Property;_Powerrainbowmask;Power rainbow mask;11;0;Create;True;0;0;0;False;0;False;9;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;419;-2772.065,-383.7479;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;243;-2923.213,2136.738;Inherit;False;Property;_Colorgradientsurface2;Color gradient surface 2;2;0;Create;True;0;0;0;False;0;False;0,0.06287718,1,0;0,0.06287718,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;242;-2937.913,1957.04;Inherit;False;Property;_Colorgradientsurface1;Color gradient surface 1;1;0;Create;True;0;0;0;False;0;False;1,0,0.06209469,0;1,0,0.06209469,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-1784.413,2802.56;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;133;-2062.524,69.82977;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;361;-120.4219,1275.193;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;344;-137.356,949.6707;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;360;113.6671,1482.765;Inherit;False;Constant;_Vector3;Vector 3;27;0;Create;True;0;0;0;False;0;False;1,0.2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;343;96.73297,1157.245;Inherit;False;Constant;_Vector2;Vector 2;27;0;Create;True;0;0;0;False;0;False;1,0.2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ObjectScaleNode;296;-382.6082,637.0911;Inherit;False;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;415;391.6824,2152.159;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;362;101.8194,1392.617;Inherit;False;Constant;_Float12;Float 12;27;0;Create;True;0;0;0;False;0;False;5;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;379;-346.9291,2065.694;Inherit;False;Constant;_Float15;Float 15;30;0;Create;True;0;0;0;False;0;False;0.015;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;311;387.7815,132.9615;Inherit;False;Constant;_Float7;Float 7;27;0;Create;True;0;0;0;False;0;False;0.02;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;307;281.9404,-52.07761;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;291;24.23517,629.7346;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;341;84.88531,1067.094;Inherit;False;Constant;_Float11;Float 11;27;0;Create;True;0;0;0;False;0;False;5;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;363;463.5203,1278.372;Inherit;True;Polygon;-1;;47;6906ef7087298c94c853d6753e182169;0;4;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;402;2414.293,1230.575;Inherit;False;381;Textrectangle;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;170;-1667.231,2801.175;Inherit;False;Stars;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;330;446.5857,952.8494;Inherit;True;Polygon;-1;;49;6906ef7087298c94c853d6753e182169;0;4;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;368;-150.2722,1788.246;Inherit;True;Rounded Rectangle;-1;;48;8679f72f5be758f47babb3ba1d5f51d3;0;4;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;302;215.8334,621.4875;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;304;-13.18182,445.0901;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;144;-2599.156,-539.1753;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;416;542.8933,2150.155;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;306;606.9492,18.20853;Inherit;False;Rounded Rectangle;-1;;45;8679f72f5be758f47babb3ba1d5f51d3;0;4;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;406;2348.144,1310.699;Inherit;False;Property;_starsintextarea;stars in text area;32;0;Create;True;0;0;0;False;0;False;0.98;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;220;-2544.1,1686.79;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;241;-2325.276,2048.513;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;113;-2728.914,-729.855;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;70;-2369.996,781.8253;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;94;-1915.03,62.36274;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;6.56;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;67;-2258.687,696.2753;Inherit;False;0;2;2;0.5633206,0,1,0;1,0.554893,0,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.Vector2Node;106;-2445.124,-494.3167;Inherit;False;Constant;_Vector0;Vector 0;11;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;173;-1657.423,59.02667;Inherit;False;StarsMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;282;-2150.294,1999.454;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-2416.659,-331.6587;Inherit;False;Property;_Angleglare;Angle glare;14;0;Create;True;0;0;0;False;0;False;-2.18;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;102;-2465.835,-619.0872;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;377;668.2609,1809.3;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;357;750.987,949.6238;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;305;534.9507,451.4261;Inherit;True;Rounded Rectangle;-1;;50;8679f72f5be758f47babb3ba1d5f51d3;0;4;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;432;851.4499,27.52786;Inherit;False;OuterEdge;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;404;2611.894,1263.075;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;405;2590.194,1151.976;Inherit;False;Constant;_Float16;Float 16;33;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;171;2468.551,1042.001;Inherit;False;170;Stars;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;449;1325.083,925.7932;Inherit;False;805.6643;418.9053;Comment;7;434;441;438;436;250;303;442;Borders;1,1,1,1;0;0
Node;AmplifyShaderEditor.GradientSampleNode;68;-2011.469,696.8474;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;198;-2304.739,1685.395;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;401;2759.194,1144.976;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;174;2731.586,1013.3;Inherit;False;173;StarsMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;-2097.312,1696.447;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;210;-2034.058,1943.227;Inherit;False;Constant;_Float1;Float 1;18;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;440;824.8044,1798.909;Inherit;False;biggertextRectangle;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;434;1375.083,975.7932;Inherit;False;432;OuterEdge;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;203;-2207.876,-606.8602;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;435;822.8696,454.5755;Inherit;False;WindowBorder;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;176;-1680.042,695.1593;Inherit;False;Rainbow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;437;889.4611,952.6355;Inherit;False;Pentagons;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;383;3003.335,955.9764;Inherit;False;Property;_Patternintextarea;Pattern in text area;29;0;Create;True;0;0;0;False;0;False;0.894;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;436;1421.605,1063.49;Inherit;False;435;WindowBorder;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;2963.271,1127.734;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;177;2854.361,1294.48;Inherit;False;176;Rainbow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;438;1434.606,1137.33;Inherit;False;437;Pentagons;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-1975.453,-439.7085;Inherit;False;Property;_glareopacity;glare opacity;16;0;Create;True;0;0;0;False;0;False;0.025;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;441;1443.99,1228.698;Inherit;False;440;biggertextRectangle;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;417;-2007.661,-605.3209;Inherit;False;Rectangle;-1;;51;6b23e0c975270fb4084c354b2c83366a;0;3;1;FLOAT2;0,0;False;2;FLOAT;0.5;False;3;FLOAT;20;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;209;-1870.351,1698.325;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;250;1559.972,983.0583;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;448;3208.944,1595.199;Inherit;False;1109.253;548.0061;;11;276;275;272;392;397;271;395;394;393;274;445;Metallic / Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;3080.732,1188.946;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;96;3079.663,1352.282;Inherit;False;Property;_Emission;Emission;12;0;Create;True;0;0;0;False;0;False;40;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;391;3296.991,952.7416;Inherit;False;patternintextarea;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;159;-1811.388,-609.7449;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;303;1737.266,1029.314;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;200;-1733.796,1697.705;Inherit;False;surface;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;394;3258.944,1890.657;Inherit;False;391;patternintextarea;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;393;3262.144,1802.757;Inherit;False;381;Textrectangle;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;446;3130.781,-486.8011;Inherit;False;1177.363;852.7185;;14;280;277;278;265;56;279;263;443;439;354;355;398;399;400;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;167;-1675.516,-607.8997;Inherit;False;Glare;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;270;-2135.861,1607.456;Inherit;False;patternsurfacegrey;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;433;3253.461,1393.375;Inherit;False;432;OuterEdge;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;442;1906.748,1056.089;Inherit;False;Borders;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;206;3224.062,504.09;Inherit;False;200;surface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;208;3251.46,588.9334;Inherit;False;Property;_surface;surface;19;0;Create;True;0;0;0;False;0;False;0.14;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;382;3315.967,866.9898;Inherit;False;381;Textrectangle;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;3244.225,1199.695;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;385;3505.29,874.9725;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;266;3419.038,1198.898;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;207;3487.079,537.6185;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;280;3180.781,-161.9913;Inherit;False;270;patternsurfacegrey;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;168;3425.077,1094.4;Inherit;False;167;Glare;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;443;3232.165,-234.3776;Inherit;False;442;Borders;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;271;3409.953,1645.199;Inherit;False;270;patternsurfacegrey;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;397;3334.918,1731.53;Inherit;False;Property;_smoothnesstextarea;smoothness text area;31;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;395;3481.465,1802.301;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;388;3390.797,660.6467;Inherit;False;Constant;_Color3;Color 3;32;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;389;3674.038,815.0894;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;201;3625.076,1174.047;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;392;3664.468,1768.011;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;277;3470.468,-189.4869;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;276;3780.401,1954.39;Inherit;False;Property;_smoothnessborder;smoothness border;23;0;Create;True;0;0;0;False;0;False;0.68;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;398;3772.261,78.11254;Inherit;False;381;Textrectangle;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;218;3833.358,1144.963;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;124;3798.26,1274.383;Inherit;False;Property;_Bordermultiplier;Border multiplier;13;0;Create;True;0;0;0;False;0;False;7.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;265;3497.08,-436.8011;Inherit;False;Property;_Colorborde;Color borde;3;0;Create;True;0;0;0;False;0;False;0.08627451,0.08627451,0.08627451,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;278;3643.14,-201.9261;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;399;3767.447,153.9178;Inherit;False;Property;_ColorrectanguloTexto;Color rectangulo Texto;30;0;Create;True;0;0;0;False;0;False;0,0.1137255,0.2627451,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;354;3412.013,60.16186;Inherit;False;Constant;_Color2;Color 2;28;0;Create;True;0;0;0;False;0;False;0.2641509,0.2641509,0.2641509,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;439;3453.384,-44.40839;Inherit;False;437;Pentagons;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;272;3824.883,1772.76;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;275;3979.503,1853.289;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;445;3941.908,2027.205;Inherit;False;442;Borders;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;400;4023.74,102.1786;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;444;3957.554,1407.704;Inherit;False;442;Borders;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;273;3983.04,1224.413;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;56;3782.435,-419.6609;Inherit;False;Property;_ColorBrackground;Color Brackground;0;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;355;3662.655,-43.40253;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;279;3836.557,-224.3871;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;261;4153.107,1151.974;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;263;4155.145,-313.5269;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;274;4136.197,1777.661;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;5;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;6;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthNormals;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;7;-2372.193,1018.466;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;True;5;False;-1;255;False;-1;255;False;-1;6;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalGBuffer;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;4478.445,1018.466;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;challenge/Foil;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;18;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=1;True;0;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;True;True;True;5;False;-1;255;False;-1;255;False;-1;6;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;38;Workflow;1;Surface;0;  Refraction Model;0;  Blend;0;Two Sided;1;Fragment Normal Space,InvertActionOnDeselection;0;Transmission;0;  Transmission Shadow;0.5,False,-1;Translucency;0;  Translucency Strength;1,False,-1;  Normal Distortion;0.5,False,-1;  Scattering;2,False,-1;  Direct;0.9,False,-1;  Ambient;0.1,False,-1;  Shadow;0.5,False,-1;Cast Shadows;0;  Use Shadow Threshold;0;Receive Shadows;0;GPU Instancing;0;LOD CrossFade;0;Built-in Fog;0;_FinalColorxAlpha;0;Meta Pass;0;Override Baked GI;0;Extra Pre Pass;0;DOTS Instancing;0;Tessellation;0;  Phong;0;  Strength;0.5,False,-1;  Type;0;  Tess;16,False,-1;  Min;10,False,-1;  Max;25,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Write Depth;0;  Early Z;0;Vertex Position,InvertActionOnDeselection;1;0;8;False;True;False;True;False;True;True;True;False;;True;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;-2372.193,1018.466;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;61;0;58;3
WireConnection;61;1;58;4
WireConnection;229;0;240;0
WireConnection;229;1;228;1
WireConnection;14;0;13;0
WireConnection;14;1;12;0
WireConnection;193;1;231;0
WireConnection;239;0;229;0
WireConnection;239;1;228;2
WireConnection;59;0;61;0
WireConnection;59;1;14;0
WireConnection;192;0;193;0
WireConnection;365;0;370;3
WireConnection;365;1;370;4
WireConnection;60;0;58;1
WireConnection;60;1;58;2
WireConnection;194;0;192;0
WireConnection;194;1;239;0
WireConnection;194;2;195;0
WireConnection;421;0;142;0
WireConnection;421;1;422;0
WireConnection;11;0;60;0
WireConnection;11;1;59;0
WireConnection;375;0;370;2
WireConnection;375;1;378;2
WireConnection;367;1;365;0
WireConnection;413;0;412;3
WireConnection;413;1;412;4
WireConnection;373;0;370;1
WireConnection;373;1;378;1
WireConnection;148;0;421;0
WireConnection;148;1;149;0
WireConnection;41;0;11;1
WireConnection;41;1;427;0
WireConnection;212;1;194;0
WireConnection;63;0;50;0
WireConnection;63;1;55;0
WireConnection;51;0;50;0
WireConnection;51;2;55;0
WireConnection;414;1;413;0
WireConnection;372;1;367;0
WireConnection;372;2;373;0
WireConnection;372;3;375;0
WireConnection;372;4;371;0
WireConnection;85;0;83;3
WireConnection;85;1;83;4
WireConnection;418;1;148;0
WireConnection;191;0;212;0
WireConnection;191;1;199;0
WireConnection;79;1;41;0
WireConnection;351;0;352;1
WireConnection;351;1;352;2
WireConnection;65;0;63;0
WireConnection;65;1;53;0
WireConnection;52;0;51;0
WireConnection;52;1;53;0
WireConnection;411;1;414;0
WireConnection;411;2;412;1
WireConnection;411;3;412;2
WireConnection;381;0;372;0
WireConnection;353;0;352;3
WireConnection;353;1;352;4
WireConnection;359;0;358;1
WireConnection;359;1;358;2
WireConnection;364;0;358;3
WireConnection;364;1;358;4
WireConnection;308;0;256;3
WireConnection;308;1;256;4
WireConnection;73;0;72;0
WireConnection;73;1;71;0
WireConnection;84;0;83;1
WireConnection;84;1;83;2
WireConnection;114;0;112;3
WireConnection;114;1;112;4
WireConnection;86;0;85;0
WireConnection;86;1;73;0
WireConnection;222;0;191;0
WireConnection;419;0;418;0
WireConnection;419;1;422;0
WireConnection;419;2;423;0
WireConnection;66;0;52;0
WireConnection;66;1;65;0
WireConnection;133;0;79;0
WireConnection;361;0;359;0
WireConnection;361;1;364;0
WireConnection;344;0;351;0
WireConnection;344;1;353;0
WireConnection;415;0;381;0
WireConnection;415;1;411;0
WireConnection;307;1;308;0
WireConnection;291;0;289;3
WireConnection;291;1;289;4
WireConnection;363;1;361;0
WireConnection;363;2;362;0
WireConnection;363;3;360;1
WireConnection;363;4;360;1
WireConnection;170;0;66;0
WireConnection;330;1;344;0
WireConnection;330;2;341;0
WireConnection;330;3;343;1
WireConnection;330;4;343;1
WireConnection;368;1;367;0
WireConnection;368;2;370;1
WireConnection;368;3;370;2
WireConnection;368;4;379;0
WireConnection;302;1;291;0
WireConnection;304;0;289;1
WireConnection;304;1;296;3
WireConnection;144;0;114;0
WireConnection;144;1;419;0
WireConnection;416;0;415;0
WireConnection;306;1;307;0
WireConnection;306;2;256;1
WireConnection;306;3;256;2
WireConnection;306;4;311;0
WireConnection;220;0;222;0
WireConnection;220;1;221;0
WireConnection;241;0;242;0
WireConnection;241;1;243;0
WireConnection;241;2;191;0
WireConnection;113;0;112;1
WireConnection;113;1;112;2
WireConnection;70;0;84;0
WireConnection;70;1;86;0
WireConnection;94;0;133;0
WireConnection;94;1;95;0
WireConnection;173;0;94;0
WireConnection;282;0;241;0
WireConnection;102;0;113;0
WireConnection;102;1;144;0
WireConnection;377;0;368;0
WireConnection;377;1;416;0
WireConnection;357;0;330;0
WireConnection;357;1;363;0
WireConnection;305;1;302;0
WireConnection;305;2;289;1
WireConnection;305;3;304;0
WireConnection;305;4;289;2
WireConnection;432;0;306;0
WireConnection;404;0;402;0
WireConnection;404;1;406;0
WireConnection;68;0;67;0
WireConnection;68;1;70;1
WireConnection;198;0;220;0
WireConnection;401;0;171;0
WireConnection;401;1;405;0
WireConnection;401;2;404;0
WireConnection;219;0;282;0
WireConnection;219;1;198;0
WireConnection;440;0;377;0
WireConnection;203;0;102;0
WireConnection;203;1;106;0
WireConnection;203;2;105;0
WireConnection;435;0;305;0
WireConnection;176;0;68;0
WireConnection;437;0;357;0
WireConnection;54;0;174;0
WireConnection;54;1;401;0
WireConnection;417;1;203;0
WireConnection;209;0;219;0
WireConnection;209;1;210;0
WireConnection;250;0;434;0
WireConnection;69;0;54;0
WireConnection;69;1;177;0
WireConnection;391;0;383;0
WireConnection;159;0;417;0
WireConnection;159;1;160;0
WireConnection;303;0;250;0
WireConnection;303;1;436;0
WireConnection;303;2;438;0
WireConnection;303;3;441;0
WireConnection;200;0;209;0
WireConnection;167;0;159;0
WireConnection;270;0;198;0
WireConnection;442;0;303;0
WireConnection;97;0;69;0
WireConnection;97;1;96;0
WireConnection;385;0;382;0
WireConnection;385;1;391;0
WireConnection;266;0;97;0
WireConnection;266;1;433;0
WireConnection;207;0;206;0
WireConnection;207;1;208;0
WireConnection;395;0;393;0
WireConnection;395;1;394;0
WireConnection;389;0;207;0
WireConnection;389;1;388;0
WireConnection;389;2;385;0
WireConnection;201;0;168;0
WireConnection;201;1;266;0
WireConnection;392;0;271;0
WireConnection;392;1;397;0
WireConnection;392;2;395;0
WireConnection;277;0;443;0
WireConnection;277;1;280;0
WireConnection;218;0;389;0
WireConnection;218;1;201;0
WireConnection;278;0;277;0
WireConnection;272;0;392;0
WireConnection;275;0;272;0
WireConnection;275;1;276;0
WireConnection;400;0;398;0
WireConnection;400;1;399;0
WireConnection;273;0;218;0
WireConnection;273;1;124;0
WireConnection;355;0;439;0
WireConnection;355;1;354;0
WireConnection;279;0;265;0
WireConnection;279;1;278;0
WireConnection;261;0;218;0
WireConnection;261;1;273;0
WireConnection;261;2;444;0
WireConnection;263;0;56;0
WireConnection;263;1;279;0
WireConnection;263;2;355;0
WireConnection;263;3;400;0
WireConnection;274;0;272;0
WireConnection;274;1;275;0
WireConnection;274;2;445;0
WireConnection;1;0;263;0
WireConnection;1;2;261;0
WireConnection;1;3;274;0
WireConnection;1;4;274;0
ASEEND*/
//CHKSM=6325EBBBE5E25289943435E24107015BE203EA72