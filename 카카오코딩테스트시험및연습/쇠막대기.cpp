#include <string>
#include <vector>

using namespace std;

int solution(string arrangement) {
	int answer = 0;
	int height = 0;

	for (int i = 0; i < arrangement.size(); i++) {
		if (arrangement[i] == ')') {		// end of iron
			height--;
			answer++;
		}
		else if (arrangement[i] == '(') {
			if (arrangement[i+1]== ')') {	// laser
				i++;
				answer += height;
			}
			else {							// new iron
				height++;
			}
		}
	}
	return answer;
}