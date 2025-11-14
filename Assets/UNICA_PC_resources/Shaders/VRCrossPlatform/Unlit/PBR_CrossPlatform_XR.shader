Shader "SJM-TECH/Unlit/PBR_CrossPlatform_XR"
{
    Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
        _Color("Color Tint", Color) = (1,1,1,1)

        _OcclusionMap("Occlusion", 2D) = "white" {}
        _AOIntensity("AO Intensity", Range(0,1)) = 1.0

        _LightMap("Lightmap (custom)", 2D) = "white" {}
        _LightmapIntensity("Lightmap Intensity", Range(0,5)) = 1.0
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
            LOD 100

            Pass
            {
                Name "FORWARD"
                Tags { "LightMode" = "Always" }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #pragma target 3.0

            // XR / instancing compatibili
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _OcclusionMap;
            float4 _OcclusionMap_ST;

            sampler2D _LightMap;
            float4 _LightMap_ST;

            float4 _Color;
            float _AOIntensity;
            float _LightmapIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uvAlbedo : TEXCOORD0;
                float2 uvAO     : TEXCOORD1;
                float2 uvLM     : TEXCOORD2;

                UNITY_VERTEX_OUTPUT_STEREO
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvAlbedo = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvAO = TRANSFORM_TEX(v.uv, _OcclusionMap);
                o.uvLM = TRANSFORM_TEX(v.uv, _LightMap);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                fixed4 albedo = tex2D(_MainTex, i.uvAlbedo) * _Color;
                fixed aoTex = tex2D(_OcclusionMap, i.uvAO).r;
                fixed ao = lerp(1.0, aoTex, _AOIntensity);
                fixed3 lm = tex2D(_LightMap, i.uvLM).rgb * _LightmapIntensity;

                fixed3 finalCol = albedo.rgb * ao * lm;
                return fixed4(finalCol, 1.0);
            }
            ENDCG
        }
        }

            FallBack "Unlit/Texture"
}
