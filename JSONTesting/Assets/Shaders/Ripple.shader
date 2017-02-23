Shader "Unlit/Ripple"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Value("Value", float) = 1
	}

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
       
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert


        sampler2D _MainTex;
        fixed4 _Color;

        struct Input {
            float2 uv_MainTex;
            float3 customValue;
        };

		float _Value;

        void vert(inout appdata_full v, out Input o){
        	UNITY_INITIALIZE_OUTPUT(Input, o);
        	v.vertex.y += _Value;
        	o.customValue = _Value;
        }
 
        void surf (Input IN, inout SurfaceOutput o) {  
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * _Color.rgb;
            o.Alpha = c.a* _Color.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
	

