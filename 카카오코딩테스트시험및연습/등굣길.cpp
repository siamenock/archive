// 메모리	O(n +      puddle )
// 시간		O(mn * log(puddle))
// 왜 에러나지

#include <vector>
#include <set>
using namespace std;



int solution(int m, int n, vector<vector<int>> puddles) {
	vector<int> line;
	set<pair<int, int>> puddle_set;
	

	line.resize(n, 0);
	
	m--;	// 사이즈 그대로인게 저 위쪽에선 편하긴 한데
	n--;	// 왜 (1,1)을 시작으로 잡은거임 대체?

	// set에 하나씩 입력시키면 실행시간 길어진다. 테스트에서 걸리더라....
	pair<int, int>* puddle_arr = new pair<int,int>[puddles.size()];
	for (int i = 0; i < puddles.size(); i++) {
		auto& var = puddles[i];
		puddle_arr[i] = pair<int, int>(var[0] - 1, var[1] - 1);	// 왜 (1,1)을 시작으로 잡은거임 대체?
	}
	puddle_set.insert(puddle_arr, puddle_arr + puddles.size());
	delete[] puddle_arr;


	
	int i; // for m
	int j; // for n
	line[0] = 1;
	for (i = 0; i <= m; i++) {
		for (j = 1; j <= n; j++) {
			// if puddle    set 0
			if (puddle_set.end() != puddle_set.find(pair<int, int>(i, j))) {
				line[j] = 0;
				continue;
			}

			line[j] += line[j - 1];
			line[j] %= 1000000007;
			// line[j]   위쪽 숫자
			// line[j-1] 왼쪽 숫자
		}
	}

	return line[n];
}


int 등굣길() {
	int m = 4;
	int n = 3;
	vector<vector<int>> puddles;
	puddles.push_back(vector<int>(2, 2));
	int ret = solution(m, n, puddles);
	return ret;
}