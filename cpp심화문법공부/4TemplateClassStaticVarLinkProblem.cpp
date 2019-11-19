#include "4TemplateClassStaticVarLinkProblem.h"


// 이건 해봤자 별 의미 없는 정의임.
/*
template<typename T> T* Singleton<T>::instance;
*/

// main.cpp로 링크가 안됨. 원파일 코딩할거면 이렇게 해도 ㄱㅊ

int*				Singleton<int				>::instance;	// 요것들 안해주면 컴파일에러남. static 선언은 Singleton.h파일에서 해주지만, 선언이 문제
unsigned int*		Singleton<unsigned int		>::instance;	// 문제는, 몇가지 타입에 대해 선언해야할지 Singleton.cpp에서 정할 수 없음. 유저가 지맴대로 가져다 쓰자너.
unsigned char*		Singleton<unsigned char		>::instance;	// otherFile.cpp에서 정의하는 정책이면, 다른 파일에서 똑같이 정의해서 중복정의로 링크충돌남
unsigned long*		Singleton<unsigned long		>::instance;	// 그래서 extern을 떡칠해서 어떻게 하던데 찾아도 안나오네
unsigned long long*	Singleton<unsigned long long>::instance;
