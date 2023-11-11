#pragma once

#include "..\Components\ComponentsCommon.h"
#include "TransformMotivator.h"

namespace revengine::grievance {
	DEFINE_TYPED_ID(grievance_id);
	class grievance {
	public:
		constexpr explicit grievance(grievance_id id) : _id { id }{}
		constexpr grievance() : _id{ id::invalid_id } {}
		constexpr grievance_id get_id() const { return _id; }
		constexpr bool is_valid() const { return id::is_valid(_id); }

		transform::motivator transform() const;
	private:
		grievance_id _id;
	};
}