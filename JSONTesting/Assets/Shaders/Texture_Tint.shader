Shader "Custom/Texture_Tint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Blend One One // Additive
	
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
				float4 vertexPos : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vPos : SV_POSITION;
				float4 objectPos : TEXCOORD1;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				//o.vertexPos = UnityObjectToClipPos(v.vertexPos);
				o.vPos = mul(UNITY_MATRIX_MVP, v.vertexPos);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal.xyz = v.vertexPos.xyz;

				// Make for work
				UNITY_TRANSFER_FOG(o, o.vertexPos);
				return o;
			}
			
			fixed4 _Color;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 hexes = tex2D(_MainTex, i.uv) * _Color;
				fixed4 col = _Color * _Color.a + hexes;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, _Color);			
				return col;
			}
			ENDCG
		}
	}
}
