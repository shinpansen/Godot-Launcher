shader_type canvas_item;

uniform bool background = true;
uniform bool gray_scale = false;
uniform vec4 tint_color : source_color = vec4(1.0);

void fragment() {
	vec4 input_color = texture(TEXTURE, UV);
	vec4 output_color = input_color;
	
	if (!background && input_color.b < 0.45) {
		output_color.a = 0.0;
	}
	
	if (gray_scale) {
		float color_average = (output_color.r + output_color.g + output_color.b) / 3.0;
		output_color = vec4(color_average, color_average, color_average, output_color.a);
	}
	
	output_color = vec4(output_color.r * tint_color.r,
		output_color.g * tint_color.g,
		output_color.b * tint_color.b,
		output_color.a);
	
	COLOR = output_color;
}
