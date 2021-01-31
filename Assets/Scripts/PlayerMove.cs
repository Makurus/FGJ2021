using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
public class PlayerMove : MonoBehaviour
{
    public Transform cursor;
    public Humanity humanity;
    public float enemysSee;
    public bool isMonster;
    Rigidbody2D body;
    public float maxSpeed;
    float slowDownTime;
    float speedUpTime;
    public Transform faceDir;
    public GameObject actionBox;
    public GameObject basicAttack;
    public GameObject superAttack;
    [SerializeField] float throwPowerMultiplier;

    HearthMove hearth;
    public Transform ThrowIndicator;

    public Animator golemAnim;
    public Animator huggerAnim;
    public SpriteRenderer humanSR;
    public SpriteRenderer monsterSR;

    // Start is called before the first frame update
    void Start()
    {
        isMonster = true;
        Cursor.visible = false;
        var lockMode = CursorLockMode.Confined;
        Cursor.lockState = lockMode;

        hearth = GameObject.FindObjectOfType<HearthMove>();
        body = GetComponent<Rigidbody2D>();

        transformMOnster();



        /////
        ///
        isMonster = false;
        transformMOnster();
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);
        hearth.stop = true;
        hearth.transform.DOMove(transform.position, 0.2f);
        hearth.transform.DOScale(0, 0.2f);
        hearth.transform.parent = transform;

