import queue
import enum


DIRECTORY_PATH  = """C:\\Users\\toodu\\OneDrive\\대학자료\\3학년 2학기\\인공지능\\HW\\HW.py\\"""
FILE_NAMES      = ("first_floor", "second_floor", "third_floor", "fourth_floor", "fifth_floor")
TXT             = ".txt"

#===================== CLASS =======================#
#                   Counting Queue                  #
#===================================================#
class CQ:
    class CountMode(enum.Enum):
        Queue = 0
        PrioQ = 1
        Stack = 2

    count       = 0
    q           = queue.Queue()
    countMode   = CountMode.Queue
    
    @staticmethod
    def Init(countWhat):
        CQ.count = 0
        CQ.countMode = countWhat
        if  (countWhat == CQ.CountMode.Queue):
            CQ.q = queue.Queue()
        elif(countWhat == CQ.CountMode.PrioQ):
            CQ.q = queue.PriorityQueue()
        elif(countWhat == CQ.CountMode.Stack):
            CQ.q = []
        else:
            CQ.Init(CQ.CountMode.Queue)
        return 

    @staticmethod
    def Put(item):
        if(CQ.countMode == CQ.CountMode.Stack):
            return CQ.q.append(item)
        return CQ.q.put(item)

    @staticmethod
    def Get():
        if(CQ.countMode == CQ.CountMode.Stack):
            CQ.count = CQ.count + 1
            return CQ.q.pop()
        
        if(not CQ.q.empty()):
            CQ.count = CQ.count + 1

        return CQ.q.get()

    def GetCount():
        return CQ.count




