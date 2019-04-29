
//请配合ARSetRenderQueue.cs脚本设置被遮挡物体的渲染顺序

Shader "BToolkit/AR/HideInAlphaBox" {
	SubShader{
		Tags{"Queue" = "Background" }
		ColorMask 0
		ZWrite On
		Pass {}
	}
}