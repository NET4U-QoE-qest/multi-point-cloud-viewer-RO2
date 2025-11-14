Shader "SJM-TECH/FakePBR_Unlit_VR_Final"
{
    Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
        _Color("Color Tint", Color) = (1,1,1,1)

        _OcclusionMap("Occlusion", 2D) = "white" {}
        _AOIntensity("AO Intensity", Range(0, 1)) = 1.0

        _LightMap("Lightmap", 2D) = "white" {}
        _LightmapIntensity("Lightmap Intensity", Range(0, 5)) = 1.0
    }

        SubShader
        {
            Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }
            LOD 100
            ZWrite On
            Cull Back

            Pass
            {
                Name "FORWARD"
                Tags { "LightMode" = "Always" }

                CGPROGRAM
                #pragma target 2.0
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing
                #pragma instancing_options renderinglayer

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed4 _Color;

                sampler2D _OcclusionMap;
                float4 _OcclusionMap_ST;
                float _AOIntensity;

                sampler2D _LightMap;
                float4 _LightMap_ST;
                float _LightmapIntensity;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float2 uvAlbedo : TEXCOORD0;
                    float2 uvAO : TEXCOORD1;
                    float2 uvLM : TEXCOORD2;
                    float4 vertex : SV_POSITION;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uvAlbedo = TRANSFORM_TEX(v.uv, _MainTex);
                    o.uvAO = TRANSFORM_TEX(v.uv, _OcclusionMap);
                    o.uvLM = TRANSFORM_TEX(v.uv, _LightMap);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                    fixed4 albedo = tex2D(_MainTex, i.uvAlbedo) * _Color;
                    fixed aoTex = tex2D(_OcclusionMap, i.uvAO).r;
                    fixed ao = lerp(1.0, aoTex, _AOIntensity);

                    fixed3 light = tex2D(_LightMap, i.uvLM).rgb * _LightmapIntensity;

                    fixed3 finalColor = albedo.rgb * ao * light;
                    return fixed4(finalColor, 1.0);
                }
                ENDCG
            }
        }

            FallBack "Unlit/Texture"
}
