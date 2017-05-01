// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Texture_Tint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Brightness("Brightness", float) = 5
		_MinBright("Minimum Brightness (Alpha)", float) = 5

		_Speed("Pulse Frequency", Range(0,200)) = 10
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}	

		LOD 100

		Blend One One // Additive alpha blending

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertexPos : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 screenuv : TEXCOORD1;
				float3 objectPos : TEXCOORD3;
				float4 vertex : SV_POSITION;
				float depth : DEPTH;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertexPos);
				OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

				OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
				OUT.normal.xyz = IN.vertexPos.xyz;

				OUT.objectPos = IN.vertexPos.xyz;

				return OUT;
			}

			fixed4 _Color;
			float _Brightness;
			float _Speed;
			float _MinBright;

			fixed4 texColor(v2f i)
			{
				fixed4 mainTex = tex2D(_MainTex, i.uv);

				mainTex.g *= sin(_Time.x * _Speed) * _Brightness + _MinBright;

				return mainTex.r * _Color + mainTex.g * _Color;
			}

			sampler2D _CameraDepthNormalsTexture;

			fixed4 frag (v2f i) : SV_Target
			{
				float screenDepth = DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, i.screenuv).zw);
				float diff = screenDepth - i.depth;
				float intersect = 0;

				if (diff > 0)
					intersect = 1 - smoothstep(0, _ProjectionParams.w * 0.5, diff);

				fixed4 hexes = texColor(i) * _Color;

				fixed4 col = _Color * _Color.a + intersect + hexes;
				return col;
			}
			ENDCG
		}
	}
}
