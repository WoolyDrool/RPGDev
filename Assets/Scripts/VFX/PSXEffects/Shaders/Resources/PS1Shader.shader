Shader "PSXEffects/PS1Shader"
{
	Properties
	{
		[Toggle] _Unlit("Unlit", Float) = 0.0
		[Toggle] _DrawDist("Affected by Polygonal Draw Distance", Float) = 1.0
		_VertexInaccuracy("Vertex Inaccuracy Override", Float) = -1.0
		_Color("Color", Color) = (1,1,1,1)
		[KeywordEnum(Vertex, Fragment)] _DiffModel("Diffuse Model", Float) = 0.0
		_MainTex("Texture", 2D) = "white" {}
		_LODTex("LOD Texture", 2D) = "white" {}
		_LODAmt("LOD Amount", Float) = 0.0
		_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalMapDepth("Normal Map Depth", Float) = 1
		[KeywordEnum(Gouraud, Phong)] _SpecModel("Specular Model", Float) = 0.0
		_SpecularMap("Specular Map", 2D) = "white" {}
		_Specular("Specular Amount", Float) = 0.0
		_MetalMap("Metal Map", 2D) = "white" {}
		_Metallic("Metallic Amount", Range(0.0,1.0)) = 0.0
		_Smoothness("Smoothness Amount", Range(0.0,1.0)) = 0.5
		_Emission("Emission Map", 2D) = "white" {}
		_EmissionAmt("Emission Amount", Float) = 0.0
		_Cube("Cubemap", Cube) = "" {}
		_CutoutThreshold("Cutout Threshold", Float) = 0.5

		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		[HideInInspector] _Cul("__cul", Float) = 0.0
		[HideInInspector] _BlendOp("__bld", Float) = 0.0

		[HideInInspector] _RenderMode("__rnd", Float) = 0.0
	}

	SubShader
	{
		Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }
		LOD 100
		Lighting On
		Offset[_Offset], 1
		Cull[_Cul]
		Blend[_SrcBlend][_DstBlend]
		BlendOp[_BlendOp]
		ZWrite[_ZWrite]

		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#include "UnityStandardUtils.cginc"
			#include "AutoLight.cginc"
			#include "PSXEffects.cginc"

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ VERTEXLIGHT_ON
			#pragma multi_compile OPAQUE TRANSPARENT CUTOUT
			#pragma multi_compile _ WORLD_SPACE_SNAPPING
			#pragma multi_compile _ AFFINE_MAPPING
			#pragma shader_feature_local _ UNLIT
			#pragma shader_feature_local SPEC_GOURAUD SPEC_PHONG
			#pragma shader_feature_local DIFF_VERTEX DIFF_FRAGMENT
			#pragma shader_feature SHADOW_DEFAULT SHADOW_PSX
			#pragma shader_feature_local BFC
			#pragma shader_feature_local DEPTH_WRITE

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 color : COLOR;
				float3 tangent: TANGENT;
				float4 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				fixed4 color : COLOR;
				fixed4 diff : COLOR1;
				fixed3 spec : COLOR2;
				float4 pos : SV_POSITION;
				float4 vertPos : TEXCOORD1;
				float3 normal : NORMAL;
				float3 normalDir : TEXCOORD2;
				float3 viewDir : TEXCOORD3;
				float3 lightDir : TEXCOORD4;

				float3 T : TEXCOORD5;
				float3 B : TEXCOORD6;
				float3 N : TEXCOORD7;
				LIGHTING_COORDS(8, 9)
				UNITY_FOG_COORDS(10)
				float4 uv1 : TEXCOORD11;
			};

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

				// Vertex inaccuracy block
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float3 viewDir = mul((float3x3)unity_CameraToWorld, float3(0, 0, 1));
				worldPos.xyz += _WorldSpaceCameraPos.xyz * _CamPos;
				worldPos.xyz += viewDir * 100 * _CamPos;
				o.pos = UnityObjectToClipPos(v.vertex);
				if (_VertexInaccuracy < 0) _VertexInaccuracy = _VertexSnappingDetail;
				#if defined(WORLD_SPACE_SNAPPING)
					_VertexInaccuracy /= 2048;
					worldPos.xyz /= _VertexInaccuracy;
					worldPos.xyz = round(worldPos.xyz);
					worldPos.xyz *= _VertexInaccuracy;
					worldPos.xyz -= _WorldSpaceCameraPos.xyz * _CamPos + viewDir * 100 * _CamPos;
					v.vertex = mul(unity_WorldToObject, worldPos);
					o.pos = UnityObjectToClipPos(v.vertex);
				#else
					worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.pos = PixelSnap(o.pos);
				#endif

				// Set UV outputs
				float wVal = mul(UNITY_MATRIX_P, o.pos).z;
				#if defined(AFFINE_MAPPING)
					o.uv = float4(v.texcoord.xy * wVal, wVal, 0);
				#else
					o.uv = float4(v.texcoord.xyz, 0);
				#endif

				// Currently no difference from non-affine mapping
				#if defined(LIGHTMAP_ON)
					#if defined(AFFINE_MAPPING)
						o.uv1 = float4(v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw, wVal, 0);
					#else
						o.uv1 = float4(v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw, 0, 0);
					#endif
				#endif

					

				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
					
				// Set cutoff value for vertex render distance
				o.diff.a = (_DrawDistance > 0 && distance(worldPos, _WorldSpaceCameraPos) > _DrawDistance);
				// Set value for LOD distance
				o.uv.a = (distance(worldPos, _WorldSpaceCameraPos) > _LODAmt && _LODAmt > 0);

				// Various outputs needed for fragment
				o.color = v.color;
				o.normal = v.normal;
				o.vertPos = v.vertex;

				o.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
				o.normalDir = normalize(mul(v.normal, unity_WorldToObject).xyz);

				// Gouraud (per-vertex) specular model
				float3 lightDir;
				if (_WorldSpaceLightPos0.w == 0.0) {
					lightDir = normalize(_WorldSpaceLightPos0.xyz);
				} else {
					float3 vertToLight = _WorldSpaceLightPos0.xyz - mul(unity_ObjectToWorld, v.vertex).xyz;
					float dist = length(vertToLight);
					lightDir = normalize(vertToLight);
				}

				o.spec = float3(0.0, 0.0, 0.0);
				if (dot(o.normalDir, lightDir) >= 0.0 || _SpecModel == 1) {
					float3 reflection = reflect(lightDir, worldNormal);
					float3 viewDir = normalize(o.viewDir);
					o.spec = saturate(dot(reflection, -o.viewDir));
					o.spec = pow(o.spec, 20.0f);
				}

				// Calculate vertex lighting
				o.diff.rgb = float3(0, 0, 0);
				#if defined(DIFF_VERTEX)
					float nl = (max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz)));
					o.diff.rgb = nl * _LightColor0;
					o.diff.rgb += ShadeSH9(half4(worldNormal, 1));
				#endif


				#ifdef VERTEXLIGHT_ON
				o.diff.rgb += Shade4PointLights(
					unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
					unity_LightColor[0].rgb, unity_LightColor[1].rgb,
					unity_LightColor[2].rgb, unity_LightColor[3].rgb,
					unity_4LightAtten0, worldPos, worldNormal
				);
				#endif

					

				o.lightDir = lightDir;

				// Outputs needed for calculating normal in fragment

				// World normal
				o.N = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				// World tangent
				o.T = normalize(mul(unity_ObjectToWorld, v.tangent).xyz);
				// World binormal
				o.B = normalize(cross(o.N, o.T));

				UNITY_TRANSFER_FOG(o, o.pos);
				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{

				float2 adjUv = PerformAffineMapping(i.uv, _MainTex_ST);
				float2 adjUV1 = PerformAffineMapping(i.uv1, unity_LightmapST);
				float4 albedo = tex2D(_MainTex, adjUv);
				// Switch between main texture and LOD texture depending on LOD distance
				float4 lod = tex2D(_LODTex, adjUv);
				albedo = lerp(albedo, lod, i.uv.a && lod.r + lod.g + lod.b < 3.0);
				float4 col = float4(1,1,1,1);

				#if !UNITY_COLORSPACE_GAMMA
					albedo.rgb = LinearToGammaSpace(albedo.rgb);
				#endif

				#if !defined(UNLIT)
					// Normal mapping
					float3 unpacked = UnpackScaleNormal(tex2D(_NormalMap, adjUv), _NormalMapDepth);
					float3x3 TBN = float3x3(i.T, i.B, i.N);
					float3 normalDir = normalize(mul(unpacked, TBN));

					// Calculate metal/smoothness map
					float3 reflectedDir = reflect(i.viewDir, normalize(i.normalDir));
					float4 metalMap = tex2D(_MetalMap, adjUv);
					#if !UNITY_COLORSPACE_GAMMA
						metalMap.rgb = LinearToGammaSpace(metalMap.rgb);
					#endif
					UnityIndirect indirectLight;
					indirectLight.diffuse = max(0, ShadeSH9(half4(i.normal, 1)));
					indirectLight.specular = 0;
					_Smoothness *= metalMap.a;
					float roughness = 1 - _Smoothness;
					float3 reflectionDir = reflect(-i.viewDir, i.normal);
					float4 envRefl = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectionDir, roughness * 6);
					indirectLight.specular = DecodeHDR(envRefl, unity_SpecCube0_HDR);

					// Apply lighting
					float3 lightDir = normalize(i.lightDir);
					float4 diffuse = float4(1, 1, 1, albedo.a);
					#if !defined(LIGHTMAP_ON)
						#if defined(DIFF_FRAGMENT)
							diffuse.rgb = _LightColor0.rgb * saturate(dot(normalDir, normalize(_WorldSpaceLightPos0.xyz)));
							#if UNITY_COLORSPACE_GAMMA
								diffuse.rgb += ShadeSH9(half4(normalDir, 1));
								diffuse.rgb += i.diff.rgb;
							#else
								diffuse.rgb += LinearToGammaSpace(ShadeSH9(half4(normalDir, 1)));
								diffuse.rgb += LinearToGammaSpace(i.diff.rgb);
							#endif
						#else
							#if UNITY_COLORSPACE_GAMMA
								diffuse.rgb = i.diff.rgb;
							#else
								diffuse.rgb = LinearToGammaSpace(i.diff.rgb);
							#endif
						#endif
					#endif

					diffuse.rgb *= albedo.rgb;
						

					// Phong specular model
					float3 specular = i.spec;
					#if defined(SPEC_PHONG)
						if (diffuse.r > 0) {
							if (_WorldSpaceLightPos0.w == 0.0) {
								lightDir = normalize(_WorldSpaceLightPos0.xyz);
							} else {
								float3 vertToLight = _WorldSpaceLightPos0.xyz - mul(unity_ObjectToWorld, i.pos).xyz;
								lightDir = normalize(vertToLight);
							}

							float3 reflection = reflect(lightDir, normalDir);
							float3 viewDir = normalize(i.viewDir);
							specular = pow(saturate(dot(reflection, -viewDir)), 20.0f);
						}
					#endif
					float4 specularIntensity;
					#if UNITY_COLORSPACE_GAMMA
						specularIntensity.rgb = tex2D(_SpecularMap, adjUv) * _Specular;
					#else
						specularIntensity.rgb = LinearToGammaSpace(tex2D(_SpecularMap, adjUv)) * _Specular;
					#endif
					specular *= specularIntensity;

					// Apply lighting calculations from above
					col.rgb = diffuse.rgb * (indirectLight.diffuse + indirectLight.specular) * _Metallic * metalMap.r + diffuse * (1 - _Metallic);
					#if defined(LIGHTMAP_ON)
						col.rgb *= LinearToGammaSpace(DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1))) + 0.6 * UNITY_LIGHTMODEL_AMBIENT;
					#endif
					// Add cubemap to output color
					#if UNITY_COLORSPACE_GAMMA
						col.rgb += texCUBE(_Cube, reflectedDir) / 2 - 0.25;
					#else
						col.rgb += LinearToGammaSpace(texCUBE(_Cube, reflectedDir)) / 2 - 0.25;
					#endif
					// Tint material
					col *= i.color * _Color;
					col.rgb += specular;
					// Darken darks
					col.rgb -= max(0, (1 - diffuse.rgb) * i.color) * _DarkMax;
					// Emission map
					#if UNITY_COLORSPACE_GAMMA
						col.rgb += tex2D(_Emission, adjUv) * _EmissionAmt;
					#else
						col.rgb += LinearToGammaSpace(tex2D(_Emission, adjUv)) * _EmissionAmt;
					#endif
					// Lighting/shadows
					#if defined(SHADOW_DEFAULT)
						col.rgb *= LIGHT_ATTENUATION(i);
					#elif defined(SHADOW_PSX)
						col.rgb -= 1 - LIGHT_ATTENUATION(i);
					#endif
					col.a = albedo.a * i.color.a * _Color.a;
				#else
					// If material is unlit, just set color to the albedo, tinting, and vertex colors
					col = albedo * i.color * _Color;
				#endif

				// Don't draw if outside render distance or cutout mode finds an alpha value below threshold
				float alphaCutoff = _CutoutThreshold;
				#if defined(TRANSPARENT)
					alphaCutoff = 0;
				#endif
				#if defined(TRANSPARENT) || defined(CUTOUT)
					clip(-(albedo.a <= alphaCutoff));
				#endif
				clip(-(i.diff.a && _DrawDist == 1));
				

				col.rgb = saturate(col.rgb);

				#if !UNITY_COLORSPACE_GAMMA
					col.rgb = GammaToLinearSpace(col.rgb * 1.1);
				#endif

				UNITY_APPLY_FOG(i.fogCoord, col.rgb);

				return col;
			}
			ENDCG
		}

		// Pass for realtime shadows
		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma multi_compile OPAQUE TRANSPARENT CUTOUT
			#pragma multi_compile _ WORLD_SPACE_SNAPPING
			#pragma multi_compile _ AFFINE_MAPPING
			#include "UnityCG.cginc"
			#include "PSXEffects.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 color : COLOR;
				float3 tangent: TANGENT;
			};

			struct v2f {
				V2F_SHADOW_CASTER;
				float3 uv : TEXCOORD1;
				float4 data4 : TEXCOORD2;
			};
			struct v2f_fragment {
				UNITY_VPOS_TYPE vpos : VPOS;
				float3 uv : TEXCOORD1;
				float4 data4 : TEXCOORD2;
			};

			sampler3D _DitherMaskLOD;

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

				// Vertex inaccuracy block
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.data4.x = (_DrawDistance > 0 && distance(worldPos, _WorldSpaceCameraPos) > _DrawDistance);
				float3 viewDir = mul((float3x3)unity_CameraToWorld, float3(0, 0, 1));
				worldPos.xyz += _WorldSpaceCameraPos.xyz * _CamPos;
				worldPos.xyz += viewDir * 100 * _CamPos;
				o.pos = UnityObjectToClipPos(v.vertex);
				if (_VertexInaccuracy < 0) _VertexInaccuracy = _VertexSnappingDetail;
				#if defined(WORLD_SPACE_SNAPPING)
					_VertexInaccuracy /= 2048;
					worldPos.xyz /= _VertexInaccuracy;
					worldPos.xyz = round(worldPos.xyz);
					worldPos.xyz *= _VertexInaccuracy;
					worldPos.xyz -= _WorldSpaceCameraPos.xyz * _CamPos + viewDir * 100 * _CamPos;
					v.vertex = mul(unity_WorldToObject, worldPos);
					o.pos = UnityObjectToClipPos(v.vertex);
				#else
					worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.pos = PixelSnap(o.pos);
				#endif


				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
				float wVal = mul(UNITY_MATRIX_P, v.vertex).z;
				#if defined(AFFINE_MAPPING)
					o.uv = float3(v.texcoord.xy * wVal, wVal);
				#else
					o.uv = v.texcoord;
				#endif
					
				return o;
			}

			float4 frag(v2f_fragment i) : SV_Target
			{
				#if defined(TRANSPARENT) || defined(CUTOUT)
					float2 adjUv = i.uv.xy;
					#if defined(AFFINE_MAPPING)
						adjUv = (i.uv / i.uv.z + _MainTex_ST.zw) * _MainTex_ST.xy;
					#else
						adjUv = (i.uv + _MainTex_ST.zw) * _MainTex_ST.xy;
					#endif

					fixed4 texcol = tex2D(_MainTex, adjUv);
					float alpha = texcol.a * _Color.a;
					#if defined(TRANSPARENT)
						_CutoutThreshold = 0.5;
					#endif
					clip((tex3D(_DitherMaskLOD, float3(i.vpos.xy * 0.25, alpha * 0.9375)).a - 0.01) * alpha - _CutoutThreshold);
				#endif

				clip(-(i.data4.x && _DrawDist == 1.0));
				SHADOW_CASTER_FRAGMENT(i);
			}
			ENDCG
		}

		// Pass for extra lights
		// Most of the code here is from the first pass
		Pass
		{
			Tags { "LightMode" = "ForwardAdd" }
			Blend [_SrcBlend] One
			ZWrite Off
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog
			#pragma multi_compile OPAQUE TRANSPARENT CUTOUT
			#pragma multi_compile _ WORLD_SPACE_SNAPPING
			#pragma multi_compile _ AFFINE_MAPPING

			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"
			#include "AutoLight.cginc"
			#include "PSXEffects.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				LIGHTING_COORDS(2, 3)
				float3 normal : TEXCOORD4;
				fixed4 diff : COLOR;
				float3 T : TEXCOORD5;
				float3 B : TEXCOORD6;
				float3 N : TEXCOORD7;
				UNITY_FOG_COORDS(10)
				float4 uv1 : TEXCOORD11;
			};

			v2f vert(appdata_tan v) {
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float3 viewDir = mul((float3x3)unity_CameraToWorld, float3(0, 0, 1));
				worldPos.xyz += _WorldSpaceCameraPos.xyz * _CamPos;
				worldPos.xyz += viewDir * 100 * _CamPos;
				o.pos = UnityObjectToClipPos(v.vertex);
				if (_VertexInaccuracy < 0) _VertexInaccuracy = _VertexSnappingDetail;
				#if defined(WORLD_SPACE_SNAPPING)
					_VertexInaccuracy /= 2048;
					worldPos.xyz /= _VertexInaccuracy;
					worldPos.xyz = round(worldPos.xyz);
					worldPos.xyz *= _VertexInaccuracy;
					worldPos.xyz -= _WorldSpaceCameraPos.xyz * _CamPos + viewDir * 100 * _CamPos;
					v.vertex = mul(unity_WorldToObject, worldPos);
					o.pos = UnityObjectToClipPos(v.vertex);
				#else
					worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.pos = PixelSnap(o.pos);
				#endif
				o.worldPos = worldPos;

				o.diff.a = (_DrawDistance > 0 && distance(worldPos, _WorldSpaceCameraPos) > _DrawDistance);

				// Set UV outputs
				float wVal = mul(UNITY_MATRIX_P, o.pos).z;
				#if defined(AFFINE_MAPPING)
					o.uv = float4(v.texcoord.xy * wVal, wVal, 0);
				#else
					o.uv = float4(v.texcoord.xyz, 0);
				#endif

				// Currently no difference from non-affine mapping
				#if defined(LIGHTMAP_ON)
					#if defined(AFFINE_MAPPING)
						o.uv1 = float4(v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw, wVal, 0);
					#else
						o.uv1 = float4(v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw, 0, 0);
					#endif
				#endif
				o.uv.a = (distance(worldPos, _WorldSpaceCameraPos) > _LODAmt && _LODAmt > 0);
				#if defined(LIGHTMAP_ON)
				o.uv1 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				o.normal = v.normal;

				// Outputs needed for calculating normal in fragment

				// World normal
				o.N = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				// World tangent
				o.T = normalize(mul(unity_ObjectToWorld, v.tangent).xyz);
				// World binormal
				o.B = normalize(cross(o.N, o.T));

				UNITY_TRANSFER_FOG(o, o.pos);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			fixed4 _LightColor0;

			fixed4 frag(v2f i) : COLOR
			{
				float2 adjUv = PerformAffineMapping(i.uv, _MainTex_ST);
				float2 adjUV1 = PerformAffineMapping(i.uv1, unity_LightmapST);
				fixed4 albedo = tex2D(_MainTex, adjUv);
				// Lerp between main texture and LOD texture depending on LOD distance
				float4 lod = tex2D(_LODTex, adjUv);
				albedo = lerp(albedo, lod, i.uv.a && lod.r + lod.g + lod.b < 3.0);
				// Don't draw if outside render distance
				float alphaCutoff = _CutoutThreshold;
				#if defined(TRANSPARENT)
					alphaCutoff = 0;
				#endif
				#if defined(TRANSPARENT) || defined(CUTOUT)
					clip(-(albedo.a <= alphaCutoff));
				#endif
				clip(-(i.diff.a && _DrawDist == 1));

				#if !UNITY_COLORSPACE_GAMMA
					albedo.rgb = LinearToGammaSpace(albedo.rgb);
				#endif

				float3 lightDir;

				#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
					lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
				#else
					lightDir = _WorldSpaceLightPos0.xyz;
				#endif

				UNITY_LIGHT_ATTENUATION(attenuation, 0, i.worldPos.xyz);

				// Normal mapping
				float3 unpacked = UnpackScaleNormal(tex2D(_NormalMap, adjUv), _NormalMapDepth);
				float3x3 TBN = float3x3(i.T, i.B, i.N);
				float3 normalDir = normalize(mul(unpacked, TBN));

				albedo *= _Color;

				float4 col;
				float diff = saturate(dot(normalDir, lightDir));
					
				#if !UNITY_COLORSPACE_GAMMA
					col.rgb = LinearToGammaSpace((albedo.rgb * _LightColor0.rgb * diff) * (attenuation * 2) / unity_ColorSpaceDouble);
				#else
					col.rgb = albedo.rgb * _LightColor0.rgb * diff * attenuation;
				#endif

				#if defined(LIGHTMAP_ON)
				col.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1));
				#endif

				col.rgb = saturate(col.rgb);

				#if !UNITY_COLORSPACE_GAMMA
					col.rgb = GammaToLinearSpace(col.rgb * 1.1);
				#endif
				col.a = 1;

				UNITY_APPLY_FOG(i.fogCoord, col.rgb);

				return col;
			}

			ENDCG
		}
	}
	CustomEditor "PS1ShaderEditor"
}
