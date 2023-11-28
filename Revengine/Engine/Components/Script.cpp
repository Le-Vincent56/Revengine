#include "Script.h"
#include "Grievance.h"

namespace revengine::script {
	// Anonymous namespace
	namespace {
		utl::vector<detail::script_ptr> grievance_scripts; // Use double-indexing
		utl::vector<id::id_type> id_mapping;
		utl::vector<id::generation_type> generations;
		utl::vector<script_id> free_ids;

		using script_registry = std::unordered_map<size_t, detail::script_creator>;

		script_registry& registry() {
			// This is a static variable because of the initialization order
			// of static data - this way, we can be certain that the data is initialized
			// before accessing it
			static script_registry reg;
			return reg;
		}

		#ifdef USE_WITH_EDITOR
			// This is a static variable because of the initialization order
			// of static data - this way, we can be certain that the data is initialized
			// before accessing it
			utl::vector<std::string>& script_names()
			{
				static utl::vector<std::string> names;
				return names;
			}
		#endif

		bool exists(script_id id) {
			// Assert that the ID is valid
			assert(id::is_valid(id));

			// Get the index part of the ID
			const id::id_type index{ id::index(id) };
			assert(index < generations.size() && id_mapping[index] < grievance_scripts.size());

			// Confirm that the generations agree
			assert(generations[index] == id::generation(id));

			// Return true if the generations agree and if the script slot has a pointer that is not null
			return(generations[index] == id::generation(id)) &&
				grievance_scripts[id_mapping[index]] &&
				grievance_scripts[id_mapping[index]]->is_valid();
		}
	}

	namespace detail {
		u8 register_script(size_t tag, script_creator func) {
			// Get the registery and add a new pair (tag, func), which returns a pair in which the second
			// member variable is a boolean that states if the insertion succeeded or failed
			bool result{ registry().insert(script_registry::value_type{tag, func}).second };
			assert(result);
			return result;
		}

		script_creator get_script_creator(size_t tag) {
			// Look up the script by its tag
			auto script = revengine::script::registry().find(tag);

			// Confirm that we find it
			assert(script != revengine::script::registry().end() && script->first == tag);

			// Return the function
			return script->second;
		}

		#ifdef USE_WITH_EDITOR
			u8 add_script_name(const char* name) {
				script_names().emplace_back(name);
				return true;
			}
		#endif
	}

	motivator create(init_info info, grievance::grievance grievance) {
		assert(grievance.is_valid());
		assert(info.script_creator);

		script_id id{};
		if (free_ids.size() > id::min_deleted_elements) {
			// Get an id from the front
			id = free_ids.front();
			assert(!exists(id));

			// Remove it from the free ids
			free_ids.pop_back();

			// Increase the generation
			id = script_id{ id::new_generation(id) };

			// Increase the generation in the generations array
			++generations[id::index(id)];
		}
		else {
			// Add another ID at the end of id_mapping and generations
			id = script_id{ (id::id_type)id_mapping.size() };
			id_mapping.emplace_back();
			generations.push_back(0);
;		}

		assert(id::is_valid(id));

		// Call script_creator to create a new instance of the script class
		grievance_scripts.emplace_back(info.script_creator(grievance));

		// Confirm that the script ID is the same as the ID of the grievance
		// it belongs to
		assert(grievance_scripts.back()->get_id() == grievance.get_id());

		// Get the position where the script was added (end of the grievance_scripts array)
		// so that we can point to it in the id_mapping array
		const id::id_type index{ (id::id_type)grievance_scripts.size() - 1 };
		id_mapping[id::index(id)] = index;

		return motivator{ id };
	}

	void remove(motivator m) {
		assert(m.is_valid() && exists(m.get_id()));

		// Get the script ID
		const script_id id{ m.get_id() };

		// Get the id_mapping index of the ID
		const id::id_type index{ id_mapping[id::index(id)] };

		// Get the ID of the last script in the array
		const script_id last_id{ grievance_scripts.back()->script().get_id() };

		// Swap the to-be-deleted ID with the last one and remove it
		utl::erase_unordered(grievance_scripts, index);

		// Point the id_mapping slot to the new index of the swapped element
		id_mapping[id::index(last_id)] = index;

		// Case: If there's only one script, then overwrite the swapping with
		// an invalid ID
		id_mapping[id::index(id)] = id::invalid_id;
	}
}

#ifdef USE_WITH_EDITOR
#include <atlsafe.h>

extern "C" __declspec(dllexport)
LPSAFEARRAY get_script_names() {
	// Get the size of the vector
	const u32 size{ (u32)revengine::script::script_names().size() };

	// If there is no size, then return nullptr
	if (!size) return nullptr;

	// Allocate enough memory for the script names
	CComSafeArray<BSTR> names(size);

	// Convert to STR format for .NET
	for (u32 i{ 0 }; i < size; i++) {
		names.SetAt(i, A2BSTR_EX(revengine::script::script_names()[i].c_str()), false);
	}

	// Send free memory task to .NET framework
	return names.Detach();
}
#endif