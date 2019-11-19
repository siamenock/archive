#include "4TemplateClassStaticVarLinkProblem.h"
#include <iostream>

using namespace std;
//==========================================//
//				otherFile.cpp				//
//==========================================//
int main4() {
	Singleton<int				>::GetInstance() = 1;
	Singleton<unsigned int		>::GetInstance() = 2;
	Singleton<unsigned char		>::GetInstance() = 3;
	Singleton<unsigned long		>::GetInstance() = 4;
	Singleton<unsigned long long>::GetInstance() = 5;

	cout << Singleton<int				>::GetInstance() << endl;
	cout << Singleton<unsigned int		>::GetInstance() << endl;
	cout << Singleton<unsigned char		>::GetInstance() << endl;
	cout << Singleton<unsigned long		>::GetInstance() << endl;
	cout << Singleton<unsigned long long>::GetInstance() << endl;
	
	return 0;

}