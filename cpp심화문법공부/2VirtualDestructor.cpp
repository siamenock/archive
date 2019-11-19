#include <iostream>


class Parent {
public:
	int p;
	Parent(int p_) : p(p_) { std::cout << "constructor: parent" << std::endl; };
	~Parent() { std::cout << "destructor: parent" << std::endl; };
};

class Child : public Parent {
public:
	int c;
	Child(int p_, int c_) : Parent(p_), c(c_) { std::cout << "constructor: child" << std::endl; };
	~Child() { std::cout << "destructor: child" << std::endl; };
};

class ParentV {
public:
	int p;
	//�����ڴ� virtual �ȵ˴ϴ� Human.
	ParentV(int p_) : p(p_) { std::cout << "constructor: parentV" << std::endl; };
	virtual ~ParentV() { std::cout << "destructor: parentV" << std::endl; };
};

class ChildV : public ParentV {
public:
	int c;
	//�����ڴ� virtual �ȵ˴ϴ� Human.
	ChildV(int p_, int c_) : ParentV(p_), c(c_) { std::cout << "constructor: childV" << std::endl; };
	virtual ~ChildV() { std::cout << "destructor: childV" << std::endl; };
};


int main2() {
	Parent *p_ = new Child(1, 2);
	ParentV*pv = new ChildV(1, 2);
	delete p_;		// virtual destructor �ƴϸ� child destructor ���� �ȵ�
	delete pv;
	return 0;
}


