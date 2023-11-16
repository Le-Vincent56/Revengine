#pragma once
#include "ComponentsCommon.h"

namespace revengine::script {
	struct init_info {
		detail::script_creator script_creator;
	};

	motivator create(init_info info, grievance::grievance grievance);
	void remove(motivator m);
}