#===================== CLASS =======================#
#                   Cordinate                       #
#===================================================#
class Cd(object):
    class SortMode(enum.Enum):
        MDistance   = 0
        Cost        = 1

    # 스테틱 변수
    mapSize = 3
    maxCost  = 3*3
    mazeMap  = ((1,3,1),(1,2,1),(1,4,1))
    costMap  = ((9,9,9),(9,9,9),(9,9,9))
    
    posStart = None
    posKey   = None
    posExit  = None

    curStart       = None
    curDestination = None               # 목적지. priority 비교할 때 curDestination과의 거리가 가까울수록 우선순위임
                                
    sortMode    = SortMode.MDistance    # default : 맨하탄 디스턴스
    dirPriority = (0,1,2,3)             # default : down -> right -> left -> up

    @staticmethod
    def MapScan(map_num):
        global DIRECTORY_PATH, FILE_NAMES, TXT
        filePath = DIRECTORY_PATH + FILE_NAMES[map_num] + TXT
        f = open(filePath, 'r')

        map_num, w, h = [int(x) for x in next(f).split()] # read first line
        if w !=h:
            print("FILE FORMAT WRONG 1")
            return
        Cd.mapSize = w
        Cd.maxCost  = Cd.mapSize * Cd.mapSize
        Cd.mazeMap  = []
        for line in f: # read rest of lines
            Cd.mazeMap.append([int(x) for x in line.split()])
        f.close()

        Cd.InitCostMap()

        for x in range(Cd.mapSize):
            for y in range(Cd.mapSize):
                if Cd.mazeMap[y][x] == 1:
                    continue
                elif Cd.mazeMap[y][x] == 2:
                    continue
                elif Cd.mazeMap[y][x] == 3:
                    Cd.posStart = Cd(x,y)
                    continue
                elif Cd.mazeMap[y][x] == 4:
                    Cd.posExit = Cd(x,y)
                    continue
                elif Cd.mazeMap[y][x] == 6:
                    Cd.posKey = Cd(x,y)
                    continue

        
        
        return       
    
    @staticmethod
    def UpdateRouteOnMap():     # 2 -> 5
        curPos  = Cd.curDestination
        curPos  = Cd(curPos.x, curPos.y)        # update curCost
        curCost = curPos.curCost

        while (curCost >= 0):
            Cd.mazeMap[curPos.y][curPos.x] = 5
            curCost -= 1

            for newCd in curPos.NearByMovableTiles():
                if newCd.curCost == curCost:
                    curPos = newCd
                    break
        return
            


    @staticmethod
    def InitCostMap():
        Cd.costMap  = []
        for y in range(Cd.mapSize):
            line        = []
            for x in range(Cd.mapSize):
                line.append(Cd.maxCost)
            Cd.costMap.append(line)

        return

    @staticmethod
    def SetDestination(newDestination):
        Cd.curDestination  = newDestination
        return

    @staticmethod
    def SetCriteria(sortMode):
        Cd.sortMode = sortMode
        return

    @staticmethod
    def Map2String():
        ret = ""
        for line in Cd.mazeMap:
            str_line = " ".join( [str(i) for i in line])
            ret += str_line + "\n"
        return ret

    def CostMap2String():
        
        len = 1
        max_cost = Cd.costMap[ Cd.curDestination.y] [Cd.curDestination.x]
        while max_cost > 10:
            max_cost /= 10
            len      += 1

        ret = ""
        for line in Cd.costMap:
            line_list = []
            for i in line:
                if(i != Cd.maxCost):
                    line_list.append(str(i).zfill(len))
                else :
                    line_list.append("x" * len)

            str_line = " ".join( line_list)
            ret += str_line + "\n"
        return ret

    
    # 생성자 안에 선언된 변수가 attribute가 됨.
    def __init__(self, x, y, curCost = -1): # curCost를 입력하지 않을경우, Cd.costMap의 값을 복사해 생성
        self.x, self.y = x, y
        self.curCost = curCost
        if(curCost < 0):
            if(0 <= x and x < Cd.mapSize and 0 <= y and y < Cd.mapSize):
                self.curCost = Cd.costMap[y][x]
            else:
                self.curCost = Cd.maxCost

        return

    
    def IsMovable(self):
        if(self.y < 0 or Cd.mapSize <= self.y or
           self.x < 0 or Cd.mapSize <= self.x):
            return False
        return (Cd.mazeMap[self.y][self.x] != 1)

    def NearByMovableTiles(self):       # 자기주변 이동가능한 타일 list return.
        list = []                       # 가장자리 초과시 return되지 않음
        for dir in Cd.dirPriority:      # 상하좌우 탐색순서는 Cd.searchSequence에 의해 결정됨
            newX = self.x               
            newY = self.y               
            if (dir == 0):              
                newY += 1
            elif (dir == 1):            
                newX += 1
            elif (dir == 2):
                newX -= 1
            else:
                newY -= 1

            newCd = Cd(newX, newY)
            if(newX < 0 or Cd.mapSize <= newX or
               newY < 0 or Cd.mapSize <= newY or
               not newCd.IsMovable()):
                continue
            list.append(newCd)
            
        return list
    
    def NearByCheaperList(self):        
        list = []                                                   # 인근의 이동가능한 타일 list return
        for newCd in self.NearByMovableTiles():                     # 신규 타일의 cur_cost는 현재타일의 +1
            newCd.curCost = self.curCost + 1                        # 상하좌우 탐색순서는 Cd.searchSequence에 의해 결정됨
            if  Cd.costMap[newCd.y][newCd.x] > newCd.curCost:       # 단, 이미 더 짧은 경로가 탐색된 경우에는 list에 포함하지 않음
                Cd.costMap[newCd.y][newCd.x] = newCd.curCost        # 탐색되지 않은 타일들은 Cd.costMap에서 maxVal을 가짐
                list.append(newCd)                                  # list에 return되는 경우 Cd.costMap의 해당 타일 코스트를 업데이트 해 줌
        return list


    def GetMDistance(self):
        return (Cd.curDestination.y - self.y, Cd.curDestination.x - self.x)

    def CriteriaMDestination(self, other):    # Destination과의 맨하탄 디스턴스가 기준임
        gap1 = (Cd.curDestination.y - self.y, Cd.curDestination.x - self.x)
        gap2 = (Cd.curDestination.y -other.y, Cd.curDestination.x -other.x)
        return (abs(gap1[0]) + abs(gap1[1])) < (abs(gap2[0]) + abs(gap2[1]))

    def CriteriaCost(self, other):
        return self.curCost < other.curCost
    
    def __eq__(self, other):
        return self.x == other.x and self.y == other.y
    
    def __lt__(self, other):    # Cd.sortMode에 따라서 다른 기준 적용
        if Cd.sortMode == Cd.SortMode.MDistance:
            return self.CriteriaMDestination(other)
        #if Cd.sortMode == Cd.SortMode.Cost:
        return self.CriteriaCost(other)


    #string으로 변환할때 사용하는 함수임. str([Cd타입의변수])
    def __repr__(self):
        if(self.curCost == 0):
            return "(" + str(self.y) + ", " + str(self.x) + ")"
        else:
            return "(" + str(self.y) + ", " + str(self.x) + "){" + str(self.curCost) + "}"
    
