// time	O(cost + n*n) = O(n^2)		// 트리구조 사용해 최적화하면 nlogn 까지 가능
// mem	O(n) + input_size

#include <algorithm>
#include <vector>


using namespace std;




bool compare_cost(vector<int> const& a, vector<int> const& b) {	
	return a[2] < b[2];
}



int solution(int n, vector<vector<int>> costs) {
	int answer = 0;
	sort(costs.begin(), costs.end(), compare_cost);			// ascending order
	
	vector<int>group(costs.size(), 0);
	
	for (int i = 1; i < costs.size(); i++) {		// each node belong to itself only at 1st
		group[i] = i;
	}

	for (auto& cost: costs) {	// from min -> max cost
		int& node0  = cost[0];
		int& node1  = cost[1];
		int& group0 = group[node0];
		int& group1 = group[node1];
		
		if (group0 == group1) continue;		// already in same group. do noting

		answer += cost[2];					// build bridge
		
		for (auto& belong_to : group) {		// merge group1 into group0
			if (belong_to == group1) {
				belong_to = group0;
			}
		}
	}
	return answer;
}



