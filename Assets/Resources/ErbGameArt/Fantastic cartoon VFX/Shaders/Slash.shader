Shader "ErbGameArt/Particles/Blend_Slash" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (0,0.5019608,1,1)
        _Strensh ("Strensh", Range(0, 2)) = 1
        _Noise ("Noise", 2D) = "black" {}
        [MaterialToggle] _Usedistortion ("Use distortion?", Float ) = 0
        _Opacitypower ("Opacity power", Float ) = 70
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog 
            uniform sampler2D _GrabTexture;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform float _Strensh;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform fixed _Usedistortion;
            uniform float _Opacitypower;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
                float sss = (saturate(_Noise_var.r)*_Strensh);
                float2 uval = ((i.uv0+float2(0.0,(i.uv0.b*1.0+-1.0)))*float2(1.0,(i.uv0.a*-18.0+20.0)));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(uval, _MainTex));
                float3 emissive = (lerp( 0.0, saturate(tex2D( _GrabTexture, (i.uv0.g+(sss*sss))).rgb), _Usedistortion )+(_MainTex_var.rgb*i.vertexColor.rgb*_TintColor.rgb*2.0));
                float uuu = uval.g;
                fixed4 finalRGBA = fixed4(emissive,(_MainTex_var.a*i.vertexColor.a*_TintColor.a*(saturate(((1.0 - uuu)*_Opacitypower))*saturate((uuu*_Opacitypower)))));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
}
