using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//this script is a  controller and game manager
public class Player_controller : MonoBehaviour
{
    Rigidbody rb;
    float speed;
    [SerializeField]
    Road_Manager road_manager;

    float current_gravity = -9.81f*2;

    Animator anim_main;

    bool flipped;
    bool able_to_flip = true;
    Transform holder;

    float health;
    bool alive = false;
    check_ground get_check;

    [SerializeField]
    Transform cam;

    [SerializeField]
    Image blood_img;
    Color blood_color;
    [SerializeField]
    Image health_bar, start_btn, restart_btn, screen_cover;

    float from_to,score, distance;
    [SerializeField]
    Text score_txt, dis_txt, score_end_text;
    [SerializeField]
    GameObject note_img;
    int score_check_counter;

    List<GameObject> lost_bots = new List<GameObject>();
    [SerializeField]
    GameObject end_panel, start_panel, game_pnl;

    ParticleSystem die_effect;
    void Start()
    {
        //set scree size to portrait
        Screen.SetResolution(1080, 1920, true);

        die_effect = transform.GetChild(1).GetComponent<ParticleSystem>();
        Physics.gravity = new Vector3(0, current_gravity, 0);
        make_button_animation(start_btn);
        score_check_counter = 5;
        health = 100;
        speed = 15;
        blood_color = blood_img.color;
        rb = GetComponent<Rigidbody>();
        holder = transform.GetChild(0);
        get_check = holder.GetComponent<check_ground>();
        anim_main = holder.GetChild(0).transform.GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        if (alive)
        {
            player_move();
            //if player is far from road then do damage
            if (farPlayer_check())
            {
                do_damage(.75f);
            }
            //change distance 
            distance += .05f;
            dis_txt.text = distance.ToString("0");
        }
        //make blood color fade
        if (blood_color.a > 0f)
        {
            blood_color.a -= .01f;
            blood_img.color = blood_color;
        }
        //change score
        if (from_to < score)
        {
            from_to += 2;
        }
        else
        {
            note_img.SetActive(false);
        }
        score_txt.text = from_to.ToString("0");
    }
    void LateUpdate()
    {
        if (able_to_flip && alive) 
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                flip(180);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                flip(-180);
            }
        }
    }
    void player_move()
    {
        //move player right and left
        Vector3 movement_dir;
        if (!flipped)
        {
            movement_dir = new Vector3(Input.GetAxis("Horizontal"), 0, 1);
        }
        else
        {
            movement_dir = new Vector3(-Input.GetAxis("Horizontal"), 0, 1);
        }
        #region control telt right and left - - remove if you don't want that feature
        float x_value = movement_dir.x;
        if (x_value > 0)
        {
            if (!flipped)
            {
                rotate_player(-10);
            }
            else
            {
                rotate_player(190);
            }
        }
        else if (x_value < 0)
        {
            if (!flipped)
            {
                rotate_player(10);
            }
            else
            {
                rotate_player(170);
            }
        }
        else
        {
            if (!flipped)
            {
                rotate_player(0);
            }
            else
            {
                rotate_player(180);
            }
        }
        #endregion

        //fall when not grounded
        if (get_check.grounded)
        {
            anim_main.SetBool("fall", false);
        }
        else
        {
            anim_main.SetBool("fall", true);
        }
        rb.transform.position += movement_dir * speed * Time.deltaTime;
        //making game harder with time
        if (speed < 30)
        {
            speed += .025f * Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //check the road end to change road index
        if (other.CompareTag("block_end"))
        {
            road_manager.move_road_blocks();
            //update score after 5 road blocks
            score_check_counter--;
            if (score_check_counter < 1)
            {
                score_check_counter = 5;
                //update score
                score += 100;
                note_img.SetActive(true);
            }
        }
        //trap hit check
        if (other.CompareTag("trap"))
        {
            //add lost bot to the list so it return back after replay
            lost_bots.Add(other.gameObject);

            do_damage(25);
            anim_main.SetTrigger("damage");
            other.gameObject.SetActive(false);
        }
    }
    void flip(float deg)
    {
        //used for fliping player with Q and E
        if (able_to_flip)
        {
            able_to_flip = false;
            current_gravity = -current_gravity;
            Physics.gravity = new Vector3(0, current_gravity, 0);

            if (flipped)
            {
                flipped = false;
            }
            else
            {
                flipped = true;
            }
            transform.DORotate(new Vector3(0, 0, deg), .5f, RotateMode.LocalAxisAdd).SetRelative();
            StartCoroutine(set_flip());
        }
    }
    void rotate_player(float rot_value)
    {
        //used for telt player right and left
        holder.DORotate(new Vector3(0, 0, rot_value), 1f);
    }
    IEnumerator set_flip()
    {
        yield return new WaitForSeconds(.6f);
        able_to_flip = true;
    }
    bool farPlayer_check()
    {
        //check if player is far from the road
        return transform.position.x > 20 || transform.position.x < -20 || transform.position.y > 25 || transform.position.y < -25;
    }
    void do_damage(float dmg)
    {
        if (health > dmg)
        {
            //make damage
            health -= dmg;
        }
        else
        {
            //die
            health = 0;
            die();
        }
        blood_color.a = .8f;
        health_bar.fillAmount = health/100;
        cam.DOShakeRotation(.5f, 2f).OnComplete(() => cam.localRotation = Quaternion.Euler(14, 0, 0));
    }
    void die()
    {
        //:(
        die_effect.Play();
        alive = false;
        rb.isKinematic = true;
        score_end_text.text = "Score\n"+score.ToString("0");
        end_panel.SetActive(true);
        make_button_animation(restart_btn);
        game_pnl.SetActive(false);
        holder.gameObject.SetActive(false);
        GetComponent<CapsuleCollider>().enabled = false;
    }
    public void restart()
    {
        //restart game
        end_panel.SetActive(false);
        foreach(GameObject b in lost_bots)
        {
            b.SetActive(true);
        }
        lost_bots.Clear();
        reset_allData();
        screen_cover.DOFade(1f, 0f);
        screen_cover.DOFade(0f, 2f);
    }
    private void reset_allData()
    {
        //reset all data when restarting
        rotate_player(0);
        transform.position = new Vector3(0, 1.5f, transform.position.z);
        transform.rotation = Quaternion.identity;
        current_gravity = -9.81f * 2;
        Physics.gravity = new Vector3(0, current_gravity, 0);
        holder.gameObject.SetActive(true);
        score = 0;
        from_to = 0;
        distance = 0;
        health = 100;
        speed = 15;
        health_bar.fillAmount = 1;
        dis_txt.text = "0";
        score_txt.text = "0";
        flipped = false;
        able_to_flip = true;
        start_panel.SetActive(true);
        make_button_animation(start_btn);
    }
    public void start()
    {
        rb.isKinematic = false;
        GetComponent<CapsuleCollider>().enabled = true;
        alive = true;
        game_pnl.SetActive(true);
        start_panel.SetActive(false);
    }
    void make_button_animation(Image img)
    {
        //make animation to start and restart buttons
        img.rectTransform.DOSizeDelta(new Vector2(10, 10), 0f);
        img.rectTransform.DOSizeDelta(new Vector2(450, 450), .5f).OnComplete(()=> img.rectTransform.DOSizeDelta(new Vector2(420, 420), .1f));
    }
}
