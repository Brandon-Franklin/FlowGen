// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WaterFlow"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_flowmap("flowmap", 2D) = "white" {}
		_Color0("Color 0", Color) = (0.4632353,0.4632353,0.4632353,0.266)
		_FlowSpeed("FlowSpeed", Float) = 1
		_Texture0("Texture 0", 2D) = "bump" {}
		_Texture1("Texture 1", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha  vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float2 uv_texcoord;
		};

		uniform sampler2D _Texture0;
		uniform sampler2D _flowmap;
		uniform float4 _flowmap_ST;
		uniform float _FlowSpeed;
		uniform float4 _Color0;
		uniform sampler2D _Texture1;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_flowmap = i.uv_texcoord * _flowmap_ST.xy + _flowmap_ST.zw;
			float2 componentMask37 = tex2D( _flowmap,uv_flowmap).xy;
			float2 temp_cast_0 = (0.0).xx;
			float2 temp_cast_1 = (1.0).xx;
			float2 temp_cast_2 = (-0.5).xx;
			float2 temp_cast_3 = (0.5).xx;
			float2 temp_output_43_0 = ( (temp_cast_2 + (componentMask37 - temp_cast_0) * (temp_cast_3 - temp_cast_2) / (temp_cast_1 - temp_cast_0)) * ( 0.5 * -1.0 ) );
			float temp_output_51_0 = ( ( _Time.y / 20.0 ) * _FlowSpeed );
			float temp_output_54_0 = frac( temp_output_51_0 );
			float2 temp_output_61_0 = ( i.texcoord_0 + ( temp_output_43_0 * temp_output_54_0 ) );
			float2 temp_cast_4 = (0.0).xx;
			float2 temp_cast_5 = (1.0).xx;
			float2 temp_cast_6 = (-0.5).xx;
			float2 temp_cast_7 = (0.5).xx;
			float2 temp_output_62_0 = ( i.texcoord_0 + ( temp_output_43_0 * frac( ( temp_output_51_0 + 0.5 ) ) ) );
			float temp_output_64_0 = abs( ( ( temp_output_54_0 - 0.5 ) / 0.5 ) );
			o.Normal = lerp( UnpackNormal( tex2D( _Texture0,temp_output_61_0) ) , UnpackNormal( tex2D( _Texture0,temp_output_62_0) ) , temp_output_64_0 );
			float2 temp_cast_9 = (0.0).xx;
			float2 temp_cast_10 = (1.0).xx;
			float2 temp_cast_11 = (-0.5).xx;
			float2 temp_cast_12 = (0.5).xx;
			float2 temp_cast_13 = (0.0).xx;
			float2 temp_cast_14 = (1.0).xx;
			float2 temp_cast_15 = (-0.5).xx;
			float2 temp_cast_16 = (0.5).xx;
			o.Albedo = ( _Color0 * lerp( tex2D( _Texture1,temp_output_61_0) , tex2D( _Texture1,temp_output_62_0) , temp_output_64_0 ) ).xyz;
			o.Alpha = _Color0.a;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=7003
