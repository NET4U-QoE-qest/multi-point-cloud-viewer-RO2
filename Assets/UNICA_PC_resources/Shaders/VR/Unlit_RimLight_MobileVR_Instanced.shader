Shader "SJM-TECH/UnlitRimLight_MobileVR"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (1,1,1,1)
        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimPower("Rim Power", Range(1.0, 8.0)) = 4.0
    }

        SubShader
        {
            Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }
            LOD 100
            ZWrite On
            Cull Back

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing
                #pragma instancing_options renderinglayer
                #pragma target 2.0

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed4 _Color;
                fixed4 _RimColor;
                float _RimPower;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 worldNormal : TEXCOORD1;
                    float3 worldViewDir : TEXCOORD2;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                    float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    o.worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                    fixed4 tex = tex2D(_MainTex, i.uv) * _Color;

                    float rim = 1.0 - saturate(dot(normalize(i.worldNormal), normalize(i.worldViewDir)));
                    rim = pow(rim, _RimPower);

                    tex.rgb += rim * _RimColor.rgb;
                    return tex;
                }
                ENDCG
            }
        }

            FallBack "Unlit/Texture"
}
