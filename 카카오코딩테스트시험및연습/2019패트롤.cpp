#include <string>
#include <vector>
#include <algorithm>

using namespace std;


#define MAX_WEAK 15			// 문제 조건에 따라 15개가 최대 (weak 갯수)
#define FAIL	MAX_WEAK+1	// 여기까지 올라가면 망한거

void solution_fast(int const& n, int const cur_friend_inserted, vector<int> & weak, vector<int> & dist, int& cur_min) {

	if (cur_friend_inserted >= cur_min)
		return;
	if (weak.size() == 0) {
		if (cur_min > cur_friend_inserted)
			cur_min = cur_friend_inserted;
		return;
	}
	if (dist.size() == 0)
		return;
	
	
	
	int i, j, remove_dist;
	for (i = dist.size() - 1; 0 <= i; i--) {
		if (dist.size() == 4) {	// debug
			j = 0;
		}

		int cur_dist = dist[i];					// 투입(삭제)할 친구
		int limit	 = weak[0] + cur_dist;		// 아몰랑 무식하게 1번부터 집어넣어. 최적화 신경끔

		for (j = 0; j < weak.size(); j++) {		
			if (weak[j] > limit) break;
		}
		auto it = weak.begin();
		vector<int> weak_removed(it, it + j);	// 임시로 삭제할 데이터 저장
		dist.erase(dist.begin() + i);			// 삭제
		weak = vector<int>(it + j, weak.end());	// 삭제

		solution_fast(n, cur_friend_inserted + 1, weak, dist, cur_min);	// 파싱

		dist.insert(dist.begin() + i, cur_dist);							//복구
		weak.insert(weak.begin(), weak_removed.begin(), weak_removed.end());//복구

	}
}


int solution(int n, vector<int> weak, vector<int> dist) {
	int cur_min = FAIL;

	solution_fast(n, 0, weak, dist, cur_min);

	for (int i = 1; i < weak.size(); i++) {
		vector<int> weak_rotated = weak;
		for (auto& var : weak_rotated) { var = (n + var - weak[i]) % n; }
		sort(weak_rotated.begin(), weak_rotated.end());	// 더 효율적으로 할 방법은 있지만 귀찮습니다

		solution_fast(n, 0, weak_rotated, dist, cur_min);
	}	
	
	if (cur_min == FAIL)	return -1;
	else					return cur_min;
}


#ifdef DEBUG




int main() {
	vector<int> weak;
	vector<int> dist;
	int result;
	int _ = 0;


	{
		int n = 12;
		int a[] = { 1, 5, 6, 10 };
		int b[] = { 1, 2, 3, 4 };
		weak = vector<int>(a, a + (sizeof(a) / sizeof(int)));
		dist = vector<int>(b, b + (sizeof(b) / sizeof(int)));
		result = solution(n, weak, dist);
		_++;
	}
	{
		int n = 12;
		int a[] = { 1, 3, 4, 9, 10 };
		int b[] = { 3, 5, 7 };
		weak = vector<int>(a, a + (sizeof(a) / sizeof(int)));
		dist = vector<int>(b, b + (sizeof(b) / sizeof(int)));
		result = solution(n, weak, dist);
		_++;
	}

}
#endif // DEBUG