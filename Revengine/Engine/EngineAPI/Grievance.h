#pragma once

#include "..\Components\ComponentsCommon.h"
#include "TransformMotivator.h"
#include "ScriptMotivator.h"
#include <string>

namespace revengine {
	namespace grievance {
		DEFINE_TYPED_ID(grievance_id);
		class grievance {
		public:
			constexpr explicit grievance(grievance_id id) : _id{ id } {}
			constexpr grievance() : _id{ id::invalid_id } {}
			constexpr grievance_id get_id() const { return _id; }
			constexpr bool is_valid() const { return id::is_valid(_id); }

			transform::motivator transform() const;
			script::motivator script() const;
		private:
			grievance_id _id;
		};
	}

	namespace script {
		class grievance_script : public grievance::grievance {
		public:
			virtual ~grievance_script() = default;
			virtual void begin_play() {}
			virtual void update(float) {}
		protected:
			constexpr explicit grievance_script(revengine::grievance::grievance grievance)
				: revengine::grievance::grievance{ grievance.get_id() } { }
		};

		namespace detail {
			using script_ptr = std::unique_ptr<grievance_script>;
			using script_creator = script_ptr(*)(grievance::grievance grievance);
			using string_hash = std::hash<std::string>;

			u8 register_script(size_t, script_creator);

			template<class script_class>
			script_ptr create_script(grievance::grievance grievance) {
				assert(grievance.is_valid());
				return std::make_unique<script_class>(grievance);
			}

			#define REGISTER_SCRIPT(TYPE)										\
				class TYPE;														\
				namespace {														\
					const u8 _reg##TYPE											\
					{															\
						revengine::script::detail::register_script(				\
							revengine::script::detail::string_hash()(#TYPE),	\
							&revengine::script::detail::create_script<TYPE>		\
						)														\
					};															\
				}
		}
	}
}
	