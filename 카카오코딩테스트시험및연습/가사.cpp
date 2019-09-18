#include <string>
#include <vector>
#include <algorithm>
using namespace std;

static enum { DEFAULT, LO, HI };
int find_pattern_pos(vector<string> const& words, string const& qurey, int lo_hi = DEFAULT, int defult_lo = 0) {
	int lo = defult_lo;
	int hi = words.size();

	int mid;

	while (lo < hi) {
		mid = (lo + hi) / 2;
		const int cmp = words[mid].compare(qurey);

		if (cmp == 0) return mid;
		if (cmp < 0)	lo = mid + 1;
		else			hi = mid;
	}

	// ¾Æ¸ô¶û ÀÌÁøÅ½»ö¿¡ LO, HI·Î ÀÌ»Ú°Ô Ã³¸®ÇÏ·Á´ø°Å °è¼Ó »¶³­´Ù. ´ëÃæ ¶«ÁúÇØ
	if (lo_hi == LO && 0 > words[mid].compare(qurey))	mid++;
	if (lo_hi == HI && 0 < words[mid].compare(qurey))	mid--;

	switch (lo_hi) {
	case LO:	return mid;
	case HI:	return mid;
	default:	return -1;
	}

}

int find_pattern_count(vector<string> const& words, string const& qurey) {
	const int len = qurey.length();
	const int new_len = qurey.find("?");
	string str_lo = qurey.substr(0, new_len);				// ab???? -> ab
	string str_hi = str_lo + string(len - new_len, 'z');	// ab???? -> abzzz		// ab < abzzz

	int lo = find_pattern_pos(words, str_lo, LO);
	int hi = find_pattern_pos(words, str_hi, HI, lo);



	int answer = 0;
	for (int i = lo; i <= hi; i++) {
		if (words[i].length() == len)
			answer++;
	}
	return answer;
}

vector<int> solution(vector<string> words, vector<string> queries) {
	vector<int> answer(queries.size(), 0);
	sort(words.begin(), words.end());


	for (int i = 0; i < queries.size(); i++) {
		if (queries[i][0] == '?') continue;
		answer[i] = find_pattern_count(words, queries[i]);
	}

	for (auto& var : words) { reverse(var.begin(), var.end()); }
	for (auto& var : queries) { reverse(var.begin(), var.end()); }

	sort(words.begin(), words.end());


	for (int i = 0; i < queries.size(); i++) {
		if (queries[i][0] == '?') continue;
		answer[i] = find_pattern_count(words, queries[i]);
	}

	return answer;
}


//#define DEBUG
#ifdef DEBUG

int main() {
	string a = "a";
	string aa = "aa";
	string b = "b";
	string _ = "_";
	int result;
	result = a.compare(aa);
	result = a.compare(b);
	result = a.compare(_);


	string words_arr[] = { "frodo", "front", "frost", "frozen", "frame", "kakao" };
	string quries_arr[] = { "fro??", "????o", "fr???", "fro???", "pro?" };

	vector<string> words(words_arr, words_arr + 6);
	vector<string> quries(quries_arr, quries_arr + 5);
	auto ret = solution(words, quries);
	return 0;
}
#endif // DEBUG

