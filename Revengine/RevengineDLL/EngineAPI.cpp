#include "Common.h"
#include "CommonHeaders.h"
#include "..\Engine\Components\Script.h"

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#include  <Windows.h>
#include <atlsafe.h>

using namespace revengine;

namespace {
	HMODULE game_code_dll{ nullptr };
	using _get_script_creator = revengine::script::detail::script_creator(*)(size_t);
	_get_script_creator get_script_creator{ nullptr };
	using _get_script_names = LPSAFEARRAY(*)(void);
	_get_script_names get_script_names{ nullptr };
}

EDITOR_INTERFACE u32 LoadGameCodeDLL(const char* dll_path) {
	// Check that the game_code_dll exists
	if (game_code_dll) return FALSE;

	// Load the library and assert
	game_code_dll = LoadLibraryA(dll_path);
	assert(game_code_dll);

	// Get a pointer to get_script_creator
	get_script_creator = (_get_script_creator)GetProcAddress(game_code_dll, "get_script_creator");

	// Get a pointer to get_script_names
	get_script_names = (_get_script_names)GetProcAddress(game_code_dll, "get_script_names");
	
	return (game_code_dll && get_script_creator && get_script_names) ? TRUE : FALSE;
}

EDITOR_INTERFACE u32 UnloadGameCodeDLL() {
	// Check that the game_code_dll exists
	if (!game_code_dll) return FALSE;
	assert(game_code_dll);

	// Free the library and assert the result
	int result{ FreeLibrary(game_code_dll) };
	assert(result);

	// Set to nullptr for memory
	game_code_dll = nullptr;

	return TRUE;
}

EDITOR_INTERFACE script::detail::script_creator GetScriptCreator(const char* name) {
	// If we have a valid pointer and a valid game code DLL, get the tag and the pointer to the creation function for the string
	return (game_code_dll && get_script_creator) ? get_script_creator(script::detail::string_hash()(name)) : nullptr;
}

EDITOR_INTERFACE LPSAFEARRAY GetScriptNames() {
	return (game_code_dll && get_script_names) ? get_script_names() : nullptr;
}