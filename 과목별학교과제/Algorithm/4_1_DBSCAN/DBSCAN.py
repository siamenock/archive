import numpy as np
import sys

DEBUGSHOW = False
# If you want to see GUI, change code and pip install them
#import math
#import matplotlib.pyplot as plt
#import matplotlib.cm as cm
#DEBUGSHOW = True


DEFAULT_CLUSTER = -1
OUTLIER_CLUSTER = -2


class Pt:
    # static
    sDict = dict()
    # private
    def __init__(self, id:int, x:float, y:float):
        self.id = id
        self.x  = x
        self.y  = y
        self.clusterId = DEFAULT_CLUSTER
        return

    # public
    # read file data from path
    def ReadData(path:str)-> None:
        with open(path, 'r') as f:
            lines= f.readlines()
        ptList = []

        for line in lines:
            id, x, y = Pt.SpitLine(line)
            pt = Pt(id, x, y)
            ptList.append(pt)
        for p in ptList:
                Pt.sDict[p.id] = p
        return
    
    # private
    def SpitLine(line : str) -> (float):
        if line[-1] == '\n':
            line = line[:-1]
        attriTupl = line.split('\t')
        attriTupl = [float(elem) for elem in attriTupl]
        return attriTupl

    # public
    # return distance^2     // root not included
    def Distance2(self, other):
        return Distance(self, other)
    def Distance2(a, b):
        xGap = (a.x - b.x)
        yGap = (a.y - b.y)
        return xGap * xGap + yGap * yGap
    
    # public
    # return all nearby [Pt] within radius
    def GetNear(self, radius:float):
        return Pt.GetNear(self, radius)
    def GetNear(origin, radius:float):
        radius2 = radius * radius   # r^2
        ret = []
        for key, p in Pt.sDict.items():
            if origin.Distance2(p) <= radius2 and p != origin:
                ret.append(p)
        return ret
    

