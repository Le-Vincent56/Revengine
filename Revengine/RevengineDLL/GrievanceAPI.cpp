#include "CommonHeaders.h"
#include "Id.h"
#include "..\Engine\Components\Grievance.h"
#include "..\Engine\Components\Transform.h"
#include "Common.h"

using namespace revengine;

// Need to convert from Editor format to Engine format - using an anonymous namespace for this
namespace {

	struct transform_component {
		f32 position[3];
		f32 rotation[3];
		f32 scale[3];

		transform::init_info to_init_info() {
			using namespace DirectX;
			transform::init_info info{};

			// Copy over position and scale
			memcpy(&info.position[0], &position[0], sizeof(f32) * _countof(position));
			memcpy(&info.scale[0], &scale[0], sizeof(f32) * _countof(scale));

			// Convert Euler Angles to Quaternion
			XMFLOAT3A rot{ &rotation[0] };
			XMVECTOR quat{ XMQuaternionRotationRollPitchYawFromVector(XMLoadFloat3A(&rot)) };
			XMFLOAT4A rot_quat{};
			XMStoreFloat4A(&rot_quat, quat);
			memcpy(&info.rotation[0], &rot_quat.x, sizeof(f32) * _countof(info.rotation));

			return info;
		}
	};

	struct grievance_descriptor {
		transform_component transform;
	};

	grievance::grievance grievance_from_id(id::id_type id) {
		// Put the ID into a grievance_id and return a grievance from that
		// grievance_id
		return grievance::grievance{ grievance::grievance_id{id} };
	}
}

EDITOR_INTERFACE
id::id_type CreateGrievance(grievance_descriptor* g) {
	// Confirm the grievance descriptor is valid
	assert(g);

	// Create a grievance_info from the description by turning the transform
	// from the editor to a the transform format of the engine and constructing
	// a grievance_info
	grievance_descriptor& desc{ *g };
	transform::init_info transform_info{ desc.transform.to_init_info() };
	grievance::grievance_info grievance_info{
		&transform_info,
	};

	// Return the ID given by the newly created grievance from the grievance_info to the editor
	return grievance::create(grievance_info).get_id();
}

EDITOR_INTERFACE
void RemoveGrievance(id::id_type id) {
	// Confirm that the ID is valid
	assert(id::is_valid(id));

	// Remove the grievancw attached to the ID
	grievance::remove(grievance::grievance_id{ id });
}