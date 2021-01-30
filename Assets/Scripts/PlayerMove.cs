using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
public class PlayerMove : MonoBehaviour
{
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

    HearthMove hearth;
    public Transform ThrowIndicator;
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
    }

    int canMove, canRotate, animationPlaying,stupid;
    float holdTimer;
    float maxDist = 2;
    // Update is called once per frame
    void Update()
    {
        body.velocity = Vector2.zero;
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
                    hearth.stop = true;
                    hearth.transform.DOMove(transform.position, 0.2f);
                    hearth.transform.DOScale(0, 0.2f);
                    hearth.transform.parent = transform;

                    StartCoroutine(activateH());
                    stupid = 0;
                    
                }

                
                //isMonster = !isMonster;

            }

           
            if (Input.GetKey(KeyCode.Mouse1))
            {
                holdTimer += Time.deltaTime * 3;
                if (!hearth.gameObject.activeSelf)
                {
                    ThrowIndicator.gameObject.SetActive(true);
                    RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.2f, (actionBox.transform.position - transform.position).normalized,10);
                    Debug.DrawLine(transform.position, transform.position + (actionBox.transform.position - transform.position).normalized);
                    foreach (var hit in hits)
                    {
                        if (hit.transform.tag == "wall") 
                        {
                            maxDist = Vector2.Distance(transform.position, hit.point);
                            break;
                        }
                    }

                    ThrowIndicator.position = transform.position + (actionBox.transform.position - transform.position).normalized * Mathf.Min(maxDist,holdTimer);

                }
            }

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
                            transformMOnster();
                            StartCoroutine(StopStop(0.5f));
                            hearth.transform.position = transform.position;
                            //hearth.GetComponent<Rigidbody2D>().velocity = (actionBox.transform.position - transform.position).normalized * 20;
                            float dist = Mathf.Min(maxDist, holdTimer);
                            float throwtime = 0.2f + dist / 4f;
                            hearth.transform.DOMove(transform.position + (actionBox.transform.position - transform.position).normalized * dist, 0.2f + dist/6f);
                            hearth.transform.parent = transform.parent;
                            var seq = DOTween.Sequence();
                            seq.Append( hearth.transform.DOScale(hearth.origScale * 2f, throwtime/2));
                            seq.Append(hearth.transform.DOScale(hearth.origScale, throwtime));
                            seq.Play();
                            ThrowIndicator.gameObject.SetActive(false);
                        }
                    
                        canMove++;
                    }
                    else
                        stupid++;
              
                }
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


                }
                else 
                    actionBox.SetActive(true);
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
                if(enemy == null)
                    humanity.heal = false;
                actionBox.SetActive(false);
                canRotate++;
                canMove++;
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (!isMonster)
                {
                    var enemy = actionBox.GetComponent<Hug>().enemyToHug;
                    if (enemy != null)
                    {
                        if(!enemy.dying)
                            enemy.hug();
                        else if (enemy.canUseForHealing)
                            humanity.heal = true;

                    }
                }
                    
            }

            if (canRotate >= 0)
            {
                Vector2 screenMiddle = new Vector2(Screen.width / 2, Screen.height / 2);
                Vector2 facingDir = (screenMiddle - new Vector2(Input.mousePosition.x, Input.mousePosition.y)).normalized;
                faceDir.rotation = Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, facingDir)));
            }
        }
      
      
       
    }

    void transformMOnster()
    {
        if (isMonster)
            transform.DOScale(1.2f, 0.2f);
        else
            transform.DOScale(1f, 0.2f);

        if (isMonster)
            GetComponent<SpriteRenderer>().color = Color.gray;
        else
            GetComponent<SpriteRenderer>().color = Color.white;

    }

    IEnumerator StopStop(float time)
    {
        yield return new WaitForSeconds(time);
        hearth.stop = false;
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
}
