
#include <string>
#include <vector>
#include <algorithm>
using namespace std;

class Component {
	static vector<Component> data;	// ¾Æ¸ô¶û ÃÖÀûÈ­ ¾ÈÇØ

	int type;
	pair<int, int> pos;

	Component(int type_, pair<int, int> pos_) : type(type_), pos(pos_) {}		// ¾Æ¸ô¶û »ó¼Ó¾ÈÇØ

	vector<pair<int, int>> make_safe() {
		vector<pair<int, int>> ret;
		switch (type)
		{
		case PILLAR:
			ret.push_back(pair<int, int>(pos.first, 1 + pos.second));
			break;
		case FLOOR:
			ret.push_back(pair<int, int>(pos.first, pos.second));
			ret.push_back(pair<int, int>(pos.first + 1, pos.second));
			break;

		default:
			break;
		}
		return ret;
	}

	vector<pair<int, int>> need_safe() {
		vector<pair<int, int>> ret;
		switch (type)
		{
		case FLOOR:
			ret.push_back(pair<int, int>(pos.first + 1, pos.second));
		case PILLAR:
			ret.push_back(pair<int, int>(pos.first, pos.second));
			break;

		default:
			break;
		}
		return ret;
	}

	static bool compare(Component const& a, Component const& b) {
		if (a.pos.first  != b.pos.first ) return a.pos.first  < b.pos.first ;
		if (a.pos.second != b.pos.second) return a.pos.second < b.pos.second;
		return a.type < b.type;
	}


public:
	enum { PILLAR = 0, FLOOR = 1 };

	static void InitGround(int size) {			
		for (int i = 0; i < size; i++) {		// ¾Æ¸ô¶û È¿À² ¹ö·Á
			auto c = Component(FLOOR, pair<int, int>(i, 0));
			data.push_back(c);		
		}
	}

	static bool build(int type_, pair<int, int> pos_) {
		auto c_new = Component(type_, pos_);
		auto safe_need = c_new.need_safe();
		for (auto& c_old : data) {
			auto safe = c_old.make_safe();
			if ((c_new.type == FLOOR && find(safe.begin(), safe.end(), safe_need[1]) != safe.end())
				||						find(safe.begin(), safe.end(), safe_need[0]) != safe.end()
				) {
				data.push_back(c_new);
				return true;
			}
		}
	}
	static bool destroy(int type_, pair<int, int> pos_) {
		auto c = Component(type_, pos_);
		// todo
		return false;
	}


	static vector<vector<int>> print(){
		sort(data.begin(), data.end(), compare);
		vector<vector<int>> ret;
		for (auto& d : data) {
			vector<int> v;
			v.push_back(d.pos.first);
			v.push_back(d.pos.second);
			v.push_back(d.type);
			ret.push_back(v);
		}
		return ret;
	}
	
};


// Á¤ÀÇ
vector<Component> Component::data;


vector<vector<int>> solution(int n, vector<vector<int>> build_frame) {
	Component::InitGround(n);
	for (auto& build : build_frame) {
		if(build[3] == 1)	Component::build  (build[2], pair<int, int>(build[0], build[1]));
		if(build[3] == 0)	Component::destroy(build[2], pair<int, int>(build[0], build[1]));
	}
	return Component::print();
}


int main() {
	vector<vector<int> > input;
	int darr[8][4] = {
		{1, 0, 0, 1},
		{1, 1, 1, 1}, 
		{2, 1, 0, 1},
		{2, 2, 1, 1},
		{5, 0, 0, 1},
		{5, 1, 0, 1}, 
		{4, 2, 1, 1},
		{3, 2, 1, 1}
	};
	for (int i = 0; i < 8; i++) {
		input.push_back(vector<int>(darr[i], darr[i] + 4));
	}
	auto ret = solution(5, input);
	return 0;
}
