shader_type spatial;

uniform vec3 color : source_color = vec3(1.0);
uniform float emission = 20.0;
uniform float speed = 0.1;
uniform sampler2D mask_tex : hint_default_white;
uniform sampler2D cam_view_tex : hint_default_transparent;
uniform sampler2D border_noise_tex : repeat_enable;

float remap(float value, float low1, float high1, float low2, float high2) {
    return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
}

vec2 rotateUV(vec2 uv, float rotation) {
    float mid = 0.5;
    return vec2(
        cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
        cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
    );
}

void fragment() {
    vec2 r = vec2(0.8, 0.7);
    vec2 uv = r * UV * 2.0 - r;

    float baseMask = 1.0 - smoothstep(
        0.0, 0.3, length(uv) - 0.4);
    float ringMask = 1.0 - pow(baseMask, 0.03);
    ringMask *= remap(UV.y, 0.0, 1.0, 0.3, 2.0);

    vec3 col = vec3(ringMask);
    float noise = pow(texture(
        border_noise_tex, vec2(UV.x, UV.y + TIME * 0.1 * speed
    )).r + 0.2, 8.0);

    //float radius = (sin(TIME) + 1.0) / 2.0;
    //float ring = smoothstep(0, 0.03, length(uv) - radius);
    //ring -= smoothstep(0, 0.1, length(uv) - radius * 1.1);

    vec4 view = texture(cam_view_tex, SCREEN_UV).rgba;
    ALBEDO = mix(
        mix(vec3(0.0), view.rgb, view.a),
        color * col * emission,
        texture(mask_tex, rotateUV(UV, TIME * 0.1)).r);
    ALPHA = smoothstep(0.0, 0.01, baseMask);
}
