#include <vector>
#include <algorithm>

//#define DEBUG_MAIN
using namespace std;

int solution(vector<int> people, int limit) {
	sort(people.begin(), people.end());
	
	int boat = 0;
	int light = 0;
	int heavy = people.size() - 1;

	while ( light < heavy) {
		if (people[light] + people[heavy] <= limit) {	// ���� ������ ����� ���� ���ſ� ����̶� �ռ� ����
			boat++;
			light++;
			heavy--;
		}
		if (people[light] + people[heavy] > limit) {	// ���ſ� ��� ����Ұ�
			boat++;
			heavy--;
		}
	}
	if (light == heavy) boat++;	// 1�� ����

	return boat;
}

#ifdef DEBUG_MAIN
int main() {
	vector<int> people;
	int result;
	{
		people.push_back(70);
		people.push_back(50);
		people.push_back(80);
		people.push_back(50);
		result = solution(people, 100);
		people.clear();
	} {
		people.push_back(70);
		people.push_back(50);
		people.push_back(80);
		result = solution(people, 100);
		people.clear();
	}
	
	return 1;
}
#endif