Shader "Custom/MirrorWithMask" {
    Properties {
        _MirrorTex ("Mirror Texture", 2D) = "white" {}
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Vector) = (0.5, 0.5, 0, 0)
        _Feather ("Feather", Range(0, 0.5)) = 0.1
        _BorderColor ("Border Color", Color) = (0,0,0,1)
        _BorderWidth ("Border Width", Range(0, 0.1)) = 0.02
    }
    
    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MirrorTex;
            float4 _MirrorTex_ST;
            float2 _Center;
            float2 _Radius;
            float _Feather;
            fixed4 _BorderColor;
            float _BorderWidth;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MirrorTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                // Calculate distance from center (oval shape)
                float2 uv = i.uv - _Center;
                float distance = (uv.x * uv.x) / (_Radius.x * _Radius.x) + 
                                (uv.y * uv.y) / (_Radius.y * _Radius.y);
                
                // Sample mirror texture
                fixed4 mirrorColor = tex2D(_MirrorTex, i.uv);
                
                // Initialize output color
                fixed4 finalColor = mirrorColor;
                
                // Handle border and transparency
                if (distance < (1.0 - _BorderWidth)) {
                    // Inside oval (excluding border) - show mirror
                    finalColor.a = 1.0;
                }
                else if (distance < 1.0) {
                    // Border area
                    float borderFactor = (distance - (1.0 - _BorderWidth)) / _BorderWidth;
                    finalColor = lerp(mirrorColor, _BorderColor, borderFactor);
                    finalColor.a = 1.0;
                }
                else if (distance < (1.0 + _Feather)) {
                    // Feathering area
                    float featherFactor = (distance - 1.0) / _Feather;
                    finalColor.a = 1.0 - featherFactor;
                }
                else {
                    // Outside oval - fully transparent
                    finalColor.a = 0.0;
                }
                
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}