#--------------------------------------
#Cd 클래스 스테틱 변수 초기화 작업 (스크립트 언어라서 타입정의 끝나기 전에는 자기 타입 못 쓰나봐)
Cd.SetDestination(Cd(0,0))
Cd.curStart     = Cd(0,0)
Cd.posStart     = Cd(0,0)
Cd.posKey       = Cd(0,0)
Cd.posExit      = Cd(0,0)   
#--------------------------------------



#=======================================================#
# SearchSegement알고리즘들 돌리기 전에 필요한 설정들    #
#       1.CQ를 어떤 형태로 사용할지                     #
#       2.소팅기준                                      #
#       3.cur시작지/cur목적지                           #
#       4.상하좌우 탐색순서                             #
#=======================================================#

def SearchSegment_BreathFirstOrDepthFirst():    # CQ -> 일반 큐면 Breath-First, Stack이면 Depth-first
    cur_pos = Cd.curStart;                      # 소팅기준 필요 없음
    cur_pos.curCost = 0                         
    Cd.costMap[cur_pos.y][cur_pos.x] = 0

    while(not cur_pos == Cd.curDestination):
        add_list = cur_pos.NearByCheaperList()
        for add in add_list:
            CQ.Put(add)
        cur_pos = CQ.Get()
    return cur_pos.curCost


#========================================================#
#   Ans 함수들은 정답을 구해주는 함수들임                #  
#   작동 전에 Cd.MapScan(~)을 해 줘야 함                 #
#   상하좌우우선순위를 param으로 받음                    #
#   Default 우선순위는 하우좌상. 0~3의 숫자 배열으로 입력#
#========================================================#

def Ans_BreathFirst(dirPriorityList = (0,1,2,3), debug = False):
    CQ.Init(CQ.CountMode.Queue)
    Cd.dirPriority      = dirPriorityList

    Cd.curStart         = Cd.posStart
    Cd.curDestination   = Cd.posKey

    ans1 = SearchSegment_BreathFirstOrDepthFirst()

    if(debug):
        print(Cd.CostMap2String())
    
    Cd.UpdateRouteOnMap()
    Cd.InitCostMap()

    Cd.curStart         = Cd.posKey
    Cd.curDestination   = Cd.posExit
    ans2 = SearchSegment_BreathFirstOrDepthFirst()
    
    if(debug):
        print(Cd.CostMap2String())

    Cd.UpdateRouteOnMap()

    
    return ans1+ans2, CQ.GetCount()

def Ans_GreedyBestFirst(dirPriorityList = (0,1,2,3), debug = False):
    CQ.Init(CQ.CountMode.PrioQ)
    Cd.SetCriteria(Cd.SortMode.MDistance)
    Cd.dirPriority      = dirPriorityList

    Cd.curStart         = Cd.posStart
    Cd.curDestination   = Cd.posKey

    ans1 = SearchSegment_BreathFirstOrDepthFirst()
    
    if(debug):
        print(Cd.CostMap2String())

    Cd.UpdateRouteOnMap()
    Cd.InitCostMap()

    Cd.curStart         = Cd.posKey
    Cd.curDestination   = Cd.posExit
    ans2 = SearchSegment_BreathFirstOrDepthFirst()

    if(debug):
        print(Cd.CostMap2String())

    Cd.UpdateRouteOnMap()

    
    return ans1+ans2, CQ.GetCount()






