using System.Collections;
using HEScripts.Camera;
using UnityEngine;

namespace HEScripts.UI
{
    public class UICloseup : Closeup
    {
        [SerializeField] GameObject Content;

        protected override void Start()
        {
            base.Start();
            Content.SetActive(false);
        }
        public override IEnumerator ActivationRoutine()
        {
            Content.SetActive(true);
            yield return null;
        }

        public override IEnumerator DectivationRoutine()
        {
            Content.SetActive(false);
            yield return null;
        }
    }
}