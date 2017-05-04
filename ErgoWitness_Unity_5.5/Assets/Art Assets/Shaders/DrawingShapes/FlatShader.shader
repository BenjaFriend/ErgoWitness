Shader "Flat/Simple" {
	Properties {
		_ForegroundColor ("Foreground Color", Color) = (1,1,1,1)
		_BackgorundColor ("Background Color", Color) = (0,0,0,0)
		_ForegroundMask ("Foreground Mask Texture", 2D) = "white" {}
		_ForegroundCutoff("Cutoff", Range(0,1)) = 0.5
		_BackgoundCuttoff("Background Cutoff", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _ForegroundMask;

		struct Input {
			float2 uv_ForegroundMask;
		};

		fixed4 _ForegroundColor;
		float _ForegroundCutoff;
		float _BackgoundCuttoff;
		float4 _BackgorundColor;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_ForegroundMask, IN.uv_ForegroundMask);
			clip(c.r - _BackgoundCuttoff);

			o.Albedo = _BackgorundColor;
			if (c.r > _ForegroundCutoff) {
				o.Albedo = _ForegroundCutoff;
			}

			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
