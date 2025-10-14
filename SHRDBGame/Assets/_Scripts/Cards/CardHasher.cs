using UnityEngine;

public static class CardHasher
{
    private static int nextID = 0;
    //Hasher sencillo para no tener que escribir los ids a mano
    public static int GetUniqueID()
    {
        return ++nextID;
    }

}
