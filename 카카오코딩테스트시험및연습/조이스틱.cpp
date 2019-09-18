// time	O(len)
// mem	O(1)
#include <string>
#include <algorithm>
#include <vector>
using namespace std;

int min_move_horizontal(const string name) {
	const int LEN = name.length();
	vector<pair<int, int>> vec_blank;
	int start, end; // start & end pos of 'AA...AA' pattern
	
	// extract 'AA...AA' pattern data in "[start,end)" form    // 이상,미만
	for (start = 0; start < LEN; start++) {
		for (end = start; name[end] == 'A'; end++) {
			if (end == LEN)
				break;
		}
		if (start == end) continue;

		vec_blank.push_back(pair<int, int>(start, end));
	}

	// find best move cost
	int min_move = LEN -1; // if no AA pattern
	for (auto& var : vec_blank) {
		int front = var.first;			// 전방의 A 아닌 패턴 길이
		int back  = LEN - var.second;	// 후방의 A 아닌 패턴 길이

		if (!front && !back) return 0;	// A only string

		if (front) front--;		// 이동거리
		if (back ) back --;		// 이동거리

		int repeat = min(front, back);
		int new_move = front + back + 1 + repeat;

		if (new_move < min_move) min_move = new_move;
	}
	return min_move;
}

int min_move_vertical(char target) {
	int gap = target - 'A';
	return min(gap, 26 - gap);
}
int solution(string name) {
	const int LEN = name.length();
	int cur_pos = 0;

	int total = min_move_horizontal(name);
	for (int i = 0; i < LEN; i++) {
		total += min_move_vertical(name[i]);
	}
	return total;
}

int 조이스틱() {
	int ret = solution("JAN");
	return ret;
}