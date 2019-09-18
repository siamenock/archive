// time	O(n)
// mem	O(1)

#include <algorithm>
#include <vector>

#define MAX_VAL 30000
using namespace std;

bool compare_begin(vector<int> const& a, vector<int> const& b) {
	return a[0] < b[0];
}

int solution(vector<vector<int>> routes) {
	sort(routes.begin(), routes.end(), compare_begin);
		
	int min_out = MAX_VAL;		// min out point of cars already come in but never meet CCTV

	int camera = 0;
	for (auto& var: routes) {
		int new_in  = var[0];	// new car incomming point
		int new_out = var[1];

		if (min_out < new_in) {
			camera++;
			min_out = MAX_VAL;
		}
		
		if (min_out > new_out) {
			min_out = new_out;
		}
	}
	if (min_out < MAX_VAL) {	// some cars left
		camera++;
	}
	return camera;
}

//#define DEBUG_MAIN
#ifdef DEBUG_MAIN
#include <iostream>
int main() {

	
	vector<vector<int>> routes(
		4, vector<int>(2, 0)
	);
	
	routes[0][0] = -20;
	routes[0][1] = +15;

	routes[1][0] = -14;
	routes[1][1] = -05;

	routes[2][0] = -18;
	routes[2][1] = -13;

	routes[3][0] = -05;
	routes[3][1] = +03;
	

	solution(routes);
}

#endif // DEBUG_MAIN
