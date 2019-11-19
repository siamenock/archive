using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// common AI module to evade attack
public class WarningSensorSystem : MonoBehaviour
{
    private static bool         DEBUG_VISIBLE = false;
    private readonly static int RESOLUTION = 2,      // (character size)의 공간을 n*n개로 쪼개서 테스트 함
                                MAX_LENG = 5,      // (tile total lenghth) == (character size) * n
                                MAP_SIZE = RESOLUTION * (2 * MAX_LENG + 1),
                                LAYER_MASK = (int)MyEnum.Layer.WARNING;
    public CharacterInteractor.TEAM team;
    BoxCollider mybox;

    
    readonly static float MARGIN    = 0.2f;   // 여분 사이즈
    
    private static GameObject pos_marker   = null;

    private GameObject  debug_display_box   = null;
    private float       last_update_time    = -1;
    private float       Debug_logtime       = -1f;
    private int         radius_multiple     = MAX_LENG;
    // 매 프레임 업데이트되는 입력 데이터들
    float radius;
    Vector3 size;
    Vector3 origin;
    // 매 프레임 업데이트되는 출력 데이터들
    bool nothing_found = true;
    List<Warning>    [,] warning_map = new List<Warning> [MAP_SIZE, MAP_SIZE];

    public bool IsNearbyWarning() {
        UpdateAll();
        return ! nothing_found;
    }

    public List<Warning>[,] GetWarningMap() {
        UpdateAll();
        return warning_map;
    }
    

    public Vector3 GetNearSafePos() {
        UpdateAll();

        int m = MAP_SIZE/2;
        if (nothing_found == false)
        {
            for (int z = 0; z < m; z++)
                for (int x = 0; x < m; x++)
                {
                    if (_IsSafe(m + x, m + z)) return Pos(m + x, m + z);
                    if (_IsSafe(m - x, m + z)) return Pos(m - x, m + z);
                    if (_IsSafe(m + x, m - z)) return Pos(m + x, m - z);
                    if (_IsSafe(m - x, m - z)) return Pos(m - x, m - z);
                }
        }
        Vector2Int center = GetCenterPos();
        return Pos(center.x, center.y);
    }

    // MAP_SIZE == 21인 경우 x,z는 0 ~ +20 범위임. 이를 내부적으로는 -10~+10으로 생각해서 계산함
    public Vector3 Pos(int x, int z)
    {
        UpdateInputParameters();

        return _Pos(x, z);
    }

    // global pos -> warning_map (x,z)좌표. 캐릭터중심이 (0,0)아님. 주의.
    public Vector2 Pos(Vector3 pos)
    {
        UpdateInputParameters();

        pos -= origin;
        pos.y = 0;
        pos = Quaternion.Inverse(this.transform.rotation) * pos;

        Vector2 ret = new Vector2(pos.x, pos.z);
        ret.x /= this.size.x;
        ret.y /= this.size.z;
        ret += GetCenterPos();
        return ret;
    }

    Vector2Int GetCenterPos() { return new Vector2Int(MAP_SIZE/2, MAP_SIZE/2);}

    public bool IsSafe(Vector3 pos_real) {
        Vector2 v = this.Pos(pos_real);
        int x = (int) v.x;
        int y = (int)v.y;
        int xa = x % 1 == 0 ? 0 : 1;
        int ya = y % 1 == 0 ? 0 : 1;
        
        return _IsSafe(x, y) && _IsSafe(x + xa, y) && _IsSafe(x, y + ya) && _IsSafe(x + xa, y + ya);
    }
    

    // public 버전보다 좀더 가벼움
    private bool _IsSafe(int x, int z) {
        if(warning_map[x,z].Count == 0)
            return true;
        else
            return false;
    }

    // 1차 Rough Check하는 Sphere가 캐릭터 크기의 몇배가 되게 할지.
    // 0으로 세팅하면 거의 아무것도 안 하는 셈임.
    public void Set1stRadius(int multiple_of_character) {
        radius_multiple = multiple_of_character;
        this.radius = mybox.size.x * (radius_multiple);
    }

    public bool AmIDangerous(int size = 2)
    {
        UpdateAll();
        int gap = WarningSensorSystem.RESOLUTION * size; // warning map에서 이 범위만큼 찾으면 됨
        Vector2Int center = GetCenterPos();
        for (int x = center.x - gap; x < center.x + gap; x++)
            for (int z = center.y - gap; z < center.y + gap; z++)
                if (warning_map[x, z].Count != 0)
                    return true;
        return false;
    }

    // pos 위치로 직선이동 중 위험한 warning이 있는지 여부 return. 단, 현재위치한 타일은 체크하지 않음.
    public bool IsSafeToMoveToward(Vector2Int pos) {
        UpdateAll();

        Vector2Int gap = pos - GetCenterPos();
        float ratio = Slope(gap);

        // list에 중간경우지점 전부 다 등록함
        List<Vector2Int> list_mid_pos = new List<Vector2Int>();
        for (Vector2Int cur = new Vector2Int(0,0);  gap != cur ;  /* cur을 한칸씩 이동 */)
        {
            float cur_ratio = Slope(cur);

            //if (Mathf.Abs(cur_ratio) == Mathf.Abs(ratio))

            if (     gap.x == 0 || Mathf.Abs(cur_ratio) < Mathf.Abs(ratio))
                cur.y += (gap.y > 0) ? +1 : -1;
            else if (gap.y == 0 || Mathf.Abs(cur_ratio) > Mathf.Abs(ratio))
                cur.x += (gap.x > 0) ? +1 : -1;
            else  // if ( == )
            {
                int        xadd = gap.x > 0 ? +1 : -1,
                           yadd = gap.y > 0 ? +1 : -1;
                Vector2Int vx = cur,
                           vy = cur;
                vx.x += xadd;
                vy.y += yadd;
                list_mid_pos.Add(vx);
                list_mid_pos.Add(vy);

                cur += new Vector2Int(xadd, yadd);
            }
            list_mid_pos.Add(cur);
        }

        // 중간경유지점 전부 체크
        foreach (Vector2Int mid in list_mid_pos) {
            Vector2Int v = mid + GetCenterPos(); // v = real pos
            if(warning_map[v.x, v.y].Count != 0) // if warning detected
                return false;
        }
        return true;
    }