        StartCoroutine(activateH());
        stupid++;
        ////
    }

    int canMove, canRotate, animationPlaying,stupid;
    float holdTimer;
    float maxDist = 2;
    bool hugStart,hugging;
    // Update is called once per frame
    void Update()
    {

        body.velocity = Vector2.zero;
        cursor.GetChild(0).gameObject.SetActive(!isMonster);
        cursor.GetChild(1).gameObject.SetActive(isMonster);

        print("FREEZE " + freeze);
        if (freeze)
            return;
        //INPUTS
        if (animationPlaying == 0)
        {
            if (canMove >= 0)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    body.velocity += Vector2.left * maxSpeed;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    body.velocity += Vector2.right * maxSpeed;
                }
                if (Input.GetKey(KeyCode.W))
                {
                    body.velocity += Vector2.up * maxSpeed;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    body.velocity += Vector2.down * maxSpeed;
                }
            }


       
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (!isMonster)
                {
                    canMove--;
                }
                if (isMonster && hearth.closeToPlayer)
                {
                    isMonster = false;
                    transformMOnster();
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(0).gameObject.SetActive(true);
                    hearth.stop = true;
                    hearth.transform.DOMove(transform.position, 0.2f);
                    hearth.transform.DOScale(0, 0.2f);
                    hearth.transform.parent = transform;

                    StartCoroutine(activateH());
                    stupid = 0;
                    
                }
                else
                    hearth.callHearth = true;



                //isMonster = !isMonster;

            }


            if (Input.GetKey(KeyCode.Mouse1))
            {
                holdTimer += Time.deltaTime * throwPowerMultiplier;
                maxDist = 10;
                if (!hearth.gameObject.activeSelf)
                {
                    //ThrowIndicator.gameObject.SetActive(true);
                    RaycastHit2D[] hits = Physics2D.CircleCastAll(actionBox.transform.position, 0.2f, (actionBox.transform.position - actionBox.transform.parent.position).normalized, 10);
                    Debug.DrawLine(transform.position, transform.position + (actionBox.transform.position - transform.position).normalized);
                    foreach (var hit in hits)
                    {
                        if (hit.transform.tag == "wall")
                        {
                            maxDist = Vector2.Distance(transform.position, hit.point) - 1.5f;
                            break;
                        }
                    }

                    ThrowIndicator.position = actionBox.transform.position + (actionBox.transform.position - actionBox.transform.parent.position).normalized * Mathf.Min(maxDist, holdTimer);
                    cursor.position = ThrowIndicator.position;
                    huggerAnim.SetBool("throwing", true);
                }
                else
                    cursor.position = actionBox.transform.parent.GetChild(1).position;

            }
            else
                cursor.position = actionBox.transform.parent.GetChild(1).position;

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                if (!isMonster)
                {
                    if (stupid == 1)
                    {
                        if (!hearth.gameObject.activeSelf)
                        {
                            hearth.gameObject.SetActive(true);
                            isMonster = true;

                            huggerAnim.SetBool("throwing", false);
                         

                            //Wait anim to complete and then turn to monster
                            StartCoroutine(MonsterTransform());

                            StartCoroutine(StopStop(0.5f));
                            hearth.transform.position = actionBox.transform.position;
                            //hearth.GetComponent<Rigidbody2D>().velocity = (actionBox.transform.position - transform.position).normalized * 20;
                            float dist = Mathf.Min(maxDist, holdTimer);
                            float throwtime = 0.2f + dist / 7f;

                          

                            hearth.transform.DOMove(actionBox.transform.position + (actionBox.transform.position - actionBox.transform.parent.position).normalized * dist, throwtime);
                            hearth.transform.parent = transform.parent;
                            var seq = DOTween.Sequence();
                            seq.Append( hearth.transform.DOScale(hearth.origScale * 2f, throwtime/2));
                            seq.Append(hearth.transform.DOScale(hearth.origScale, throwtime/2));
                            seq.Play();
                            ThrowIndicator.gameObject.SetActive(false);
                        }
                    
                        canMove++;
                    }
                    else
                        stupid++;
              
                }
                hearth.callHearth = false;
                holdTimer = 0;
                
            }

           



            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (isMonster)
                {
                    if(Vector2.Distance(transform.position,hearth.transform.position)> humanity.safeZone)
                        Instantiate(superAttack, actionBox.transform.position, faceDir.rotation);
                    else
                        Instantiate(basicAttack, actionBox.transform.position, faceDir.rotation);
                    golemAnim.SetTrigger("att");

                }
                else
                {
                    actionBox.SetActive(true);
                    hugStart = true;
                    hugging = true;
                } 
                canRotate--;
                canMove--;
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                var enemy = actionBox.GetComponent<Hug>().enemyToHug;
                if (enemy != null && enemy.canUseForHealing)
                {
                    Destroy(enemy.gameObject);
                    humanity.heal = false;
                }
                if (enemy == null)
                    humanity.heal = false;
                else
                    enemy.isBeeingHugged = false;
                actionBox.SetActive(false);
                canRotate++;
                canMove++;
                if (!isMonster)
                {
                    huggerAnim.Play("player_normal_idle");
                    hugging = false;
                }
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (!isMonster)
                {
                    var enemy = actionBox.GetComponent<Hug>().enemyToHug;
                    if (enemy != null)
                    {
                        if (!enemy.dying)
                        {
                            enemy.hug();
                            if (hugStart)
                            {
                                huggerAnim.Play("player_normal_hug");
                                hugStart = false;
                            }
                        
                        }
                        else if (enemy.canUseForHealing)
                        {
                            humanity.heal = true;
                          
                        }

                    }
                    else
                    {
                        if (hugStart)
                        {
                            huggerAnim.Play("player_normal_hug_miss");
                            hugStart = false;
                        }
                    }
                }
                    
            }

            if (canRotate >= 0)
            {
                Vector2 mouse2D = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                Vector2 screenMiddle = Camera.main.WorldToScreenPoint(actionBox.transform.parent.position);// //new Vector2(Screen.width / 2, Screen.height / 2);
                Vector2 facingDir = (screenMiddle - new Vector2(Input.mousePosition.x, Input.mousePosition.y)).normalized;
                float a = Vector2.SignedAngle(oldMouse, mouse2D);
                faceDir.rotation = Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, facingDir)));//faceDir.rotation.eulerAngles.z + a));//  360 * ((Input.mousePosition.x / Screen.width) + (Input.mousePosition.y / Screen.height))));// 
                oldMouse = mouse2D;
            }
        }

        var mag = body.velocity.magnitude;
        print("velocity " + body.velocity.magnitude);
        if (mag < 0.1f)
            mag = -1;
        huggerAnim.SetFloat("speed", mag);
        golemAnim.SetFloat("speed", mag);
        monsterSR.flipX = body.velocity.x < 0;


    }

    private void LateUpdate()
    {
        if (hugging)
            humanSR.flipX = actionBox.transform.position.x < transform.position.x;
        else
            humanSR.flipX = body.velocity.x < 0;
    }
    public Vector3 oldMouse;
    void transformMOnster()
    {
        //if (isMonster)
        //    transform.DOScale(1.2f, 0.2f);
        //else
        //    transform.DOScale(1f, 0.2f);

        //if (isMonster)
        //    GetComponent<SpriteRenderer>().color = Color.gray;
        //else
        //    GetComponent<SpriteRenderer>().color = Color.white;

    }

    IEnumerator StopStop(float time)
    {
        yield return new WaitForSeconds(time);
        hearth.stop = false;
    }

    IEnumerator MonsterTransform()
    {

        
        yield return new WaitForSeconds(0.5f);
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        transformMOnster();
    }

    IEnumerator activateH()
    {
        yield return new WaitForSeconds(0.5f);
        hearth.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.(transform.position, enemysSee);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, enemysSee);
    }


    public bool freeze;
    public void freezePlayer(float t)
    {
        if (freeze)
            return;
        StartCoroutine(freeze_(t));
    }
    IEnumerator freeze_(float t)
    {

        freeze = true;
        yield return new WaitForSeconds(t);
        freeze = false;
    }
}
