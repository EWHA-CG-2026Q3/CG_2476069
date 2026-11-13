Shader "CG/S16_MyCoal"
{
     Properties
    {
        _BaseColor ("Base Color", Color) = (0, 0, 0, 1)
        [HDR] _EmissionColor ("Emission Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 vert(float3 positionOS : POSITION) : SV_POSITION
            {
                return TransformObjectToHClip(positionOS);
            }

            float4 frag() : SV_Target
            {
                return float4(1.1, 0.5, 1.7, 1);
            }
            ENDHLSL
        }
    }
}

/*
// Shader 이름: 재질의 Shader 목록에 보이는 경로 (CG > S16_MyShader)
// 이름 규칙: "CG/" + 파일 이름에서 "Shader_"를 뺀 것
Shader "CG/S16_MyShader"
{
    // SubShader: 물체를 그리는 방법 한 세트
    // - 여러 개를 두면 Unity가 기기에 맞는 하나만 고름
    // - 이 셰이더에는 하나만 있음
    SubShader
    {
        // Pass: GPU가 물체를 한 번 그리는 과정
        // - 여러 개를 두면 GPU가 모두 차례로 실행
        // - 이 셰이더에는 하나만 있음
        Pass
        {
            // HLSLPROGRAM ~ ENDHLSL: GPU에서 실행될 코드 영역 (HLSL 언어)
            HLSLPROGRAM

            // #pragma vertex vert: 정점 셰이더(vertex shader) 단계에서 vert 함수를 실행하라는 지시
            // - vertex: 파이프라인 단계 이름
            // - vert: 함수 이름
            #pragma vertex vert
            // #pragma fragment frag: 프래그먼트 셰이더(fragment shader) 단계에서 frag 함수를 실행하라는 지시
            #pragma fragment frag

            // #include: URP가 제공하는 함수 모음(Core.hlsl)을 가져옴
            // - TransformObjectToHClip의 정의가 이 안에 있음
            // - 이 줄이 없으면 컴파일 오류
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // vert: 메시의 정점마다 한 번씩 실행
            // - 입력 positionOS : POSITION = 메시에 저장된 정점 위치 (Object Space, 물체 자신의 좌표)
            // - 출력 : SV_POSITION = 그 정점의 Clip Space 좌표
            // - semantic(POSITION, SV_POSITION): 값의 용도를 GPU에 알려 주는 이름
            float4 vert(float3 positionOS : POSITION) : SV_POSITION
            {
                // TransformObjectToHClip: (x, y, z, 1)에 P × V × M을 곱함 (워밍업 S08–S12의 행렬 계산)
                // - M(모델 행렬): 물체 좌표에서 월드 좌표로 (Transform의 Position, Rotation, Scale)
                // - V(뷰 행렬): 월드 좌표에서 카메라 좌표로
                // - P(투영 행렬): 카메라 좌표에서 Clip Space 좌표로
                return TransformObjectToHClip(positionOS);
            }

            // frag: 삼각형이 덮는 픽셀마다 한 번씩 실행
            // - 출력 : SV_Target = render target에 쓸 이 픽셀의 색
            // - 빈 괄호: 입력이 없음, 그래서 모든 픽셀이 같은 색
            float4 frag() : SV_Target
            {
                // float4(R, G, B, A): 빨강, 초록, 파랑, 알파(불투명도)
                // - 각 값: 0(없음) ~ 1(가득)이 기본, 1을 넘는 값도 가능
                // - 지금 값: 빨강
                return float4(1, 0, 0, 1);
            }

            ENDHLSL
        }
    }
}
*/