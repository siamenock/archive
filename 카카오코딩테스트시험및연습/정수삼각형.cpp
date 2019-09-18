#include <algorithm>
#include <vector>

using namespace std;

static vector<vector<int>> *g_tri;

int get_max_parent(int h, int i) {
	if (i == 0) return (*g_tri)[h - 1][i];
	if (i == h) return (*g_tri)[h - 1][i - 1];
	else		return max((*g_tri)[h - 1][i], (*g_tri)[h - 1][i - 1]);
}

int solution(vector<vector<int>> triangle) {
	g_tri = &triangle;
	
	for (int h = 1; h < triangle.size(); h++) {
		for (int i = 0; i <= h; i++) {
			(*g_tri)[h][i] += get_max_parent(h, i);
		}
	}
	
	int h	= (*g_tri).size() - 1;
	int max	= (*g_tri)[0][0];
	for (int i = 0; i <= h; i++) {
		if (max < (*g_tri)[h][i]) {
			max = (*g_tri)[h][i];
		}
	}
	return max;
}

int Á¤¼ö»ï°¢Çü() {

	vector<vector<int>> input;
	for (int h = 0; h < 3; h++) {
		vector<int> vec;
		for (int i = 0; i < h+1; i++) {
			vec.push_back(i+1);
		}
		input.push_back(vec);
	}

	int ret = solution(input);
	return ret;
}


