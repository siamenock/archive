#include "4TemplateClassStaticVarLinkProblem.h"


// �̰� �غ��� �� �ǹ� ���� ������.
/*
template<typename T> T* Singleton<T>::instance;
*/

// main.cpp�� ��ũ�� �ȵ�. ������ �ڵ��ҰŸ� �̷��� �ص� ����

int*				Singleton<int				>::instance;	// ��͵� �����ָ� �����Ͽ�����. static ������ Singleton.h���Ͽ��� ��������, ������ ����
unsigned int*		Singleton<unsigned int		>::instance;	// ������, ��� Ÿ�Կ� ���� �����ؾ����� Singleton.cpp���� ���� �� ����. ������ ���ɴ�� ������ ���ڳ�.
unsigned char*		Singleton<unsigned char		>::instance;	// otherFile.cpp���� �����ϴ� ��å�̸�, �ٸ� ���Ͽ��� �Ȱ��� �����ؼ� �ߺ����Ƿ� ��ũ�浹��
unsigned long*		Singleton<unsigned long		>::instance;	// �׷��� extern�� ��ĥ�ؼ� ��� �ϴ��� ã�Ƶ� �ȳ�����
unsigned long long*	Singleton<unsigned long long>::instance;
