// �ð�	O(len * k)
// ����	O(k)			// input������ ����
#include <string>
using namespace std;

int find_max(string const& num, int start, int back_border) {
	const int END = num.length() - back_border;
	int i_max = start;
	for (int i = start + 1; i < END; i++) {
		if (num[i_max] < num[i])	i_max = i;
	}
	return i_max;
}



string solution(string num, int k) {
	k = num.length() - k;	// k�� ���Ű� �ƴ϶� k�� ����� �����ϰ���
	string ret = "";
	int start = 0, i_max;
	for (; 0 < k; k--) {
		i_max = find_max(num, start, k - 1);
		ret += num[i_max];
		start = i_max + 1;
	}
	return ret;
}


#ifdef DEBUG
int main() {
	string result;
	result = solution("1924", 2);


	return 1;
}
#endif