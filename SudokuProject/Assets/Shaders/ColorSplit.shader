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

                float2 sectionUV;
                if (_Sections == 2)
                {
                     float section = uv.y > uv.x ? 0 : 1; // (angle < 3.14159) ? 0 : 1; // Determine section based on angle for two-color scenario

                    sectionUV = float2(section, 0.5); // Assign each section a unique color based on a split
                }
                else
                {
                    float section = floor((angle / (3.14159 * 2)) * _Sections);
                    float sectionAngle = section / _Sections * 3.14159 * 2;
                    sectionUV = float2(cos(sectionAngle), sin(sectionAngle)) * 0.5 + 0.5;
                }
                
                
                // float section = _Sections == 2 ? uv.y > uv.x ? 0 : 1 
                //                     : floor((angle / (3.14159 * 2)) * _Sections);
                
                

                fixed4 col = tex2D(_Colors, sectionUV);
                
                return col;
            }
            ENDCG 
        }
    }
}