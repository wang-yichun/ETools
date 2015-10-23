Shader "Vertex Colors/Alpha" {
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
	}
	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

		SubShader {
			Tags { "RenderType"="Opaque"}
			LOD 200
			Cull Off 
			ZWrite On 
			Blend SrcAlpha 
			OneMinusSrcAlpha 
			Pass{
				BindChannels {
					Bind "Color", color 
					Bind "Vertex", vertex
				}
			}
			
		} 
	}

	
}
