Shader "BToolkit/FlowLight" {
	Properties{
	    _Mask("Mask", 2D) = "white" {}
		_MainTex("Light", 2D) = "white" {}

		//以下参数仅用于：当对象处于遮罩里时避免黄色警告
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilComp("Stencil Comparison", Float) = 8
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
		_ColorMask("Color Mask", Float) = 15
	}

	Category{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }
		BindChannels{
		    Bind "Color", color
		    Bind "Vertex", vertex
		    Bind "TexCoord", texcoord
	    }

	    SubShader{
	    	Pass{
		        SetTexture[_Mask]{ combine texture * primary }
		        SetTexture[_MainTex]{
		            combine texture * previous
	            }
	        }
	    }
	}
}