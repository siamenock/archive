#include <iostream>
#include <vector>
using namespace std;

template <typename T>
class MyVector {
public:
	vector<T> vec;
};

class C1 {
	char a;
};
class C2 {
	char a, b;
};
class C3v {
	char a, b, c;
	
	inline virtual
	void Print() { cout << "C3" << "\t" << "hell no world" << endl; }		// 이거 virtual로 바꾸면 4byte 추가 소모
};

class C3I1 {
	char a, b, c;
	int i;
};
class C3In1 {
	char a;
	int i;
	char b, c;
};
class C3_I1v :C3v {
	int i;

	inline virtual
	void Print() { cout << "C3_I1" << "\t" << "hell no world" << endl; }
};

int main1() {
	cout << "C1"	<< "\t" << "" << sizeof(C1) << endl;	// 1
	cout << "C2"	<< "\t" << "" << sizeof(C2) << endl;	// 2
	cout << "C3v"	<< "\t" << "" << sizeof(C3v) << endl;	// 8 = 3 + padding1 + vtable4
	cout << "C3I1v"	<< "\t" << "" << sizeof(C3I1) << endl;	// 8 = 3 + padding1 + int4
	cout << "C3in1" << "\t" << "" << sizeof(C3In1) << endl;	// 12 = 8 + int4
	cout << "C3_I1" << "\t" << "" << sizeof(C3_I1v) << endl;// 12 = 1 + pad3 + int4 + 2 + pad2

	int intArr[10];
	C3v C3Arr[10];
	C3_I1v C3_I1Arr[10];
	cout << "int[10]"	<< "\t" << "" << sizeof(intArr) << endl;
	cout << "C3v[10]"	<< "\t" << "" << sizeof(C3Arr) << endl;
	cout << "C3_I1v[10]" << "\t" << "" << sizeof(C3_I1Arr) << endl;

	return 0;
}