    public List<Warning> GetWarningsAt(Vector2Int pos) {
        UpdateAll();
        return warning_map[pos.x, pos.y];
    }

    // return 기울기
    private float Slope(Vector2Int pos) {
        if (pos.x != 0) return ((float)pos.y) / pos.x;
        else            return (pos.y > 0) ? +Mathf.Infinity : -Mathf.Infinity;
    }

    // Use this for initialization
    private void Start()
    {
        this.team  = this.GetComponent<CharacterInteractor>().team;
        this.mybox = this.GetComponent<BoxCollider>();

        if (DEBUG_VISIBLE)
        {
            debug_display_box = Resources.Load("DebugPrintBox", typeof(GameObject)) as GameObject;
            debug_display_box = Instantiate(debug_display_box, new Vector3(0, -999, 0), this.transform.rotation);
            debug_display_box.SetActive(false);
            debug_display_box.transform.localScale = (mybox.size);       // 넉넉하게 20%정도 더 크게 잡음.
            debug_display_box.name = "Sensor of [" + this.name + "]";
        }

        for (int x = 0; x < MAP_SIZE; x++)
            for (int z = 0; z < MAP_SIZE; z++)
            {
                warning_map[x,z] = new List<Warning>();
            }

        if (pos_marker == null) {
            pos_marker = Resources.Load("PosMarker", typeof(GameObject)) as GameObject;
        }

        // 시작시 주변 감지범위 보여주는 디버그용 코드
        UpdateInputParameters();
     
    }

    // Update 역할이긴 한데, 사실 매 프레임 불릴 필요는 없어서 lazy하게 동작할래
    // Update 상태가 최신상태면 암것도 안함.
    // 모든 public 함수 호출시 UpdateAll 실행됨
    private void UpdateAll()
    {
        if (last_update_time == Time.time)
            return;
        else
            last_update_time = Time.time;

        UpdateInputParameters();
        // Debug_ShowRange();
        this.nothing_found = !RoughScan();
        if ( nothing_found ) 
            return;
        
        UpdateWarningMap();
    }

    private void UpdateInputParameters() {
        this.size = mybox.size;
        this.radius = mybox.size.x * (radius_multiple);
        this.origin = this.transform.position + mybox.center;
    }

    private bool RoughScan() {
        Collider[] colliders = Physics.OverlapSphere(origin, radius, LAYER_MASK);
        if (FindDanger(colliders).Count == 0)
            return false;
        else
        {
            return true;
        }
    }

    private void UpdateWarningMap() {
        Vector3   size = mybox.size;
        Quaternion rot = this.transform.rotation;
        for (int x = 0; x < MAP_SIZE; x++)
            for (int z = 0; z < MAP_SIZE; z++) {
                Collider[] colliders = Physics.OverlapBox(Pos(x,z), size * (1+MARGIN), rot, LAYER_MASK);
                List<Warning> warnings = FindDanger(colliders);
                if (warnings.Count == 0)
                {
                    warning_map[x, z].Clear();
                    continue;
                }
                else {
                    warning_map[x, z] = warnings;
                }

                

                //디버그용으로 시각화하는 파트
                if (DEBUG_VISIBLE && warning_map[x, z].Count != 0) {
                    GameObject o = Instantiate(debug_display_box, Pos(x,z), rot);
                    o.name += ("["+x+","+z+"]");
                    o.SetActive(true);
                }

            }
    }

    private List<Warning> FindDanger(Collider[] colliders) {
        List<Warning> ret = new List<Warning>();
        foreach (Collider c in colliders)
        {
            Warning w;
            if (c.transform.parent != null)
            {
                GameObject parent = c.transform.parent.gameObject;
                w = parent.GetComponent<Warning>();
            }
            else
                w = c.GetComponent<Warning>();

            if (w != null)
            {
                if (this.team == w.attacker.GetComponent<CharacterInteractor>().team)
                    continue;
                else
                    ret.Add(w);
            }
            
        }
        return ret;
    }


    /*
    void Debug_ShowRange() {
        if (Debug_logtime + 5 < Time.time)
        {
            Debug_logtime = Time.time;
        }
        else {
            return;
        }

        Quaternion rot = this.transform.rotation;
        for (int x = 0; x < MAP_SIZE; x++)
            for (int z = 0; z < MAP_SIZE; z++)
            {
                GameObject o = Instantiate(debug_display_box, Pos(x, z), rot);
                o.SetActive(true);
            }
    }*/

    private Vector3 _Pos(int x, int z)
    {
        x -= MAP_SIZE / 2;
        z -= MAP_SIZE / 2;
        return origin + this.transform.rotation * new Vector3(x * size.x, 0, z * size.z);
    }

    private void RefreshWarningMap() {
        for (int x = 0; x < MAP_SIZE; x++)
            for (int z = 0; z < MAP_SIZE; z++)
            {
                warning_map[x,z].Clear();
            }
    }
}
