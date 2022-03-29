using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace itemSystem
{

    public class Item : ScriptableObject
    {
        public enum TYPE { HP, MP, SWORD }

        public TYPE type;           // 아이템의 타입.
        public Sprite DefaultImg;   // 기본 이미지.
        public int MaxCount;        // 겹칠수 있는 최대 숫자.

        public virtual void Use()
        {

        }

    }
}
