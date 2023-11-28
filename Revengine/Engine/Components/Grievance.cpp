#include "Grievance.h"
#include "Transform.h"
#include "Script.h"
#include <iostream>

namespace revengine::grievance {
	// Anonymous namespace
	namespace {
		utl::vector<transform::motivator> transforms;
		utl::vector<script::motivator> scripts;
		utl::vector<id::generation_type> generations;
		utl::deque<grievance_id> free_ids;
	}


	grievance create(const grievance_info& info) {
		// Make sure all Grievances have a transform component
		assert(info.transform);
		if (!info.transform) return grievance{};

		grievance_id id;

		// Check if there's enough free slots available
		if (free_ids.size() > id::min_deleted_elements) {
			// Pick the first free slot
			id = free_ids.front();

			// Confirm if it is a "dead" grievance
			assert(!is_alive(id));

			// Remove it from the free_ids deque
			free_ids.pop_front();

			// Increase the ID's generation
			id = grievance_id{ id::new_generation(id) };

			// Remember the generations for this ID
			++generations[id::index(id)];
		}
		else {
			// Add a new element to the end of the list of grievances
			id = grievance_id{ (id::id_type)generations.size() };
			generations.push_back(0);

			// Add a default component to the end of the transforms array
			// array.emplace_back() is better for memory than array.resize()
			//		- the number of memory allocations stay low, don't need
			//		  to resize by 1 every time
			transforms.emplace_back();
			scripts.emplace_back();
		}

		// Assign the ID to the new grievance
		const grievance new_grievance{ id };
		const id::id_type index{ id::index(id) };

		// Create transform motivator
		assert(!transforms[index].is_valid());
		transforms[index] = transform::create(*info.transform, new_grievance);

		// Check if the transforms index is invalid, if so return a default grievance class
		// with an invalid index
		if (!transforms[index].is_valid()) return {};

		// Create script motivator if not null and has a valid
		// function pointer
		if (info.script && info.script->script_creator) {
			// Assume there is no valid component
			assert(!scripts[index].is_valid());
			
			// Create the script motivator
			scripts[index] = script::create(*info.script, new_grievance);

			// Confirm there is now a valid component
			assert(scripts[index].is_valid());
		}

		// Return the new grievance
		return new_grievance;
	}

	void remove(grievance_id id) {
		const id::id_type index{ id::index(id) };

		// Confirm if the grievance is alive
		assert(is_alive(id));

		// Remove transforms
		transform::remove(transforms[index]);

		// Put a default component in that slot
		transforms[index] = {};

		// Remove scripts
		if (scripts[index].is_valid()) {
			script::remove(scripts[index]);
			scripts[index] = {};
		}

		// Push back the ID
		free_ids.push_back(id);
	}

	bool is_alive(grievance_id id) {
		// Confirm if the grievance is valid
		assert(id::is_valid(id));

		// Acquire the id and index
		const id::id_type index{ id::index(id) };

		// Confirm if the index is less than the array size and that
		// the current generation for the index is correct with its ID
		assert(index < generations.size());
		assert(generations[index] == id::generation(id));

		// Return if the generation is correct, as it will be alive if so
		return (generations[index] == id::generation(id) && transforms[index].is_valid());
	}

	transform::motivator grievance::transform() const {
		// Confirm that the grievance is alive
		assert(is_alive(_id));

		// Get the index of the grievance
		const id::id_type index{ id::index(_id) };

		// Return the transform at that index
		return transforms[index];
	}

	script::motivator grievance::script() const {
		// Confirm that the grievance is alive
		//assert(is_alive(_id));

		// Get the index of the grievance
		const id::id_type index{ id::index(_id) };

		// Return the transform at that index
		return scripts[index];
	}
}