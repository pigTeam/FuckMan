using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightSlashPool : MonoBehaviour
{
    private static Transform PARENT;

    public Transform rightPosition;
    public Transform leftPosition;
    public Transform downPosition;
    public GameObject slashPf;

    private List<KnightSlash> slashPool = new List<KnightSlash>();

    public void GenerateSlash(CharacterBase owner,Direction dir)
    {
        if(PARENT == null)
        {
            GameObject go = new GameObject("Knight Slash");
            PARENT = go.transform;
        }

        KnightSlash slash;

        if(slashPool.Count > 0)
        {
            slash = slashPool[0];
            slashPool.Remove(slash);
        }
        else
        {
            GameObject slashGo = Instantiate(slashPf);
            slashGo.transform.parent = PARENT;
            slash = slashGo.GetComponent<KnightSlash>();
        }

        Vector3 pos = rightPosition.position;
        Quaternion rotation = Quaternion.identity;
        if(dir == Direction.Left)
        {
            pos = leftPosition.position;
            rotation = Quaternion.Euler(0, 0, 180);
        }
        else if(dir == Direction.Down)
        {
            pos = downPosition.position;
            rotation = Quaternion.Euler(0, 0, -90);
        }

        slash.transform.position = pos;
        slash.transform.rotation = rotation;
        slash.gameObject.SetActive(true);
        slash.Act(owner,(target)=> {
            slash.gameObject.SetActive(false);
            slashPool.Add(slash);
            if(target != null)
            {
                CharacterBase targetCharacter = target.GetComponent<CharacterBase>();
                if(targetCharacter != null)
                {
                    owner.DamageOtherCharacter(targetCharacter);
                }
            }
        });
    }

}
