Shader "Custom/RAMVolatile"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Chaos ("Chaos", Range(0,1)) = 0.25
        _GlitchSpeed ("Glitch Speed", Range(0,10)) = 4.0
        _TileSize ("Tile Size", Float) = 16.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _Color;
            float _Chaos;
            float _GlitchSpeed;
            float _TileSize;

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f    { float4 pos:SV_POSITION;  float2 uv:TEXCOORD0; };

            v2f vert(appdata v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float hash(float2 p){
                return frac(sin(dot(p, float2(127.1,311.7)))*43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // coordinate tile
                float2 tileCoord = floor(i.uv * _MainTex_TexelSize.zw / _TileSize);

                // --- FADE lento (tile che spariscono) ---
                float tSlow = floor(_Time.y * 1.5) / 1.5;
                float nFade  = hash(tileCoord + float2(tSlow*0.07, tSlow*0.13));
                float nFade2 = hash(tileCoord + float2(tSlow*0.19, tSlow*0.05) + 47.0);
                float fading = step(1.0 - _Chaos, nFade);
                float fadeAlpha = lerp(1.0, nFade2 * 0.1, fading);

                // --- GLITCH rapido (flicker) ---
                float tFast  = floor(_Time.y * _GlitchSpeed * 8.0);
                float nGlitch = hash(tileCoord + float2(tFast*0.3, tFast*0.7));
                float nGlitch2= hash(tileCoord + float2(tFast*0.5, tFast*0.2) + 13.0);
                // glitch colpisce meno tile ma più spesso
                float glitching = step(1.0 - _Chaos * 0.4, nGlitch);
                float glitchAlpha = lerp(1.0, nGlitch2 * 0.3, glitching);

                // combina i due effetti
                col.a *= min(fadeAlpha, glitchAlpha);
                return col;
            }
            ENDCG
        }
    }
}