import itertools
import math
import sys

"""
    gEntropyMode값 목록 및 성능테스트 결과
    0 : basic entropy      336/346
    1 : gain ratio entropy 330/346
    2 : gini               336/346      
    3 : gini ratio entropy 330/346
"""
gEntropyMode = 0


gDataTypeDic = {                                   
    "someAttribute"  : ["high", "low", "med"],    # example. will be reset. not need to be orederd.
    }
gAttriList = ["Buying", "Doors", "..."]       # example, will be reset
gLastAttri = "last_attribute_name"        # example. will be reset

def GetLastValIndex(val:str):
    valTupl = gDataTypeDic[gLastAttri]
    return valTupl.index(val)

def GetOrder(colNum : int, val:str):
    colName = gAttriList[colNum]
    valTupl = gDataTypeDic[colName]
    return valTupl.index(val)

def GetValue(colNum : int, valIndex:int):
    colName = gAttriList[colNum]
    valTupl = gDataTypeDic[colName]
    return valTupl[valIndex]

def RegisterDataType(cols, lastCol):
    global gDataTypeDic
    gDataTypeDic.clear()
    for col in cols:
        gDataTypeDic[col] = []
    gDataTypeDic[lastCol] = []
    return

def RegisterDataIfUnknown(vals, lastVal):
    global gDataTypeDic
    for i in range(len(vals)):
        val = vals[i]
        col = gAttriList[i]
        if not val in gDataTypeDic[col]:
            gDataTypeDic[col].append(val)
    if not lastVal in gDataTypeDic[gLastAttri]:
        gDataTypeDic[gLastAttri].append(lastVal)

    return

def SumLastCol(data:dict):
    sumVector = [0.0,] * len(gDataTypeDic[gLastAttri])
    for t in data.values():
        for i in range (len(t)):
            sumVector[i] += t[i]
    return sumVector

def Entropy(data: dict)->float:
    if gEntropyMode == 0:
        return BasicEntropy(data)
    elif gEntropyMode == 1:
        return BasicEntropy(data)
    elif gEntropyMode == 2:
        return Gini(data)
    elif gEntropyMode == 3:
        return Gini(data)
    else: # error
        print("gEntropyMode setting failure!")
        return 1.0    


def BasicEntropy(data : dict) -> float:
    sumVector = SumLastCol(data)
    sumVal = sum(sumVector)
    if sumVal == 0.0:
        return 1.0
    
    for i in range(len(sumVector)):
        sumVector[i] /= sumVal
    sumVal = 0.0
    for p in sumVector:
        if p == 0.0:
            continue
        sumVal += -(p * math.log(p))
    return sumVal

def Gini(data : dict) -> float:
    sumVector = SumLastCol(data)
    sumVal = sum(sumVector)
    if sumVal == 0.0:
        return 1.0
    
    for i in range(len(sumVector)):
        sumVector[i] /= sumVal
    sumVal = 1.0
    for p in sumVector:
        sumVal -= p*p
    return sumVal

def EntropyGain(dataParent:dict, dataChild0:dict, dataChild1:dict) -> float:    
    lp = len(dataParent)
    l0 = len(dataChild0)
    l1 = len(dataChild1)

    if lp != l0 + l1:
        return -1.0

    ep = Entropy(dataParent)
    e0 = Entropy(dataChild0)
    e1 = Entropy(dataChild1)

    l0 /= lp            # len -> ratio
    l1 /= lp
    g0 = (ep - e0) * l0 # gain0
    g1 = (ep - e1) * l1 # gain1    
    
    if l0 == 0.0:
        gr0 = 0.0
    else:
        gr0 = g0 * -math.log(l0)# gain Ratio 적용
    if l1 == 0.0:
        gr1 = 0.0
    else:
        gr1 = g1 * -math.log(l1)
    gain      = g0 + g1     # 기본방식 엔트로피
    gainRatio = gr0 + gr1
    
    if gEntropyMode == 0 or gEntropyMode == 2:
        return gain
    else:
        return gainRatio

#public class
class DecisionNode:
    def __init__(self, parentNode , dataDic:dict):      # parentNode :DecisionNode
        self.parent   = parentNode
        self.dataDic  = dataDic;
        self.filter   = None
        self.childYes = None        # if (filter.IsMatch(sample))
        self.childNo  = None        # else
        return

    #public function
    def BuildTree(self):
        e = Entropy(self.dataDic)
        if e == 0.0:
            return
        
        self.filter = Filter.GetBestFilter(self.dataDic)
        if(self.filter == None):
            return

        dic0, dic1 = self.filter.SplitData(self.dataDic)
        self.childYes = DecisionNode(self, dic0)
        self.childNo  = DecisionNode(self, dic1)
        self.childYes.BuildTree()
        self.childNo .BuildTree()
        return
    #public function
    def GuessWhat(self, attriTupl : tuple)->str:
        if self.filter == None:
            mySum = SumLastCol(self.dataDic)
            maxIndex = mySum.index(max(mySum))
            return gDataTypeDic[gLastAttri][maxIndex]

        if self.filter.IsMatch(attriTupl):
            return self.childYes.GuessWhat(attriTupl)
        else :
            return self.childNo .GuessWhat(attriTupl)
    #public function
    def Print(self, depth:int):
        if depth == 0:
            print (str(gDataTypeDic[gLastAttri]) + " is value appear")
            print("Decision tree is made as binary tree, brenching like this---------")
        print("|\t"*depth + str(SumLastCol(self.dataDic)) + "\tEntropy:" + str(round(Entropy(self.dataDic),2)))
        if self.filter == None:
            return

        print("|\t"*depth + "filter:'" + gAttriList[self.filter.colNum] + "' is " + str(self.filter.valList) + "\tE_Gain:" + str(round(EntropyGain(self.dataDic, self.childYes.dataDic, self.childNo.dataDic),2)))
        self.childYes.Print(depth + 1)
        self.childNo .Print(depth + 1)
        return