class Cluster:
    # DBSCAN settings
    sDict       = dict()
    eps         = 0
    minPts      = 0
    targetNum   = 0
    fileName    = "inputN"
    appendName  = ".txt"

    # DBSCAN settings
    def InitSetting(targetNum,epsRadius, minPts, fileName):
        # 초기화 및 기존결과 삭제
        if not len(Cluster.sDict) == 0:
            print("reset!")
            for p in Pt.sDict.values():
                p.idCluster = DEFAULT_CLUSTER
        Cluster.sDict       = dict()
        Cluster.targetNum  = targetNum
        Cluster.eps         = epsRadius
        Cluster.minPts      = minPts
        
        splitedName = fileName.split('.')

        Cluster.fileName    = splitedName[0]
        Cluster.appendName  = "."+ splitedName[1]   # 아몰랑 점 여러개 넣으면 지원 안해줘

        return

    # private
    def __init__(self, seed:Pt):
        self.id = Cluster.GetEmptyId()
        Cluster.sDict[self.id] = self
        
        self.ptSet = set()  # pt set belong to this cluster
        self.Insert(seed)
        return

    # private
    def GetEmptyId() -> int:
        id = 1
        while id in Cluster.sDict.keys():
            id += 1
        return id
    #insert data on cluster
    def Insert(self, p:Pt):
        if p.clusterId == DEFAULT_CLUSTER or p.clusterId == OUTLIER_CLUSTER:    
            p.clusterId = self.id
            self.ptSet.add(p)
            return
        elif p.clusterId != self.id :
            self.Merge(p.clusterId)
            return
        return    
    # merge clusters
    def Merge(self, idOpponent: int):
        #print("\tmerge event. " +str(self.id) +" merge " + str(idOpponent))
        opponent = Cluster.sDict[int(idOpponent)]
        self.ptSet |= opponent.ptSet      # merge, set union
        for p in opponent.ptSet:
            p.clusterId = self.id        
        del Cluster.sDict[idOpponent]
        return
    # get all reachable except Pt already in Cluster[self] 
    def GetReachable(self, p:Pt) -> [Pt]:
        newScaned = p.GetNear(Cluster.eps)
        if len(newScaned) >= Cluster.minPts:
            i = 0
            while i < len(newScaned):
                if newScaned[i] in self.ptSet:
                    del newScaned[i]
                else:
                    i += 1
            return newScaned
        return []
    # expand cluster into full
    def Grow(self):
        seed        = list(self.ptSet)[0]   # 1개밖에 없어야 정상작동함. grow를 여러번 호출하면 책임못짐
        reachList   = self.GetReachable(seed)
        needScan    = set([p.id for p in reachList])        # 신규 PtId들을 여기 저장. 얘들 주변으론 스캔 아직 안 해봄
        for p in reachList:
            self.Insert(p)

        while len(needScan) > 0:
            seedId      = needScan.pop()
            seed        = Pt.sDict[seedId]
            reachList   = self.GetReachable(seed)
            needScan    |= set([p.id for p in reachList])        # 신규 Pt들을 합칩합 해서 추가함
            for p in reachList:
                self.Insert(p)
        #if len(self.ptSet) >= Cluster.minPts:
        #    print("cluster id:"+str(self.id)+"\tsize:" + str(len(self.ptSet))+" scanned")
        return

    # public
    # do DBSCAN, 
    # return number of clusters made
    def DBSCAN():
        for p in Pt.sDict.values():
            if p.clusterId == DEFAULT_CLUSTER:
                
                curCluster = Cluster(p)
                curCluster.Grow()
        # minor cluser들 솎아주는 작업
        print("b4 killing outliers, total " + str(len(Cluster.sDict.keys())) + " clusters registered")
        keyList = list(Cluster.sDict.keys());   # shallow copy??
        for id in keyList:
            count = len(Cluster.sDict[id].ptSet)
            if Cluster.minPts > count:
                del Cluster.sDict[id]       
        print("after kill outliers, total " + str(len(Cluster.sDict.keys())) + " clusters registered")
        return len(Cluster.sDict.items())

    # for test only
    def DebugShow():
        if DEBUGSHOW == False:
            print("debug show not allowed on current version.")
            print("matplotlib.pyplot, matplotlib.cm       must be imported. if you want to see GUI, change code and pip install them")
            return;
        clusterCount = 0
        
        iColor = 0
        colors = cm.rainbow(np.linspace(0, 1, len(Cluster.sDict.items())))
        for id, cluster in Cluster.sDict.items():
            count = len(cluster.ptSet)
            clusterCount += 1
            print ("cid:" + str(cluster.id) + "\tlen:" +  str(count))
            xyNumpy = np.array([(p.x,p.y) for p in cluster.ptSet])
            x, y = xyNumpy.T
            plt.scatter(x,y, color=colors[iColor])
            iColor+= 1
        plt.show()
        print ("total countable cluster #" + str(clusterCount))
        return

    # write result on file
    def WriteResult() -> None:
        newId = 0 # 실제 id와 출력되는 id는 다름.
        for c in Cluster.sDict.values():
            fileName = Cluster.fileName + "_cluster_" + str(newId) + Cluster.appendName
            with open(fileName, 'w') as f:
                for pt in c.ptSet:
                    f.write(str(int(pt.id)) + "\n")
                oldId = c.id
                print(fileName + "\t id:" + str(oldId) +"->"+str(newId) + "\t len:"+ str(len(c.ptSet)))
            newId += 1
        return

    # force merge of small clusters
    def ForcedMergeTowardN() -> None:
        while(Cluster.targetNum < len(Cluster.sDict.items())):
            minKey0 = min(Cluster.sDict.items(), key=lambda x: len(x[1].ptSet))[0]
            minCluster0 = Cluster.sDict[minKey0]
            del Cluster.sDict[minKey0]
            minKey1 = min(Cluster.sDict.items(), key=lambda x: len(x[1].ptSet))[0]      # 2nd min
            Cluster.sDict[minKey0] = minCluster0
            
            print("FORCED MERGE : " + str(minKey0) +"," + str(minKey1) )
            minCluster1 = Cluster.sDict[minKey1]
            minCluster1.Merge(minCluster0.id)
        return



def main():
    
    pathRead    = sys.argv[1]
    n           = int  (sys.argv[2])
    eps         = float(sys.argv[3])
    minPts      = int  (sys.argv[4])
    """
    pathRead    = "input1.txt"
    n           = 8
    eps         = 15
    minPts      = 22
    """
    
    triedEps = []

    while True:
        print("=======START SCAN===========")
        print("n\t" + str(n))
        print("eps\t" + str(eps))
        print("minPts\t" + str(minPts))
        Pt.ReadData(pathRead)
        #DbscanUntilSuccess(n,eps,minPts,pathRead)
        Cluster.InitSetting(n, eps, minPts, pathRead)
        numCluster = Cluster.DBSCAN()

        #디버그용
        #Cluster.DebugShow()

        if n == numCluster:
            break;
        
        triedEps.append(eps)

        gap = numCluster - n
        if gap > 0:
            eps += 1
            if (eps in triedEps):
                print("======================================")
                print("============STOP REPEATING============")
                Cluster.DebugShow();
                Cluster.ForcedMergeTowardN()
                
                break
        else:
            eps -= 1
            
        print("SCAN ERROR!")

    print("----")
    print("final result-----------------")

    # n 입력값이랑 잘 안맞으면 param 바꿔서 다시해야됨. 
    # 입력받은 eps랑 minPts는 참고용으로만 사용할 것
    Cluster.WriteResult()
    Cluster.DebugShow();
    

    

if __name__ == "__main__":
    main()
