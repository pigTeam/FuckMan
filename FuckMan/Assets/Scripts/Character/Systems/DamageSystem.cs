using UnityEngine;
using System.Collections;
using Unity.Entities;

public class DamageSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<DamageComponent, InjuryComponent>()
            .ForEach((Entity id,ref DamageComponent damage,ref InjuryComponent injury)=> {
                //Audio
                AudioHandler fromAudio = GetAudioHandler(damage.fromUserId);
                AudioHandler toAudio = GetAudioHandler(damage.targetUserId);
                if (fromAudio != null)
                {
                    fromAudio.OnHitSomeBody();
                }
                if(toAudio!= null)
                {
                    toAudio.OnGetHited();
                }

                //Phsic effect
                Rigidbody2D rigidBody = EntityUtility.Instance.GetComponent<Rigidbody2D>(id);
                if (rigidBody != null)
                {
                    int force = 3 + (int)(5 * (float)injury.value / 500f);
                    //rigidBody.AddForce(new Vector2(force, 0), ForceMode2D.Impulse);
                    rigidBody.velocity += force * damage.dir.ToVector2();
                }

                //Debug.LogError("Get damage from " + damage.fromUserId + ",damge = " + damage.value);
                injury.value += damage.value;
                EntityManager.RemoveComponent(id, typeof(DamageComponent));
               
            });
    }

    private AudioHandler GetAudioHandler(uint characterID)
    {
        AudioHandler fromAudio = null;
        CharacterBase fromCharacter = GameManager.Inst.GetCharacter(characterID);
        if (fromCharacter != null)
        {
            fromAudio = fromCharacter.GetComponent<AudioHandler>();
        }
        return fromAudio;
    }
}