#private class
class Filter:
    def __init__(self, colNum:int, valIndexList:list):
        self.colNum = colNum
        self.valList= []
        for valIndex in valIndexList:
            self.valList.append(GetValue(colNum, valIndex))
        return 

    # return: bool
    def IsMatch(self, valTupl: tuple):
        if valTupl[self.colNum] in self.valList:
            return True
        else:
            return False
    
    # return: dataMatch, dataUnMatch
    def SplitData(self, data : dict):
        data0 = {}
        data1 = {}
        for key, val in data.items():
            if(self.IsMatch(key)):
                data0[key] = val       # todo:이부분 전부 다 문법체크좀 해보자. 인터넷 없이 하니 암걸리네
            else:
                data1[key] = val
        return data0,data1

    def GetBestFilter(data : dict):
        bestFilter = None
        bestGain   = 0.0           # goal of this function is finding bestFilter of bestGain

        vecLen = len(gAttriList)
        colsAble = list(range(len(gAttriList)))
        
        for colIndex in range(len(gAttriList)):      # 데이터를 절반으로 나눌 수 있는 모든 조합을 다 해보려고 3중for
            col = gAttriList[colIndex]
            valTypes = gDataTypeDic[col]
            typeCount= len(valTypes)

            for i in range(1, 1 + round(typeCount/2)): 
                combs = itertools.combinations(range(typeCount-1), i)   # 마지막 col은 필터에 포함 안한다.
                for valList in combs:                                   # 어차피 yes/no 상관없이 둘로 쪼개기만 하면 되니까 마지막꺼 안넣는걸로 해도 상관없음. (연산시간 down)
                    filterNew = Filter(colIndex, valList)
                    data0, data1 = filterNew.SplitData(data)
                    gain = EntropyGain(data, data0, data1)
                    if bestGain < gain:
                        bestGain = gain
                        bestFilter = filterNew        
        if 0.0 < bestGain:
            return bestFilter
        else :
            return None
        
        



def Line2TrainData(line : str):
    if line[-1] == '\n':
        line = line[:-1]
    attriTupl = line.split('\t')
    val       = attriTupl[-1]
    attriTupl = tuple(attriTupl[:-1])
    return attriTupl, val

#public function
def ReadTrainingData(filepath: str) -> dict:
    global gAttriList, gLastAttri
    ret  = {}
    f    = open(filepath, 'r')
    lines= f.readlines()
    f.close()

    line = lines[0][0:-1]
    gLastAttri = line.split('\t')[-1]
    gAttriList = line.split('\t')[0:-1]
    RegisterDataType(gAttriList, gLastAttri)
    
    # 모든 타입 탐색
    for line in lines[1:]:
        attriTupl, val = Line2TrainData(line)
        RegisterDataIfUnknown(attriTupl, val)
    
    # 두번째 읽기에서 진짜 데이터 추출
    lastLen = len(gDataTypeDic[gLastAttri])   # lastVal의 가능한 경우의 수
    for line in lines[1:]:
        attriTupl, val = Line2TrainData(line)
        if attriTupl in ret:
            lastColCount = ret[attriTupl]
            while len(lastColCount) < lastLen:
                lastColCount.append(0.0)
        else : 
            lastColCount = [0,] * lastLen        # [0,0,0,0...] 

        order = GetLastValIndex(val)
        lastColCount[order] += 1
        ret[attriTupl] = lastColCount
    
    return ret

def Line2TestData(line : str):
    if line[-1] == '\n':
        line = line[:-1]
    attriTupl = line.split('\t')
    return attriTupl

def AppendLastElement(line:str, lastVal):
    if line[-1] == '\n':
        line = line[:-1]
    line += ("\t" + lastVal + "\n")
    return line

#public function
def ReadTestDataWriteResult(testpath:str, resultpath:str, root: DecisionNode):
    lines = []
    with open(testpath, 'r') as f:
        lines= f.readlines()
    
    with open(resultpath, 'w') as f:
        names = lines[0]
        names = AppendLastElement(names, gLastAttri)
        f.write(names)

        for line in lines[1:]:
            attriTupl = Line2TestData(line)
            lastVal = root.GuessWhat(attriTupl)
            line    = AppendLastElement(line, lastVal)
            f.write(line)    
    return


def main():
    pathTrain   = sys.argv[1]
    pathTest    = sys.argv[2]
    pathResult  = sys.argv[3]

    dictData = ReadTrainingData(pathTrain)
    treeRoot = DecisionNode(None, dictData)
    treeRoot.BuildTree()# 최적화: 완성된 Node는 dataDic을 삭제해줘야 하지만 (이미 child에 다 전달됨. 중복데이터)
    treeRoot.Print(0)   # print할때 사용하기 위해서 최적화 안함. 메모리 필요 이상 소모.
    ReadTestDataWriteResult(pathTest, pathResult, treeRoot)
    return

main()