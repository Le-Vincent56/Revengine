#pragma comment(lib, "engine.lib");

#define TEST_GRIEVANCE_MOTIVATORS 1

#if TEST_GRIEVANCE_MOTIVATORS
#include "TestGrievancesMotivators.h"
#else
#error One of these tests need to be enabled
#endif

int main() {
// Check for memory leaks
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif

	engine_test test{};

	// Check if the test is initialized
	if (test.initialize()) {
		// Run the test
		test.run();
	}

	// Shutdown the test when done
	test.shutdown();
}