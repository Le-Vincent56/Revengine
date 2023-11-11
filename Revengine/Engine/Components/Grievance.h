#pragma once
#include "ComponentsCommon.h"

namespace revengine {

// Forward declaration to be able to state transform components
#define INIT_INFO(component) namespace component {struct init_info;}
	INIT_INFO(transform);
#undef INIT_INFO // End the forward declaration after using it - prevents further pollution of header files

	namespace grievance {
		struct grievance_info {
			transform::init_info* transform{ nullptr };
		};

		grievance_id create_grievance(const grievance_info& info);
		void remove_grievance(grievance_id id);
		bool is_alive(grievance_id id);
	}
}