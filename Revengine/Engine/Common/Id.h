#pragma once
#include "CommonHeaders.h"

namespace revengine::id {
	// Revengine uses a generation/index system to reference certain ids - these will help us lookup a Grievance or Motivator within
	// their respective arrays easily. Generations are used to distinguish Grievances created at the same index slot, and will take
	// up the first 8 bits, meaning that our 30 bits are split into 22 index bits and 8 generation bits, which is a minimum of 
	// 4 million simultaneous entries. We can only, however, distinguish between 256 entries created in the same slot.

	// Set the amount of bits for an id
	using id_type = u32;

	// Establish generation and index bits and masks
	constexpr u32 generation_bits{ 8 };
	constexpr u32 index_bits{ sizeof(id_type) * 8 - generation_bits };
	constexpr id_type index_mask{ (id_type{1} << index_bits) - 1};
	constexpr id_type generation_mask{ (id_type{1} << generation_bits) - 1 };
	constexpr id_type id_mask{ id_type{-1} };

	using generation_type = std::conditional_t<generation_bits <= 16, std::conditional_t<generation_bits <= 8, u8, u16>, u32>;
	
	// Check that generation_type is not bigger than generation_bits and that 
	// id_type is not bigger than index_bits
	static_assert(sizeof(generation_type) * 8 >= generation_bits);
	static_assert((sizeof(id_type) - sizeof(generation_type)) > 0);

	/// <summary>
	/// Check if the ID is valid (if it's not -1)
	/// </summary>
	/// <param name="id">The ID to check</param>
	/// <returns>True if the ID is valid, false otherwise</returns>
	inline bool is_valid(id_type id) {
		return id != id_mask;
	}

	/// <summary>
	/// Masks the ID and gives only the index part
	/// </summary>
	/// <param name="id">The ID to retrieve the index part from</param>
	/// <returns>The index part of the ID</returns>
	inline id_type index(id_type id) {
		return id & index_mask;
	}

	/// <summary>
	/// Masks the ID and gives only the generation part
	/// </summary>
	/// <param name="id">The ID to retrieve the generation part from</param>
	/// <returns>The generation part of the ID</returns>
	inline id_type generation(id_type id) {
		return (id >> index_bits) & generation_mask;
	}

	/// <summary>
	/// Increment the generation
	/// </summary>
	/// <param name="id">The ID of whose generation to increment</param>
	/// <returns>The ID with an incremented generation</returns>
	inline id_type new_generation(id_type id) {
		// Get the generation and add 1
		const id_type generation{ id::generation(id) + 1 };

		// Asser that the generation is smaller than 255 - it will wrap
		// around otherwise and give the wrong id when asked for
		assert(generation < 255);

		// Get the new generation and shift it back to it's original place
		return index(id) | (generation << index_bits);
	}

#if _DEBUG
	namespace internal {
		struct id_base {
			constexpr explicit id_base(id_type id) : _id{ id } {}
			constexpr operator id_type() const { return _id; }

		private:
			id_type _id;
		};
	}

	// Define named types that can be used as distinctions between IDs
#define DEFINE_TYPED_ID(name)											\
		struct name final : id::internal::id_base {						\
			constexpr explicit name(id::id_type id)						\
				: id_base{ id } {}										\
			constexpr name() : id_base { id::id_mask } {}				\
		};
	}
#else
#define DEFINE_TYPED_ID(name) using name = id::id_type;
#endif