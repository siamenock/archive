import numpy as np

print("ㅁ a = np.arange(2,27)")
a = np.arange(2,27)
print (a)

print("");
print("ㅁ a = a.reshape(5,5)")
a = a.reshape(5,5)
print (a)

print("");
print("ㅁ a[1:4, 1:4] = 0")
a[1:4, 1:4] = 0
print(a)

print("");
print("ㅁ a = a@a")
a = a@a
print(a)

print("");
print("ㅁ magnitude of a's first line")

sum = 0;
line = a[0]
for val in line:
    sum += (val*val);

print(np.sqrt(sum))