"""
def TestCQ():
    Cd.SetDestination(Cd(0,0))
    ##==========================
    ##      일반 큐 모드
    ##================
    CQ.Init(CQ.CountMode.Queue)
    CQ.Put(Cd(4,7))
    CQ.Put(Cd(0,3))
    CQ.Put(Cd(0,0))

    print("cur count : " + str(CQ.GetCount()) + "일반 큐 모드")
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))
    
    ##==========================
    ##      스택 모드
    ##================
    CQ.Init(CQ.CountMode.Stack)
    CQ.Put(Cd(4,7))
    CQ.Put(Cd(0,3))
    CQ.Put(Cd(0,0))
    print()
    print("cur count : " + str(CQ.GetCount()) + "스택 모드")
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))
    

    ##==========================
    ##      priority queue 모드, Cd타입만 사용 가능
    ##================
    CQ.Init(CQ.CountMode.PrioQ)
    Cd.SetDestination(Cd(0,0))
    Cd.SetCriteria(Cd.SortMode.MDistance)
    
    print("")
    print("CQ.Init(CQ.CountMode.PrioQ)            // (우선순위 큐로 초기화)")
    print("Cd.SetCriteria(Cd.SortMode.MDistance)  // (목적지까지의 거리가 기준)")
    print("Cd.SetDestination(Cd(0,0))             // (기준점을 0,0으로 설정)")

    
    CQ.Put(Cd(4,7))
    CQ.Put(Cd(0,3))
    CQ.Put(Cd(0,0))
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))

    CQ.Init(True)
    Cd.SetDestination(Cd(5,5))
    print("")
    print("CQ.Init(CQ.CountMode.PrioQ)            // (우선순위 큐로 초기화)")
    print("Cd.SetCriteria(Cd.SortMode.MDistance)  // (목적지까지의 거리가 기준)")
    print("Cd.SetDestination(Cd(5,5))             // (기준점을 5,5로 설정)")
    
    
    CQ.Put(Cd(4,7))
    CQ.Put(Cd(0,3))
    CQ.Put(Cd(0,0))
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))

    CQ.Init(True)
    Cd.SetCriteria(Cd.SortMode.Cost)
    
    print("")
    print("CQ.Init(CQ.CountMode.PrioQ)            // (우선순위 큐로 초기화)")
    print("Cd.SetCriteria(Cd.SortMode.Cost)       // (추가입력된 코스트가 기준임)")
    
    CQ.Put(Cd(4,7,2))
    CQ.Put(Cd(0,3,5))
    CQ.Put(Cd(0,0))
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))
    
    print("get " + str(CQ.Get()), end = '\t')
    print("cur count : " + str(CQ.GetCount()))

    return;

def TestCd():
    for i in range(5):
        Cd.MapScan(i)

    return


TestCQ()
TestCd()
"""

def AllDirectionSequence(recursive = []):
    list = []
    if len(recursive) == 4:
        list = []
        list.append(recursive)
        return list

    for i in range(4):
        if not i in recursive:
            copied = recursive[:]
            copied.append(i)
            nexts = AllDirectionSequence(copied)
            list.extend( nexts)

    return list


def SearchTest():
    MAX = 999999999999999999999999999
    seqAll = AllDirectionSequence()
    for mapNum in range(5):
        if(mapNum != 4):
            continue

        print("map no : " + str(mapNum))
        best_leng = MAX
        best_time = MAX
        best_option = "default_string"
        
        """
        for seq in seqAll:
            Cd.MapScan(mapNum)
            leng, time = Ans_BreathFirst(seq)
            print("\tBreathFirst\t" + str([i for i in seq]) + "\t" + str(leng) + "\t" + str(time))
            
            if(leng != best_leng and best_leng != MAX):
                print("\tBreathFirst\t" + str([i for i in seq]) + "\t" + str(leng) + "\t" + str(time) + "\tERROR : leng not match")
                print(Cd.Map2String())
                Cd.MapScan(mapNum)
                Ans_BreathFirst(seq, True)
            else:
                print("\tBreathFirst\t" + str([i for i in seq]) + "\t" + str(leng) + "\t" + str(time))

            best_leng = leng

            if(time < best_time):
                best_time = time
                best_option = "\tBreathFirst\t" + str([i for i in seq])
        """
        for seq in seqAll:
            Cd.MapScan(mapNum)
            leng, time = Ans_GreedyBestFirst(seq)
            
            
            if(leng != best_leng and best_leng != MAX):
                print("\tGreedyBest\t" + str([i for i in seq]) + "\t" + str(leng) + "\t" + str(time) + "\tERROR : leng not match")
                print(Cd.Map2String())
                Cd.MapScan(mapNum)
                Ans_GreedyBestFirst(seq, True)
            else:
                print("\tGreedyBest\t" + str([i for i in seq]) + "\t" + str(leng) + "\t" + str(time))

            best_leng = leng

            if(time < best_time):
                best_time = time
                best_option = "\tGreedyBest\t" + str([i for i in seq])
        

        print("map no : " + str(mapNum))
        print(best_option + "\t" + str(best_leng) + "\t" + str(best_time))
    return


SearchTest()
