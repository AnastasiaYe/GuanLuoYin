Shader "UI/NoiseOverlay_GrayColor"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Contrast("Contrast", Range(0.1,5)) = 5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        ZWrite Off
        Cull Off
        Blend One One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Contrast;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // �����������
                fixed4 texcol = tex2D(_MainTex, i.uv);

                // ��������
                float lum = dot(texcol.rgb, float3(0.2126, 0.7152, 0.0722));

                // ��ǿ�ҶȶԱȶ�
                lum = pow(lum, 1.0 / _Contrast);

                // �����ȿ��� alpha
                texcol.a = lum;

                // ������ɫ��ͬʱ�� Tint
                texcol.rgb *= _Color.rgb;

                // ���� Tint alpha ��������͸����
                texcol.a *= _Color.a;

                return texcol;
            }
            ENDCG
        }
    }
}
