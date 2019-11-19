#include <memory>
#include <iostream>

using namespace std;

template <typename T>
class Node {
public:
	T val;
	Node* childr;
	Node* childl;

	Node(T const& val_) {
		val = val_;
		childr = childl = NULL;
	}
};

template <typename T>
class SmartPtr {
	T* t;
	int* copyCount;
public:
	SmartPtr() { t = NULL, copyCount = NULL; }
	SmartPtr(T t_) {
		copyCount = new int(1);
		t = new T(t_);
		cout << "construct. cur count " << *copyCount << endl;
	}

	SmartPtr(SmartPtr<T> const& that) {
		t = that.t;
		copyCount = that.copyCount;
		(*copyCount)++;
		cout << "construct. cur count " << *copyCount << endl;
	}

	~SmartPtr() { this->Delete(); }

	SmartPtr<T>& operator = (SmartPtr<T> const& that) {
		this->Delete();
		return this = SmartPtr<T>(that);
	}
	T& operator * () { return *t; }
	T* operator ->() { return t; }	// 이게 unary인지도 의문이고, .은 알아서 붙이나봐..? 혼란스럽지만 동작함

	void Delete() {
		if (copyCount) {
			--(*copyCount);
			cout << "deleting. cur count " << *copyCount << endl;
			if (*copyCount <= 0) {
				delete copyCount;
				delete t;
			}
			t = NULL, copyCount = NULL;
		}
	}
};


template <typename T> void SomeFunc   (T  t) { return; }
template <typename T> void SomeFuncRef(T& t) { return; }


int main3() {
	int i = 4;
	Node<int> n(20);
	{
		auto nptr1 = SmartPtr<Node<int>>(n);
		auto iptr1 = SmartPtr<int>(20);
		{
			auto iptr2 = iptr1;
			auto iptr3(iptr2);
			auto nptr2 = nptr1;
			auto nptr3(nptr2);

			(*iptr1)++;
			(*iptr2)++;
			(*iptr3)++;
			(*nptr1).val++;
			(*nptr2).val++;
			(*nptr3).val++;
			
			iptr1.Delete();
			nptr1.Delete();
			
			nptr2->val++;

			SomeFunc   (nptr3);	// 생성후 삭제 함
			SomeFuncRef(nptr3);	// 안해여

		}//	ptr2,3은 destroy됨.
	}	//	ptr1도 알아서 destroy, delete 됨

	// std shared ptr
	{
		auto iptr1 = make_shared<int>(20);
		auto nptr1 = make_shared<Node<int>>(n);
		{
			auto iptr2 = iptr1;
			auto iptr3(iptr2);
			auto nptr2 = nptr1;
			auto nptr3(nptr2);

			(*iptr1)++;
			(*iptr2)++;
			(*iptr3)++;
			(*nptr1).val++;
			(*nptr2).val++;
			(*nptr3).val++;

			iptr1.reset();
			nptr1.reset();

			nptr2->val++;

			SomeFunc(nptr3);	// 생성후 삭제 함
			SomeFuncRef(nptr3);	// 안해여

		}//	ptr2,3은 destroy됨.
	}
	return 0;
}