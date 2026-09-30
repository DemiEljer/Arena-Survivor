using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Standard.Objects
{
    public static class UnityObjectExtentions
    {
        public static Bounds GetTotalObjectBounds(this GameObject gameObject)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                return new Bounds(gameObject.transform.position, Vector3.zero);
            }

            Bounds totalBounds = renderers[0].bounds;

            foreach (Renderer rend in renderers)
            {
                if (rend.enabled)
                {
                    totalBounds.Encapsulate(rend.bounds);
                }
            }

            return totalBounds;
        }
    }
}
