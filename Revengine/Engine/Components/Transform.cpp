#include "Transform.h"
#include "Grievance.h"

namespace revengine::transform {
	// Anonymous namespace
	namespace {
		utl::vector<math::v3> positions;
		utl::vector<math::v4> rotations;
		utl::vector<math::v3> scales;
	}

	motivator create (const init_info& info, grievance::grievance grievance) {
		assert(grievance.is_valid());
		const id::id_type grievance_index{ id::index(grievance.get_id()) };

		// Check if the motivator we are trying to add is within the the length
		// of the array
		if (positions.size() > grievance_index) {
			// Override that slot with new values
			positions[grievance_index] = math::v3(info.position);
			rotations[grievance_index] = math::v4(info.rotation);
			scales[grievance_index] = math::v3(info.scale);
		}
		else {
			// Confirm that the index is the maximum size of the positions
			assert(positions.size() == grievance_index);

			// Emplace the arrays and add the data at the back
			positions.emplace_back(info.position);
			rotations.emplace_back(info.rotation);
			scales.emplace_back(info.scale);
		}

		return motivator(transform_id{(id::id_type)positions.size() - 1});
	}

	void remove(motivator m) {
		// Confirm that the motivator is valid
		assert(m.is_valid());
	}

	// Initialize positions, rotations, and scales according to the index
	math::v3 motivator::position() const {
		assert(is_valid());
		return positions[id::index(_id)];
	}

	math::v4 motivator::rotation() const {
		assert(is_valid());
		return rotations[id::index(_id)];
	}

	math::v3 motivator::scale() const {
		assert(is_valid());
		return scales[id::index(_id)];
	}
}