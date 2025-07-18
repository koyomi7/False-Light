Shader "Custom/ShapeMask" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Vector) = (0.5, 0.5, 0, 0)
        _Feather ("Feather", Range(0, 0.5)) = 0.1
        _BorderColor ("Border Color", Color) = (1,1,1,1)
        _BorderWidth ("Border Width", Range(0, 0.5)) = 0.02
    }
    
    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 200
        
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
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _Center;
            float2 _Radius;
            float _Feather;
            fixed4 _BorderColor;
            float _BorderWidth;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                // Calculate distance from center (oval shape)
                float2 uv = i.uv - _Center;
                float distance = (uv.x * uv.x) / (_Radius.x * _Radius.x) + 
                                (uv.y * uv.y) / (_Radius.y * _Radius.y);
                
                // Sample texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Create transparent oval mask
                float alpha = 1.0;
                
                // Feathering for smooth edges
                if (distance < 1.0) {
                    alpha = 0.0;  // Fully transparent inside oval
                    
                    // Add border if needed
                    if (_BorderWidth > 0 && distance > (1.0 - _BorderWidth)) {
                        float borderFactor = (distance - (1.0 - _BorderWidth)) / _BorderWidth;
                        col = lerp(col, _BorderColor, borderFactor);
                        alpha = borderFactor;
                    }
                }
                // Smooth transition at edges
                else if (distance < (1.0 + _Feather)) {
                    alpha = (distance - 1.0) / _Feather;
                }
                
                col.a = alpha;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}