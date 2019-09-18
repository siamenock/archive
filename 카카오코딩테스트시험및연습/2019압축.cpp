#include <string>
#include <vector>

using namespace std;

int solution(string s) {
	int max_compress = 0;
	for (int tok_len = 1; tok_len <= s.length() / 2; tok_len++) {
		string tok = "";
		int repeat = 0;
		int compress = 0;
		for (int i = 0; i < s.length(); i += tok_len) {

			if (s.compare(i, tok_len, tok) == 0) {	// keep repeat
				repeat++;
			}
			else { // repeat down!
				if (1 < repeat) {
					compress += (repeat - 1) * tok_len;	// remove pattern
					while (repeat) {
						compress--;						// number
						repeat /= 10;					// 몇자리수?
					}
				}
				repeat = 0;
			}
			if (repeat == 0) {
				tok = s.substr(i, tok_len);
				repeat = 1;
				
			}


		}
		if (1 < repeat) {
			compress += (repeat - 1) * tok_len;	// remove pattern
			while (repeat) {
				compress--;						// number
				repeat /= 10;					// 몇자리수?
			}
		}
		


		if (max_compress < compress) {
			max_compress = compress;
		}
	}

	return s.length() - max_compress;
}


//#define DEBUG
#ifdef DEBUG



int main() {
	string input_arr[] = {
		"aabbaccc",
		"ababcdcdababcdcd",
		"abcabcdede",
		"abcabcabcabcdededededede",
		"abcabcabcabcdededededede"
	};

	for (string& var : input_arr) {
		int result = solution(var);
		result--;
	}
}

#endif // DEBUG