using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public interface IAnimationable
    {
        void Animate(GameObject buildObject, float duration, float dropHeight);
    }
}
