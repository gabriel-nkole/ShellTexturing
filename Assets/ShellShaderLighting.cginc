#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

struct MeshData {
    float3 vertex : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
};

struct Interpolators {
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : TEXCOORD1;
    float3 wPos : TEXCOORD2;
    LIGHTING_COORDS(3,4)
};

float _maxHeight;
uint _density;
float _noiseMin;
float _noiseMax;
float _thickness;

float _curvature;
float _dispStrength;
float3 _D;

float4 _shellColor;

float _h;

Interpolators vert (MeshData v) {
    Interpolators o;

    v.vertex += v.normal * _h * _maxHeight;
    v.vertex += _D * _dispStrength * pow(_h, _curvature);
    
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.wPos = mul(unity_ObjectToWorld, v.vertex);
    TRANSFER_VERTEX_TO_FRAGMENT(o);
    return o;
}

float hash( uint n ) {
	n = (n << 13U) ^ n;
    n = n * (n * n * 15731U + 789221U) + 1376312589U;
    return float( n & uint(0x7fffffffU))/float(0x7fffffff);
}

float4 frag (Interpolators i) : SV_Target {
    float2 local_space = frac(i.uv * _density) * 2 - 1;
    float dist = length(local_space);

    int2 uv = floor(i.uv * _density);
    float rng = clamp(hash(uv.x * uv.y), _noiseMin, _noiseMax);

    clip((_thickness*(rng-_h)) - dist); 

    float3 N = normalize(i.normal);
    float3 L = normalize(UnityWorldSpaceLightDir(i.wPos));
    float attenuation = LIGHT_ATTENUATION(i);
    float lambert = dot(N, L) * 0.5 + 0.5;

    float3 lambertionDiffuse = lambert * attenuation * _LightColor0.xyz;
    return float4(_shellColor.xyz * lambertionDiffuse * _h, _shellColor.w);
}