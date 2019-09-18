#include <vector>
#include <algorithm>

using namespace std;


bool try_match(int more_need, int index_limit, vector<int> const& weight) {
	for (int i = index_limit; 0 <= i; i--) {
		int more_need_new = more_need - weight[i];
		
		if (more_need_new <  0) continue;
		if (more_need_new == 0) return true;
		if (try_match(more_need_new, i - 1, weight)) return true;
	}
	return false;
}



int solution(vector<int> weight) {
	int answer;
	int more_need;
	sort(weight.begin(), weight.end());
	

	const int SIZE = weight.size();

	for (answer = 1; try_match(answer, SIZE -1, weight); answer++) { continue; }

	return answer;
}


int main() {
	

	return 0;
}