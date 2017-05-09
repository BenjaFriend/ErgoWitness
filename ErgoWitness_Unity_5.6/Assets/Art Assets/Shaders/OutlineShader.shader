// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OutlineShader"{

	// Variables that will show up the materail editor
	Properties{
		_OutlineAmount("Outline Width", Range(0,1)) = 1
		_OutlineColor("Color", Color) = (1,1,1,1)
	}
	CGINCLUDE
	// Define the functions first

	#include "UnityCG.cginc"

	// Vertices, Normal, color, UV
	struct appdata {
		// Import the verticies
		// Float 4 is like 1,1,1,1
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f{
		float4 pos : SV_POSITION;
		float4 color : COLOR;
	};

	// Import the values in from the properties
	float4 _OutlineColor;
	float _OutlineAmount;

	v2f vert(appdata v) {
		// just make a copy of incoming vertex data but scaled according to normal direction
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
	 
		float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		float2 offset = TransformViewToProjection(norm.xy);
	 
		o.pos.xy += offset * o.pos.z * _OutlineAmount;
		o.color = _OutlineColor;
		return o;
	}
	
	ENDCG

	// You can use more then one sub shader for different 
	SubShader{
		Tags { "Queue" = "Transparent" }

		// Each pass is a draw call, don't use a lot of them
		/*Pass {
			Name "BASE"
			Cull Back
			Blend Zero One

			// uncomment this to hide inner details:
			//Offset -8, -8

			SetTexture [_OutlineColor] {
				ConstantColor (0,0,0,0)
				Combine constant
			}
		}*/

			// note that a vertex shader is specified here but its using the one above
		Pass {
			Name "OUTLINE"
			//Tags { "LightMode" = "Always" }
			Cull Front

			// you can choose what kind of blending mode you want for the outline
			//Blend SrcAlpha OneMinusSrcAlpha // Normal
			//Blend One One // Additive
			Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			 
			half4 frag(v2f i) :COLOR {
				return i.color;
			}
			ENDCG
		}

	}

	Fallback "Diffuse"

}



