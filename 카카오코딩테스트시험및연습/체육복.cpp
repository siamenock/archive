// time	O(n)
// mem	O(n)
#include <vector>

using namespace std;

int solution(int n, vector<int> lost, vector<int> reserve) {
	vector<int> count(n, 1);

	for (int i : lost)		{ count[i-1]--; }
	for (int i : reserve)	{ count[i-1]++; }
	
	int i, ret = 0;
	for (i = 0; i < n -1; i++) {
		if (count[i] == 0 && count[i + 1] == 2)		{ count[i] = count[i + 1] = 1; }
		if (count[i] == 2 && count[i + 1] == 0)		{ count[i] = count[i + 1] = 1; }
	}
	for (i = 0; i < n; i++) {
		if (1 <= count[i]) ret++; 
	}

	return ret;
}

int Ã¼À°º¹() {
	vector<int> lost, reserve;
	{
		int n = 5;
		lost.clear();
		lost.push_back(2);
		lost.push_back(4);

		reserve.clear();
		reserve.push_back(1);
		reserve.push_back(3);
		reserve.push_back(5);

		int result = solution(n, lost, reserve);
		result = 0;
	}
	return 0;
}