// This shader fills the mesh shape with a color predefined in the code.
Shader "Example/shdr_RaymarchedCloud"
{
    // The Properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties 
    { 
        [HideInInspector] _pixelsPerUnit ("_pixelsPerUnit", Integer) = 1
        [HideInInspector] _boundingBoxCenter ("_boundingBoxCenter", Vector) = (.25, .5, .5, 1)
        [HideInInspector] _boundingBoxSize  ("_boundingBoxSize", Vector) = (.25, .5, .5, 1)
        [NoScaleOffset] _densityField ("Density Texture", 3D) = "" {}
        
        _stepSize ("Raymarch Step Size", Float) = 1.0
        _SunRayStepSize ("Sun Raymarch Step Size", Float) = 1.0
        _densityMultiplier ("Density Multiplier", Float) = 1.0
        _scatteringCoef ("Scattering Coefficient", Float) = .5
        _absorbtionCoef ("Absorbtion Coefficient", Float) = .5
        
        
    }

    // The SubShader block containing the Shader code.
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"  "RenderPipeline" = "UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader.
            #pragma vertex vertex
            // This line defines the name of the fragment shader.
            #pragma fragment frag
            
            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                // The positionOS variable contains the veboxMaxex positions in object
                // space.
                float4 positionOS   : POSITION;
            };

            struct V2f
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
                float4 worldPos  : TEXCOORD0;
 
            };

            int _pixelsPerUnit;
            float3 _boundingBoxCenter;
            float3 _boundingBoxSize;
            
            Texture3D _densityField;
            float _stepSize;
            float _SunRayStepSize;
            
            float _densityMultiplier;
            float _scatteringCoef;
            float _absorbtionCoef;
            //interpolation functions

            float smoothstep(float a,float b,float alpha)
            {
                float t = alpha * alpha * (3.0 - 2.0 * alpha);
                return lerp(a,b,t);
            }
            
            float interpolate_noise(float a,float b,float alpha) //the function used by the 3D noise computation
            {
                return smoothstep(a,b,alpha);
            }

            float aces(float v)
            {
                // Apply tonemapping curve
                // Narkowicz 2016, "ACES Filmic Tone Mapping Curve"
                // https://knarkowicz.wordpress.com/2016/01/06/aces-filmic-tone-mapping-curve/
                const float a = 2.51;
                const float b = 0.03;
                const float c = 2.43;
                const float d = 0.59;
                const float e = 0.14;
                return saturate((v * (a * v + b)) / (v * (c * v + d) + e));
            }

            float3 aces(float3 v)
            {
                return float3(aces(v.x),aces(v.y),aces(v.z));
            }
            
            //noise functions
            float3 hash3( float3 p ) // replace this by something better
            {
	            p = float3( dot(p,float3(127.1,311.7, 74.7)),
			          dot(p,float3(269.5,183.3,246.1)),
			          dot(p,float3(113.5,271.9,124.6)));

	            return -1.0 + 2.0*frac(sin(p)*43758.5453123);
            }

            float hash(float3 p)
            {
                return frac( -1.0 + 2.0*frac(sin(dot(p,float3(127.1,311.7, 74.7)))*43758.5453123));
            }
            float noise(float3 p)
            {
                float3 alpha = frac(p);
                float3 pMin = trunc(p);
                float3 pMax = pMin + float3(1,1,1);
                float a = hash(float3(pMin.x,pMax.y,pMin.z));
                float b = hash(float3(pMin.x,pMax.y,pMax.z));
                float c = hash(pMax);
                float d = hash(float3(pMax.x,pMax.y,pMin.z));
                
                float e = hash(pMin);
                float f = hash(float3(pMin.x,pMin.y,pMax.z));
                float g = hash(float3(pMax.x,pMin.y,pMax.z));
                float h = hash(float3(pMax.x,pMin.y,pMin.z));

                float eh = interpolate_noise(e,h,alpha.x);
                float fg = interpolate_noise(f,g,alpha.x);
                float efgh = interpolate_noise(eh,fg,alpha.z);

                float ad = interpolate_noise(a,d,alpha.x);
                float bc = interpolate_noise(b,c,alpha.x);
                float abcd = interpolate_noise(ad,bc,alpha.z);

                return interpolate_noise(efgh,abcd,alpha.y);
            }
            float fractal_noise(float3 p,int levels,float roughness,float lacunarity)
            {
                float value = 0;
                float tiling = 1;
                float intensity = 1;
                float totalIntensity = 1;
                for (int i = 1; i<=levels;i++)
                {
                    value += noise(p*tiling)*intensity;
                    intensity *= roughness;
                    totalIntensity+=intensity;
                    tiling*=lacunarity;
                }
                return value/totalIntensity;
            }

            //texture sampling helper functions
            float3 get_texture_coordinates_at_world_pos(float3 ws,float3 boxMin,float3 invSize){
                return (ws - boxMin) * invSize;
                //return inv_lerp(boxMin,boxMax,ws);
                //return 1 * _pixelsPerUnit * (ws-_boundingBoxCenter) * _InveboxMaxResolution + float3(1,1,1)*.5;
            }
            
            //raymarching helper functions
            float compute_ray_length(float3 origin,float3 direction,float3 boxMin, float3 boxMax){
                float3 T_1, T_2; // vectors to hold the T-values for every direction
                float t_near = -Max_float(); // maximums defined in float.h
                float t_far = Max_float();
                
                for (int i = 0; i < 3; i++)
                { //we test slabs in every direction
                    if (direction[i] == 0)
                    { // ray parallel to planes in this direction
                        if ((origin[i] < boxMin[i]) || (origin[i] > boxMax[i]))
                        {
                            return false; // parallel AND outside box : no intersection possible
                        }
                    }
                    else
                    { // ray not parallel to planes in this direction
                        T_1[i] = (boxMin[i] - origin[i]) / direction[i];
                        T_2[i] = (boxMax[i] - origin[i]) / direction[i];

                        if(T_1[i] > T_2[i]){ // we want T_1 to hold values for intersection with near plane
                            float temp = T_1[i];
                            T_1[i] = T_2[i];
                            T_2[i] = temp;
                        }
                        if (T_1[i] > t_near){
                            t_near = T_1[i];
                        }
                        if (T_2[i] < t_far){
                            t_far = T_2[i];
                        }
                        if( (t_near > t_far) || (t_far < 0) ){
                            return false;
                        }
                    }
                }
               
                return t_far; // if we made it here, there was an intersection - YAY

                //TODO : take scene depth into account.
                
            }
            bool is_in_bounding_box(float3 pos,float3 boxMin, float3 boxMax){ 
                // float3 boxMin = _boundingBoxCenter - _boundingBoxSize*0.5;
                // float3 boxMax = _boundingBoxCenter + _boundingBoxSize*0.5;
                return 
                pos.x <= boxMax.x && pos.x >= boxMin.x 
                && pos.y <= boxMax.y && pos.y >= boxMin.y 
                && pos.z <= boxMax.z && pos.z >= boxMin.z;
            }
            float get_density_at_point(float3 p,float3 uvw)
            {
                float density = _densityField.SampleLevel(sampler_LinearClamp,uvw,0);
                //density*=density * density *2;
                density = saturate(density);
                
                p = p*float3(1,1.2,1)*.3;
                float noise = fractal_noise(p,8,.7,1.5)*.5+.5;
                //return density;
                noise = noise * noise;
                noise = noise * noise;
                //noise = noise * noise;
                
                //noise = saturate(noise-noise%.1);
                density = saturate(density-noise*.2);
                //return  saturate(density);
                //return  saturate(density-.1);
                
                //cool quantization effect
                density = lerp(density,density-density%.2,.5);
                //density = noise;
                return max(0,density*_densityMultiplier);
            }
            
            
            //== shader functions ==
            V2f vertex(Attributes IN)
            {
                // Declaring the output object (OUT) with the V2f struct.
                V2f OUT; 
                // The TransformObjectToHClip function transforms veboxMaxex positions
                // from object space to homogenous clip space.
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                OUT.worldPos = mul(unity_ObjectToWorld,IN.positionOS);
                    
                // Returning the output. 
                return OUT;
            }

            float phase(float teta)
            {
                //todo : mie ou rayleigh
                return 1;
            }
            
            // The fragment shader definition.
            half4 frag(V2f IN) : SV_Target
            {
                if (_stepSize==0) return half4(1,0,0,1);

                //define bounding box
                float3 boxMin = _boundingBoxCenter - _boundingBoxSize*0.5;
                float3 boxMax = _boundingBoxCenter + _boundingBoxSize*0.5;
                float3 bbWorldSize = (boxMax - boxMin);
                float3 bbInvWorldSize = 1/bbWorldSize;
                
                //define ray
                const float3 rayOrigin = IN.worldPos.xyz;
                const float3 rayDirection = -GetWorldSpaceNormalizeViewDir(IN.worldPos.xyz);// normalize(IN.worldPos.xyz- _WorldSpaceCameraPos.xyz );
                const float totalRayLength = compute_ray_length(rayOrigin,rayDirection,boxMin ,boxMax);
                    //float3 textureCoordinate = get_texture_coordinates_at_world_pos(IN.worldPos.xyz);
                
                const float extinctionCoef = _absorbtionCoef + _scatteringCoef;
                
                float totalDensity = 0;
                float3 outputColor = 0;
                for (float rayLength = 0;rayLength< totalRayLength;rayLength += _stepSize)
                {
                    //main sample
                    float3 samplePoint = rayOrigin+rayDirection*rayLength;
                    float3 uvw = get_texture_coordinates_at_world_pos(samplePoint,boxMin,bbInvWorldSize);
                    float density = get_density_at_point(samplePoint,uvw);
                    
                    if (density>0) // on skip les calculs quand la densité est egale a 0
                    {
                        //compute base transmittance
                        totalDensity += density *_stepSize ;
                        float baseTransmittance = exp(-totalDensity*rayLength);
                        
                        //compute sun transmittance
                        const float SunTotalRayLength = compute_ray_length(samplePoint,_MainLightPosition,boxMin ,boxMax);
                        float totalDensityTowardSun = 0;
                        for (float secondRayLength = 0;secondRayLength< SunTotalRayLength;secondRayLength += _SunRayStepSize)
                        {
                            const float3 SunSamplePoint = samplePoint+_MainLightPosition*secondRayLength;
                            const float3 SunUvw = get_texture_coordinates_at_world_pos(SunSamplePoint,boxMin,bbInvWorldSize);
                            const float Sundensity = get_density_at_point(SunSamplePoint,SunUvw);
                            totalDensityTowardSun += Sundensity;
                        }
                        totalDensityTowardSun *= _SunRayStepSize;
                        float SunTransmittance = exp(-SunTotalRayLength*totalDensityTowardSun);

                        //float3 skyColor = float3(.5,.8,1)*.5;
                        float3 skyColor = float3(.8,.3,.7)*.3;
                        float3 lightColor = (_MainLightColor * SunTransmittance + skyColor );
                        //baseTransmittance-=baseTransmittance%.3f;
                        outputColor += lightColor * baseTransmittance * _stepSize;
                        
                    }
                }
  
                
                //float3 uvw = get_texture_coordinates_at_world_pos(rayOrigin,boxMin,invSize);
                //float density = _densityField.SampleLevel(sampler_LinearClamp,uvw,0);

                float transmittance = exp(-totalDensity*totalRayLength);
                transmittance -= transmittance%.4-.2;
                float alpha = 1.0-transmittance;//( totalDensity);
                
                //outputColor = aces(outputColor);
                //return half4(1,1,1,0);
                return half4(outputColor,saturate(alpha)); 
            }
            ENDHLSL
        }
    }
}

//notes : 
//transmittance : amount of light that a volume absorbs. 0->opaque, 1->transparent
//beer's law : transmittance = e^(-distance*density)
//totalDensity = integral of the density over distance function ( area under the curve )
//             ~= sum of discrete samples * distance between samples -> Reimann Sum

//outscattering : how much light is reflected / scattered outward ~= diffuse lightning