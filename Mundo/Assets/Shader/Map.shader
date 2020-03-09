Shader "Hidden/Map"
{
    Properties
    {
	    _MainTex ("Texture", 2D) = "white" ｛｝
        _SpecularColor("SpecularColor",Color )=(1,1,1,1)
        _SpecularGloss("Gloss",Range(8.0,256)) = 20
        _Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
		_Color("Diffuse", Color) = (1,1,1,1)
		_Specular("Specular", Color) = (1,1,1) // Memo: Postfix from material.(Revision>=0)
		_Ambient("Ambient", Color) = (1,1,1)
		_Shininess("Shininess", Float) = 0
		_ShadowLum("ShadowLum", Range(0,10)) = 1.5
		_AmbientToDiffuse("AmbientToDiffuse", Float) = 5
		_EdgeColor("EdgeColor", Color) = (0,0,0,1)
		_EdgeScale("EdgeScale", Range(0,2)) = 0 // Memo: Postfix from material.(Revision>=0)
		_EdgeSize("EdgeSize", float) = 0 // Memo: Postfix from material.(Revision>=0)
		_ToonTex("ToonTex", 2D) = "white" {}

		_SphereCube("SphereCube", Cube) = "white" {} // Memo: Postfix from material.(Revision>=0)

		_Emissive("Emissive", Color) = (0,0,0,0)
		_ALPower("ALPower", Float) = 0

		_AddLightToonCen("AddLightToonCen", Float) = -0.1
		_AddLightToonMin("AddLightToonMin", Float) = 0.5

		_ToonTone("ToonTone", Vector) = (1.0, 0.5, 0.5, 0.0) // ToonTone, ToonTone / 2, ToonToneAdd, Unused

		_NoShadowCasting("__NoShadowCasting", Float) = 0.0

		_TessEdgeLength("Tess Edge length", Range(2,50)) = 5
		_TessPhongStrength("Tess Phong Strengh", Range(0,1)) = 0.5
		_TessExtrusionAmount("TessExtrusionAmount", Float) = 0.0

		_Revision("Revision",Float) = -1.0 // Memo: Shader setting trigger.(Reset to 0<=)
    }
    SubShader
    {
        Tags { "Queue" = "Geometry+2" "RenderType" = "Transparent" }
		LOD 200

		Cull Front
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB

		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

			Offset -0.1, -1 // Transparent to near

			CGPROGRAM
			#pragma target 2.0
			#pragma exclude_renderers flash
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase nolightmap nodirlightmap
			#pragma multi_compile _ SPECULAR_ON
			#pragma multi_compile _ EMISSIVE_ON
			#pragma multi_compile _ SPHEREMAP_MUL SPHEREMAP_ADD
			#pragma multi_compile _ SELFSHADOW_ON
			#pragma multi_compile _ AMB2DIFF_ON
			#include "MMD4Mecanim-MMDLit-Surface-ForwardBase.cginc"
			ENDCG
        Cull Off ZWrite Off ZTest Always

				 Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc" 
            #include "AutoLight.cginc"
            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal :TEXCOORD1;
                float3 worldPos :TEXCOORD2;
                SHADOW_COORDS(3)
            };
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _SpecularColor;
            float  _SpecularGloss;
            float4  _Diffuse;
            v2f vert (a2v v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv,_MainTex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos  = mul(unity_ObjectToWorld,v.vertex).xyz;
                TRANSFER_SHADOW(o);
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                // 1.ambientColor
                float3  ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                // 2.diffuseColor
                float3 N =  normalize(i.worldNormal);
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float3 diffuseColor = _LightColor0.rgb * _Diffuse.rgb * max(0.0,dot(N,L));
                // 3.specularColor
                float3  V = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                // H替代了Phong中的reflectDic = normalize(reflect(-L,N));
                float3 H = normalize(L+V);
                // 这里需要注意的是计算高光反射的时候使用的是【半角向量 H】和法向量的点积
                float3 specularColor = _LightColor0.rgb * _SpecularColor.rgb * pow(max(0,dot(H,N)),_SpecularGloss);
                fixed  shadow = SHADOW_ATTENUATION(i);
                fixed4 col =  tex2D(_MainTex,i.uv) * float4 ((ambient+diffuseColor+specularColor) * shadow,1);
                return col;
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }
            ENDCG
        }
        Pass {
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }

			Offset -0.1, -1 // Transparent to near

			ZWrite Off Blend One One Fog { Color (0,0,0,0) }
			CGPROGRAM
			#pragma target 2.0
			#pragma exclude_renderers flash
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdadd
			#pragma multi_compile _ SPECULAR_ON
			#pragma multi_compile _ SELFSHADOW_ON
			#include "MMD4Mecanim-MMDLit-Surface-ForwardAdd.cginc"
			ENDCG
		}

		Cull Back
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB

		Pass {
			Name "FORWARD2"
			Tags { "LightMode" = "ForwardBase" }

			Offset -0.1, -1 // Transparent to near

			CGPROGRAM
			#pragma target 2.0
			#pragma exclude_renderers flash
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase nolightmap nodirlightmap
			#pragma multi_compile _ SPECULAR_ON
			#pragma multi_compile _ EMISSIVE_ON
			#pragma multi_compile _ SPHEREMAP_MUL SPHEREMAP_ADD
			#pragma multi_compile _ SELFSHADOW_ON
			#pragma multi_compile _ AMB2DIFF_ON
			#include "MMD4Mecanim-MMDLit-Surface-ForwardBase.cginc"
			ENDCG
		}

		Pass {
			Name "FORWARD_DELTA2"
			Tags { "LightMode" = "ForwardAdd" }

			Offset -0.1, -1 // Transparent to near

			ZWrite Off Blend One One Fog { Color (0,0,0,0) }
			CGPROGRAM
			#pragma target 2.0
			#pragma exclude_renderers flash
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdadd
			#pragma multi_compile _ SPECULAR_ON
			#pragma multi_compile _ SELFSHADOW_ON
			#include "MMD4Mecanim-MMDLit-Surface-ForwardAdd.cginc"
			ENDCG
		}

		Cull Off
		Blend Off
		ColorMask RGBA

		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			Fog {Mode Off}
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
			CGPROGRAM
			#pragma target 2.0
			#pragma exclude_renderers flash
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster
			#include "MMD4Mecanim-MMDLit-Surface-ShadowCaster.cginc"
			ENDCG
		}

		Pass {
			Name "ShadowCollector"
			Tags { "LightMode" = "ShadowCollector" }
			Fog {Mode Off}
			ZWrite On ZTest LEqual
			CGPROGRAM
			#pragma target 2.0
			#pragma exclude_renderers flash
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcollector
			#include "MMD4Mecanim-MMDLit-Surface-ShadowCollector.cginc"
			ENDCG
		}
    }
		Fallback Off
	CustomEditor "MMD4MecanimMaterialInspector"
}
