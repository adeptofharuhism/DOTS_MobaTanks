using System.Collections;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}
