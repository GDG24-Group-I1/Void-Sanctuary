Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _Albedo("Albedo",Color)=(1,1,1,1)
        _Shades("Shades",Range(1,20))=3

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;

            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            float4 _Albedo;
            float _Shades;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // calculate the cosineof the angle between the normal vector and the light direction
                // the world space light direction is stored in _WorldSpaceLightPos0
                // the world space normal is stored in i.worldNormal
                // all what we have to do now is to normalize both vectors and calculate the dot product
                float cosinAngle= dot(normalize(i.worldNormal), normalize(_WorldSpaceLightPos0.xyz));
                //sdt the min to zero as the result can be negative in case where the light is behind the shaded point
                cosinAngle=max(cosinAngle, 0.0);
                cosinAngle=floor(cosinAngle * _Shades)/ _Shades;
                return _Albedo * cosinAngle;
            }
            ENDCG
        }
    }

    Fallback "VertexLit"
}
