#pragma once
template<typename T>
class Singleton {
	static T* instance;		// = NULL로 자동초기화됨. BSS 영역의 un-initialized global var의 특징
public:
	static inline T& GetInstance() {
		if (!instance) { instance = new T; }
		return *instance;
	}
};
