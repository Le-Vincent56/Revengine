#pragma once
#include "ComponentsCommon.h"

namespace revengine::transform {
	DEFINE_TYPED_ID(transform_id);

	struct init_info {
		f32 position[3]{}; // Position
		f32 rotation[4]{}; // Rotation quaternions
		f32 scale[3]{ 1.f, 1.f, 1.f }; // Scale with default values of 1
	};

	transform_id create_transform(const init_info& info, grievance::grievance_id grievance_id);
	void remove_transform(transform_id id);
}