using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    AIPath aiPath;
    AIDestinationSetter aiDS;
    public SpriteRenderer sr;
    PlayerMove player;
    HearthMove hearth;
    public GameObject healParticle;
    Rigidbody2D body;
    public float movSpeed;
    public float MaxCoolDown;
    public float MinCoolDown;
    public Animator animator;
    float newCoolDown;
    float coolDownTimer;
    bool stop;
    public bool stunned;
   
    public bool attackOnlyHearth;
    [Space]
    public Transform enemyWeapon;
    int attacking;
    public UnityEvent Attack;

    [SerializeField] Animator enemyAnim;


    [SerializeField] bool playSounds;
    [SerializeField] AudioSource aus;
    [SerializeField] AudioClip[] audioclips;

    bool moves;
    //public GameObject projectilePrefab;
    //float attackLag;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindObjectOfType<MonsterManager>().enemies++;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();// transform;
        hearth = GameObject.Find("Hearth").GetComponent<HearthMove>();//.transform;
        body = GetComponent<Rigidbody2D>();
        newCoolDown = Random.Range(MinCoolDown, MaxCoolDown);
        //animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
        aiDS = GetComponent<AIDestinationSetter>();

        StartCoroutine(GruntSounds());

    }
    Transform target;
    // Update is called once per frame


    void Update()
    {

        float distancePlayer = Vector3.Distance(transform.position, player.transform.position);
        float distanceHearth = Vector3.Distance(transform.position, hearth.transform.position);
        bool seesPlayer = distancePlayer < player.enemysSee;
        bool seesHearth = distanceHearth < hearth.enemysSee;
       
        moves = false;
        if(!attackOnlyHearth && seesHearth && seesPlayer)
        {
            target = Vector3.Distance(transform.position, player.transform.position) < Vector3.Distance(transform.position, hearth.transform.position) ? player.transform : hearth.transform;
            moves = true;
        }
        else if (!attackOnlyHearth && seesPlayer)
        {
            target = player.transform;
            moves = true;
        }
        else if (seesHearth)
        {
            target = hearth.transform;
            moves = true;
        }

        if (!stunned && target != null)
        {
            if (target.position.x > transform.position.x)
                sr.flipX = true;
            else
                sr.flipX = false;

            coolDownTimer += Time.deltaTime;
            if (attacking > 0)
            {
                Vector2 facingDir = (target.position - transform.position).normalized;

                enemyWeapon.rotation = Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, -facingDir)));

                if (coolDownTimer > newCoolDown)
                {
                    coolDownTimer = 0;
                    newCoolDown = Random.Range(MinCoolDown, MaxCoolDown);
                    Attack.Invoke();
                
                }
            }

            aiDS.target = target;
            //if (moves && !stop)
            //    body.velocity = (target.position - transform.position).normalized * movSpeed;

            aiPath.canMove = moves && !stop;

            animator.SetBool("walking", moves && !stop);
        }
        else
        {
            aiPath.canMove = false;
            animator.SetBool("walking", false);

        }


    }

   
    public void ExecuteAttack(GameObject prefab)
    {
        float lag = prefab.GetComponent<Projectile>().lag;
        StartCoroutine(_ExecuteAttack(prefab, lag));
    }
    IEnumerator _ExecuteAttack(GameObject prefab, float lag)
    {
        yield return new WaitForSeconds(lag);
        var g = Instantiate(prefab, enemyWeapon.GetChild(0).position, enemyWeapon.rotation, null);
        g.GetComponent<Projectile>().direction = (target.position - transform.position).normalized;
    }
    public void Stop(float time)
    {
        if (stop)
            return;
        StartCoroutine(Stop_(time));
    }

    IEnumerator Stop_(float time)
    {
        stop = true;
        yield return new WaitForSeconds(time);
        if(!GetComponent<Health>().dying)
            stop = false;
    }

    public void pushBack(float force)
    {
        body.velocity = (transform.position - player.transform.position).normalized * force;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
     
        if (!collision.isTrigger && (collision.name == "Player" || collision.name == "Hearth"))
        {
            attacking++;
               
        }
        

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (!collision.isTrigger && (collision.name == "Player" || collision.name == "Hearth"))
        {
            attacking--;

        }


    }


    public void playAnimation(string name)
    {
        animator.Play(name);
    }


    public void spawnHeal()
    {
        StartCoroutine(healS());
    }
    IEnumerator healS()
    {
        yield return new WaitForSeconds(3);
         var g = Instantiate(healParticle, transform);
        g.transform.position = transform.position;
    }
    private void OnDestroy()
    {
        GameObject.FindObjectOfType<MonsterManager>().enemies--;
    }

    IEnumerator GruntSounds()
    {
        while (playSounds) {
            

            yield return new WaitForSeconds(Random.Range(3f, 9f));

            aus.PlayOneShot(audioclips[Random.Range(0, audioclips.Length)]);

        }
    }
}
