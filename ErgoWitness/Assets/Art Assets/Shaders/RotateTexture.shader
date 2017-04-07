Shader "Custom/RotateTexture" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_RotationSpeed ("Rotation Speed", Float) = 2.0
        _Color ("Tint Color", color) = (1,1,1,1)
    }
    
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
       
			Blend One OneMinusSrcAlpha // Premultiplied transparency

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        sampler2D _MainTex;
		sampler2D _BumpMap;

        struct Input {
            float2 uv_MainTex;
			float2 uv_BumpMap;
        };
 
        float _RotationSpeed;
        fixed4 _Color;

        void vert (inout appdata_full v) {
            float sinX = sin ( _RotationSpeed * _Time );
            float cosX = cos ( _RotationSpeed * _Time );
            float sinY = sin ( _RotationSpeed * _Time );

            float2x2 rotationMatrix = float2x2( cosX, -sinX, sinY, cosX);

            v.texcoord.xy = mul ( v.texcoord.xy,  float2x2( 1, -1, sinY, cosX) );
        }
 
        void surf (Input IN, inout SurfaceOutput o) {  
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * _Color.rgb;
            o.Alpha = c.a * _Color.a;
			// set the normal map
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
    }
    FallBack "Diffuse"
}