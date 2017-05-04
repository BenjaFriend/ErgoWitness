// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader ".New Custom/Diffuse"
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
				// The stuff after the colan is called a 'symantic' 
				// Symantic's are things that Unity knows how to fill in
				// automatically
				float4 vertex : POSITION;
				// Request the texture coordinates from unity (the UV map)
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			// Takes in vertex and sends them to fragment
			// Everything that you have in appdata, typcially needs to be in v2f
			struct v2f
			{
				float4 pos : SV_POSITION;
				// Make sure that the vertecies know about the texture coordinates too
				float2 texcoord : TEXCOORD0;
				// Get the information about the normals
				float3 normal : NORMAL;
			};

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _LightColor0;

			// The vertex shader
			v2f vert (appdata IN)
			{
				v2f OUT;
				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.normal = mul(float4(IN.normal, 0.0), unity_ObjectToWorld).xyz;
				return OUT;
			}

			// The fragment shader, basically shades in all the geometry between the vertex
			// Vaules get interpolated across things, so normals can get un-normalized
			fixed4 frag (v2f IN) : COLOR
			{
				fixed4 texColor = tex2D(_MainTex, IN.texcoord);

				float3 normalDirection = normalize(IN.normal);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuse = _LightColor0.rgb * max(0.0, dot(normalDirection, lightDirection));

				return _Color * texColor * float4(diffuse,1);
			}



			ENDCG
		}


	}


}