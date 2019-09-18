#include <string>
#include <vector>

using namespace std;

bool solution(vector<string> phone_book) {
	const int SIZE_P = phone_book.size();
	const int SIZE_K = phone_book[0].size();
	int i, j;
	for (i = 1; i < SIZE_P; i++) {
		for (j = 0; j < SIZE_K; j++) {
			if (phone_book[0][j] != phone_book[i][j])
				break;
		}
		if (j == SIZE_K) return false;
	}

	return true;
}