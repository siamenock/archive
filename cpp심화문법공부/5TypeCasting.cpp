#include <iostream>
#include <vector>
using namespace std;


class Parent {
public:
	int p;
	Parent(int p_) : p(p_) { std::cout << "constructor: parent" << std::endl; };
	~Parent() { std::cout << "destructor: parent" << std::endl; };
};

class Child0 : public Parent {
public:
	int c;
	char c0Char;
	Child0(int p_, int c_, char c0Char_) : Parent(p_), c(c_), c0Char(c0Char_) { std::cout << "constructor: child0" << std::endl; };
	~Child0() { std::cout << "destructor: child" << std::endl; };
};
class Child1 : public Parent {
public:
	int c1Int;		// 순서가 다름!
	int c;
	Child1(int p_, int c_, int c1Int_) : Parent(p_), c(c_), c1Int(c1Int_) { std::cout << "constructor: child1" << std::endl; };
	~Child1() { std::cout << "destructor: child" << std::endl; };
};


class ParentV {
public:
	int p;
	ParentV(int p_) : p(p_) { std::cout << "constructor: parentV" << std::endl; };
	virtual ~ParentV() { std::cout << "destructor: parentV" << std::endl; };
};

class Child0V : public ParentV {
public:
	int c;
	char c0Char;
	Child0V(int p_, int c_, char c0Char_) : ParentV(p_), c(c_), c0Char(c0Char_) { std::cout << "constructor: child0V" << std::endl; };
	virtual ~Child0V() { std::cout << "destructor: child0V" << std::endl; };
};
class Child1V : public ParentV {
public:
	int c1Int;		// 순서가 다름!
	int c;
	Child1V(int p_, int c_, int c1Int_) : ParentV(p_), c(c_), c1Int(c1Int_) { std::cout << "constructor: child1V" << std::endl; };
	virtual ~Child1V() { std::cout << "destructor: child1V" << std::endl; };
};


int main() {
	// 기본동작 실험
	{
		Parent * p_ = new Parent(1);
		Child0* c0 = new Child0(2, 3, 'a');
		Child1* c1 = new Child1(4, 5, 6);

		ParentV* pv = new ParentV(7);
		Child0V* c0v = new Child0V(8, 9, 'b');
		Child1V* c1v = new Child1V(10, 11, 12);

		int i = 13;
		float f = 14.0f;



		auto p01 = static_cast<Parent*>(c0);
		auto p02 = dynamic_cast<Parent*>(c0);
		auto p03 = (Parent*)c0;

		auto p04 = static_cast<ParentV*>(c0v);
		auto p05 = dynamic_cast<ParentV*>(c0v);
		auto p06 = (ParentV*)c0v;

		auto p07 = static_cast<Child0*>(p_);	// trash
		//auto p08 = dynamic_cast<Child0*>(p_);	// 컴파일 에러
		auto p09 = (Child0*)p_;					// trash

		auto p10 = static_cast<Child0V*>(pv);	// trash값
		auto p11 = dynamic_cast<Child0V*>(pv);	// return NULL
		auto p12 = (Child0V*)pv;

		auto p13 = static_cast<float>(i);
		auto p14 = static_cast<int>(f);
		//auto p13 = dynamic_cast<float>(i);	// 다이나믹은 포인터만 다룸
		//auto p14 = dynamic_cast<int>(f);		// 다이나믹은 포인터ㅏㄴ 다
		auto p15 = (float)(i);
		auto p16 = (int)(f);

		//auto p17 = static_cast<Child0*>(c1);	// 컴파일 에러
		//auto p18 = dynamic_cast<Child0*>(c1);	// 컴파일 에러
		auto p19 = (Child0*)(c1);				// really unsafe

		//auto p20 = static_cast<Child0V*>(c1v);// 컴파일 에러
		auto p21 = dynamic_cast<Child0V*>(c1v);	// NULL
		auto p22 = (Child0V*)(c1v);				// really unsafe

	}
	// 실전적 실험
	{
		vector<Parent* > p_Vector;
		vector<ParentV*> pvVector;

		p_Vector.push_back(new Parent (1));			//[0]
		pvVector.push_back(new ParentV(1));
		p_Vector.push_back(new Child0 (1, 2, '3'));	//[1]
		pvVector.push_back(new Child0V(1, 2, '3'));
		p_Vector.push_back(new Child1 (1, 2, 3));	//[2]
		pvVector.push_back(new Child1V(1, 2, 3));

		cout << "with static cast," << endl;		// 3x2개 다됨
		for (int i = 0; i < pvVector.size(); i++) {
			auto p_ = p_Vector[i];
			auto pv = pvVector[i];

			if (auto p = static_cast<Child0* >(p_)) cout << "[" << i << "]:" << "Child0  possible" << endl;
			if (auto p = static_cast<Child1* >(p_)) cout << "[" << i << "]:" << "Child1  possible" << endl;
			if (auto p = static_cast<Child0V*>(pv)) cout << "[" << i << "]:" << "Child0V possible" << endl;
			if (auto p = static_cast<Child1V*>(pv)) cout << "[" << i << "]:" << "Child1V possible" << endl;

		}
		cout << "with dynamic cast," << endl;		// 정확하게 타입구분 가능
		for (int i = 0; i < pvVector.size(); i++) {
			auto p_ = p_Vector[i];
			auto pv = pvVector[i];
			// 컴파일 에러
			/*
			if (auto p = dynamic_cast<Child0*>(p_)) cout << "[" << i << "]:" << "Child0  possible" << endl;
			if (auto p = dynamic_cast<Child1*>(p_)) cout << "[" << i << "]:" << "Child1  possible" << endl; */
			if (auto p = dynamic_cast<Child0V*>(pv)) cout << "[" << i << "]:" << "Child0V possible" << endl;
			if (auto p = dynamic_cast<Child1V*>(pv)) cout << "[" << i << "]:" << "Child1V possible" << endl;

		}
		cout << "with regular cast," << endl;		// 3x2개 다됨
		for (int i = 0; i < pvVector.size(); i++) {
			auto p_ = p_Vector[i];
			auto pv = pvVector[i];
			// 컴파일 에러
			
			if (auto p = (Child0* )(p_)) cout << "[" << i << "]:" << "Child0  possible" << endl;
			if (auto p = (Child1* )(p_)) cout << "[" << i << "]:" << "Child1  possible" << endl;
			
			if (auto p = (Child0V*)(pv)) cout << "[" << i << "]:" << "Child0V possible" << endl;
			if (auto p = (Child1V*)(pv)) cout << "[" << i << "]:" << "Child1V possible" << endl;

		}
	}

	return 0;
}
