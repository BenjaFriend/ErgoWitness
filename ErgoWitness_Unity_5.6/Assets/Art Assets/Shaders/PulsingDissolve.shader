// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PulsingDissolve"{

	// Variables that will show up the materail editor
	Properties{
		_MainTexture("Main Color (RGB) Hello!", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)

		_DissolveTexture("Dissolve", 2D) = "white" {}
		_DissolveAmount("Dissolve Amount", Range(0,1)) = 1
		
		_MaxDissolve("Max Dissolve Amount", Range(0,1)) = 1
		_MinDissolve("Min Dissolve Amount", Range(0,1)) = 0

		_ExtrudeAmount("Extrude Amount", Range(-1,1)) = 0

	}

		// You can use more then one sub shader for different 
		SubShader{
        	LOD 200
       

			// Each pass is a draw call, don't use a lot of them
			Pass {
				Name "DissolvePass"

				// Up until now it has been ShaderLab, now we can actuall do some generation
				// in NVIDI'AS CG language
				// CG is NVIDIA's graphics language, like GLSL or HLSL
				CGPROGRAM
				// Define the functions first
				#pragma vertex vertexFunction
				#pragma fragment fragmentFunction

				#include "UnityCG.cginc"

				// Vertices, Normal, color, UV
				struct appdata {
					// Import the verticies
					// Float 4 is like 1,1,1,1
					float4 vertex : POSITION;
					//float 2 is 1,1
					float2 uv :TEXCOORD0;	
					// Color would be float4, RGBA
					// Bring in the normals of the cat
					float3 normal : NORMAL;
				};

				struct v2f{
					float4 position: SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				// Import the values in from the properties
				float4 _Color;
				sampler2D _MainTexture; 

				sampler2D _DissolveTexture;

				float _DissolveAmount;
				float _MaxDissolve;
				float _MinDissolve;
				float _ExtrudeAmount;

		        float _RotationSpeed;

		  
				// Build our object! This is where you can make the vertecies bounce around
				v2f vertexFunction(appdata IN){
					v2f OUT;
					// Manipulate the vertex before passing it into the stream
					// Offset the vertex positions
					IN.vertex.xyz += IN.normal.xyz * _ExtrudeAmount * abs(sin(_Time.y));

					// MVP = Model View Projection
					// Pass in the vertex
					OUT.position = UnityObjectToClipPos(IN.vertex);
					// Take the UV's from the fragment and send it to the vertex
					OUT.uv = IN.uv;

					return OUT;
				}

				// Color it in!
				fixed4 fragmentFunction(v2f IN) : SV_Target {
					// Change the amount of dissolving happening
					//_DissolveAmount += abs(cos(_Time.y));
					// Clamp this value
					//_DissolveAmount = clamp(_DissolveAmount, _MinDissolve, _MaxDissolve);

					float4 textureColor = tex2D(_MainTexture, IN.uv);
					float4 dissolveColor = tex2D(_DissolveTexture, IN.uv);

					clip(dissolveColor.rgb - _DissolveAmount);

					return textureColor * _Color;
				}

				ENDCG

			}
		}

		FallBack "Diffuse"


}