Shader "Custom/ColorSplit"
{
    Properties
    {
        _Sections ("Number of Sections", Range(1, 9)) = 6
        _Colors("Color Palette", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            int _Sections;
            sampler2D _Colors;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5; // Center the UV coordinates
                float angle = atan2(uv.y, uv.x);
                if (angle < 0)
                {
                    angle += 3.14159 * 2; 
                }

                float section = floor((angle / (3.14159 * 2)) * _Sections);
                // if (_Sections == 2)
                // {
                //     section = uv.y > uv.x ? 0 : 1; // Split into two sections based on UV comparison
                // }
                // else
                // {
                //     section = floor((angle / (3.14159 * 2)) * _Sections);
                // }
                
                float sectionAngle = section / _Sections * 3.14159 * 2;
                float2 sectionUV = float2(cos(sectionAngle), sin(sectionAngle)) * 0.5 + 0.5;
                
                fixed4 col = tex2D(_Colors, sectionUV);
                
                return col;
            }
            ENDCG 
        }
    }
}
