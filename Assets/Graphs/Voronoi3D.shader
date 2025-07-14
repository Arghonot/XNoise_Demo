Shader "Xnoise/Generators/LibnoiseVoronoiFixed"
{
    Properties
    {
        _Seed ("Seed", Int) = 1
        _Frequency ("Frequency", Float) = 1.0
        _Distance ("Distance", Float) = 1.0
        _Displacement ("Displacement", Float) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            int _Seed;
            float _Frequency;
            float _Distance;
            float _Displacement;

            // Constants matching your implementation
            static const int GeneratorNoiseX = 1619;
            static const int GeneratorNoiseY = 31337;
            static const int GeneratorNoiseZ = 6971;
            static const int GeneratorSeed = 1013;

            // Value noise functions matching your implementation
            int ValueNoise3DInt(int x, int y, int z, int seed)
            {
                int n = (GeneratorNoiseX * x + GeneratorNoiseY * y + GeneratorNoiseZ * z +
                        GeneratorSeed * seed) & 0x7fffffff;
                n = (n >> 13) ^ n;
                return (n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff;
            }

            float ValueNoise3D(int x, int y, int z, int seed)
            {
                return 1.0 - (ValueNoise3DInt(x, y, z, seed) / 1073741824.0);
            }

            // Voronoi function matching your implementation
            float VoronoiGetValue(float3 position, int seed, float frequency, float distance, float displacement)
            {
                float x = position.x;
                float y = position.y;
                float z = position.z;
                
                // Scale by frequency
                x *= frequency;
                y *= frequency;
                z *= frequency;
                
                // Get integer coordinates (proper floor for negative numbers)
                int xInt = (x > 0.0 ? (int)x : (int)x - 1);
                int yInt = (y > 0.0 ? (int)y : (int)y - 1);
                int zInt = (z > 0.0 ? (int)z : (int)z - 1);
                
                float minDist = 2147483647.0;
                float xCandidate = 0;
                float yCandidate = 0;
                float zCandidate = 0;
                
                // Search 5x5x5 neighborhood (from -2 to +2)
                for (int zCur = zInt - 2; zCur <= zInt + 2; zCur++) 
                {
                    for (int yCur = yInt - 2; yCur <= yInt + 2; yCur++) 
                    {
                        for (int xCur = xInt - 2; xCur <= xInt + 2; xCur++) 
                        {
                            // Calculate seed point position within this cell
                            float xPos = xCur + ValueNoise3D(xCur, yCur, zCur, seed);
                            float yPos = yCur + ValueNoise3D(xCur, yCur, zCur, seed + 1);
                            float zPos = zCur + ValueNoise3D(xCur, yCur, zCur, seed + 2);
                            
                            // Calculate squared distance to seed point
                            float xDist = xPos - x;
                            float yDist = yPos - y;
                            float zDist = zPos - z;
                            float dist = xDist * xDist + yDist * yDist + zDist * zDist;
                            
                            if (dist < minDist) 
                            {
                                // Record closest seed point
                                minDist = dist;
                                xCandidate = xPos;
                                yCandidate = yPos;
                                zCandidate = zPos;
                            }
                        }
                    }
                }
                
                float value;
                if (distance > 0)
                {
                    // Calculate distance to nearest seed point (sphere falloff)
                    float xDist = xCandidate - x;
                    float yDist = yCandidate - y;
                    float zDist = zCandidate - z;
                    value = 1.0 - (sqrt(xDist * xDist + yDist * yDist + zDist * zDist) * 1.73205080757);
                }
                else 
                {
                    // No distance falloff
                    value = 0.0;
                }
                
                // Apply displacement based on seed point's cell
                float displacementValue = displacement * ValueNoise3D(
                    (int)(floor(xCandidate)),
                    (int)(floor(yCandidate)),
                    (int)(floor(zCandidate)),
                    seed);
                
                return value + displacementValue;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                // Calculate world position for 3D sampling
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos.xyz;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample Voronoi at world position
                float voronoiValue = (VoronoiGetValue(i.worldPos, _Seed, _Frequency, _Distance, _Displacement) + 1 ) /2;
                
                // Clamp and output as grayscale
                voronoiValue = saturate(voronoiValue);
                return fixed4(voronoiValue, voronoiValue, voronoiValue, 1.0);
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}