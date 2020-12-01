Shader "Custom/MapShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaTexEnabled ("Alpha Texture Enabled",float)=0.0
        _AlphaTex ("Alpha Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent"} 
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AlphaTex;
            float _AlphaTexEnabled;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                #ifdef PIXELSNAP_ON
                    o.vertex=UnityPixelSnap(o.vertex);
                #endif 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if(_AlphaTexEnabled)  
                {
                    fixed4 alphaValue=tex2D(_AlphaTex,i.uv);
                    col.a=alphaValue.a;
                }
                return col;
            }
            ENDCG
        }
    }
}
