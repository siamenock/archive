long long solution(int N) {
	long long x = 1, y = 1;	
	for (int n = 2; n <= N; n++) {
		if (n % 2)	x += y;
		else		y += x;
	}
	return 2*(x+y);
}