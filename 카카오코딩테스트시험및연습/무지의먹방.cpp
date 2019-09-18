#include <string>
#include <vector>
#include <algorithm>
#include <utility>

using namespace std;

// return <cycle, cur_sum>
pair<int, int> cycling(const vector<int>& food_times, const long long & k) {
	vector<int> sort_time = vector<int>(food_times);
	sort(sort_time.begin(), sort_time.end());


	int cycle = 0;
	int len = food_times.size();
	int sum = 0;
	int sum_last = 0;
	int index_min = 0;
	for (cycle = 0; sum <= k; cycle++) {
		if (cycle >= sort_time[index_min]) {
			len--;				// one of food is done.
			index_min++;		// next min value
			if (len == 0) {
				cycle ++;
				break;
			}
		}

		sum_last = sum;
		sum += len;
	}
	return pair<int, int>(cycle - 1, sum_last);
}


int solution(vector<int> food_times, long long k) {	
	pair<int, int> cycle_sum = cycling(food_times, k);
	int cycle = cycle_sum.first;
	int sum   = cycle_sum.second;

	

	int eat_more = k - sum;
	int eat;
	int i;
	for (eat = i = 0; eat < eat_more && i < food_times.size(); i++) {
		if (food_times[i] <= cycle) {	// already eat all
			continue;
		}
		else {							// yet eating
			eat++;
		}
	}
	
	while(i < food_times.size() && food_times[i] <= cycle) {		// move i to next eatable food
		i++;
	}
	

	if (eat < eat_more        ) return -1;
	if (i == food_times.size()) i = 0;	// if full cycle
	return i + 1;	// index start from 1 in this problem
}




#include <iostream>

int muzi_main() {
	int arr[] = {3,1,2,4};
	int k = 4;
	vector<int> food_times(arr, arr + sizeof(arr)/sizeof(int));


	int ret = 1;
	for (k = 0; ret != -1; k++) {
		ret = solution(food_times, k);
		cout << "k = " << k << "\t=>" << ret << endl;
	}

	
	return 0;
}