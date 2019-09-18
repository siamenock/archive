// �޸�	O(n +      puddle )
// �ð�		O(mn * log(puddle))
// �� ��������

#include <vector>
#include <set>
using namespace std;



int solution(int m, int n, vector<vector<int>> puddles) {
	vector<int> line;
	set<pair<int, int>> puddle_set;
	

	line.resize(n, 0);
	
	m--;	// ������ �״���ΰ� �� ���ʿ��� ���ϱ� �ѵ�
	n--;	// �� (1,1)�� �������� �������� ��ü?

	// set�� �ϳ��� �Է½�Ű�� ����ð� �������. �׽�Ʈ���� �ɸ�����....
	pair<int, int>* puddle_arr = new pair<int,int>[puddles.size()];
	for (int i = 0; i < puddles.size(); i++) {
		auto& var = puddles[i];
		puddle_arr[i] = pair<int, int>(var[0] - 1, var[1] - 1);	// �� (1,1)�� �������� �������� ��ü?
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
			// line[j]   ���� ����
			// line[j-1] ���� ����
		}
	}

	return line[n];
}


int ���() {
	int m = 4;
	int n = 3;
	vector<vector<int>> puddles;
	puddles.push_back(vector<int>(2, 2));
	int ret = solution(m, n, puddles);
	return ret;
}