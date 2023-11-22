#include "Common.h"
#include "CommonHeaders.h"

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#include  <Windows.h>

using namespace revengine;

namespace {
	HMODULE game_code_dll{ nullptr };
}

EDITOR_INTERFACE u32 LoadGameCodeDLL(const char* dll_path) {
	// Check that the game_code_dll exists
	if (game_code_dll) return FALSE;

	// Load the library and assert
	game_code_dll = LoadLibraryA(dll_path);
	assert(game_code_dll);

	return game_code_dll ? TRUE : FALSE;
}

EDITOR_INTERFACE u32 UnloadGameCodeDLL() {
	// Check that the game_code_dll exists
	if (game_code_dll) return FALSE;
	//assert(game_code_dll);

	// Free the library and assert the result
	int result = FreeLibrary(game_code_dll);
	//assert(result);

	// Set to nullptr for memory
	game_code_dll = nullptr;

	return TRUE;
}