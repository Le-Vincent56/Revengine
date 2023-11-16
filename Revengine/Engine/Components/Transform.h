#pragma once
#include "ComponentsCommon.h"

namespace revengine::transform {
	struct init_info {
		f32 position[3]{}; // Position
		f32 rotation[4]{}; // Rotation quaternions
		f32 scale[3]{ 1.f, 1.f, 1.f }; // Scale with default values of 1
	};

	motivator create(const init_info& info, grievance::grievance grievance);
	void remove(motivator m);
}