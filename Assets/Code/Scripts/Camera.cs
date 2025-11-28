// Source - https://stackoverflow.com/a
// Posted by Zaphod, modified by community. See post 'Timeline' for change history
// Retrieved 2025-11-24, License - CC BY-SA 4.0

using UnityEngine;

public class Follow_player : MonoBehaviour
{

    public Transform player;

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + new Vector3(0, 1, -5);
    }
}
