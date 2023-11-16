#pragma once
#include "..\Components\ComponentsCommon.h"

namespace revengine::script {
	DEFINE_TYPED_ID(script_id);

	class motivator final {
	public:
		constexpr explicit motivator(script_id id) : _id{ id } {}
		constexpr motivator() : _id{ id::invalid_id } {}
		constexpr script_id get_id() const { return _id; }
		constexpr bool is_valid() const { return id::is_valid(_id); }

	private:
		script_id _id;
	};
}