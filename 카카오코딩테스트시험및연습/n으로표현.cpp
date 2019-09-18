#include <vector>


using namespace std;


class Nxp {	// Num or expression
private:
	// data[cost] is    vector<Nxp> of all possible combination in that cost
	static vector<vector<Nxp>> data;
	static int N;

	int val;
	bool isNum = false;


	Nxp(int val, bool isNum) {
		this->val	= val;
		this->isNum = isNum;
	}

	static void Init(int N) {
		Nxp::N = N;
		Nxp first_data(N, true);
		vector<Nxp> vec0cost;
		vector<Nxp> vec1cost;
		vec1cost.push_back(first_data);
		data.clear();
		data.push_back(vec0cost);
		data.push_back(vec1cost);
	}

	// try finding and dynamic bottom-up
	static bool TryFinding(int cost, int val) {
		int cost0, cost1;
		vector<Nxp> ret_temp;
		vector<Nxp> ret;
		for (cost0 = 1; cost0 < (cost + 1) / 2; cost0++) {
			cost1 = cost - cost0;

			ret_temp = combination(cost0, cost1);
			if(TryFindingAndMerge(ret, ret_temp, val))	return true;

		}
		ret_temp = num_increment(cost -1);
		if (TryFindingAndMerge(ret, ret_temp, val))		return true;

		data.push_back(ret);

		return false;
	}

	// find value in ret_temp.
	// if not exists merge into ret
	// if     exists return cost
	static bool TryFindingAndMerge(vector<Nxp>& ret, vector<Nxp>& ret_temp, const int& val) {
		for (int i = 0; i < ret_temp.size(); i++) {
			if (val == ret_temp[i].val)
				return true;
		}

		ret.reserve(ret.size() + ret_temp.size());
		ret.insert(ret.end(), ret_temp.begin(), ret_temp.end());
		
		return false;
	}

	// all combination of cost0 Nxp and cost1 Nxp
	static vector<Nxp> combination(int cost0, int cost1) {
		const vector<Nxp>& data0 = data[cost0];
		const vector<Nxp>& data1 = data[cost1];
		vector<Nxp> ret;

		for (int i0 = 0; i0 < data0.size(); i0++) {
			for (int i1 = 0; i1 < data1.size(); i1++) {
				const int val0 = data0[i0].val;
				const int val1 = data1[i1].val;

				ret.push_back(Nxp(val0 + val1, false));
				ret.push_back(Nxp(val0 * val1, false));
				ret.push_back(Nxp(val0 - val1, false));
				ret.push_back(Nxp(val1 - val0, false));
				
				if (val1) {	ret.push_back(Nxp(val0 / val1, false)); }
				if (val0) {	ret.push_back(Nxp(val1 / val0, false));	}
			}
		}
		return ret;
	}

	// allowed  : 5		-> 55
	// not allow: (5+5)	-> (5+5)5
	static vector<Nxp> num_increment(int cost) {
		vector<Nxp> ret;
		const vector<Nxp>& data0 = data[cost];
		for (auto& var: data0)
		{
			if (var.isNum) {
				ret.push_back(Nxp(var.val * 10 + Nxp::N, true));
			}
		}
		return ret;
	}


public:
	static int Find(int N, int val) {
		bool success = false;
		Init(N);
		int cost = 1;
		while(! success && cost <= 8){
			cost++;
			success = TryFinding(cost, val);
		}
		if (success)	return cost;
		else			return -1;
	}
};

vector<vector<Nxp>> Nxp::data;
int Nxp::N;


int solution(int N, int number) {
	return Nxp::Find(N, number);

}



int N으로표현() {
	int ret = solution(1, 12);
	return 0;
}