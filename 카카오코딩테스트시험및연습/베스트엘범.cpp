#include <string>
#include <vector>
#include <map>
#include <algorithm>


using namespace std;

static enum {
	ID0 = 0,	// best   elbum in that genre
	ID1,		// second 
	PLAY0,
	PLAY1,
	TOTAL_PLAY,
	_VECTOR_SIZE_
};


// 역순정렬
bool compare_total_play_desc(vector<int> const& a, vector<int> const& b) {
	return a[TOTAL_PLAY] > b[TOTAL_PLAY];
}


vector<int> solution(vector<string> genres, vector<int> plays) {
	map<string, vector<int>> data;

	vector<int> default_data(_VECTOR_SIZE_, -1);
	default_data[TOTAL_PLAY] = 0;

	for (int i = 0; i < genres.size(); i++) {
		if (data.find(genres[i]) == data.end())
			data[genres[i]] = default_data;

		
		vector<int>& data_genre = data[genres[i]];

		data_genre[TOTAL_PLAY] += plays[i];

		if (data_genre[PLAY1] < plays[i]) {
			data_genre[ID1]		= i;
			data_genre[PLAY1]	= plays[i];

			if (data_genre[PLAY0] < data_genre[PLAY1]) {
				swap(data_genre[ID0  ], data_genre[ID1  ]);
				swap(data_genre[PLAY0], data_genre[PLAY1]);
			}
		}
		
	}

	vector<vector<int>> data_genre;
	for (auto it = data.begin(); it != data.end(); it++) {
		data_genre.push_back(it->second);
	}
	//data.clear();

	sort(data_genre.begin(), data_genre.end(), compare_total_play_desc);
	
	vector<int> answer;
	for (auto& var : data_genre) {
		if (var[ID0] != -1) answer.push_back(var[ID0]);
		if (var[ID1] != -1) answer.push_back(var[ID1]);
	}

	
	return answer;
}