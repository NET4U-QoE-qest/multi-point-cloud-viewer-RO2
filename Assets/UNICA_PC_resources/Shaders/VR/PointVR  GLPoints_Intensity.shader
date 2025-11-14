
    Shader "SJM-TECH/Point VR Instanced Intensity"
    {
        Properties
        {
            _Tint("Tint", Color) = (1, 1, 1, 1)
            _Intensity("Color Intensity", Range(0, 2)) = 1.0
        }
            SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Pass
            {
                CGPROGRAM

                #pragma vertex Vertex
                #pragma fragment Fragment

                #pragma multi_compile_fog
                #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
                #pragma multi_compile _ _COMPUTE_BUFFER

                #pragma multi_compile_instancing
                #pragma instancing_options renderinglayer

                #include "UnityCG.cginc"

                struct Attributes
                {
                    float4 position : POSITION;
                    half3 color : COLOR;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct Varyings
                {
                    float4 position : SV_POSITION;
                    half3 color : COLOR;
                    UNITY_FOG_COORDS(0)
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                half4 _Tint;
                float _Intensity;
                float4x4 _Transform;

            #if _COMPUTE_BUFFER
                StructuredBuffer<float4> _PointBuffer;
            #endif

            #if _COMPUTE_BUFFER
                Varyings Vertex(uint vid : SV_VertexID)
            #else
                Varyings Vertex(Attributes input)
            #endif
                {
                    Varyings o;

                    UNITY_SETUP_INSTANCE_ID(input);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                #if _COMPUTE_BUFFER
                    float4 pt = _PointBuffer[vid];
                    float4 pos = mul(_Transform, float4(pt.xyz, 1));

                    uint encoded = asuint(pt.w);
                    half3 col = half3(
                        (encoded >> 0) & 0xFF,
                        (encoded >> 8) & 0xFF,
                        (encoded >> 16) & 0xFF
                    ) / 255.0;
                #else
                    float4 pos = input.position;
                    half3 col = input.color;
                #endif

                #ifdef UNITY_COLORSPACE_GAMMA
                    col *= _Tint.rgb * _Intensity;
                #else
                    col *= LinearToGammaSpace(_Tint.rgb) * _Intensity;
                    col = GammaToLinearSpace(col);
                #endif

                    o.position = UnityObjectToClipPos(pos);
                    o.color = col;

                    UNITY_TRANSFER_FOG(o, o.position);
                    return o;
                }

                half4 Fragment(Varyings input) : SV_Target
                {
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                    half4 c = half4(input.color, _Tint.a);
                    UNITY_APPLY_FOG(input.fogCoord, c);
                    return c;
                }

                ENDCG
            }
        }
    }