1569;280;598;754;2156.798;-551.7054;1.6;False;False
Node;AmplifyShaderEditor.RangedFloatNode;80;-1646.936,1098.507;Float;False;Constant;_Float6;Float 6;5;0;20;0;0;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;86;-1679.102,950.8048;Float;False;0;FLOAT;1.0;False;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;50;-1490.49,1169.708;Float;False;Property;_FlowSpeed;FlowSpeed;3;0;1;0;0;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-1866.488,588.9718;Float;True;Property;_flowmap;flowmap;1;0;Assets/flowmap.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;79;-1470.471,1038.936;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-1277.89,1109.508;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;52;-1424.192,1325.107;Float;False;Constant;_Float7;Float 7;4;0;0.5;0;0;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;42;-1482.488,876.9718;Float;False;Constant;_Float3;Float 3;3;0;0.5;0;0;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;37;-1557.687,440.172;Float;True;True;True;False;False;0;FLOAT4;0,0,0,0;False;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;41;-1482.488,780.9718;Float;False;Constant;_Float2;Float 2;3;0;-0.5;0;0;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;40;-1498.487,716.9718;Float;False;Constant;_Float1;Float 1;3;0;1;0;0;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;39;-1498.487,652.9718;Float;False;Constant;_Float0;Float 0;3;0;0;0;0;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;45;-1274.988,806.0717;Float;False;Constant;_Float4;Float 4;3;0;0.5;0;0;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;46;-1281.088,909.4716;Float;False;Constant;_Float5;Float 5;3;0;-1;0;0;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-1147.595,1271.51;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1130.688,777.9717;Float;True;0;FLOAT;0,0;False;1;FLOAT;0;False;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;38;-1255.488,544.9718;Float;True;0;FLOAT2;0.0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,0;False;3;FLOAT2;0,0;False;4;FLOAT2;1,0;False;FLOAT2
Node;AmplifyShaderEditor.FractNode;54;-983.1232,994.4925;Float;True;0;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;58;-829.6498,1436.747;Float;False;Constant;_Float8;Float 8;4;0;0.5;0;0;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-910.8881,691.8709;Float;True;0;FLOAT2;0.0;False;1;FLOAT;0,0;False;FLOAT2
Node;AmplifyShaderEditor.FractNode;57;-1015.391,1250.309;Float;True;0;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;60;-306.4913,529.7074;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-322.4913,689.7073;Float;False;0;FLOAT2;0.0;False;1;FLOAT;0.0,0;False;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-306.4913,897.7073;Float;False;0;FLOAT2;0.0;False;1;FLOAT;0.0,0;False;FLOAT2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;59;-622.4603,1308.795;Float;True;0;FLOAT;0.0;False;1;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-18.49115,705.7073;Float;False;0;FLOAT2;0.0;False;1;FLOAT2;0,0;False;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;62;-18.49115,865.7073;Float;False;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;FLOAT2
Node;AmplifyShaderEditor.TexturePropertyNode;78;-177.6132,264.8892;Float;True;Property;_Texture1;Texture 1;6;0;Assets/perlinNoise.png;False;white;Auto;SAMPLER2D
Node;AmplifyShaderEditor.SimpleDivideOpNode;63;-388.649,1362.956;Float;True;0;FLOAT;0.0;False;1;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.SamplerNode;72;149.7975,506.2458;Float;True;Property;_TextureSample3;Texture Sample 3;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;64;-124.1139,1344.859;Float;False;0;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;69;-207.4645,1022.471;Float;True;Property;_Texture0;Texture 0;6;0;Assets/4141-normal.jpg;True;bump;Auto;SAMPLER2D
Node;AmplifyShaderEditor.SamplerNode;73;110.146,285.5768;Float;True;Property;_TextureSample4;Texture Sample 4;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;76;517.0046,520.0375;Float;False;0;FLOAT4;0,0,0;False;1;FLOAT4;0,0,0;False;2;FLOAT;0.0;False;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;74;277.7971,1048.335;Float;True;Property;_TextureSample5;Texture Sample 5;4;0;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;5;197.8508,26.82394;Float;False;Property;_Color0;Color 0;3;0;0.4632353,0.4632353,0.4632353,0.266;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;71;272.8651,819.6663;Float;True;Property;_TextureSample2;Texture Sample 2;4;0;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;584.7952,259.4052;Float;False;0;COLOR;0.0;False;1;FLOAT4;0,0,0,0;False;FLOAT4
Node;AmplifyShaderEditor.LerpOp;75;654.4887,989.163;Float;False;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0;False;2;FLOAT;0.0;False;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;909.2677,360.3541;Float;False;True;2;Float;ASEMaterialInspector;0;Standard;WaterFlow;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;OBJECT;0.0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
WireConnection;79;0;86;0
WireConnection;79;1;80;0
WireConnection;51;0;79;0
WireConnection;51;1;50;0
WireConnection;37;0;2;0
WireConnection;53;0;51;0
WireConnection;53;1;52;0
WireConnection;44;0;45;0
WireConnection;44;1;46;0
WireConnection;38;0;37;0
WireConnection;38;1;39;0
WireConnection;38;2;40;0
WireConnection;38;3;41;0
WireConnection;38;4;42;0
WireConnection;54;0;51;0
WireConnection;43;0;38;0
WireConnection;43;1;44;0
WireConnection;57;0;53;0
WireConnection;55;0;43;0
WireConnection;55;1;54;0
WireConnection;56;0;43;0
WireConnection;56;1;57;0
WireConnection;59;0;54;0
WireConnection;59;1;58;0
WireConnection;61;0;60;0
WireConnection;61;1;55;0
WireConnection;62;0;60;0
WireConnection;62;1;56;0
WireConnection;63;0;59;0
WireConnection;63;1;58;0
WireConnection;72;0;78;0
WireConnection;72;1;62;0
WireConnection;64;0;63;0
WireConnection;73;0;78;0
WireConnection;73;1;61;0
WireConnection;76;0;73;0
WireConnection;76;1;72;0
WireConnection;76;2;64;0
WireConnection;74;0;69;0
WireConnection;74;1;62;0
WireConnection;71;0;69;0
WireConnection;71;1;61;0
WireConnection;82;0;5;0
WireConnection;82;1;76;0
WireConnection;75;0;71;0
WireConnection;75;1;74;0
WireConnection;75;2;64;0
WireConnection;0;0;82;0
WireConnection;0;1;75;0
WireConnection;0;9;5;4
ASEEND*/
//CHKSM=70E491A191C7D2A6CE5F29957016026755E14F24