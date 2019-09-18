#include <string>
#include <vector>
#include <map>

using namespace std;

int solution(vector<vector<string>> clothes) {
	map<string, int> data;
	for (auto & var : clothes) {
		string& item = var[0];
		string& category = var[1];

		if (data.find(category) == data.end()) {
			data[category] = 1;
		}
		else {
			data[category]++;
		}
	}

	int answer = 1;
	for (auto it = data.begin(); it != data.end(); ++it)
		answer *= (it->second + 1);
	
	answer--;		// pi( num+1 ) -1
	
	return answer;
}