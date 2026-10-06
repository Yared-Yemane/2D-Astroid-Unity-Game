Shader "Asteroids/ShipHitFlash"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        _Intensity ("Hit Intensity", Range(0, 1)) = 0
        _AberrationAmount ("Aberration Amount", Float) = 0.035
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            struct Attributes
            {
                COMMON_2D_INPUTS
                half4 color : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_OUTPUTS
                half4 color : COLOR;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/2DCommon.hlsl"

            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE

            // NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
            // Do not include _MainTex_ST / _TexelSize — unsupported by 2D SRP Batcher.
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half _Intensity;
                half _AberrationAmount;
            CBUFFER_END

            Varyings UnlitVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                Varyings o = CommonUnlitVertex(input);
                o.color = input.color * _Color * unity_SpriteColor;
                return o;
            }

            half4 UnlitFragment(Varyings input) : SV_Target
            {
                half amount = _AberrationAmount * _Intensity;
                half2 redUV = input.uv + half2(-amount, 0);
                half2 greenUV = input.uv + half2(0, amount);
                half2 blueUV = input.uv + half2(amount, 0);

                half4 center = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, redUV).r;
                half g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, greenUV).g;
                half b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, blueUV).b;

                half4 result = half4(r, g, b, center.a) * input.color;
                result.rgb = lerp(result.rgb, result.rgb + half3(0.65, 0.15, 0.15), _Intensity);

#if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;
                InitializeSurfaceData(result.rgb, result.a, surfaceData);
                InitializeInputData(input.uv, inputData);
                SETUP_DEBUG_TEXTURE_DATA_2D_NO_TS(inputData, input.positionWS, input.positionCS, _MainTex);
                if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
#endif
                return result;
            }
            ENDHLSL
        }
    }
}
