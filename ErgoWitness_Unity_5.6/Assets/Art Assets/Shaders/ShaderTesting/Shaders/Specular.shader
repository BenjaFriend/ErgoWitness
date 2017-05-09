// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader ".New Custom/Specular"
{
	// Shows up in the material inspector
	Properties 
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_SpecShininess("Shininess", Range(1.0, 100.0) = 2.0
	}

	SubShader
	{
		Pass
		{
		Tags { "LightMode" = "ForwardBase" }
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
				float4 posWorld : TEXCOORD1;
			};

			float4 _LightColor0;

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _SpecColor;
			float _SpecShininess;

			// The vertex shader
			v2f vert (appdata IN)
			{
				v2f OUT;
				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.posWorld = mul(unity_ObjectToWorld, IN.vertex);
				OUT.normal = mul(float4(IN.normal, 0.0), unity_ObjectToWorld).xyz;
				//OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
				return OUT;
			}

			// The fragment shader, basically shades in all the geometry between the vertex
			// Vaules get interpolated across things, so normals can get un-normalized
			fixed4 frag (v2f IN) : COLOR
			{
				// Get the coordinates for the texture to match up with the vertcies
				fixed4 texColor = tex2D(_MainTex, IN.texcoord);

				float3 normalDirection = normalize(IN.normal);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - IN.posWorld.xyz);
				float3 diffuse = _LightColor0.rgb * _Color.rgb *  max(0.0, dot(normalDirection, lightDirection));

				float3 specular;
				// So if I am not facing it
				if(dot(normalDirection, lightDirection) < 0.0)
				{
					// Set the specular to 0
					specular = float3(0.0, 0.0, 0.0);
				}
				else
				{
				// Reflect it along the normal, 
					specular = _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)),_SpecShininess);
				}

				float3 diffuseSpecular = diffuse + specular;

				return float4(diffuseSpecular, 1) * texColor;
			}

			ENDCG
		}


	}


}