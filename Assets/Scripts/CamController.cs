using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CamController : MonoBehaviour
{
    private GameObject player;
    public GameObject target, focus;
    public float focusDistant;

    private Vector3 movePoint, refVector3;

    private CinemachineVirtualCamera vcam;
    private float shakeForce;
    private float timer;

    private float refFloat;
    public float zoomTime;
    public float threshold;
    float checkMobilethreshold;

    public float bossDistant, zoomDistant;
    public bool slowZoom, getHurt, lastBoss;

    public static CamController instance;
    [SerializeField] GameObject playerHp;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ResetCamShake();
    }

    private void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0)
            {
                ResetCamShake();
            }
        }

        if (focus != null)
        {
            movePoint = focus.transform.position;
            movePoint.z = -10;
            Time.timeScale = 1;
            transform.position = Vector3.SmoothDamp(transform.position, movePoint, ref refVector3, Mathf.Sqrt(0.05f));

            vcam.m_Lens.OrthographicSize = Mathf.SmoothDamp(vcam.m_Lens.OrthographicSize, focusDistant, ref refFloat, zoomTime);
            playerHp.SetActive(false);
            foreach (Slider abilityMeter in GameManager.instance.abilityMeters)
            {
                abilityMeter.gameObject.SetActive(false);
            }
            return;
        }

        if (!playerHp.activeInHierarchy)
        {
            playerHp.SetActive(true);
            foreach (Slider abilityMeter in GameManager.instance.abilityMeters)
            {
                abilityMeter.gameObject.SetActive(true);
            }
        }

        if (player == null)
            return;

        if (!player.activeInHierarchy)
            return;

        movePoint = player.transform.position;

        TargetLock();

        movePoint.z = -20;


        if (slowZoom)
        {
            transform.position = Vector3.SmoothDamp(transform.position, movePoint, ref refVector3, Mathf.Sqrt(0.35f));
            return;
        }

        transform.position = Vector3.SmoothDamp(transform.position, movePoint, ref refVector3, Mathf.Sqrt(0.02f));
    }

    void TargetLock()
    {
        if (target == null)
        {
            zoomDistant = 12;
            if (Application.isMobilePlatform)
                zoomDistant *= 0.75f;
            vcam.m_Lens.OrthographicSize = Mathf.SmoothDamp(vcam.m_Lens.OrthographicSize, zoomDistant, ref refFloat, zoomTime);
            return;
        }
        if (!target.activeInHierarchy)
        {
            zoomDistant = 12;
            if (Application.isMobilePlatform)
                zoomDistant *= 0.75f;
            vcam.m_Lens.OrthographicSize = Mathf.SmoothDamp(vcam.m_Lens.OrthographicSize, zoomDistant, ref refFloat, zoomTime);
            return;
        }

        movePoint = (player.transform.position + target.transform.position) / 2f;

        checkMobilethreshold = threshold * 0.75f;
        if (Application.isMobilePlatform)
        {
            movePoint = new Vector3(Mathf.Clamp(movePoint.x, player.transform.position.x - checkMobilethreshold, player.transform.position.x + checkMobilethreshold),
                Mathf.Clamp(movePoint.y, player.transform.position.y - checkMobilethreshold * 0.6f, player.transform.position.y + checkMobilethreshold * 0.6f), movePoint.z);
        }
        else
        {
            movePoint = new Vector3(Mathf.Clamp(movePoint.x, player.transform.position.x - threshold, player.transform.position.x + threshold),
                Mathf.Clamp(movePoint.y, player.transform.position.y - threshold * 0.6f, player.transform.position.y + threshold * 0.6f), movePoint.z);
        }


        if (slowZoom)
        {
            zoomDistant = 19;
            if (Application.isMobilePlatform)
                zoomDistant *= 0.75f;
            vcam.m_Lens.OrthographicSize = Mathf.SmoothDamp(vcam.m_Lens.OrthographicSize, zoomDistant, ref refFloat, zoomTime * 3);
            return;
        }
        if (getHurt)
        {
            Time.timeScale = 0.3f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;

            zoomDistant = 16;
            if (Application.isMobilePlatform)
                zoomDistant *= 0.75f;
            vcam.m_Lens.OrthographicSize = Mathf.SmoothDamp(vcam.m_Lens.OrthographicSize, zoomDistant, ref refFloat, zoomTime / 5);
            return;
        }
        else
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
        if (lastBoss == true)
        {
            zoomDistant = bossDistant;
            if (Application.isMobilePlatform)
                zoomDistant *= 0.75f;
            vcam.m_Lens.OrthographicSize = Mathf.SmoothDamp(vcam.m_Lens.OrthographicSize, zoomDistant, ref refFloat, zoomTime);
            return;
        }

        zoomDistant = 19;
        if (Application.isMobilePlatform)
            zoomDistant *= 0.75f;
        vcam.m_Lens.OrthographicSize = Mathf.SmoothDamp(vcam.m_Lens.OrthographicSize, zoomDistant, ref refFloat, zoomTime);
    }
    public void ResetCamShake()
    {
        CinemachineBasicMultiChannelPerlin cbmp = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cbmp.m_AmplitudeGain = 0;
        shakeForce = 0;
        timer = 0;
    }

    public void ShakeCam(float theShakeTime, float theShakeForce)
    {
        CinemachineBasicMultiChannelPerlin cbmp = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (theShakeForce > shakeForce || theShakeTime > timer)
        {
            cbmp.m_AmplitudeGain = theShakeForce;
            timer = theShakeTime;
            shakeForce = theShakeForce;
        }
    }
}
