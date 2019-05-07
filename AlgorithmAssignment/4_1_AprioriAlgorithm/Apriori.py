import sys
import time
#---------------------------------------------
# 지금은 사실상 무의미한 클래스. 
# 처음 만들 땐 스캔할때마다 file을 새로 한번씩 읽을 생각이엇음.
# 그냥 한줄 읽어서 return frozenset 하는거 외에 기능 없음.
#---------------------------------------------
class FileReader:
    def __init__(self, fileName_):
        self.fileName = fileName_
        self.pFile = open(fileName_, 'r')
        return

    #def Reset(self):
    #    self.pFile = open(self.fileName, 'r')
    #    return

    # 한줄 읽어서 return set([item list]).
    # 파일 끝났으면 return None
    def ReadItemSet(self):
        line = self.pFile.readline()
        if line == "":
            self.pFile.close()
            return None
        return frozenset([int(i) for i in line.split('\t')])

    
#---------------------------------------------
# Apriori Class: How to use
#   a = Apriori(min, readFile)
#   a.ExtractFreqPattern()
#   a.PrintResult(outputFile)
#---------------------------------------------
class Apriori:
    # public
    # Apriori(minSupportPercent, fileName) 에서 
    #       파일을 읽어서 데이터를 메모리로 올리는 작업까지 진행함.
    # self.totalCount   :itemSet의 총 개수
    #     .minSupportNum:minSupportPercent를 맞추기 위해 필요한 최소 support num
    #     .data         :[itemSet] 목록. 단, itemSet은 frozenset의 형태로 저장된다.
    #     .freqPatterns :[{pattern:count}] 형태로 저장됨.
    #                    패턴 길이별로 서로 다른 집합에 저장되고,
    #                    len1부터 len max까지 순서대로 저장됨.
    def __init__(self, minSupportPercent, fileName:str):
        self.freqPatterns = []
        
        fData = FileReader(fileName)
        self.totalCount = 0
        self.data       = []

        while True :
            frozenSet = fData.ReadItemSet()
            if frozenSet == None:
                break
            self.data.append(frozenSet)
            self.totalCount += 1

        ## 전체 갯수 업데이트됨.
        self.minSupportNum  = self.totalCount * minSupportPercent / 100
        return 

    # public
    # 패턴 추출작업 진행
    def ExtractFreqPattern(self):
        self.freqPatterns = []
        patternNew = self.GetLen1Pattern()

        while 0 < len(patternNew):
            self.freqPatterns.append(patternNew)
            patternNew = self.LenUpPattern(patternNew)
        return

    # public
    # 추출결과를 과제요구사항대로 출력함.
    def PrintResult(self, fileNameOutput:str):
        fOut = open(fileNameOutput, 'w')

        for level in range(len(self.freqPatterns) -1):                      # len0부터 시작해서 dictionary를 선택하고
            for small in self.freqPatterns[level]:                          # small을 그 dictionary 중에서 고르고
                for biggerLenPatternSet in self.freqPatterns[level+1 :]:    # small dictionary보다 큰 각각의 dictionary에서
                    for big in biggerLenPatternSet:                         # big을 골라내서
                        if Apriori.IsSubset(big, small):                   # 비교
                            intBig   = biggerLenPatternSet[big]
                            intSmall = self.freqPatterns[level][small]
                            intTotal = self.totalCount

                            big = big - small

                            s = str(set(small)) + '\t'
                            b = str(set(big))   + '\t' 
                            support     = "%.2f"%round(100 * intBig / intTotal, 2) + '\t'
                            confidence  = "%.2f"%round(100 * intBig / intSmall, 2)

                            fOut.write(s + b + support + confidence + "\n")
        return

    
    # private
    # return (small이 big의 부분집합인가?)
    def IsSubset(big:frozenset, small:frozenset):
        difference = small - big
        if len(difference) == 0:
            return True
        return False
    
    # private
    # 동일 len 패턴간 교집합len이 len-1인 경우 Apriori에서 서로 합성가능한 패턴으로 인식.
    # 합성불가능하다면 return None
    # 합성  가능하다면 두 아이템셋의 합집합을 return.
    def TryUnion(pattern0 : frozenset, pattern1 :frozenset ):
        if len(pattern0) != len(pattern1):
            return None

        unionSet = pattern0| pattern1
        if (len(pattern0)+1) != len(unionSet):
            return None
        return unionSet

    # private
    # self.minSupportNum 보다 작은 count의 패턴 제거
    def KillMinorPattern(self, patternNew : dict):
        return {k:v for k,v in patternNew.items() if v >= self.minSupportNum}

    # private
    # Len 1 짜리 패턴 추출용 함수.
    # Len1을 바탕으로 LenUpPAttern() 반복해서패턴들을 갱신함.
    # 처음에 공집합 패턴을 패턴목록에 추가한다음 LenUpOattern만 반복하면 됐는데...
    # 다소 불필요한 함수.
    def GetLen1Pattern(self):  
        patternNew= {}
        for itemSet in self.data:
            for item in itemSet:
                item = frozenset([item])
                if None == patternNew.get(item):
                    patternNew[item] = +1
                else :
                    patternNew[item] += 1

        # 솎아내기
        ret = self.KillMinorPattern(patternNew)
        return ret
        

    # private
    # 전 단계 패턴을 바탕으로 Apriori식 확장 패턴후보 정함.
    # 그 뒤 패턴후보들의 갯수를 모든 데이터를 읽으면서 전수조사
    def LenUpPattern(self, patternOld : dict):
        patternOld = list(patternOld.keys())
        patternNew = {}
        ## 신규패턴 생성
        for i in range (len(patternOld)):
            for j in range (i + 1, len(patternOld)):
                p = Apriori.TryUnion(patternOld[i], patternOld[j])
                if p != None and None == patternNew.get(p) :    
                    patternNew[p] = 0

        ## 신규패턴 발견횟수 전수조사
        for itemSet in self.data:
            for p in patternNew:
                if Apriori.IsSubset(itemSet, p):
                    patternNew[p] += 1
        ret = self.KillMinorPattern(patternNew)
        return ret

    


def main():
    minSupportPercent   = int(sys.argv[1])
    fileNameInput       = sys.argv[2]
    fileNameOutput      = sys.argv[3]
    
    #print(fileNameInput + " -> " + fileNameOutput + " with " + str(minSupportPercent) + "% min support")
    apriori = Apriori(minSupportPercent, fileNameInput)
    #timeStart = time.time()
    #print("\ttotal " + str(apriori.totalCount) + "\t, min support num " + str(apriori.minSupportNum  ))
    apriori.ExtractFreqPattern()
    #timeEnd = time.time()
    #print("time spent only on extracting : " + str(timeEnd - timeStart))
    apriori.PrintResult(fileNameOutput)
    return

main()

  
