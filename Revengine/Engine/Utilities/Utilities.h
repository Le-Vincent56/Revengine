#pragma once

#define USE_STL_VECTOR 1
#define USE_STL_DEQUE 1

#if USE_STL_VECTOR
#include<vector>
#include<algorithm>
namespace revengine::utl {
	template<typename T>
	using vector = std::vector<T>;

	template<typename T>
	void erase_unordered(std::vector<T>& v, size_t index) {
		// Check if the vector contains two or more elements
		if (v.size() > 1) {
			// Swap the element at the given index and the last element
			std::iter_swap(v.begin() + index, v.end() - 1);

			// Delete the last element
			v.pop_back();
		}
		else {
			// Otherwise, clear the vector
			v.clear();
		}
	}
}
#endif

#if USE_STL_DEQUE
#include <deque>
namespace revengine::utl {
	template<typename T>
	using deque = std::deque<T>;
}
#endif

namespace revengine::utl {
	// TODO: Implement our own containers

}