using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Standard.TimeHandlers
{
    public class BaseTimer
    {
        public float TimeLeft { get; private set; }

        public bool IsTimeEllapsed => TimeLeft <= 0;

        public Action TimeHasEllapsed;

        public void Update(float time)
        {
            TimeLeft = time;
        }

        public void Invoke()
        {
            var prevTimeEllapsedStatus = IsTimeEllapsed;

            TimeLeft -= Time.deltaTime;

            if (!prevTimeEllapsedStatus && IsTimeEllapsed)
            {
                TimeHasEllapsed?.Invoke();
            }
        }
    }
}
