using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float CamLerpRate;
    GameMaster gm;
    Camera mc;
    GameObject ActiveTarget;

    // Start is called before the first frame update
    void Awake()
    {
        gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
        mc = gm.MainCam;
    }
    private void Start()
    {
        SetActiveTarget(gm.Player.gameObject);
    }
    private void Update()
    {
        transform.position = new Vector3(ActiveTarget.transform.position.x, ActiveTarget.transform.position.y, -10);
    }

    public void SetActiveTarget(GameObject NewTarget)
    {
        ActiveTarget = NewTarget;
    }
    bool IsShake;
    public IEnumerator CameraShake(float ShakeDuration, float ShakeAmount)
    {
        if (IsShake)
        {
            yield break;
        }
        IsShake = true;
        Debug.Log("START SHAKE");
        for (float i = 0; i < ShakeDuration; i += Time.deltaTime)
        {
            float x = Random.Range(-1f, 1f) * ShakeAmount;
            float y = Random.Range(-1f, 1f) * ShakeAmount;
            mc.transform.localPosition = new Vector2(x, y);
            Debug.Log("SHAKING" + i);
            yield return null;
        }
        Debug.Log("END SHAKE");
        mc.transform.localPosition = Vector2.zero;
        IsShake = false;
    }
    bool IsRecoil;
    public IEnumerator CameraRecoil(Vector2 RecoilDir,float RecoilAmount)
    {
        if (IsRecoil)
        {
            yield break;
        }
        IsRecoil = true;
        mc.transform.localPosition = RecoilDir * RecoilAmount;
        Vector2 ForwardPos = RecoilDir * -RecoilAmount;
        for (float i = 0; i < 0.1;)
        {
            mc.transform.localPosition = Vector2.Lerp(mc.transform.localPosition, ForwardPos, 10 * Time.deltaTime);
            i += Time.deltaTime;
            yield return null;
        }
        for (float i = 0; i < 0.05f;)
        {
            mc.transform.localPosition = Vector2.Lerp(mc.transform.localPosition, Vector2.zero, 10 * Time.deltaTime);
            i += Time.deltaTime;
            yield return null;
        }
        IsRecoil = false;
        mc.transform.localPosition = Vector2.zero;
        
    }
}
