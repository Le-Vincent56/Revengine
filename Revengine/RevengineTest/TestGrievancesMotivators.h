#pragma once

#include "Test.h"
#include "..\Engine\Components\Grievance.h"
#include "..\Engine\Components\Transform.h"

#include <iostream>
#include <ctime>

using namespace revengine;

class engine_test : public test {
public:
	bool initialize() override {
		// Get a random seed
		srand((u32)time(nullptr));
		return true;
	}

	void run() override {
		do {
			for (u32 i{ 0 }; i < 10000; i++) {
				create_random();
				remove_random();
				_num_grievances = (u32)_grievances.size();
			}
			print_results();
		} while (getchar() != 'q');
	}
	void shutdown() override { }

private:
	utl::vector<grievance::grievance> _grievances;
	
	u32 _added{ 0 };
	u32 _removed{ 0 };
	u32 _num_grievances{ 0 };

	void create_random() {
		// Set a random amount of grievances to create between 0-20
		u32 count = rand() % 20;

		// If there are no grievances, set count to 1000
		if (_grievances.empty()) count = 1000;

		// Create a new grievance info
		transform::init_info transform_info{};
		grievance::grievance_info grievance_info{
			&transform_info,
		};

		while (count > 0) {
			// Increment countt
			_added++;

			// Create a new grievance
			grievance::grievance grievance{ grievance::create(grievance_info) };

			// Confirm the grievance is valid
			assert(grievance.is_valid() && id::is_valid(grievance.get_id()));

			// Add the grievance to the vector
			_grievances.push_back(grievance);

			assert(grievance::is_alive(grievance.get_id()));

			// Decrement count
			count--;
		}
	}

	void remove_random() {
		// Set a random amount of grievances to create between 0-20
		u32 count = rand() % 20;

		// If the grievances size is less than 1000, return
		if (_grievances.size() < 1000) return;

		while (count > 0) {
			// Increment removed
			_removed++;

			// Determine where to remove an entity
			const u32 index{ (u32)rand() % (u32)_grievances.size() };

			// Get the entity at the determined index
			const grievance::grievance grievance{ _grievances[index] };

			// Confirm the grievance is valid
			assert(grievance.is_valid() && id::is_valid(grievance.get_id()));

			// If the grievance is valid, then remove it and erase it from the array
			if (grievance.is_valid()) {
				grievance::remove(grievance.get_id());
				_grievances.erase(_grievances.begin() + index);
				assert(!grievance::is_alive(grievance.get_id()));
			}

			// Decrement count
			count--;
		}
	}

	void print_results() {
		// Print results
		std::cout << "Grievances created: " << _added << "\n";
		std::cout << "Grievances deleted: " << _removed << "\n";
	}
};