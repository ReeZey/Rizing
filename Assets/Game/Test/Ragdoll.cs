using System.Linq;
using Rizing.Abstract;
using Rizing.Interface;
using UnityEngine;

public class Ragdoll : BaseEntity
{
    [SerializeField]
    private GameObject spine;

    private float die_time;
    private Animator anim;
    private bool dead = false;
    
    protected override void Start()
    {
        base.Start();
        spine.GetComponentsInChildren<Rigidbody>().ToList().ForEach(r => r.isKinematic = true);

        die_time = Random.Range(3.0f, 10.0f);
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Die")]
    void Die() {
        dead = true;

        anim.enabled = false;
        spine.GetComponentsInChildren<Rigidbody>().ToList().ForEach(r => {
            r.isKinematic = false;
            r.AddForce(Vector3.up * Random.Range(0.5f, 1.0f), ForceMode.Impulse);
            r.tag = "Pickupable";
        });
    }

    public override void Play()
    {
        if (dead) return;
        
        anim.enabled = true;
    }

    public override void Pause()
    {
        anim.enabled = false;
    }

    public override void Process(float deltaTime)
    {
        if (die_time > 0) {
            die_time -= deltaTime;
            if (die_time <= 0) {
                Die();
            }
        }
    }
}
