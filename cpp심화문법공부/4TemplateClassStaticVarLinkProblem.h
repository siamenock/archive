#pragma once
template<typename T>
class Singleton {
	static T* instance;		// = NULL�� �ڵ��ʱ�ȭ��. BSS ������ un-initialized global var�� Ư¡
public:
	static inline T& GetInstance() {
		if (!instance) { instance = new T; }
		return *instance;
	}
};
