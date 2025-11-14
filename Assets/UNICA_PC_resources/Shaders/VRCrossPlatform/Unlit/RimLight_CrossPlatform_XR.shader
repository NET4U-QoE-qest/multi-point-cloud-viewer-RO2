Shader "SJM-TECH/Unlit/RimLight_CrossPlatform_XR"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Base Color", Color) = (1,1,1,1)
        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimPower("Rim Power", Range(1,8)) = 4
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
                #pragma vertex   vert
                #pragma fragment frag

                #pragma multi_compile_instancing
                #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON

                #include "UnityCG.cginc"

                sampler2D _MainTex; float4 _MainTex_ST;
                float4 _Color, _RimColor; float _RimPower;

                struct appdata {
                    float4 vertex:POSITION;
                    float3 normal:NORMAL;
                    float2 uv:TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f {
                    float4 pos:SV_POSITION;
                    float2 uv:TEXCOORD0;
                    float3 wn:TEXCOORD1;
                    float3 wv:TEXCOORD2;
                    UNITY_VERTEX_OUTPUT_STEREO
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                v2f vert(appdata v) {
                    v2f o; UNITY_SETUP_INSTANCE_ID(v); UNITY_TRANSFER_INSTANCE_ID(v,o); UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv,_MainTex);
                    o.wn = UnityObjectToWorldNormal(v.normal);
                    o.wv = normalize(_WorldSpaceCameraPos.xyz - wpos);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    UNITY_SETUP_INSTANCE_ID(i);
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                    fixed4 baseCol = tex2D(_MainTex, i.uv) * _Color;

                    // rim semplice (view-aligned)
                    float ndotv = saturate(dot(normalize(i.wn), normalize(i.wv)));
                    float rim = pow(1.0 - ndotv, _RimPower);
                    baseCol.rgb += rim * _RimColor.rgb;

                    return baseCol;
                }
                ENDCG
            }
        }
            FallBack "Unlit/Texture"
}
