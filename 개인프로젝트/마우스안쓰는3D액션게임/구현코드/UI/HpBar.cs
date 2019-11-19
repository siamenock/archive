using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour {
    private static Dictionary<GameObject, HpBar> match;
    private static GameObject resource;
    private static readonly float DESTROY_TIMEOUT_MAX = 5f;
    private static GameObject cam;

    public const string resourcePath = "UI/HP_Canvas";

    private GameObject      character;
    private Stat            characterStat;
    private BoxCollider     characterBoxCollider;
    private RectTransform   hpPercentBar;
    private float           destroyTimer    =   DESTROY_TIMEOUT_MAX;
    private float           hpLastCheck     = -1;

    // Get으로만 생성, 호출 가능. 근데 사실 생성자 호출할일 없음.
    // 이미 만들어진 prefab의 컴포넌트로만 사용할거임. prefab 째로 복사해서 사용
    private HpBar() { }

    // Dictionary에서 character의 HpBar를 찾아서 가져다 줌. 없으면 생성해서 줌.
    // 
    public static HpBar GetMyHpBarWork(GameObject character)
    {

        try
        {
            return GetDictionary()[character];
        }
        catch (KeyNotFoundException)
        {
            GameObject  o   = Instantiate(GetResource());
            HpBar       ret = o.GetComponent<HpBar>();
            ret.character   = character;

            GetDictionary().Add(character, ret);

            return ret;
        }
    }
       
    private static Dictionary<GameObject, HpBar> GetDictionary()
    {
        return match ?? (match = new Dictionary<GameObject, HpBar>());
    }

    private static GameObject GetResource() {
        return resource ?? (resource = Resources.Load(resourcePath, typeof(GameObject)) as GameObject);
    }

    private static GameObject GetCamera() {
        if(cam != null)
            return cam;

        Camera c =  Camera.main;
        if(c == null)
            return null;

        cam = c.gameObject;
        return cam;
    }

    // Use this for initialization
    void Start () {
        this.destroyTimer           = DESTROY_TIMEOUT_MAX;

        this.characterStat          = character.GetComponent<Stat>();
        this.characterBoxCollider   = character.GetComponent<BoxCollider>();
        this.hpPercentBar           = this.transform.Find("hp_percent_bar").gameObject.GetComponent<RectTransform>();     // child에 존재하는 hp bar

        this.transform.position = character.transform.position + characterBoxCollider.size.y * new Vector3(0, 1, 0);
        this.transform.rotation = GetCamera().transform.rotation;
    }
	
	// hp 변동 있으면 디스플레이를 계속함. 일정시간(TIMEOUT_MAX) 동안 변동 없을 경우 사라짐. (타격시로 바꿔야할까?)
	void Update () {
        if(this.character == null)
            Destroy(this.gameObject);

        this.transform.position = character.transform.position + characterBoxCollider.size.y * new Vector3(0,1,0);
        this.transform.rotation = GetCamera().transform.rotation;

        float hp    = characterStat.Get(Stat.Name.HP);
        float hpMax = characterStat.Get(Stat.Name.HP_MAX);

        if (hp != this.hpLastCheck)
        {
            this.hpLastCheck = hp;
            this.hpPercentBar.sizeDelta = new Vector2( hp/hpMax, hpPercentBar.sizeDelta.y);
            destroyTimer = DESTROY_TIMEOUT_MAX;
        }

        this.destroyTimer -= Time.deltaTime;
        if (this.destroyTimer <= 0)
            Destroy(this.gameObject);
	}
    private void OnDestroy()
    {
        GetDictionary().Remove(this.character);
    }
}
