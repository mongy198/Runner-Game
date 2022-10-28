using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Road_Manager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> road_blocks = new List<GameObject>();
    float offset = 20;

    [SerializeField]
    Transform[] traps_points;
    [SerializeField]
    GameObject bot, strong_bot;
    void Start()
    {
        //instantiate traps randomely
        float random_r;
        float random_l;
        float strong_bot_check = 0;
        foreach (Transform p in traps_points)
        {
            random_r = Random.Range(1f, 4.5f);
            random_l = Random.Range(-1f, -4.5f);
            strong_bot_check += 1;
            if (strong_bot_check > 5)
            {
                Instantiate(strong_bot, p.position + new Vector3(4.5f, 0, 0), p.localRotation, p);
                strong_bot_check = 0;
            }
            else
            {
                Instantiate(bot, p.position + new Vector3(random_r, 0, 0), p.localRotation, p);
                Instantiate(bot, p.position + new Vector3(random_l, 0, 0), p.localRotation, p);
            }
        }
    }
    public void move_road_blocks()
    {
        //this method is used in player_controller script when triggering road end
        GameObject r0 = road_blocks[0];
        road_blocks.Remove(r0);
        float z_distance = road_blocks[road_blocks.Count - 1].transform.position.z + offset;
        r0.transform.position = new Vector3(0, 0, z_distance);
        road_blocks.Add(r0);

    }
}
