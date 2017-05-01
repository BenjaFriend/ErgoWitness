// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader ".New Custom/Fixed Unlit"
{
	// Shows up in the material inspector
	Properties 
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Unity fills this in before it gets passed into the vertex
			// shader
			struct appdata
			{
				float4 vertex : POSITION;
				// Request the texture coordinates from unity (the UV map)
				float2 texcoord : TEXCOORD0;
			};

			// Takes in vertex and sends them to fragment
			struct v2f
			{
				float4 pos : SV_POSITION;
				// Make sure that the vertecies know about the texture coordinates too
				float2 texcoord : TEXCOORD0;
			};

			fixed4 _Color;
			sampler2D _MainTex;

			// The vertex shader
			v2f vert (appdata IN)
			{
				v2f OUT;
				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				return OUT;
			}

			// The fragment shader, basically shades in all the geometry between the vertex
			fixed4 frag (v2f IN) : COLOR
			{
				fixed4 texColor = tex2D(_MainTex, IN.texcoord);
				return texColor * _Color;
				//return _Color;
			}



			ENDCG
		}


	}


}